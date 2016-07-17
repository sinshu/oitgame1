using System;
using System.Collections.Generic;
using System.Text;

namespace Yanesdk.Draw
{
	/// <summary>
	/// 2����affine�s��
	/// </summary>
	public class Affine2D
	{
		#region ctor
		/// <summary>
		/// �P�ʍs��ŏ�����
		/// </summary>
		public Affine2D()
		{
			Matrix = new Matrix2D();
		}

		/// <summary>
		/// �P�ʍs���k�{�ŏ�����
		/// </summary>
		/// <param name="a"></param>
		public Affine2D(float k)
		{
			Matrix = new Matrix2D(k);
		}

		/// <summary>
		/// �P�ʍs�� + p�ŏ�����
		/// </summary>
		/// <param name="p"></param>
		public Affine2D(Point p)
		{
			Matrix = new Matrix2D();
			Point = p;
		}

		/// <summary>
		/// �P�ʍs���k�{ + p�ŏ�����
		/// </summary>
		/// <param name="k"></param>
		/// <param name="p"></param>
		public Affine2D(float k, Point p)
		{
			Matrix = new Matrix2D(k);
			Point = p;
		}

		/// <summary>
		/// �w�肳�ꂽ2�����s��ŏ�����
		/// </summary>
		/// <param name="m"></param>
		public Affine2D(Matrix2D m)
		{
			Matrix = m;
		}

		/// <summary>
		/// �w�肳�ꂽ2�����s��ƕ��s�ړ��Ƃŏ�����
		/// </summary>
		/// <param name="m"></param>
		/// <param name="p"></param>
		public Affine2D(Matrix2D m, Point p)
		{
			Matrix = m;
			Point = p;
		}

		/// <summary>
		/// affine�s��̊e�v�f���w�肵�Ă̏�����
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="c"></param>
		/// <param name="d"></param>
		/// <param name="e"></param>
		/// <param name="f"></param>
		public Affine2D(float a, float b, float c, float d, float e, float f)
		{
			Matrix = new Matrix2D(a, b, c, d);
			Point = new Point(e, f);
		}

		/// <summary>
		/// �s���
		/// (k1 0)
		/// (0 k2)
		/// �ŏ���������B
		/// </summary>
		/// <param name="k1"></param>
		/// <param name="k2"></param>
		public Affine2D(float k1, float k2)
		{
			Matrix = new Matrix2D(k1, 0, 0, k2);
		}

		/// <summary>
		/// src Rect��dst Rect�֕ϊ�����Affine�s������߂�B
		/// src,dst Rect�͐����ł��邱�ƁB
		/// �����̐����ɂ��Ă�Rect�̐���������B
		/// 
		/// �܂����߂�ׂ�Affine�s�񂪑��݂��Ȃ��Ƃ���null���Ԃ�B
		/// </summary>
		/// <param name="src"></param>
		/// <param name="dst"></param>
		public static Affine2D RectToRect(Rect src,Rect dst)
		{
			float srcWidth = src.Width;
			if (srcWidth == 0)
				return null;
			float widthRate = dst.Width / srcWidth;

			float srcHeight = src.Height;
			if (srcHeight == 0)
				return null;
			float heightRate = dst.Height / srcHeight;

			// ���s�ړ���
			Point point = dst.TopLeft - src.TopLeft;

			return new Affine2D(widthRate, 0, 0, heightRate, point.X,point.Y);

		}

		#endregion

		#region properties
		/// <summary>
		/// 2�����s��
		/// </summary>
		public Matrix2D Matrix;

		/// <summary>
		/// affine�s��̕��s�ړ�����
		/// </summary>
		public Point Point;
		#endregion

		#region affine�ϊ��̉��Z�poperator

		/// <summary>
		/// affine�s��̊|�Z�B
		/// </summary>
		/// <remarks>
		/// affine�s��
		///		M1 + P1
		/// ��
		///		M2 + P2
		/// ���������Ƃ���B���̍s��ɂ�鍇���ϊ�
		///		(M1 + P1)(M2 + P2)�ɂ����
		/// ��_p���ǂ��Ɉړ����邩�l����B�܂�M2+P2�ɂ����
		///		(M2�Ep + P2) �Ɉړ���������
		/// M1+P1��M1(M2�Ep + P2) + P1�Ɉړ�����B
		/// M1M2�Ep + M1P2 + P1
		/// ����āA�s�� M1M2 , ���s�v�fM1P2+P1
		/// �����߂�ׂ�affine�s��ł���B
		/// </remarks>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Affine2D operator *(Affine2D a, Affine2D b)
		{
			return new Affine2D(
				a.Matrix * b.Matrix,
				a.Matrix * b.Point + a.Point);
		}

		/// <summary>
		/// �萔�{
		/// </summary>
		/// <param name="k"></param>
		/// <param name="a"></param>
		/// <returns></returns>
		public static Affine2D operator *(float k, Affine2D a)
		{
			return new Affine2D(k * a.Matrix, k * a.Point);
		}

		public static Affine2D operator *(Affine2D a, float k)
		{
			return k * a;
		}

		/// <summary>
		/// 2����affine�s���2�����x�N�g�����|����B
		/// �i�_p��affine�ϊ�a�ɂ���ĕϊ������̓_�����߂�B�j
		/// Point��struct�ł��邱�Ƃɒ��ӂ��Ȃ���g�����ƁB
		/// </summary>
		/// <param name="a"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		public static Point operator *(Affine2D a, Point p)
		{
			return a.Matrix * p + a.Point;
		}

		/// <summary>
		/// ���s�ړ��ϊ�p��affine�ϊ�a�̍����ϊ������߂�B
		/// ���Ȃ킿�Aa.Matrix + (A.Matrix * p + A.Point)
		/// Compose(Point p,Affine2D a)�Ƃ͉��ł͂Ȃ����Ƃɒ��ӁI
		/// </summary>
		/// <param name="a"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		public static Affine2D Compose(Affine2D a, Point p)
		{
			return new Affine2D(a.Matrix, a.Matrix * a.Point + p);
		}

		/// <summary>
		/// affine�s��ɕ��s�ړ��ϊ��������B
		/// ���Ȃ킿�Aa.Matrix + a.Point + p�ł���B
		/// Compose(Affine2D a, Point p)�Ƃ͉��ł͂Ȃ����Ƃɒ��ӁI
		/// </summary>
		/// <param name="a"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		public static Affine2D Compose(Point p, Affine2D a)
		{
			return new Affine2D(a.Matrix, a.Point + p);
		}

		/// <summary>
		/// �t�ϊ������߂�B�t�ϊ������݂��Ȃ��Ƃ���null���Ԃ�B
		/// 
		/// Affine2D: p' = MP + T
		/// ����āAP = M^-1 P' - M^-1 T
		/// Affine2D inverse : M^-1 - M^-1 T�ł���B
		/// </summary>
		/// <returns></returns>
		public Affine2D Inverse()
		{
			Matrix2D inverseMatrix = this.Matrix.Inverse();
			if (inverseMatrix == null)
				return null;

			return new Affine2D(inverseMatrix, inverseMatrix * -this.Point);
		}

		#endregion

		/// <summary>
		/// �f�o�b�O�p�ɕ����񉻂���@�\
		/// </summary>
		public override string ToString()
		{
			return string.Format("[{0},{1}]"
				, Matrix.ToString(), Point.ToString());
		}
	}
}
