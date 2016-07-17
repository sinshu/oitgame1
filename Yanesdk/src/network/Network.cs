using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using Yanesdk.Ytl;
using System.Net.Sockets;
using System.Net;
using System.IO.Compression;

namespace Yanesdk.Network
{
    /// <summary>
    /// TCP�őo�����ʐM���s���N���X�ł��B
    /// </summary>
    public class Network : INetwork
    {
        /// <summary>
        /// �����n���h���l
        /// </summary>
        public static long INITIAL_HANDLE_VALUE = 1;

        /// <summary>
        /// �p�P�b�g���
        /// </summary>
        private enum PacketType
        {
            /// <summary>
            /// �����ʒm���b�Z�[�W
            /// </summary>
            KeepAliveNotify = 0,
            /// <summary>
            /// �ؒf�ʒm���b�Z�[�W
            /// </summary>
            DisconnectNotify = 1,
            /// <summary>
            /// �ʏ탁�b�Z�[�W
            /// </summary>
            NormalMessage = 2,
            /// <summary>
            /// �l�b�g���[�NID�ʒm���b�Z�[�W
            /// </summary>
            NetworkIDNotify = 3,
        }

        /// <summary>
        /// �l�b�g���[�NID���擾�E�ݒ肵�܂��B
        /// �l�b�g���[�NID���g�p���Ȃ��ꍇ��null�l�ł��B
        /// </summary>
        /// <remarks>
        /// �قȂ�l�b�g���[�NID�����C���X�^���X���m�̒ʐM�͂ł��܂���B
        /// �T�[�o����Listen���\�b�h���Ăяo���O�A�N���C�A���g����Connect���\�b�h���Ăяo���O��
        /// ���ꂼ��ݒ肵�Ă����K�v������܂��B
        /// </remarks>
        public long? NetworkID
        {
            get { return _networkID; }
            set { _networkID = value; }
        }
        private long? _networkID;

        /// <summary>
        /// �����Read���\�b�h�̌Ăяo���ŁAKeepAlive���b�Z�[�W��ڑ����
        /// ���M���邩�ǂ������擾�E�ݒ肵�܂��B
		/// 
		/// �Ȃ��ARead�̌Ăяo���񐔂́A�ehandle���ƂɃJ�E���g���܂��B
		/// 
		/// �f�B�t�H���g�� 900�ŁA�b��30���Read���s�Ȃ��Ƃ��āA
		/// 900 = 30��~30�b�ł���A30�b���Ƃɐؒf���m�̂��߂�KeepAlive���b�Z�[�W�𑗐M���܂��B
		/// /// </summary>
		/// <remarks>
		/// 0���ݒ肳��Ă���Ƒ��M���܂���B
		/// ���M���Ȃ��ꍇ�A�ؒf�����m�ł��Ȃ��̂ŁA���Ȃ炸�ݒ肷��悤�ɂ��Ă��������B
		/// </remarks>
        public long KeepAliveSendInterval
        {
            get { return _keepAliveSendInterval; }
            set { _keepAliveSendInterval = value; }
        }
        private long _keepAliveSendInterval = 900;

		/// <summary>
		/// localhost����̑҂��󂯂̂��߂�port��Listen����Ƃ���
		/// WindowsXP�Ȃǂ�FireWall�̌x���_�C�A���O���o�Ȃ��悤�ɂ��邽�߂�
		/// LoopBack�̎w��B(default : false)
		/// </summary>
		/// <remarks>
		/// ������w�肷��ƊO������͐ڑ��ł��Ȃ��Ȃ�̂Œ��ӁI
		/// </remarks>
		public bool LoopBack
		{
			get { return loopBack; }
			set { loopBack = value; }
		}
		private bool loopBack = false;


        public Network()
        {
        }

        /// <summary>
        /// �T�[�o�[�ɐڑ����܂��B
        /// </summary>
        /// <returns>�ڑ����m�����ꂽ�ꍇ�͔�0�̐ڑ��n���h��</returns>
        public long Connect()
        {
            long handle = 0;
            try
            {
                TcpClient client = new TcpClient(_hostName, _port);
                if ( !client.Connected )
                {
                    //  ���s����
                    return 0;
                }
                //  timeout��10sec�ɃZ�b�g
                client.SendTimeout = 10000;
                client.ReceiveTimeout = 10000;
                //  �n���h���擾
                handle = AddClient(client);

                //  NetworkID���g�p����Ȃ瑗��
                if ( _networkID.HasValue )
                {
                    SendNetworkIDMessage(handle, client);
                }
                return handle;
            }
            catch
            {
                //  ���s�����Ȃ�ؒf
                if ( handle != 0 )
                {
                    InnerDisconnect(handle, false); //  �ʐM�G���[�Ǝv����̂Œʒm�͑���Ȃ�
                }
                return 0;
            }
        }

        /// <summary>
        /// �N���C�A���g�ɐڑ����܂��B
        /// </summary>
        /// <returns>�ڑ����m�����ꂽ�ꍇ�͔�0�̐ڑ��n���h��</returns>
        public long Listen()
        {
            if ( _listener == null )
            {
				//  TcpListener������Ă��Ȃ��ꍇ�͍쐬����

				if (LoopBack)
					_listener = new TcpListener(IPAddress.Loopback, _port);
				else
					_listener = new TcpListener(IPAddress.Any, _port);

                _listener.Start();
            }

            //  TcpListener�ɐڑ��v�������Ă��Ȃ����`�F�b�N
            if ( !_listener.Pending() )
            {
                //  ���Ă��Ȃ�
                return 0;
            }

            TcpClient client = _listener.AcceptTcpClient();
            //  timeout��10sec�ɃZ�b�g
            client.SendTimeout = 10000;
            client.ReceiveTimeout = 10000;
            //  �n���h���擾
            long handle = AddClient(client);

            //  NetworkID���g�p����Ȃ瑗��
            if ( _networkID.HasValue )
            {
                try
                {
                    SendNetworkIDMessage(handle, client);
                }
                catch
                {
                    InnerDisconnect(handle, false); //  �ʐM�G���[�Ǝv����̂Œʒm�͑���Ȃ�
                    return 0;
                }
            }
            return handle;
        }

        /// <summary>
        /// �w�肳�ꂽ�ڑ��n���h���ɑΉ������ڑ���ؒf���܂��B
        /// </summary>
        /// <param name="handle">�ڑ��n���h��</param>
        public void Disconnect(long handle)
        {
            InnerDisconnect(handle, true);
        }

        /// <summary>
        /// �w�肳�ꂽ�ڑ��n���h���ɑΉ������ڑ���ؒf���܂��B
		/// sendNotifyMessage == false�̏ꍇ�A�ؒf�ʒm�͑���܂���B
        /// </summary>
        /// <param name="handle">�ڑ��n���h��</param>
        /// <param name="sendNotifyMessage">�ؒf�ʒm��ڑ���ɑ��M����ꍇ��true</param>
        private void InnerDisconnect(long handle, bool sendNotifyMessage)
        {
            TcpClient client = GetTcpClient(handle);
            if ( client == null )
            {
                return;
            }
            if ( sendNotifyMessage )
            {
                //  �ؒf���b�Z�[�W�𑗂�
                try
                {
                    client.SendBufferSize = 0;  //  �����M�����悤��
                    InnerWrite(client, PacketType.DisconnectNotify, null, false);
                    InnerFlush(client);
                    client.GetStream().Close();
                    client.Close();
                }
                catch
                {
                }
            }
            RemoveClient(handle);
        }

        public void Dispose()
        {
            if ( _listener != null )
            {
                _listener.Stop();
                _listener = null;
            }

            //  foreach���ɂ��̃R���N�V�����̓��e��ύX����Ɨ�O����������̂ŁA
            //  key���R�s�[�����z��ɑ΂���foreach����B
			long[] handles = new long[_clientInfoMap.Count];
			_clientInfoMap.Keys.CopyTo(handles, 0);
            foreach ( long handle in handles )
            {
                InnerDisconnect(handle, true);
            }

        //    _removedHandleList.Clear();
        }

        /// <summary>
        /// �w�肳�ꂽ�ڑ��n���h���ɑΉ������ڑ�����f�[�^����M���܂��B
        /// </summary>
        /// <param name="handle">�ڑ��n���h��</param>
        /// <param name="data">��M�f�[�^���������܂��o�b�t�@</param>
        /// <returns>
        ///  0 :���튮��
        /// -1 :�n���h��������
        /// -2 :��M�f�[�^�̎�ʎ擾�Ɏ��s
        /// -3 :��M�f�[�^�̃T�C�Y�擾�Ɏ��s
        /// -4 :��M�f�[�^�̃T�C�Y���ő�p�P�b�g���𒴂��Ă���
        /// -5 :���ۂɎ�M���ꂽ�f�[�^�̃T�C�Y����������
        /// -6 :���肩��ؒf���ꂽ
        /// -7 :�l�b�g���[�NID���Ⴄ���ߐؒf����
        /// -99:�ʐM�G���[
        /// </returns>
        /// <exception cref="ArgumentException">�w�肳�ꂽ�n���h���͎g�p����Ă��܂���</exception>
        public int Read(long handle, out byte[] data)
        {
            data = null;
            TcpClient client = GetTcpClient(handle);
            if ( client == null )
            {
                return -1;
            }
            ClientInfo info = _clientInfoMap[handle];
            try
            {
                NetworkStream stream = client.GetStream();
                if ( !stream.DataAvailable )
                {
                    //  ��M�f�[�^�������Ƃ��Ɋ����`�F�b�N�i�����g�̐����ʒm�j���s�Ȃ�
                    if ( KeepAliveSendInterval > 0 && ++info.ActivityCheckCounter >= KeepAliveSendInterval )
                    {
                        info.ActivityCheckCounter = 0;
                        try
                        {
                            InnerWrite(client, PacketType.KeepAliveNotify, null, false);
                            InnerFlush(client);
                        }
                        catch
                        {
                            //  �ʐM��Q�Ǝv����
                            InnerDisconnect(handle, false);
                            return -6;
                        }
                    }
                    return 0;
                }

                byte[] buf = new byte[4];

                //  �f�[�^��ʂ̎擾
                if ( stream.Read(buf, 0, 4) != 4 )
                {
                    Disconnect(handle);
                    return -2;
                }
                int type = MemoryCopy.GetInt(buf, 0);

                //  �f�[�^�T�C�Y�̎擾
                if ( stream.Read(buf, 0, 4) != 4 )
                {
                    Disconnect(handle);
                    return -3;
                }
                int size = MemoryCopy.GetInt(buf, 0);

                //  �ő�p�P�b�g���𒴂��Ă��Ȃ����`�F�b�N
                if ( _maxPacketSize > 0 && size > _maxPacketSize )
                {
                    Disconnect(handle);
                    return -4;
                }

                //  �f�[�^�̎擾
                int originalSize = 0;
                byte[] buffer = null;
                if ( size > 0 )
                {
                    //  �W�J��f�[�^�T�C�Y�̎擾
                    if ( stream.Read(buf, 0, 4) != 4 )
                    {
                        Disconnect(handle);
                        return -3;
                    }
                    originalSize = MemoryCopy.GetInt(buf, 0);

                    //  ���k�f�[�^�̎擾
                    buffer = new byte[size];
                    int remain = size;
                    int readed = 0;
                    while ( remain != 0 )
                    {
                        int len = stream.Read(buffer, readed, remain);
                        readed += len;
                        remain -= len;
                        if ( len == 0 ) { break; }
                    }
                    if ( readed != size )
                    {
                        //  ���r���[�ɂ����ǂ߂Ă��Ȃ�
                        Disconnect(handle); //  �ʐM�G���[�̉\��������̂Œʒm�𑗂�Ȃ������悢�̂��ǂ����͓���Ƃ���B
                        return -5;
                    }
                }

                //  �f�[�^��ʂɉ������f�B�X�p�b�`
                switch ( type )
                {
                case (int)PacketType.KeepAliveNotify:
                    //  �����ʒm
                    return 0;
                case (int)PacketType.DisconnectNotify:
                    //  �ؒf�ʒm
                    InnerDisconnect(handle, false); //  ����͂������Ȃ��̂�����ʒm����K�v�͂Ȃ�
                    return -6;
                case (int)PacketType.NormalMessage:
                    //  �ʏ�f�[�^
                    if ( NetworkID != null )
                    {
                        //  �F�؍ς݂łȂ��Ȃ�f�[�^���̂ĂĐؒf
                        if ( !_clientInfoMap[handle].IsReceiveNetworkID )
                        {
                            //  �l�b�g���[�NID���Ⴄ�̂Őؒf����
                            Disconnect(handle);
                            return -7;
                        }
                    }
                    if ( size > 0 && originalSize != size )
                    {
                        //  �W�J���K�v
                        byte[] tmp = new byte[originalSize];
                        Deflate(buffer, tmp);
                        data = tmp;
                    }
                    else
                    {
                        //  ���k����Ă��Ȃ�
                        data = buffer;
                    }
                    return 0;
                case (int)PacketType.NetworkIDNotify:
                    if ( NetworkID != null )
                    {
                        long senderNetworkID = MemoryCopy.GetLong(buffer, 0);
                        if ( NetworkID == senderNetworkID )
                        {
                            //  �F�؍ς݂Ƃ��ă}�[�N
                            _clientInfoMap[handle].IsReceiveNetworkID = true;
                        }
                        else
                        {
                            //  �l�b�g���[�NID���Ⴄ�̂Őؒf����
                            Disconnect(handle);
                            return -7;
                        }
                    }
                    return 0;
                }
            }
            catch
            {
                InnerDisconnect(handle, false);
                return -99;
            }
            return 0;
        }

        /// <summary>
        /// �w�肳�ꂽ�ڑ��n���h���ɑΉ������ڑ��Ƀf�[�^�𑗐M���܂��B
        /// </summary>
        /// <param name="handle">�ڑ��n���h��</param>
        /// <param name="data">���M�f�[�^</param>
        /// <returns>
        ///  0:���튮��
        /// -1:�n���h��������
        /// -2:�G���[������
        /// </returns>
        /// <exception cref="ArgumentException">�w�肳�ꂽ�n���h���͎g�p����Ă��܂���</exception>
        public int Write(long handle, byte[] data)
        {
            TcpClient client = GetTcpClient(handle);
            if ( client == null )
            {
                return -1;
            }
            try
            {
                InnerWrite(client, PacketType.NormalMessage, data, true);
            }
            catch
            {
                InnerDisconnect(handle, false);
                return -2;
            }
            return 0;
        }

        /// <summary>
        /// �w�肳�ꂽ�ڑ��Ƀp�P�b�g��ʂ�t�����ăf�[�^�𑗐M���܂��B
        /// preferCompress == true�̎��A�f�[�^�����k���ăT�C�Y��
        /// ���̃T�C�Y��茸������ꍇ�͈��k�f�[�^�𑗐M���܂��B
        /// </summary>
        /// <param name="client">���M����TcpClient�I�u�W�F�N�g</param>
        /// <param name="type">���M�f�[�^�̎��</param>
        /// <param name="data">���M�f�[�^</param>
        /// <param name="preferCompress">�f�[�^�����k����ꍇ��true</param>
        private static void InnerWrite(TcpClient client, PacketType type, byte[] data, bool preferCompress)
        {
            NetworkStream stream = client.GetStream();
            //  �p�P�b�g��ʂ���������
            byte[] datatype = MemoryCopy.ToByteArrayFromInt((int)type);
            stream.Write(datatype, 0, datatype.Length);
            if ( data != null )
            {
                //  ���M�f�[�^�̈��k
                byte[] compressedData = null;
                if ( preferCompress )
                {
                    using ( MemoryStream ms = new MemoryStream() )
                    using ( DeflateStream ds = new DeflateStream(ms, CompressionMode.Compress) )
                    {
                        ds.Write(data, 0, data.Length);
                        ds.Close(); //  Flush�łȂ�Close�łȂ��Ƃ����Ȃ��炵���B�B
                        compressedData = ms.ToArray();
                    }
                }
                byte[] datasize = new byte[8];
                if ( compressedData != null && compressedData.Length < data.Length )
                {
                    //  ���k�f�[�^�̃T�C�Y�ƓW�J��̃T�C�Y����������
                    MemoryCopy.SetInt(datasize, 0, compressedData.Length);
                    MemoryCopy.SetInt(datasize, 4, data.Length);
                    stream.Write(datasize, 0, datasize.Length);
                    //  ���k�f�[�^����������
                    stream.Write(compressedData, 0, compressedData.Length);
                }
                else
                {
                    //  ���k���Ă��T�C�Y���������Ȃ������̂Ō��̃f�[�^����������
                    MemoryCopy.SetInt(datasize, 0, data.Length);
                    MemoryCopy.SetInt(datasize, 4, data.Length);
                    stream.Write(datasize, 0, datasize.Length);
                    stream.Write(data, 0, data.Length);
                }
            }
            else
            {
                //  ���M�f�[�^�̃T�C�Y���������ށi�l��0�j
                byte[] datasize = MemoryCopy.ToByteArrayFromInt(0);
                stream.Write(datasize, 0, datasize.Length);
            }
        }

        /// <summary>
        /// �w�肳�ꂽ�ڑ��n���h���ɑΉ������ڑ��̑���M�f�[�^��Flush���܂��B
        /// </summary>
        /// <param name="handle">�ڑ��n���h��</param>
        /// <returns>
        ///  0:���튮��
        /// -1:�n���h��������
        /// -2:�G���[������
        /// </returns>
        /// <exception cref="ArgumentException">�w�肳�ꂽ�n���h���͎g�p����Ă��܂���</exception>
        public int Flush(long handle)
        {
            TcpClient client = GetTcpClient(handle);
            if ( client == null )
            {
                return -1;
            }
            try
            {
                InnerFlush(client);
            }
            catch
            {
                InnerDisconnect(handle, false);
                return -2;
            }
            return 0;
        }

        /// <summary>
        /// Flush������s���܂��B
        /// </summary>
        /// <param name="client"></param>
        private void InnerFlush(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            stream.Flush();
            //  ��NetworkStream.Flush���\�b�h�͎��ۂɂ͉������Ȃ�
        }

        /// <summary>
        /// �w�肳�ꂽ�ڑ��n���h���ɑΉ������ڑ���IP�A�h���X���擾���܂��B
        /// </summary>
        /// <param name="handle">�ڑ��n���h��</param>
        /// <param name="ipAddress">IP�A�h���X</param>
        /// <returns>
        /// true :���튮��
        /// false:�G���[������
        /// </returns>
        /// <exception cref="ArgumentException">�w�肳�ꂽ�n���h���͎g�p����Ă��܂���</exception>
        public bool GetIpAddress(long handle, out byte[] ipAddress)
        {
            TcpClient client = GetTcpClient(handle);
            try
            {
                IPEndPoint endpoint = (IPEndPoint)client.Client.RemoteEndPoint;
                ipAddress = endpoint.Address.GetAddressBytes();
            }
            catch
            {
                ipAddress = null;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Connect���\�b�h�ɂ���Đڑ�����z�X�g���ƃ|�[�g�ԍ���ݒ肵�܂��B
        /// </summary>
        /// <param name="hostname">�z�X�g���܂���IP�A�h���X</param>
        /// <param name="portnumber">�|�[�g�ԍ�</param>
        public void SetHostName(string hostname, int portnumber)
        {
            _hostName = hostname;
            _port = portnumber;
        }
        private string _hostName = "127.0.0.1";
        private int _port = 0;

        /// <summary>
        /// ��M����ő�p�P�b�g����ݒ肵�܂��B
        /// </summary>
        /// <param name="maxPacketLength">0�Ȃ疳�����A��0�Ȃ琧������T�C�Y</param>
        public void SetMaxPacketLength(int maxPacketLength)
        {
            _maxPacketSize = maxPacketLength;
        }
        private int _maxPacketSize = 0;

        /// <summary>
        /// �w�肳�ꂽTcpClient�I�u�W�F�N�g��TcpClient�}�b�v�ɒǉ����A�ڑ��n���h���𐶐����ĕԂ��܂��B
        /// </summary>
        /// <param name="client">�ڑ����m������Ă���TcpClient�I�u�W�F�N�g</param>
        /// <returns>�ڑ��n���h��</returns>
        private long AddClient(TcpClient client)
        {

            long handle;
            /*
			if ( _removedHandleList.Count > 0 )
            {
                handle = _removedHandleList[0];
                _removedHandleList.RemoveAt(0);
            }
            else
            {
				handle = _clientInfoMap.Count + INITIAL_HANDLE_VALUE;
            }
            if ( _clientInfoMap.ContainsKey(handle) )
            {
                //  ���̃N���X�̎���������Ă��Ȃ���΂��̗�O���������邱�Ƃ͖���
                throw new ApplicationException("�g�p���̃n���h����Ԃ����Ƃ��܂����B");
            }
			 */
			handle = nextHandle++;

            _clientInfoMap[handle] = new ClientInfo(client);
            return handle;
        }

		long nextHandle = INITIAL_HANDLE_VALUE;

        /// <summary>
        /// �w�肳�ꂽ�ڑ��n���h���ɑΉ�����TcpClient�I�u�W�F�N�g��TcpClient�}�b�v����폜���܂��B
        /// TcpClient�I�u�W�F�N�g�͑O�����ĕ��Ă����K�v������܂��B
        /// </summary>
        /// <param name="handle">�ڑ��n���h��</param>
        private void RemoveClient(long handle)
        {
            CheckValidHandle(handle);
            _clientInfoMap.Remove(handle);
        //    _removedHandleList.Add(handle);
        }

        /// <summary>
        /// �w�肳�ꂽ�ڑ��n���h�����L���Ȃ��̂����`�F�b�N���A�L���łȂ��ꍇ�͗�O�𔭐������܂��B
        /// </summary>
        /// <param name="handle">�ڑ��n���h��</param>
        /// <exception cref="ArgumentException">�w�肳�ꂽ�n���h���͎g�p����Ă��܂���</exception>
        private void CheckValidHandle(long handle)
        {
            if ( !_clientInfoMap.ContainsKey(handle) )
            {
                throw new ArgumentException("�w�肳�ꂽ�n���h���͎g�p����Ă��܂���B");
            }
        }

        /// <summary>
        /// �w�肳�ꂽ�ڑ��n���h���ɑΉ�����TcpClient�I�u�W�F�N�g���擾���܂��B
        /// </summary>
        /// <param name="handle">�ڑ��n���h��</param>
        /// <returns>�ڑ��n���h���ɑΉ�����TcpClient�I�u�W�F�N�g</returns>
        private TcpClient GetTcpClient(long handle)
        {
            if ( !_clientInfoMap.ContainsKey(handle) )
            {
                return null;
            }
            TcpClient client = _clientInfoMap[handle].Connection;
            return client;
        }

        /// <summary>
        /// DeflateStream�N���X�ɂ���Ĉ��k���ꂽ�f�[�^���𓀂��܂��B
        /// </summary>
        /// <param name="compressedData">���k�f�[�^</param>
        /// <param name="originalData">�𓀂��ꂽ�f�[�^���������܂��o�b�t�@</param>
        private void Deflate(byte[] compressedData, byte[] originalData)
        {
            using ( MemoryStream ms = new MemoryStream(compressedData, 0, compressedData.Length, false) )
            using ( DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress) )
            {
                int i = 0;
                int remain = originalData.Length;
                while ( remain != 0 )
                {
                    int len = ds.Read(originalData, i, remain);
                    i += len;
                    remain -= len;
                    if ( len == 0 ) { break; }
                }
            }
        }

        /// <summary>
        /// NetworkID���b�Z�[�W�𑗐M���܂��B
        /// ���M�Ɏ��s�����ꍇ�͗�O���������܂��B
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="client"></param>
        private void SendNetworkIDMessage(long handle, TcpClient client)
        {
            InnerWrite(client, PacketType.NetworkIDNotify, MemoryCopy.ToByteArrayFromLong(_networkID.Value), false);
            InnerFlush(client);
        }


        private class ClientInfo
        {
            public TcpClient Connection;
            public bool IsReceiveNetworkID = false;
            public long ActivityCheckCounter = 0;
            public ClientInfo(TcpClient connection)
            {
                Connection = connection;
            }
        }

        /// <summary>
        /// TCP�ڑ���ҋ@���郊�X�i
        /// Listen���\�b�h�̌Ăяo���ŏ����������B
        /// </summary>
        private TcpListener _listener = null;
        /// <summary>
        /// handle�l��ClientInfo�I�u�W�F�N�g�̃}�b�v
        /// </summary>
        private Dictionary<long, ClientInfo> _clientInfoMap = new Dictionary<long, ClientInfo>();
        /// <summary>

		/*
		/// �j�����ꂽ�n���h���l�̃��X�g
        /// �ė��p�̂��߂Ɏg�p�����B
        /// </summary>
        private List<long> _removedHandleList = new List<long>();
		*/
    }
}
