
// Stop()�̎�����Release()��p����B
// Stop()��������Play()����ƁA�ǂ������ɂ���Ă̓o�O��悤�Ȃ̂Łc�B
#define RELEASE_ON_STOP

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using Sdl;
using OpenGl;
using Yanesdk.Ytl;
using Yanesdk.System;
using Yanesdk.Draw;

namespace Yanesdk.Movie
{
	/// <summary>
	/// SMPEG�ɂ��mpg�̍Đ����s���N���X�B
	/// </summary>
	/// <remarks>
	/// �ǂ����SMPEG�̐�����ASdlWindow�g�p�������g�����A
	/// ������SdlWindow�̃T�C�Y���傫�����[�r�[��ǂݍ��񂾏ꍇ�A
	/// �E���≺�����؂����Ă��܂����ۂ��B
	///		�� SDL.DLL�����C������
	///
	///	������Ȋ����Ŏg���B
	/// <code>
	///	// ������
	///	{
	///		Movie movie = new Movie();
	///		movie.Load("hoge.mpg");
	///
	///		GlTexture tx = new GlTexture();
	///		tx.SetSurface(movie.Surface);
	///		//tx.CreateSurface(640, 480, false);
	///		//tx.CreateSurface(movie.Width, movie.Height, false);
	///
	///		movie.IsLoop = true;		// �f�t�H���g��false
	///		movie.Play();
	/// }
	///
	///	// �`�惋�[�v
	/// {
	///		window.Screen.Select();
	///		if (movie.IsReadyToDraw) {
	///			movie.Draw(tx);
	///			window.Screen.Blt(tx, 0, 0);
	///			//window.Screen.Blt(tx, 0, 0, new Rect(0, 0, tx.Width, tx.Height), new Size(window.Width, window.Height));
	///		}
	///		window.Screen.Update();
	/// }
	/// </code>
	///
	/// ���邢�́A
	///
	/// <code>
	/// Movie movie = new Movie();
	/// movie.SetWindow(window);
	/// movie.Load("hoge.mpg");
	/// movie.IsLoop = true;		// �f�t�H���g��false
	/// movie.Play();
	/// // �ȍ~�A�����window�ɕ`�悳���B
	/// // �F�X�����Ȃǂ�����̂ŁASetWindow()�̃R�����g���Q�ƁB
	/// </code>
	/// </remarks>
	public class Movie : ILoader , IDisposable
	{
		#region SDLWindow�ɕ`�悷��T���v��
		/*
			FpsTimer timer = new FpsTimer();
			timer.Fps = 30;

			KeyBoardInput key = new KeyBoardInput();

			SDLWindow2DGl window = new SDLWindow2DGl();
			window.SetVideoMode(800, 600, 0);

			GlTexture tx = new GlTexture();
			Movie movie = new Movie();
			movie.Load("test.mpg");
			tx.SetSurface(movie.Surface);
			movie.IsLoop = true;
			movie.Play();

			while (SDLFrame.PollEvent() == Yanesdk.Ytl.YanesdkResult.NoError) {
				key.Update();

				if (key.IsPush(KeyCode.b)) {
					break;
				} else if (key.IsPush(KeyCode.p)) {
					movie.Play();
				} else if (key.IsPush(KeyCode.s)) {
					movie.Stop();
				} else if (key.IsPush(KeyCode.q)) {
					movie.Pause();
				} else if (key.IsPush(KeyCode.r)) {
					movie.Resume();
				}

				window.Screen.Select();
				if (movie.IsReadyToDraw) {
					movie.Draw(tx);
					//window.Screen.Blt(tx, 0, 0);
					window.Screen.Blt(tx, 0, 0,
						new Yanesdk.Draw.Rect(0, 0, tx.Width, tx.Height),
						new Yanesdk.Draw.Size(window.Width, window.Height));
				}
				window.Screen.Update();

				timer.WaitFrame();
			}
		 */
		#endregion

		#region Win32Window�ɕ`�悷��T���v���\�[�X
		/*
	public partial class Form7 : Form
	{
		public Form7()
		{
			InitializeComponent();

			init();
		}

		Movie movie = new Movie();

		// �{�^���n���h���~4�� Form�f�U�C�i�œK���Ƀ{�^����z�u���Ďg���Ă��傤�����B
		private void button1_Click(object sender, EventArgs e)
		{
			movie.Play();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			movie.Pause();
		}

		private void button3_Click(object sender, EventArgs e)
		{
			movie.Resume();
		}

		private void button4_Click(object sender, EventArgs e)
		{
			movie.Stop();
		}

		public void init()
		{
			timer.Fps = 30;

			window = new Win32Window(this.Handle);
			window.Screen.Select();

			tx = new GlTexture();
			movie.Load("test.mpg");
			tx.SetSurface(movie.Surface);
			movie.IsLoop = true;

			window.Screen.Unselect();
		//	movie.Play();
		}

		FpsTimer timer = new FpsTimer();
		Win32Window window;
		GlTexture tx;

		private void timer1_Tick(object sender, EventArgs e)
		{
			if (timer.ToBeRendered)
			{
				window.Screen.Select();
				if (movie.IsReadyToDraw)
				{
					movie.Draw(tx);
					if (rot == 0)
					{
						window.Screen.Blt(tx, 0, 0 , null,null);
					//	window.Screen.Blt(tx, 0, 0,
					//		new Yanesdk.Draw.Rect(0, 0, tx.Width, tx.Height),
					//		new Yanesdk.Draw.Size(this.Width, this.Height));
					}
					else
					{
						// �����Ƀ��[�r�[���܂킵�Ă݂�B
						window.Screen.BltRotate(tx, 0, 0, rot, 1.0f, 200, 200);
					}
				}
				window.Screen.Update();

				rot += rotateStep;

				timer.WaitFrame();
			}
		}

		int rot = 0;
		int rotateStep = 0;
		private void button5_Click(object sender, EventArgs e)
		{
			rotateStep++;
		}
	}
		 */
		#endregion

		#region ctor and Dispose
		/// <summary>
		/// �I�[�f�B�I�̍Đ����g�����́AYanesdk.Sound.Sound.SoundConfig�Őݒ肳�ꂽ�l�ŏ����������B
		/// </summary>
		public Movie()
		{
			callback = new SMPEG.SMPEG_DisplayCallback(smpeg_DisplayCallback);
			//mix_hook = new SDL.mix_func_t(SMPEG.SMPEG_playAudioSDL);
			mix_hook = new SDL.mix_func_t(mix_HookMusic);

			// �r�������I�u�W�F�N�g�̍쐬
			mutex = SDL.SDL_CreateMutex();

			// MasterVolume���ύX���ꂽ�Ƃ��ɂ���ɒǐ����čĐ�Volume���ύX�����悤�ɁB
			Yanesdk.Sound.Sound.ChunkManager.OnMasterVolumeChanged
				+= OnMasterVolumeChanged;
		}

		/// <summary>
		///  Dispose���Ăяo�������Ƃ͍Đ��ł��Ȃ��B
		/// �P�ɉ�������������Ȃ��Release�ōs�Ȃ��ׂ��B
		/// </summary>
		public void Dispose()
		{
			Release();

			// MasterVolume���ύX���ꂽ�Ƃ��ɂ���ɒǐ����čĐ�Volume���ύX�����悤�ɂ��Ă������̂�߂��B
			Yanesdk.Sound.Sound.ChunkManager.OnMasterVolumeChanged
				-= OnMasterVolumeChanged;

			initTimer.Dispose();
			initAudio.Dispose();
			initVideo.Dispose();

			SDL.SDL_DestroyMutex(mutex);
		}

		/// <summary>
		/// SDL Video��Audio�̏������p�N���X�B
		/// </summary>
		private RefSingleton<Yanesdk.Sdl.SDL_Video_Initializer>
			initVideo = new RefSingleton<Yanesdk.Sdl.SDL_Video_Initializer>();
		private RefSingleton<Yanesdk.Sdl.SDL_AUDIO_Initializer>
			initAudio = new RefSingleton<Yanesdk.Sdl.SDL_AUDIO_Initializer>();
		private RefSingleton<Yanesdk.Sdl.SDL_TIMER_Initializer>
			initTimer = new RefSingleton<Yanesdk.Sdl.SDL_TIMER_Initializer>();

		/// <summary>
		/// �}�X�^�[volume���ύX�ɂȂ����Ƃ��̃n���h��
		/// </summary>
		private void OnMasterVolumeChanged()
		{
			// �{�����[���̍Đݒ���s�Ȃ��B
			// setter�ł�master volume�ɑ΂��Ċ|���Z����Ă��邱�Ƃ𗘗p����B
			Volume = Volume;
		}
		#endregion

		#region ILoader�̎���
		/// <summary>
		/// mpeg�t�@�C����ǂݍ���
		/// </summary>
		/// <param name="filename">�t�@�C����</param>
		public YanesdkResult Load(string filename) {
			lock (lockObject)
			{
				Release();
				tmpFile = FileSys.GetTmpFile(filename);
				if (tmpFile == null || tmpFile.FileName == null)
				{
					return YanesdkResult.FileNotFound;
				}
				// �ǂݍ��݁B
				//mpeg = SMPEG.SMPEG_new(tmpFile.FileName, ref info, SDL.SDL_WasInit(SDL.SDL_INIT_AUDIO) == 0 ? 1 : 0);
				mpeg = SMPEG.SMPEG_new(tmpFile.FileName, ref info, 0);
				if (SMPEG.SMPEG_error(mpeg) != null)
				{
					Debug.Fail(SMPEG.SMPEG_error(mpeg));
					return YanesdkResult.SdlError; // �cSDL�H
				}

				// �����ݒ�
			//	loop = false;
				// ���[�v�ݒ�͂����ł͕ύX���Ȃ��ق����ǂ��B

				paused = false;
				
				SMPEG.SMPEG_enablevideo(mpeg, 1);
				SMPEG.SMPEG_enableaudio(mpeg, 0);
				if (info.has_audio != 0)
				{
					SDL.SDL_AudioSpec audiofmt = new SDL.SDL_AudioSpec();
					audiofmt.format = (ushort)Sound.Sound.SoundConfig.AudioFormat;
					audiofmt.freq = (ushort)Sound.Sound.SoundConfig.AudioRate;
					audiofmt.channels = (byte)Sound.Sound.SoundConfig.AudioChannels;
					audiofmt.samples = (ushort)Sound.Sound.SoundConfig.AudioBuffers;
					SMPEG.SMPEG_actualSpec(mpeg, ref audiofmt);
					SDL.Mix_HookMusic(mix_hook, mpeg);
					SMPEG.SMPEG_enableaudio(mpeg, 1);
				}
				SMPEG.SMPEG_enableaudio(mpeg, 1);
				Volume = volume;

				// �`���T�[�t�F�X��smpeg�ւ̓o�^�ȂǁB
				if (window != null)
				{
					// SDLWindow�ɒ��ɕ`�悷��

					if (surface != null)
					{
						surface.Dispose();
						surface = null;
					}

					uint flags = (uint)window.Option;
					flags &= ~SDL.SDL_OPENGL; // OPENGL�̓_���B
					IntPtr s = SDL.SDL_SetVideoMode(window.Width, window.Height, window.Bpp, flags);
					if (s == IntPtr.Zero)
						return YanesdkResult.SdlError;

					SMPEG.SMPEG_setdisplay(mpeg, s, IntPtr.Zero, null);
					SMPEG.SMPEG_move(mpeg, (int)windowRect.Left, (int)windowRect.Top);
					SMPEG.SMPEG_scaleXY(mpeg, (int)windowRect.Width, (int)windowRect.Height);
				}
				else if (
					Yanesdk.System.Platform.PlatformID == Yanesdk.System.PlatformID.Windows
					&& hwnd != IntPtr.Zero)
				{
					// SDLWindow��\��t���Ă���ɕ`�悷��

					if (surface != null)
					{
						surface.Dispose();
						surface = null;
					}

					// �e�E�B���h�E�̈ʒu�E�T�C�Y���擾
					POINT parentPos = new POINT();
					ClientToScreen(hwnd, ref parentPos);
					RECT parentRect = new RECT();
					GetClientRect(hwnd, out parentRect);
					int w = parentRect.right - parentRect.left, h = parentRect.bottom - parentRect.top;

					// SDL�E�B���h�E�̈ʒu�����킹��
					IntPtr sdlHwnd = GetSDLWindowHandle();
					//SetParent(sdlHwnd, hwnd); // ���ꂷ��Ƃǂ����o�O��c
					SetWindowPos(sdlHwnd, IntPtr.Zero, parentPos.x, parentPos.y, w, h, SWP_NOZORDER);

					// SDL�E�B���h�E�̍쐬
					uint flags = SDL.SDL_HWSURFACE | SDL.SDL_NOFRAME;
					IntPtr s = SDL.SDL_SetVideoMode(w, h, 0, flags);
					if (s == IntPtr.Zero)
						return YanesdkResult.SdlError;

					sdlWindowCreated = true;

					sdlHwnd = GetSDLWindowHandle(); // �ς��Ȃ��Ǝv�����A�ꉞ�Ď擾
					if (sdlHwnd == IntPtr.Zero)
						return YanesdkResult.SdlError;
					SetParent(sdlHwnd, hwnd);
					SetWindowPos(sdlHwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOZORDER | SWP_NOSIZE);

					SMPEG.SMPEG_setdisplay(mpeg, s, IntPtr.Zero, null);
					SMPEG.SMPEG_move(mpeg, 0, 0);
					SMPEG.SMPEG_scaleXY(mpeg, w, h);
				}
				else
				{
					// surface�ɕ`�悷��

					if (surface != null && (!surface.CheckRGB888() ||
						surface.Width != Width || surface.Height != Height))
					{
						surface.Dispose();
						surface = null;
					}
					if (surface == null)
					{
						surface = new Surface();
						// �g��E�k����SMPEG���ɂ�点��Əd���̂ŁA������ŁB
						YanesdkResult result = surface.CreateDIB(Width, Height, false);
						if (result != YanesdkResult.NoError)
						{
							return result;
						}
					}

					SMPEG.SMPEG_setdisplay(mpeg, surface.SDL_Surface, mutex, callback);
					SMPEG.SMPEG_move(mpeg, 0, 0);
					SMPEG.SMPEG_scaleXY(mpeg, surface.Width, surface.Height);
					//SetFilter(SMPEG.SMPEGfilter_null());
				}

				this.fileName = filename;

				// �����܂��B
				return YanesdkResult.NoError;
			}
		}

		/// <summary>
		/// �t�@�C����ǂݍ���ł���ꍇ�A�ǂݍ���ł���t�@�C������Ԃ�
		/// </summary>
		public string FileName
		{
			get { return fileName; }
		}
		private string fileName = null;

		/// <summary>
		/// SDL.Mix_HookMusic()�ɓo�^����֐�
		/// </summary>
		private void mix_HookMusic(/* void * */IntPtr udata, /* Uint8 * */IntPtr stream, int len)
		{
			// lock (lockObject) // ����̓f�b�h���b�N���邩���B�B

			if (mpeg != IntPtr.Zero && SMPEG.SMPEG_status(mpeg) == SMPEG.SMPEGstatus.SMPEG_PLAYING)
			{
				SMPEG.SMPEG_playAudio(mpeg, stream, len);
			}
		}

		/// <summary>
		/// �ǂݍ���ł�������̉��
		/// </summary>
		public void Release()
		{
			lock (lockObject)
			{
				fileName = null;

				if (Loaded)
				{
					SDL.Mix_HookMusic(null, IntPtr.Zero);

					Stop();
					SMPEG.SMPEG_delete(mpeg);
					mpeg = IntPtr.Zero;

					if (Yanesdk.System.Platform.PlatformID == Yanesdk.System.PlatformID.Windows
						&& sdlWindowCreated)
					{
						IntPtr sdlHwnd = GetSDLWindowHandle();
						SetParent(sdlHwnd, IntPtr.Zero);
						ShowWindow(sdlHwnd, SW_HIDE);
					}

				}
				if (tmpFile != null)
				{
					tmpFile.Dispose();
					tmpFile = null;
				}
				if (surface != null)
				{
					surface.Dispose();
					surface = null;
				}
			}
		}

		/// <summary>
		/// �t�@�C���̓ǂݍ��݂��������Ă��邩��Ԃ�
		/// </summary>
		public bool Loaded
		{
			get { return mpeg != IntPtr.Zero; }
		}
		
		#endregion

		#region methods

		/// <summary>
		/// �`���E�B���h�E
		/// </summary>
		private SDLWindow2DGl window = null;
		/// <summary>
		/// �E�B���h�E�̕`�����W
		/// </summary>
		private Rect windowRect;

		/// <summary>
		/// �`���E�B���h�E���Z�b�g�BLoad()����O�ɌĂяo�����ƁB
		/// </summary>
		/// <remarks>
		/// �g���k���͒x�����M�U�M�U�ɂȂ��Ă��܂��̂ŁA
		/// ������g���Ȃ瓙�{�݂̂Ƃ��������悢�B
		/// �܂��A�I�������ASDLWindow2DGl.TestVideoMode()����蒼���āA
		/// �쐬�ς݂̃e�N�X�`��������ΑS�č�蒼���K�v������B
		/// </remarks>
		public void SetWindow(SDLWindow2DGl window)
		{
			SetWindow(window, null);
		}

		/// <summary>
		/// �`���E�B���h�E���Z�b�g�BLoad()����O�ɌĂяo�����ƁB
		/// </summary>
		/// <remarks>
		/// <remarks>
		/// DirectX��overlay�ŕ`�悳��邽�߁AGlTextureSubImage��
		/// �x���}�V���ł͂��Ȃ���ʂ����҂ł���B
		/// 
		/// �����}�V���ł͊֌W�Ȃ�(�L��`)
		/// 
		/// ���������̃E�B���h�E���d�˂邱�ƂɂȂ�̂�
		/// ���Ƃ���E�B���h�E�ɍڂ��Ă���R���g���[����
		/// �ނ�overlay���Ă���Ƃ��ɂ͕\������Ȃ��Ȃ�̂Œ��ӂ��K�v�B
		/// 
		/// �g���k���͒x�����M�U�M�U�ɂȂ��Ă��܂��̂ŁA
		/// ������g���Ȃ瓙�{�݂̂Ƃ��������悢�B
		/// �܂��A�I�������ASDLWindow2DGl.TestVideoMode()����蒼���āA
		/// �쐬�ς݂̃e�N�X�`��������ΑS�č�蒼���K�v������B
		/// </remarks>
		public void SetWindow(SDLWindow2DGl w, Rect rc)
		{
			lock (lockObject)
			{
				if (Loaded)
				{
					Debug.Fail("Load()�̌��Movie.SetWindow()���Ăяo���ꂽ");
					return;
				}
				window = w;
				windowRect = rc != null ? rc : new Rect(0, 0, w.Width, w.Height);
			}
		}

		/// <summary>
		/// SDL�E�B���h�E���d�˂�E�B���h�E
		/// </summary>
		private IntPtr hwnd = IntPtr.Zero;

		/// <summary>
		/// SDL�E�B���h�E�����̃N���X���쐬�����̂��ǂ����B
		/// </summary>
		private bool sdlWindowCreated = false;

		/// <summary>
		/// ������SDL�E�B���h�E���쐬���A�w�肳�ꂽ�E�B���h�E�ɏd�˂鎖�ŁA
		/// SetWindow()�Ɠ��l�̋������AWindows��Form�Ȃǂɑ΂��Ď����o����B
		/// Load()����O�ɌĂяo�����ƁB
		/// </summary>
		/// <remarks>
		/// DirectX��overlay�ŕ`�悳��邽�߁AGlTextureSubImage��
		/// �x���}�V���ł͂��Ȃ���ʂ����҂ł���B
		/// 
		/// �����}�V���ł͊֌W�Ȃ�(�L��`)
		/// 
		/// ���������̃E�B���h�E���d�˂邱�ƂɂȂ�̂�
		/// ���Ƃ���E�B���h�E�ɍڂ��Ă���R���g���[����
		/// �ނ�overlay���Ă���Ƃ��ɂ͕\������Ȃ��Ȃ�̂Œ��ӂ��K�v�B
		/// 
		/// Windows��p�̋@�\
		/// </remarks>
		/// <param name="hwnd">�E�B���h�E�n���h��</param>
		/// <seealso cref="SetWindow()"/>
		public void Set_SDL_Overlay(IntPtr hwnd)
		{
			// Windows��p�̋@�\
			if (Yanesdk.System.Platform.PlatformID != Yanesdk.System.PlatformID.Windows)
				throw new YanesdkException(YanesdkResult.NotImplemented);
			
			this.hwnd = hwnd;
		}

		#region hwnd����p��WinAPI�Ȃ�

		[StructLayout(LayoutKind.Sequential)]
		struct RECT
		{
			public int left, top, right, bottom;
		}

		[StructLayout(LayoutKind.Sequential)]
		struct POINT
		{
			public int x, y;
		}

		[DllImport("user32.dll")]
		static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

		[DllImport("user32.dll")]
		static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

		[DllImport("user32.dll")]
		static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

		[DllImport("user32.dll")]
		static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

		readonly IntPtr HWND_TOP = new IntPtr(0);
		readonly IntPtr HWND_BOTTOM = new IntPtr(1);
		readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
		readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
		const uint SWP_NOSIZE = 0x0001;
		const uint SWP_NOMOVE = 0x0002;
		const uint SWP_NOZORDER = 0x0004;
		const uint SWP_NOREDRAW = 0x0008;
		const uint SWP_NOACTIVATE = 0x0010;
		const uint SWP_SHOWWINDOW = 0x0040;
		const uint SWP_HIDEWINDOW = 0x0080;
		const uint SWP_NOSENDCHANGING = 0x0400;

		[DllImport("user32.dll")]
		static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

		const int SW_HIDE = 0;
		const int SW_SHOWNORMAL = 1;
		const int SW_NORMAL = 1;
		const int SW_SHOWMINIMIZED = 2;
		const int SW_SHOWMAXIMIZED = 3;
		const int SW_MAXIMIZE = 3;
		const int SW_SHOWNOACTIVATE = 4;
		const int SW_SHOW = 5;
		const int SW_MINIMIZE = 6;
		const int SW_SHOWMINNOACTIVE = 7;
		const int SW_SHOWNA = 8;
		const int SW_RESTORE = 9;
		const int SW_SHOWDEFAULT = 10;
		const int SW_FORCEMINIMIZE = 11;
		const int SW_MAX = 11;

		[DllImport("user32.dll")]
		static extern int ShowWindow(IntPtr hWnd, int nCmdShow);

		const int WM_CLOSE = 0x0010;
		const int WM_QUIT = 0x0012;

		[DllImport("user32.dll")]
		static extern bool PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

		[StructLayout(LayoutKind.Sequential)]
		struct MSG
		{
			public IntPtr hwnd;
			public int message;
			public IntPtr wParam;
			public IntPtr lParam;
			public uint time;
			public POINT pt;
		}

		const uint PM_NOREMOVE = 0;
		const uint PM_REMOVE = 1;

		[DllImport("user32.dll")]
		static extern bool PeekMessage(ref MSG lpMsg,
			IntPtr hwnd, int wMsgFilterMin, int wMsgFilterMax, uint wRemoveMsg);

		[DllImport("user32.dll")]
		static extern bool TranslateMessage(ref MSG lpMsg);

		[DllImport("user32.dll")]
		static extern int DispatchMessage(ref MSG lpMsg);

		#endregion

		/// <summary>
		/// �e�N�X�`���ɕ`��B
		/// �X�V����Ă��Ȃ��ꍇ�������Ȃ��̂ŁA��p�̃e�N�X�`����p�ӂ��Ă����āA
		/// ���񂻂���w�肷�邱�ƁB
		/// </summary>
		public void Draw(GlTexture tx)
		{
			lock (lockObject)
			{
				Debug.Assert(IsReadyToDraw);
				// ������͌Ăяo�����Ń`�F�b�N���Ƃ��āA
				// false�Ȃ����͕`�悵�Ȃ����������B(�L��`)

				if (window != null)
				{
					Debug.Fail("Movie.SetWindow()���ꂽ�̂�Movie.Draw()���Ă΂ꂽ");
					return;
				}

				// lock
				int p = SDL.SDL_mutexP(mutex); Debug.Assert(p == 0);

				// �t���O����
				//*
				if (!updateRect.HasValue)
				{
					// unlock
					int vv = SDL.SDL_mutexV(mutex); Debug.Assert(vv == 0);
					return;
				}
				//Rectangle clientRect = updateRect.Value; // �������Ɏg�������ȗ\����������̂́c�B
				updateRect = null;
				/*/
				if (!updated) {
					// unlock
					int vv = SDL.SDL_mutexV(mutex); Debug.Assert(vv == 0);
					return;
				}
				updated = false;
				//*/

				// �e�N�X�`���ɕ`��
				tx.Bind();
				Gl.glTexSubImage2D((uint)tx.TextureType, 0,
					0, 0, surface.Width, surface.Height,
					surface.Alpha ? Gl.GL_RGBA : Gl.GL_RGB,
					Gl.GL_UNSIGNED_BYTE, surface.Pixels);
				tx.Unbind();

				// unlock
				int v = SDL.SDL_mutexV(mutex); Debug.Assert(v == 0);
			}
		}
		//private volatile bool updated = false;
		//private volatile Rectangle? updateRect;
		private Rectangle? updateRect;

		/// <summary>
		/// �Đ��J�n
		/// </summary>
		public YanesdkResult Play()
		{
			lock (lockObject)
			{
#if RELEASE_ON_STOP
				if (IsPlaying)
				{
					if (!paused)
					{
						InnerStop();
					}
					paused = false;
					SMPEG.SMPEG_rewind(mpeg); // �����߂�
				} 
				else
				{
					if (!Loaded && fileName != null)
					{
						YanesdkResult result = Load(fileName);
						if (result != YanesdkResult.NoError)
						{
							return result;
						}
					}
				}
#else
				Stop();
#endif
				readyToDraw = false; // �`�悵�Ȃ������܂ł͕`�揀����ԂƂ���
				InnerPlay();

				return YanesdkResult.NoError;
			}
		}

		/*

		// Window�ɒ��ڕ`�悷��Ƃ�
		// SetFilter(SMPEG.SMPEGfilter_bilinear());
		// �Ƃ�����΃X���[�Y�Ɋg��E�k�������̂��Ǝv������A�����ł��Ȃ��B
		// �����ȈႢ�͂���̂����A�g��Ƃ�����O�̒i�K�ł̏����̂悤�ŁA
		// ����ׂĂ݂Ȃ��ƕ�����Ȃ����x�B
		// ����āA�Ƃ肠�����f�t�H���g�̂܂܂ɂ��Ă����B

		private void SetFilter(IntPtr filter)
		{
			IntPtr f = SMPEG.SMPEG_filter(mpeg, filter);
			// �Â��t�B���^�̌�n��
			SMPEG.SMPEG_Filter fs = (SMPEG.SMPEG_Filter)Marshal.PtrToStructure(f, typeof(SMPEG.SMPEG_Filter));
			fs.destroy(f);
		}

		//*/

		/// <summary>
		///	play���̃��[�r�[���~������
		/// </summary>
		public void Stop()
		{
			lock (lockObject)
			{
#if RELEASE_ON_STOP
				if (fileName != null) {
					// Release()�ň�[filename��null�ɂȂ�̂ŁA�I����Ă���Đݒ肷��B(�L��`)
					string tmp = fileName;
					Release();
					fileName = tmp;
				}
#else
				Debug.Assert(Loaded);
				if (IsPlaying)
				{
					if (!paused)
					{
						InnerStop();
					}
					paused = false;
					SMPEG.SMPEG_rewind(mpeg); // �����߂�
				}
#endif
			}
		}

		/// <summary>
		/// �ꎞ��~
		/// </summary>
		public void Pause()
		{
			lock (lockObject)
			{
				if (IsPlaying && !paused)
				{
					// SMPEG_pause()��stop���Ƀo�O�肪���Ȃ̂Ŏg���̂�߂Ƃ��B
					InnerStop();
					paused = true;
				}
			}
		}

		/// <summary>
		/// pause������\�����\�b�h
		/// </summary>
		private bool paused = false;

		/// <summary>
		/// �ĊJ
		/// </summary>
		public void Resume()
		{
			lock (lockObject)
			{
				if (IsPlaying && paused)
				{
					//SMPEG.SMPEG_pause(mpeg); // �Ăяo�����ɐ؂�ւ��炵��
					InnerPlay();
					paused = false;
				}
			}
		}

		/// <summary>
		/// �Đ��J�n�ƃ��[�v�ݒ�̔��f
		/// </summary>
		private void InnerPlay()
		{
			SMPEG.SMPEG_play(mpeg);
			// ���[�v�ݒ�𔽉f������
			if (loop) SMPEG.SMPEG_loop(mpeg, loop ? 1 : 0);
		}

		/// <summary>
		/// �Đ���~
		/// </summary>
		private void InnerStop()
		{
			SMPEG.SMPEG_loop(mpeg, 0); // ���[�v���Ă�Ǝ~�܂�Ȃ��炵��
			SMPEG.SMPEG_stop(mpeg);
		}

		/// <summary>
		/// �`�掞�ɌĂяo�����R�[���o�b�N�֐��B
		/// �ʃX���b�h�炵���̂ŁAGL����̏����Ƃ��͎~�߂Ƃ��̂�������ۂ��B
		/// </summary>
		private void smpeg_DisplayCallback(/*SDL.SDL_Surface*/IntPtr dst, int x, int y, uint w, uint h)
		{
			Debug.Assert(dst == surface.SDL_Surface);

			//lock (lockObject) // ����̓f�b�h���b�N���邩���B�B

			//lock (callback) // �� SMPEG����mutex��lock����Ă�̂ł���͗v��Ȃ�

			//*
			Rectangle rc = new Rectangle(x, y, (int)w, (int)h);
			updateRect = updateRect.HasValue ? Rectangle.Union(updateRect.Value, rc) : rc;
			/*/
			updated = true;
			//*/

			// readyToDraw = true; // ��x�ł��`�悳�ꂽ��`�揀�������ƌ��Ȃ��B
			if (!readyToDraw)
			{
				// �S�ʂ��X�V���ꂽ��`�揀������
				Rectangle r = updateRect.Value;
				if (//r.Left <= 0 && r.Right <= 0 &&
					Width <= r.Width && Height <= r.Height)
				{
					readyToDraw = true;
				}
			}
		}

		/// <summary>
		/// SDL�ȃE�B���h�E��HWND���擾����B
		/// </summary>
		private IntPtr GetSDLWindowHandle()
		{
			unsafe
			{
				SDL.SDL_SysWMinfo* wminfo = stackalloc SDL.SDL_SysWMinfo[1];
				SDL.SDL_VERSION(ref wminfo->version);
				wminfo->window = IntPtr.Zero;
				SDL.SDL_GetWMInfo(new IntPtr(wminfo));
				return wminfo->window;
			}
		}

		#region �{�c���\�b�h

		/*
			����͂ǂ����x�����ۂ��B(�L��`)

		/// <summary>
		/// �E�B���h�E�֕`��
		/// </summary>
		public void Draw(SDLWindow2DGl window, int x, int y) {
			lock (lockObject)
			{
				SDL.SDL_Rect src = new SDL.SDL_Rect(), dst = new SDL.SDL_Rect();
				src.x = 0;
				src.y = 0;
				src.w = (ushort)surface.Width;
				src.h = (ushort)surface.Height;
				dst.x = (short)x;
				dst.y = (short)y;
				dst.w = (ushort)surface.Width;
				dst.h = (ushort)surface.Width;
				SDL.SDL_BlitSurface(surface.SDL_Surface, ref src, window.SDL_Surface, ref dst);
				SDL.SDL_UpdateRect(window.SDL_Surface, 0, 0, 0, 0);
			}
		}

		//*/

		#endregion

		#endregion

		#region prpoerties
		/// <summary>
		/// ���[�v����̂��ǂ����A�擾�E�ݒ�B
		/// 
		/// ��x�ύX����ƁALoad���Ă��ύX�͂���Ȃ��B
		/// �ēx�ݒ肳���܂ŗL���B������Ԃł�false�B
		/// </summary>
		public bool IsLoop
		{
			get
			{
				return loop;
			}
			set
			{
				lock (lockObject)
				{
					loop = value;
					if (IsPlaying) // �Đ�������Ȃ��Ƃ��܂����f����Ȃ��炵��
					{
						// ���ݓǂݍ��ݒ��̓��悪����΃��[�v���[�h��ݒ�
						SMPEG.SMPEG_loop(mpeg, loop ? 1 : 0);
					}
				}
			}
		}

		/// <summary>
		/// ���[�v���[�h�Ȃ̂��ǂ����B
		/// </summary>
		private bool loop = false;

		/// <summary>
		/// �ǂݍ��񂾃��\�[�X�̃T�C�Y���擾���郁�\�b�h
		/// </summary>
		public long ResourceSize
		{
			get
			{
				if (!Loaded) return 0;
				return info.total_size;
			}
		}

		/// <summary>
		/// �����̎擾
		/// </summary>
		public int Width
		{
			get { return info.width; }
		}

		/// <summary>
		/// �c���̎擾
		/// </summary>
		public int Height
		{
			get { return info.height; }
		}

		/// <summary>
		/// ���݂̓���T�[�t�F�X�̎擾
		/// </summary>
		public Surface Surface
		{
			get { return surface; }
		}
		private Surface surface = null;

		/// <summary>
		/// �Đ����Ȃ̂��ǂ����B
		/// Pause����true��Ԃ��B
		/// ���[�v���Ȃ��ꍇ�A���ꂪfalse�ɂȂ�����I������Ǝv����OK�B
		/// </summary>
		/// <seealso cref="IsReadyToDraw"/>
		public bool IsPlaying
		{
			get
			{
				lock (lockObject)
				{
					return Loaded && (paused || SMPEG.SMPEG_status(mpeg) == SMPEG.SMPEGstatus.SMPEG_PLAYING);
				}
			}
		}

		/// <summary>
		/// �`��\�ȏ�ԂȂ�true��Ԃ��B
		/// �ʏ�A���ꂪtrue�������ꍇ�̂݃e�N�X�`����Draw()���āA�����Screen��Blt����΂悢�B
		/// </summary>
		/// <remarks>
		/// ��{�I�ɂ� IsPlaying �Ɠ����Ȃ̂����A
		/// SMPEG���ɁAStop()���Ă���Play()����ƁA��uStop()�����Ƃ��̉摜��
		/// �\������Ă��܂��o�O������̂ŁA
		/// Play()���Ă΂�Ă���surface�ɂ����ƕ`�悳���܂ł�false��Ԃ��悤�ɂȂ��Ă���B
		/// </remarks>
		public bool IsReadyToDraw
		{
			get
			{
				return IsPlaying && readyToDraw;
			}
		}

		private volatile bool readyToDraw = true;

		/// <summary>
		/// �}�X�^�[volume�̐ݒ�
		/// ���ׂĂ�Sound�N���X�ɉe������B
		///		�o��volume = (master volume�̒l)�~(volume�̒l)
		/// �ł���B
		/// </summary>
		/// <param name="volume"></param>
		public static float MasterVolume
		{
			get { return Yanesdk.Sound.Sound.ChunkManager.MasterVolume; }
			set { Yanesdk.Sound.Sound.ChunkManager.MasterVolume = value; }
		}

		/// <summary>
		/// volume�̐ݒ�B0.0�`1.0�܂ł̊ԂŁB
		/// 1.0�Ȃ�100%�̈Ӗ��B
		///
		/// master volume�̂ق����ύX�ł���B
		///		�o��volume = (master volume�̒l)�~(�����Őݒ肳�ꂽvolume�̒l)
		/// �ł���B
		///
		/// �����Őݒ肳�ꂽ�l��Load/Play���ɂ���Ă͕ω����Ȃ��B
		/// �Đݒ肳���܂ŗL���ł���B
		/// </summary>
		/// <param name="volume"></param>
		/// <returns></returns>
		public float Volume
		{
			get { return volume; }
			set
			{
				lock (lockObject)
				{
					volume = value;
					if (Loaded)
					{
						SMPEG.SMPEG_setvolume(mpeg, (int)(100 * MasterVolume * volume));
					}
				}
			}
		}
		private float volume = 1.0f;

		#endregion

		#region private

		/// <summary>
		/// movie�̂��߂̃e���|�����t�@�C��
		/// </summary>
		private FileSys.TmpFile tmpFile = null;

		/// <summary>
		/// movie�t�@�C���ւ�handle(smpeg�����Ŏg���Ă���)
		/// </summary>
		private IntPtr mpeg = IntPtr.Zero;

		/// <summary>
		/// �ǂݍ��܂ꂽmovie���
		/// </summary>
		private SMPEG.SMPEG_Info info = new SMPEG.SMPEG_Info();

		/// <summary>
		/// surface�ɏ������݂��s��mpeg�f�R�[�h�X���b�h�ƁA
		/// Draw()�Ƃ̔r���������s��SDL_mutex�B
		/// </summary>
		private IntPtr mutex;

		/// <summary>
		/// SMPEG����`�掞�ɌĂяo�����R�[���o�b�N�B
		/// GC����Ȃ��悤�Ƀ����o�Ɏ����Ă��܂��B
		/// </summary>
		private SMPEG.SMPEG_DisplayCallback callback;

		/// <summary>
		/// SDL.Mix_HookMusic()�ɓo�^����delegate(���g��SMPEG.SMPEG_playAudioSDL)
		/// callback�Ɠ��l�AGC����Ȃ��悤�Ƀ����o�ɁB
		/// </summary>
		private SDL.mix_func_t mix_hook;

		/// <summary>
		/// lock�p�_�~�[�I�u�W�F�N�g�B
		/// SMPEG�͕����̃X���b�h����g���悤�ɂ͍���Ă��Ȃ��Ǝv����̂ŁA
		/// �Ăяo�����ŕЂ��[����lock���Ă����B
		/// </summary>
		private object lockObject = new object();

		#endregion
	}
}
