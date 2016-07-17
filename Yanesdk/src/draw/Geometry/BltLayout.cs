using System;
using System.Collections.Generic;
using System.Text;
using Yanesdk.Ytl;

namespace Yanesdk.Draw
{
	/// <summary>
	/// �]�����Ɋ�Ƃ���]�����̓_���w�肷�邽�߂̗񋓑�
	/// </summary>
	public enum BltLayout
	{
		TopLeft = 0,		// ����
		TopMiddle = 1,		// ������
		TopRight = 2,		// �E��
		MiddleLeft = 3,		// ������
		Center = 4,			// ����
		MiddleRight = 5,	// �E����
		BottomLeft = 6,		// ����
		BottomMiddle = 7,	// ������
		BottomRight = 8		// �E��
	}

	/// <summary>
	/// Rect��^���āA��������BltLayout�ɏ]�����|�W�V�������Z�o����B
	/// Update���Ăяo���ꂽ�Ƃ��ɂ���Rect1��BltLayout�Ŏw�肳���Point��Ԃ��B
	/// �܂�Rect1�𐳑�(Bottom >= Top , Right >= Left)�ɂ������̂�Rect2�ɕԂ��B
	/// </summary>
	/// </summary>
	public class BltLayoutHelper
	{
		/// <summary>
		/// ����Rect
		/// </summary>
		public Rect Rect1;

		/// <summary>
		/// ����Layout
		/// </summary>
		public BltLayout Layout; 

		/// <summary>
		/// �o��Point
		/// </summary>
		public Point LayoutPoint;
	
		/// <summary>
		/// �o��Rect(Rect1�𐳑�����������)
		/// </summary>
		public Rect Rect2;

		public YanesdkResult Update()
		{
			// �܂������ŁALayout�̎w����s�Ȃ��K�v������̂�
			// Top,Left,Bottom,Right���m�肳���Ă����B


			float top, left, bottom, right;
			if (Rect1.Left < Rect1.Right)
			{
				left = Rect1.Left;
				right = Rect1.Right;
			}
			else
			{
				left = Rect1.Right;
				right = Rect1.Left;
			}
			if (Rect1.Top < Rect1.Bottom)
			{
				top = Rect1.Top;
				bottom = Rect1.Bottom;
			}
			else
			{
				top = Rect1.Bottom;
				bottom = Rect1.Top;
			}

			float srcRectWidth = right - left;
			float srcRectHeight = bottom - top;
			if (srcRectWidth <= 0 || srcRectHeight <= 0)
				return YanesdkResult.InvalidParameter;

			switch (Layout)
			{
				case BltLayout.TopLeft:
					LayoutPoint = new Point(left, top); break;
				case BltLayout.TopMiddle:
					LayoutPoint = new Point(left + srcRectWidth / 2, top); break;
				//	���]������`�̍��W���[���ɂȂ�ƃA���`�G�C���A�X��
				// �������ċC���������C���������邪�A����͎d�l�Ƃ������ƂŁB
				case BltLayout.TopRight:
					LayoutPoint = new Point(right, top); break;

				case BltLayout.MiddleLeft:
					LayoutPoint = new Point(left, top + srcRectHeight / 2); break;
				case BltLayout.Center:
					LayoutPoint = new Point(left + srcRectWidth / 2, top + srcRectHeight / 2); break;
				case BltLayout.MiddleRight:
					LayoutPoint = new Point(right, top + srcRectHeight / 2); break;

				case BltLayout.BottomLeft:
					LayoutPoint = new Point(left, bottom); break;
				case BltLayout.BottomMiddle:
					LayoutPoint = new Point(left + srcRectWidth / 2, bottom); break;
				case BltLayout.BottomRight:
					LayoutPoint = new Point(right, bottom); break;

				default:
					return YanesdkResult.InvalidParameter; // �����Ă͂Ȃ�Ȃ��̂����B
			}

			Rect2 = new Rect(left, top, right, bottom);

			return YanesdkResult.NoError;
		}


	}
}
