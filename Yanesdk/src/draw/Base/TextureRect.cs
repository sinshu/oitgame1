using System;
using System.Collections.Generic;
using System.Text;
using Yanesdk.Ytl;

namespace Yanesdk.Draw
{
	/// <summary>
	/// �e�N�X�`���̓����̋�`��\������e�N�X�`���B
	/// 
	/// ��)
	///    TextureRect tr = new TextureRect();
	///    tr.SetTexture(texture,new Rect(100,200,150,250));
	/// 
	///    screen.Blt(tr,20,30);
	///    // screen.Blt(texture,20,30,new Rect(100,200,150,250));�Ɠ��`
	/// 
	/// �X�v���C�g��\������̂Ɏg���Ɨǂ��B
	/// </summary>
	public class TextureRect : ITexture
	{
		#region ctor
		/// <summary>
		/// �̂��ق�SetTexture���s�Ȃ��ׂ��B
		/// </summary>
		public TextureRect() { }

		/// <summary>
		/// SetTexture�����˂�R���X�g���N�^�B
		/// </summary>
		public TextureRect(ITexture texture)
		{
			SetTexture(texture);
		}

		/// <summary>
		/// SetTexture�����˂�R���X�g���N�^�B
		/// </summary>
		public TextureRect(ITexture texture, Rect rect)
		{
			SetTexture(texture, rect);
		}

		/// <summary>
		/// SetTexture�����˂�R���X�g���N�^�B
		/// </summary>
		public TextureRect(ITexture texture, float offsetX, float offsetY)
		{
			SetTexture(texture, offsetX, offsetY);			
		}

		/// <summary>
		/// SetTexture�����˂�R���X�g���N�^�B
		/// </summary>
		public TextureRect(ITexture texture, Rect srcRect,float offsetX, float offsetY)
		{
			SetTexture(texture, rect,offsetX, offsetY);
		}

		#endregion

		#region ITexture�̎���
		/*
		/// <summary>
		/// ���̃e�N�X�`�����w���screen��blt����B
		/// </summary>
		/// <param name="drawContext"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <remarks>
		/// visitor�p�^�[���̂��߂Ɏ������Ă���B
		/// SDLWindow.blt����Visitor�Ƃ��ČĂяo�����B
		/// </remarks>
		public void Blt(DrawContext context, float x, float y)
		{
			texture.Blt(context, x +DstOffsetX, y + DstOffsetY, rect);
		}

		/// <summary>
		/// ���̃e�N�X�`�����w���screen��blt����B
		/// </summary>
		/// <param name="drawContext"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="srcRect"></param>
		/// <remarks>
		/// visitor�p�^�[���̂��߂Ɏ������Ă���B
		/// SDLWindow.blt����Visitor�Ƃ��ČĂяo�����B
		/// </remarks>
		public void Blt(DrawContext src, float x, float y, Rect srcRect)
		{
			// ����srcRect��rect�̂Ȃ��ł̍��W���Ӗ�����̂ō��W�ϊ����{���K�v������B
			Rect r = Rect.CalcRectInRect(rect, srcRect);

			texture.Blt(src, x + DstOffsetX, y + DstOffsetY, r);
		}
		*/

		/// <summary>
		/// ���̃e�N�X�`�����w���screen��blt����B
		/// </summary>
		/// <param name="drawContext"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="srcRect"></param>
		/// <param name="dstSize"></param>
		/// <remarks>
		/// visitor�p�^�[���̂��߂Ɏ������Ă���B
		/// SDLWindow.blt����Visitor�Ƃ��ČĂяo�����B
		/// </remarks>
		public void Blt(DrawContext src, float x, float y, Rect srcRect, Size dstSize)
		{
			// ����srcRect��rect�̂Ȃ��ł̍��W���Ӗ�����̂ō��W�ϊ����{���K�v������B
		//	Rect r = Rect.CalcRectInRect(rect, srcRect);
			Rect r = srcRect;

			texture.Blt(src, x + DstOffsetX, y + DstOffsetY, r, dstSize);
		}

		/// <summary>
		/// ���̃e�N�X�`�����w���screen��blt����B
		/// </summary>
		/// <param name="src"></param>
		/// <param name="srcRect"></param>
		/// <param name="dstPoint"></param>
		/// <remarks>
		/// visitor�p�^�[���̂��߂Ɏ������Ă���B
		/// SDLWindow.blt����Visitor�Ƃ��ČĂяo�����B
		/// </remarks>
		public void Blt(DrawContext src, Rect srcRect, Point[] dstPoint)
		{
			// ����srcRect��rect�̂Ȃ��ł̍��W���Ӗ�����̂ō��W�ϊ����{���K�v������B
		//	Rect r = Rect.CalcRectInRect(rect, srcRect);
			Rect r = srcRect;

			Point[] dstPoint_ = new Point[dstPoint.Length];
			for (int i = 0; i < dstPoint_.Length; ++i)
			{
				dstPoint_[i].X = dstPoint[i].X + DstOffsetX;
				dstPoint_[i].Y = dstPoint[i].Y + DstOffsetY;
			}

			texture.Blt(src, r, dstPoint_);
		}

		/// <summary>
		/// ���̃e�N�X�`�����w���screen��blt����B
		/// </summary>
		/// <param name="src"></param>
		/// <param name="srcPoint"></param>
		/// <param name="dstPoint"></param>
		/// <remarks>
		/// visitor�p�^�[���̂��߂Ɏ������Ă���B
		/// SDLWindow.blt����Visitor�Ƃ��ČĂяo�����B
		/// ����A Textures �ł͖������B(�ʓ|������)
		/// </remarks>
		public void Blt(DrawContext src, Point[] srcPoint, Point[] dstPoint)
		{
			if (srcPoint == null)
				Blt(src, (Rect)null, dstPoint);

			// ����srcPoint��rect�̂Ȃ��ł̍��W���Ӗ�����̂ō��W�ϊ����{���K�v������B

			// outRect��Width,Height���}�C�i�X�̒l�ł��邱�Ƃ��l�����āc
			int sw = global::System.Math.Sign(rect.Right-rect.Left);
			int sh = global::System.Math.Sign(rect.Bottom-rect.Top);

			Point[] srcPoint_ = new Point[srcPoint.Length];

			for(int i=0;i<srcPoint.Length;++i)
			{
				srcPoint_[i].X = rect.Left + sw * srcPoint[i].X;
				srcPoint_[i].Y = rect.Top + sh * srcPoint[i].Y;
			}

			Point[] dstPoint_ = new Point[dstPoint.Length];
			for (int i = 0; i < dstPoint_.Length; ++i)
			{
				dstPoint_[i].X = dstPoint[i].X + DstOffsetX;
				dstPoint_[i].Y = dstPoint[i].Y + DstOffsetY;
			}

			texture.Blt(src, srcPoint_, dstPoint_);
		}

		/// <summary>
		/// Bitmap��ݒ�ł���ƕ֗��B
		/// </summary>
		/// <param name="bmp"></param>
		/// <returns></returns>
		public YanesdkResult SetBitmap(global::System.Drawing.Bitmap bmp)
		{
			return YanesdkResult.NotImplemented;
		}

		/// <summary>
		/// Surface���ݒ�ł���ƕ֗��B
		/// </summary>
		/// <param name="surface"></param>
		/// <returns></returns>
		public YanesdkResult SetSurface(Surface surface)
		{
			return YanesdkResult.NotImplemented;
		}

		/// <summary>
		/// �����I�ɕێ����Ă���Surface������B
		/// 
		/// Dispose���Ăяo�����ꍇ�́A�ēxLoad���邱�Ƃ͏o���Ȃ��B
		/// ���ꂪRelease��Dispose�Ƃ̈Ⴂ�B
		/// </summary>
		public void Release()
		{
			texture.Release();
		}

		/// <summary>
		/// ��̂���B�ʏ�Ăяo���K�v�͂Ȃ��B
		/// </summary>
		public void Dispose()
		{
			texture.Dispose();
		}

		#endregion

		#region methods

		/// <summary>
		/// �e�N�X�`���ƁA���̂ǂ̕��������̃e�N�X�`���Ƃ��Ĉ����̂���ݒ肷��B
		/// 
		/// rect == null�ł����texture�S�́B
		/// OffsetX = OffsetY = 0�ɂ���B
		/// 
		/// �����Ŏw�肷��offsetX,offsetY�͂��̃N���X�̃v���p�e�B�ł���
		/// OffsetX,OffsetY�B
		/// </summary>
		/// <param name="texture"></param>
		/// <param name="rect"></param>
		public void SetTexture(ITexture texture, Rect rect)
		{
			this.texture = texture;
			this.rect = rect;
			this.offsetX = this.offsetY = 0;
		}

		/// <summary>
		/// SetTexture(texture,null)�Ɠ��`�B
		/// </summary>
		/// <param name="texture"></param>
		public void SetTexture(ITexture texture)
		{
			this.texture = texture;
			this.rect = null;
			this.offsetX = this.offsetY = 0;
		}

		/// <summary>
		/// �e�N�X�`���ƁA���̂ǂ̕��������̃e�N�X�`���Ƃ��Ĉ����̂���ݒ肷��B
		/// 
		/// �����Ŏw�肷��offsetX,offsetY�͂��̃N���X�̃v���p�e�B�ł���
		/// OffsetX,OffsetY�B
		/// </summary>
		public void SetTexture(ITexture texture, Rect rect, float offsetX, float offsetY)
		{
			this.texture = texture;
			this.rect = rect;
			this.offsetX = offsetX;
			this.offsetY = offsetY;
		}

		/// <summary>
		/// �e�N�X�`���ƁA���̂ǂ̕��������̃e�N�X�`���Ƃ��Ĉ����̂���ݒ肷��B
		/// 
		/// �����Ŏw�肷��offsetX,offsetY�͂��̃N���X�̃v���p�e�B�ł���
		/// OffsetX,OffsetY�B
		/// </summary>
		public void SetTexture(ITexture texture, float offsetX, float offsetY)
		{
			this.texture = texture;
			this.rect = null;
			this.offsetX = offsetX;
			this.offsetY = offsetY;
		}


		#endregion

		#region properties
		/// <summary>
		/// �ێ����Ă���e�N�X�`��
		/// </summary>
		public ITexture Texture
		{
			get { return texture; }
			set { texture = value; }
		}
		private ITexture texture;

		/// <summary>
		/// ���̃e�N�X�`���̕�
		/// 
		/// Abs(right-left)���s�Ȃ��Ă���̂Ń}�C�i�X�͗L�肦�Ȃ��B
		/// </summary>
		public float Width
		{
			get
			{
				if (rect == null)
					return 0;
				return global::System.Math.Abs(rect.Right - rect.Left);
			//	return rect.Right - rect.Left;
			}
		}

		/// <summary>
		/// ���̃e�N�X�`���̍���
		/// 
		/// Abs(bottom-top)���s�Ȃ��Ă���̂Ń}�C�i�X�͂��蓾�Ȃ�
		/// </summary>
		public float Height
		{
			get
			{
				if (rect == null)
					return 0;
				return global::System.Math.Abs(rect.Bottom - rect.Top);
			//	return rect.Bottom - rect.Top;
			}
		}

		/// <summary>
		/// texture�̂ǂ̋�`�����̃e�N�X�`���͕\�����Ă���̂����擾����B
		/// �����SetTexture�Őݒ肵���l�Bnull�ł����Texture�S�́B
		/// </summary>
		public Rect Rect
		{
			get { return rect; }
		}
		/// <summary>
		/// texture�̂ǂ̋�`�����̃e�N�X�`���͕\�����Ă���̂��B
		/// </summary>
		private Rect rect = null;

		/// <summary>
		/// �`�悷��Ƃ��̃I�t�Z�b�g��
		/// </summary>
		public float OffsetX
		{
			get { return offsetX; }
			set { offsetX = value; }
		}
		private float offsetX = 0;

		/// <summary>
		/// �`�悷��Ƃ��̃I�t�Z�b�g��
		/// </summary>
		public float OffsetY
		{
			get { return offsetY; }
			set { offsetY = value; }
		}
		private float offsetY = 0;

		/// <summary>
		/// �`����W
		/// 
		/// OffsetX,Y�Ɠ����悤�ȈӖ������AOffset�̓X�v���C�g�ɕt������
		/// ���ł���A�ʗ�A�ύX���Ȃ��B�������X,Y�́A�X�v���C�g�̕`��ʒu��
		/// �\�����̂ł���A�ύX���Ȃ���g���B
		/// </summary>
		public float X
		{
			get { return x; }
			set { x = value; }
		}
		private float x=0;

		/// <summary>
		/// �`����W
		/// 
		/// OffsetX,Y�Ɠ����悤�ȈӖ������AOffset�̓X�v���C�g�ɕt������
		/// ���ł���A�ʗ�A�ύX���Ȃ��B�������X,Y�́A�X�v���C�g�̕`��ʒu��
		/// �\�����̂ł���A�ύX���Ȃ���g���B
		/// </summary>
		public float Y
		{
			get { return y; }
			set { y = value; }
		}
		private float y=0;

		/// <summary>
		/// �X�v���C�g�Ƃ��ă}�b�v��Ȃǂɕ`�悷��Ƃ���
		/// �����̃��C���B�`����WY + BaseLine����Ƀ\�[�g���ĕ��ёւ���
		/// �s�Ȃ��ĕ`�悷��B
		/// </summary>
		public float BaseLine
		{
			get { return baseLine; }
			set { baseLine = value; }
		}
		private float baseLine = 0;

		#endregion

		#region private
		/// <summary>
		/// �`���̃I�t�Z�b�g�l
		/// </summary>
		public float DstOffsetX { get { return x + OffsetX; } }

		/// <summary>
		/// �`���̃I�t�Z�b�g�l
		/// </summary>
		public float DstOffsetY { get { return y + OffsetY; } }
		#endregion
	}
}
