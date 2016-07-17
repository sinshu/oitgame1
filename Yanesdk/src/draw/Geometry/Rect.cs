using System;
using System.Collections.Generic;
using System.Text;

namespace Yanesdk.Draw
{
	/// <summary>
	/// ��`��\���Ă���N���X�B
	/// </summary>
	/// <remarks>
	/// ��`(Left,Top)-(Right,Bottom)�ł��邪�A(Right,Bottom)�͊܂܂Ȃ��B
	/// 
	/// �܂��ARect�͐����ł��邱�� (Left >= Right ,  Bottom  >= Top �ł��邱��) ���O������B
	/// ����������ȗp�r�Ŏg���ꍇ�A���̌���ł͂Ȃ��B
	/// 
	/// ���Ƃ���Blt�n�̃��\�b�h�œ]������`��\���ꍇ�́A
	/// Left��Right����ꂩ����΍��E���]���Ă̕`��ƂȂ�B
	/// Top��Bottom�Ƃ���ꂩ����Ώ㉺���]���Ă̕`��ƂȂ�B
	/// 
	/// �����̈�Ȃǂ́AGeometricTools�N���X�̂ق��ɂ܂Ƃ߂ėp�ӂ��Ă���B
	/// </remarks>
	public class Rect
	{
		#region members

		/// <summary>
		/// ���B
		/// </summary>
		public float Left;
		/// <summary>
		/// ��B
		/// </summary>
		public float Top;
		/// <summary>
		/// �E�B
		/// </summary>
		public float Right;
		/// <summary>
		/// ���B
		/// </summary>
		public float Bottom;

		#endregion

		#region properties
		/// <summary>
		/// �����擾����
		/// </summary>
		/// <remarks>
		/// Right - Left �����Ă��邾���B�������}�C�i�X�ɂȂ�ꍇ�����肤��
		/// </remarks>
		/// <returns></returns>
		public float Width
		{
			get { return Right - Left; }
		}

		/// <summary>
		/// �������擾����
		/// </summary>
		/// <remarks>
		/// Bottom - Top �����Ă��邾���B�������}�C�i�X�ɂȂ�ꍇ�����肤��B
		/// </remarks>
		public float Height
		{
			get { return Bottom - Top; }
		}

		/// <summary>
		/// ����(Top,Left)��Point�Ƃ��Ď擾����/�ݒ肷��B
		/// Point��struct�ł��邱�Ƃɒ��ӁB
		/// </summary>
		public Point TopLeft
		{
			get { return new Point(Left, Top); }
			set { Left = value.X; Top = value.Y; }
		}

		/// <summary>
		/// �E��(Right,Bottom)��Point�Ƃ��Ď擾����/�ݒ肷��B
		/// Point��struct�ł��邱�Ƃɒ��ӁB
		/// </summary>
		public Point BottomRight
		{
			get { return new Point(Right, Bottom); }
			set { Right = value.X; Bottom = value.Y; }
		}
		#endregion

		#region ctor
		public Rect()
		{
			Left = Right = Top = Bottom = 0;
		}

		/// <summary>
		/// Rect��4�_��^���ď���������R���X�g���N�^�B
		/// </summary>
		/// <param name="left_"></param>
		/// <param name="top_"></param>
		/// <param name="right_"></param>
		/// <param name="bottom_"></param>
		public Rect(float left_, float top_, float right_, float bottom_)
		{
			Left = left_;
			Top = top_;
			Right = right_;
			Bottom = bottom_;
		}

		/// <summary>
		/// ����ƉE����2�_��^����Rect������������R���X�g���N�^�B
		/// </summary>
		/// <param name="TopLeft"></param>
		/// <param name="BottomRight"></param>
		public Rect(Point TopLeft, Point BottomRight)
		{
			this.TopLeft = TopLeft;
			this.BottomRight = BottomRight;
		}

		/// <summary>
		/// rect�����deep copy���s�Ȃ��B
		/// </summary>
		/// <param name="r"></param>
		public Rect(Rect rect)
		{
			Left = rect.Left;
			Right = rect.Right;
			Top = rect.Top;
			Bottom = rect.Bottom;
		}

		/// <summary>
		/// deep copy���ĕԂ��B
		/// </summary>
		/// <returns></returns>
		public Rect Clone()
		{
			return new Rect(this);
		}
		#endregion

		#region methods
		/// <summary>
		/// �e�f�[�^��ݒ肷��B
		/// </summary>
		/// <param name="left_"></param>
		/// <param name="top_"></param>
		/// <param name="right_"></param>
		/// <param name="bottom_"></param>
		public void SetRect(float left_, float top_, float right_, float bottom_)
		{
			Left = left_;
			Top = top_;
			Right = right_;
			Bottom = bottom_;
		}
	
		/// <summary>
		/// �w��̍��W���A���̋�`�Ɋ܂܂��̂��𔻒肷��B
		/// Left >= Right ,  Bottom  >= Top �ł��邱�Ƃ��O������B
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public bool IsIn(float x, float y)
		{
			return (Left <= x && x < Right && Top <= y && y < Bottom);
		}

		#endregion


	}
}
