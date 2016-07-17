using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Yanesdk.Draw
{
	/// <summary>
	/// �􉽓I�Ȋ֌W���Z���s�Ȃ���A�̃c�[��
	/// </summary>
	/// <remarks>
	///	���̊􉽉��Z�N���X�ɂ��Ă�
	///	http://www.prefield.com/algorithm/
	///	���Q�l�ɂ����Ă����������B
	///	�������ꂽ�ɂ߂ėD�ꂽ�\�[�X�R�[�h�ł���B
	/// </remarks>
	public class GeometricTools
	{
		/// <summary>
		/// CCW(CounterClockwise:�����v���)�ł��邩�̔�����s�Ȃ��B
		/// �^����ꂽ�O�_ a, b, c �� a �� b �� c �Ɛi�ނƂ��̈ʒu�֌W�𒲂ׂ�B
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="c"></param>
		/// <returns>
		/// �ȉ��̎��v���,�����v�܂��Ƃ����̂́Ax���̐��������E�Ay���̐�������
		/// ������ɂƂ�Ƃ��̘b�ł���B
		/// 
		/// +1 : �����v�܂��
		/// -1 : ���v�܂��
		///  0 : a , c , b�̏��Ԃœ��꒼����ɂ���B(��������b��c�������_�ł���)
		/// </returns>
		public static int CCW(Point a, Point b, Point c)
		{
			b -= a; c -= a;
			float bc = b.Cross(c);

			return global::System.Math.Sign(bc);
			/*
			if (bc > 0) return +1;				// counter clockwise
			if (bc < 0) return -1;				// clockwise

			/// +2 : c , a , b�̏��Ԃœ��꒼����ɂ���B
			/// -2 : a , b , c�̏��Ԃœ��꒼����ɂ���B
			// if (b.Dot(c) < 0) return +2;		// c--a--b on line
			// if (b.Norm() < c.Norm()) return -2; // a--b--c on line
			return 0;							// a--c--b on line (or b == c)
			 */
		}

		/// <summary>
		/// �����ƒ����̌�_��Ԃ��B
		/// 
		/// ��������_�����݂��Ȃ��Ƃ���
		/// Point(float.Nan,float.Nan)���Ԃ�B
		/// </summary>
		/// <param name="line1"></param>
		/// <param name="line2"></param>
		/// <returns></returns>
		public static Point CrossPoint(Line line1,Line line2)
		{
			//�@���� L : a + s b
			//�@���� M : c + t d
			// ���������Ƃ��āA��_�� a + sb = c + td
			// sb - td = c - a �Ȃ̂ŉE�ӂ����Ƃ����B
			// s(b�~d) = (sb)�~d  = (td + ��)�~d = t d�~d + ���~d
			// ����x�N�g���̊O�ς�0�Ȃ̂�
			// = t 0 + ���~d = ���~d
			// ����� s (b�~d) = ���~d
			//   s = ���~d / (b�~d) ������ (b�~d)!=0

			Point u = line1[1] - line1[0];
			float v = u.Cross(line2[1] - line2[0]);
			if (v == 0)
				return new Point(float.NaN, float.NaN);
			float s = u.Cross(line1[0] - line2[0]) / v;
			return line2[0] + s * (line2[1] - line2[0]);
		}

		/// <summary>
		/// ����A,B�̌�_�����߂�B
		/// ����A��̓_�Aa0,a1
		/// ����B��̓_�Ab0,b1
		/// </summary>
		/// <param name="a0"></param>
		/// <param name="a1"></param>
		/// <param name="b0"></param>
		/// <param name="b1"></param>
		/// <returns></returns>
		public static Point CrossPoint(Point a0,Point a1,Point b0,Point b1)
		{
			Point u = a1 - a0;
			float s = u.Cross(a0 - b0) / u.Cross(b1- b0);
			return b0 + s * (b1 - b0);
		}

		/// <summary>
		/// �^����ꂽ�_�񂪉E���Ȃ̂������Ȃ̂��𔻒肷��B
		/// 
		/// �����(�����v�܂��) : 1
		/// ��]�Ȃ�(�ʐ�0) : 0
		/// �E���(���v�܂��) : -1
		/// 
		/// ���@x���̐��������E�Ay���̐���������ɂƂ����Ƃ��B
		/// 
		/// </summary>
		/// <remarks>
		/// ����͖ʐς��O�ς��g���ċ��߂�΂��̐����ɂ��E��肩����肩������ł���B
		/// 
		///	�����Ƒ��p�`�ւ̋����Ƃ������̈�Ȃǂɂ��ĉ��X�Ə����Ă���{�wGeometric Tools for Computer Graphics�x�ɖʔ����L�q���������̂ŏЉ�B
		///	Area(P) = 1/2 �� XiYi+1 - Xi+1Yi (i=0,�c,n-1) �Ɠ����o�������ƁA���̇��̕����� �� Xi(Yi+1 - Yi-1) + �� (XiYi-1 - Xi+1Yi)�ƕ������āA
		///	��҂� X0Y-1 - XnYn-1 = 0�ƂȂ�B����Ėʐό����͈ȉ��̂悤�ɒP�����ł���B
		///	Area(P) = 1/2 ��Xi(Yi+1 - Yi-1) { i = 0,�c,n-1 }
		///	����FAQ��Usenet��comp.graphics.algorithms��Dan Sunday(2001)�����e�������A�ȑO sci.math��Dave Rusin(1995)�����e���Ă���B
		/// �T���΂����ƌÂ��̂����邩���m��Ȃ��Ƃ̂��ƁB
		/// </remarks>
		/// <param name="point"></param>
		/// <returns></returns>
		public static int IsClockwise(Point[] points)
		{
			int n = points.Length;
			float sum2 = 0; // ���߂�ׂ��ʐς�2�{�B
			for (int i = 0; i < n; ++i)
				sum2 += points[i].X * (points[(i + 1) % n].Y - points[(i + n - 1) % n].Y);

			return global::System.Math.Sign(sum2);
		}

		/// <summary>
		/// ���p�`�̖ʐς����߂�B
		/// </summary>
		/// <param name="points"></param>
		/// <returns></returns>
		public static float Area(Point[] points)
		{
			int n = points.Length;
			float sum2 = 0; // ���߂�ׂ��ʐς�2�{�B
			for (int i = 0; i < n; ++i)
				sum2 += points[i].X * (points[(i + 1) % n].Y - points[(i + n - 1) % n].Y);

			return global::System.Math.Abs(sum2/2);
		}

		/// <summary>
		/// ���p�`a�Ƒ��p�`b�Ƃ̌����̈�����߂�B
		/// �������Aa�͓ʕ�ł������(�����v�܂��)�łȂ���΂Ȃ�Ȃ��B
		/// a�͋�W���ł͂Ȃ�Ȃ��B
		/// 
		/// �����Ō��������� x���̐��������E�Ay���̐���������ɂƂ����Ƃ��B
		/// </summary>
		/// <remarks>
		/// �]����clipping�Ȃǂɗp����Ɨǂ��B
		/// </remarks>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Point[] Intersect(Point[] a, Point[] b)
		{
			// ���̃A���S���Y���͂�˂��炨�̎�ɂ��B
			int N = a.Length;
			for(int i=0;i<N;++i)
			{
				int M = b.Length;
				if (M == 0) continue;

				// ���� (a[i],a[i-1]) ��clipping
				Point a0 = a[i];
				Point a1 = a[(i + 1) % N];

				Point Now = b[M - 1];

				Polygon c = new Polygon();
				for (int j = 0; j < M; ++j)
				{
					Point Next = b[j];

					// �O���̗̈�ȊO(������������)�Ȃ�΂��̓_���i�[�B
					if (CCW(a0, a1, Now) != -1) c.Add(Now);
					
					// ��������O���A���邢�͊O����������ɂ����Ȃ�΁A���̌�_��ۑ��B
					if (CCW(a0, a1, Now)*CCW(a0, a1, Next) < 0)
						c.Add(CrossPoint(Now,Next,a0,a1));

					Now = Next;
				}
				b = c.ToArray();
		  }
		  return b;
		}

		/// <summary>
		/// Rect��Rect�̌����͈͂����߂�B
		/// �����̈悪�Ȃ����null�B
		/// 
		/// rect1,rect2�Ƃ��ɐ����łȂ���΂Ȃ�Ȃ��B
		/// </summary>
		/// <param name="rect1"></param>
		/// <param name="rect2"></param>
		/// <returns></returns>
		public static Rect Intersect(Rect rect1, Rect rect2)
		{
			if (rect1.Right <= rect2.Left ||
				rect2.Right <= rect1.Left ||
				rect1.Bottom <= rect2.Top ||
				rect2.Bottom <= rect1.Top )
				return null;

			// �����̈�͋�W���ł͂Ȃ��B

			return new Rect
			(
				rect1.Left < rect2.Left ? rect2.Left : rect1.Left ,			// max(left)
				rect1.Top < rect2.Top ? rect2.Top : rect1.Top ,				// max(top)
				rect1.Right < rect2.Right ? rect1.Right : rect2.Right,		// min(right)
				rect1.Bottom < rect2.Bottom ? rect1.Bottom : rect2.Bottom 	// min(bottom)
			);

		}

		/// <summary>
		/// �ʑ��p�`�Ƌ�`�Ƃ̌����̈�����߂�B��`�͐����ł��邱�ƁB
		/// �����̈悪���݂��Ȃ����null�B
		/// 
		/// ���������ӎ����ăR�[�f�B���O���Ă���B
		/// �ʑ��p�`���܂邲��Rect a�Ɏ��܂�Ȃ�΁A�ʑ��p�`�ł���b�̎Q�Ƃ����̂܂ܕԂ邱�Ƃ�����B(�������̂���)
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Point[] Intersect(Rect a, Point[] b)
		{
			int N = b.Length;
			
			// ���݂̃J�����g�̓_�̓����W�O�ł��邩
			bool isLastPointOut = false, isCurrentPointOut = false;
			// �������Ő錾����͍̂s�V���������A����͖{�����������Ȃ��Ă����ϐ��Ȃ̂�
			// unroll���Ă���Ƃ���ŉ��x�����������ꂽ���Ȃ��B

			// y = top�Őؒf
			{
				bool inRect = true;  // ���ׂĂ���`��
				bool outRect = true; // ���ׂĂ���`�O
				float top = a.Top;

				for (int i = 0; i < N; ++i)
				{
					outRect = outRect && (b[i].Y <= top);
					inRect = inRect && (b[i].Y >= top);
				}
				if (outRect)
					return null;
				if (!inRect)
				{
					// �ʑ��p�`c �� y = top�Őؒf�������̂�b�Ƃ���K�v������B
					Polygon c = new Polygon();

					// ���[�v�� 0��[1,N]�ɂ���΁A[1,N]�̋�Ԃ�(i-1)���񕉂ł��邱�Ƃ�ۏ؂ł���
					for (int i = 0; i < N + 1; ++i)
					{
						isCurrentPointOut = b[i % N].Y < top;
						if (i != 0)
						{
							if (isLastPointOut ^ isCurrentPointOut)
							{
								// �ӂ̗��[���̈���܂����̂Œ�����y=top�Ƃ̌�_���o��
								Point p1 = b[(i - 1) % N];
								Point p2 = b[i % N];

								// t�EP1 + (1-t)�EP2 = { x , top }
								// t(P1 - P2) + P2 = { x, top }
								// �� t = (top - P2.Y)/(P1.Y - P2.Y)

								float t = (top - p2.Y) / (p1.Y - p2.Y);
								float x = t * p1.X + (1 - t) * p2.X;

								c.Add(new Point(x, top));
							}
							if (!isCurrentPointOut)
							{
								// �ӂ̍���̒��_�������̈�Ȃ̂�b[i]���o��
								c.Add(b[i % N]);
							}
						}
						isLastPointOut = isCurrentPointOut;
					}
					b = c.ToArray();
					N = b.Length;
				}
			}

			// y = bottom�Őؒf
			{
				bool inRect = true;  // ���ׂĂ���`��
				bool outRect = true; // ���ׂĂ���`�O
				float bottom = a.Bottom;

				for (int i = 0; i < N; ++i)
				{
					outRect = outRect && (b[i].Y >= bottom);
					inRect = inRect && (b[i].Y <= bottom);
				}
				if (outRect)
					return null;
				if (!inRect)
				{
					// �ʑ��p�`c �� y = bottom�Őؒf�������̂�b�Ƃ���K�v������B
					Polygon c = new Polygon();

					// ���[�v�� 0��[1,N]�ɂ���΁A[1,N]�̋�Ԃ�(i-1)���񕉂ł��邱�Ƃ�ۏ؂ł���
					for (int i = 0; i < N + 1; ++i)
					{
						isCurrentPointOut = b[i % N].Y > bottom;
						if (i != 0)
						{
							if (isLastPointOut ^ isCurrentPointOut)
							{
								// �ӂ̗��[���̈���܂����̂Œ�����y=bottom�Ƃ̌�_���o��
								Point p1 = b[(i - 1) % N];
								Point p2 = b[i % N];

								// t�EP1 + (1-t)�EP2 = { x , bottom }
								// t(P1 - P2) + P2 = { x, bottom }
								// �� t = (bottom - P2.Y)/(P1.Y - P2.Y)

								float t = (bottom - p2.Y) / (p1.Y - p2.Y);
								float x = t * p1.X + (1 - t) * p2.X;

								c.Add(new Point(x, bottom));
							}
							if (!isCurrentPointOut)
							{
								// �ӂ̍���̒��_�������̈�Ȃ̂�b[i]���o��
								c.Add(b[i % N]);
							}
						}
						isLastPointOut = isCurrentPointOut;
					}
					b = c.ToArray();
					N = b.Length;
				}
			}

			// x = left�Őؒf
			{
				bool inRect = true;  // ���ׂĂ���`��
				bool outRect = true; // ���ׂĂ���`�O
				float left = a.Left;

				for (int i = 0; i < N; ++i)
				{
					outRect = outRect && (b[i].X <= left);
					inRect = inRect && (b[i].X >= left);
				}
				if (outRect)
					return null;
				if (!inRect)
				{
					// �ʑ��p�`c �� y = left�Őؒf�������̂�b�Ƃ���K�v������B
					Polygon c = new Polygon();

					// ���[�v�� 0��[1,N]�ɂ���΁A[1,N]�̋�Ԃ�(i-1)���񕉂ł��邱�Ƃ�ۏ؂ł���
					for (int i = 0; i < N + 1; ++i)
					{
						isCurrentPointOut = b[i % N].X < left;
						if (i != 0)
						{
							if (isLastPointOut ^ isCurrentPointOut)
							{
								// �ӂ̗��[���̈���܂����̂Œ�����x=left�Ƃ̌�_���o��
								Point p1 = b[(i - 1) % N];
								Point p2 = b[i % N];

								// t�EP1 + (1-t)�EP2 = { left , y }
								// t(P1 - P2) + P2 = { left , y }
								// �� t = (left - P2.X)/(P1.X - P2.X)

								float t = (left - p2.X) / (p1.X - p2.X);
								float y = t * p1.Y + (1 - t) * p2.Y;

								c.Add(new Point(left, y));
							}
							if (!isCurrentPointOut)
							{
								// �ӂ̍���̒��_�������̈�Ȃ̂�b[i]���o��
								c.Add(b[i % N]);
							}
						}
						isLastPointOut = isCurrentPointOut;
					}
					b = c.ToArray();
					N = b.Length;
				}
			}

			// x = right�Őؒf
			{
				bool inRect = true;  // ���ׂĂ���`��
				bool outRect = true; // ���ׂĂ���`�O
				float right = a.Right;

				for (int i = 0; i < N; ++i)
				{
					outRect = outRect && (b[i].X >= right);
					inRect = inRect && (b[i].X <= right);
				}
				if (outRect)
					return null;
				if (!inRect)
				{
					// �ʑ��p�`c �� y = right�Őؒf�������̂�b�Ƃ���K�v������B
					Polygon c = new Polygon();

					// ���[�v�� 0��[1,N]�ɂ���΁A[1,N]�̋�Ԃ�(i-1)���񕉂ł��邱�Ƃ�ۏ؂ł���
					for (int i = 0; i < N + 1; ++i)
					{
						isCurrentPointOut = b[i % N].X > right;
						if (i != 0)
						{
							if (isLastPointOut ^ isCurrentPointOut)
							{
								// �ӂ̗��[���̈���܂����̂Œ�����x=right�Ƃ̌�_���o��
								Point p1 = b[(i - 1) % N];
								Point p2 = b[i % N];

								// t�EP1 + (1-t)�EP2 = { left , y }
								// t(P1 - P2) + P2 = { left , y }
								// �� t = (left - P2.X)/(P1.X - P2.X)

								float t = (right - p2.X) / (p1.X - p2.X);
								float y = t * p1.Y + (1 - t) * p2.Y;

								c.Add(new Point(right, y));
							}
							if (!isCurrentPointOut)
							{
								// �ӂ̍���̒��_�������̈�Ȃ̂�b[i]���o��
								c.Add(b[i % N]);
							}
						}
					}
					b = c.ToArray();
					N = b.Length;
				}
			}

			return b;
		}


		/// <summary>
		/// ���p�`a,b�̂��ׂĂ������_�ł��邩���`�F�b�N����B
		/// UnitTest�Ȃǂŗp����Ɨǂ����낤�B
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns>�����ł����true�B</returns>
		public static bool PolygonEqual(Point[] a, Point[] b)
		{
			if (a.Length != b.Length)
				return false;

			for (int i = 0; i < a.Length; ++i)
				if (a[i] != b[i])
					return false;

			return true;
		}

		/// <summary>
		/// ���p�`�̒��_���^���āA���ꂪ�ʑ��p�`���ǂ����𔻒肷��B
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public static bool IsConvex(Point[] a)
		{
			int N = a.Length;
			bool[] b = new bool[3];
			for (int i = 0; i < N; ++i)
			{
				// CCW��-1,0,1�����Ԃ��Ȃ��B
				// ���[�v�̍Œ���CCW��-1��1��Ԃ����ꍇ�ɂ̂�false
				b[CCW(a[i], a[(i + 1) % N], a[(i + 2) % N]) + 1] = true;

				if (b[0] && b[2])
					return false;
			}

		//	return !(b[0] && b[2]);
			return true;
		}

		/// <summary>
		/// UnitTest�p�̃��\�b�h
		/// </summary>
		public static void UnitTest()
		{
			Debug.Assert(
				CCW(new Point(0, 0),
					new Point(1, 0),
					new Point(1, 1)) == 1
			);

			Debug.Assert(
				CrossPoint(
					new Line(new Point(0,1),new Point(2,1)),
					new Line(new Point(1,0),new Point(1,2))
					) == new Point(1,1)
			);

			Debug.Assert(
				CrossPoint(
					new Point(0, 1), new Point(2, 1),
					new Point(1, 0), new Point(1, 2)
					) == new Point(1, 1)
			);

			Debug.Assert(
				IsClockwise(new Point[3] { new Point(0, 0), new Point(2, 0), new Point(1, 1) }) == 1
			);

			Debug.Assert(
				PolygonEqual
					(Intersect(
						new Point[4] { new Point(0, 0), new Point(10, 0), new Point(10, 5), new Point(0, 5) },
						new Point[4] { new Point(3, -5), new Point(6, -5), new Point(6, 20), new Point(3, 20) }
					),
					new Point[4] { new Point(3, 0), new Point(6, 0), new Point(6, 5), new Point(3, 5) }
				)
			);

			Debug.Assert(
				PolygonEqual
					(Intersect(
						new Point[4] { new Point(0, 0), new Point(10, 0), new Point(10, 5), new Point(0, 5) },
						new Point[4] { new Point(6, -5), new Point(3, -5), new Point(3, 20), new Point(6, 20) }
					),
					new Point[4] { new Point(6, 0), new Point(3, 0), new Point(3, 5), new Point(6, 5) }
				)
			);

			Debug.Assert(
				PolygonEqual
					(Intersect(
						new Rect(0,0,10,5),
						new Point[4] { new Point(6, -5), new Point(3, -5), new Point(3, 20), new Point(6, 20) }
					),
					new Point[4] { new Point(3, 5), new Point(6, 5), new Point(6, 0), new Point(3, 0) }
				)
			);

		}

	}
}
