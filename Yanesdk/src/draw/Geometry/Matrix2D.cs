using System;
using System.Collections.Generic;
using System.Text;

namespace Yanesdk.Draw
{
	/// <summary>
	/// 2�����s��N���X
	/// </summary>
	public class Matrix2D
	{
		/// <summary>
		/// �s��v�f
		/// (a b)
		/// (c d)
		/// </summary>
		public float A, B, C, D;

		#region ctor
		/// <summary>
		/// �P�ʍs��ŏ�����
		/// </summary>
		public Matrix2D() { A = D = 1; B = C = 0; }

		/// <summary>
		/// �P�ʍs���k�{�ŏ�����
		/// </summary>
		/// <param name="k"></param>
		public Matrix2D(float k) { A = D = k; B = C = 0; }

		/// <summary>
		/// �s��̊e�v�f���w�肵�Ă̏�����
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="c"></param>
		/// <param name="d"></param>
		public Matrix2D(float a, float b, float c, float d)
		{
			A = a; B = b; C = c; D = d;
		}

		/// <summary>
		/// ��]�s���ݒ肷��B
		/// rad = 0�`512������Ƃ���p�x�B
		/// ��]�����́Ax�����E�Ay�����������ɂƂ�Ȃ玞�v�܂��B
		/// 
		/// �^�̓����R���X�g���N�^�����ĂȂ��̂Ń}�[�J�[���̃R���X�g���N�^��
		/// �p���đΏ�����B��MatrixRotate�͒P�Ȃ�enum
		/// </summary>
		/// <returns></returns>
		public Matrix2D(MatrixRotate r, float rad)
		{
			A = D = (float)global::System.Math.Cos
				(global::System.Math.PI * rad / 256);
			C = (float)global::System.Math.Sin
				(global::System.Math.PI * rad / 256);
			B = -C;
		}

		/// <summary>
		/// ��]�g��s���ݒ肷��B
		/// rad = 0�`512������Ƃ���p�x�B
		/// rate = �g��䗦
		/// ��]�����́Ax�����E�Ay�����������ɂƂ�Ȃ玞�v�܂��B
		/// 
		/// �^�̓����R���X�g���N�^�����ĂȂ��̂Ń}�[�J�[���̃R���X�g���N�^��
		/// �p���đΏ�����B��MatrixRotate�͒P�Ȃ�enum
		/// </summary>
		/// <param name="r"></param>
		/// <param name="rad"></param>
		/// <param name="rate"></param>
		public Matrix2D(MatrixRotate dummyMarker, float rad, float rate)
		{
			A = D = (float)(global::System.Math.Cos
				(global::System.Math.PI * rad / 256) * rate);
			C = (float)(global::System.Math.Sin
				(global::System.Math.PI * rad / 256) * rate);
			B = -C;
		}

		public class MatrixRotate { }

		#endregion

		#region �s�񉉎Z�֌W�̃I�y���[�^
		/// <summary>
		/// �s��̉��Z
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Matrix2D operator +(Matrix2D a, Matrix2D b)
		{
			return new Matrix2D(a.A + b.A, a.B + b.B, a.C + b.C, a.D + b.D);
		}

		/// <summary>
		/// �s��̌��Z
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Matrix2D operator -(Matrix2D a, Matrix2D b)
		{
			return new Matrix2D(a.A - b.A, a.B - b.B, a.C - b.C, a.D - b.D);
		}

		/// <summary>
		/// �s��̊|�Z
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Matrix2D operator *(Matrix2D a, Matrix2D b)
		{
			return new Matrix2D(
				a.A * b.A + a.B * b.C,
				a.A * b.B + a.B * b.D,
				a.C * b.A + a.D * b.C,
				a.C * b.B + a.D * b.D);
		}

		/// <summary>
		/// �萔�{
		/// </summary>
		/// <param name="k"></param>
		/// <param name="a"></param>
		/// <returns></returns>
		public static Matrix2D operator *(float k, Matrix2D a)
		{
			return new Matrix2D(k * a.A, k * a.B, k * a.C, k * a.D);
		}

		/// <summary>
		/// �s��~�x�N�g��
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Point operator *(Matrix2D a, Point p)
		{
			return new Point
				(
					a.A * p.X + a.B * p.Y,
					a.C * p.X + a.D * p.Y
				);
		}
		#endregion

		#region methods
		/// <summary>
		/// �t�s������߂�B�t�s�񂪑��݂��Ȃ��Ƃ��́Anull��Ԃ��B
		/// </summary>
		/// <returns></returns>
		public Matrix2D Inverse()
		{
			float det = A * D - B * C;
			if (det == 0)
				return null;

			return new Matrix2D(D / det, -B / det, -C / det, A / det);
		}

		#endregion

		/// <summary>
		/// debug�p�ɕ�����
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("{({0},{1}),({2},{3})}", A, B, C, D);
		}
	}
}
