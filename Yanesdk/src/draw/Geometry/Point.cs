using System;
using System.Collections.Generic;
using System.Text;

namespace Yanesdk.Draw
{
	/// <summary>
	/// �_��\���Ă���\����
	/// </summary>
	public struct Point
	{
		/// <summary>
		/// x���W�B
		/// </summary>
		public float X;
		/// <summary>
		/// y���W�B
		/// </summary>
		public float Y;

		/// <summary>
		/// �R���X�g���N�^�B
		/// </summary>
		/// <param name="x_"></param>
		/// <param name="y_"></param>
		public Point(float x_, float y_)
		{
			X = x_;
			Y = y_;
		}

		/// <summary>
		/// �e�f�[�^��ݒ肷��B
		/// </summary>
		/// <param name="x_"></param>
		/// <param name="y_"></param>
		public void SetPoint(float x_, float y_)
		{
			X = x_; Y = y_;
		}

		/// <summary>
		/// ���Z
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Point operator +(Point a, Point b)
		{
			return new Point(a.X + b.X, a.Y + b.Y);
		}

		/// <summary>
		/// ���Z
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Point operator -(Point a, Point b)
		{
			return new Point(a.X - b.X, a.Y - b.Y);
		}

		/// <summary>
		/// �萔�{
		/// </summary>
		/// <param name="k"></param>
		/// <param name="a"></param>
		/// <returns></returns>
		public static Point operator *(float k, Point a)
		{
			return new Point(k * a.X, k * a.Y);
		}

		/// <summary>
		/// �萔�{
		/// </summary>
		/// <param name="a"></param>
		/// <param name="k"></param>
		/// <returns></returns>
		public static Point operator *(Point a, float k)
		{
			return k * a;
		}

		/// <summary>
		/// �|�Z(���ςƂ͈قȂ�)
		/// Point(a.X * b.X, a.Y * b.Y)��Ԃ��B
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Point operator *(Point a, Point b)
		{
			return new Point(a.X * b.X, a.Y * b.Y);
		}

		/// <summary>
		/// ��r�I�y���[�^
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator ==(Point a, Point b)
		{
			return a.X == b.X && a.Y == b.Y;
		}

		/// <summary>
		/// ��r�I�y���[�^
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator !=(Point a, Point b)
		{
			return !(a == b);
		}

		/// <summary>
		/// ==��!=���`�����Ƃ��ɂ͗p�ӂ����ق�������Ȃ̂ŗp�ӁB
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			return this == (Point)obj;
		//	return base.Equals(obj);
		}

		/// <summary>
		/// ==��!=���`�����Ƃ��ɂ͗p�ӂ����ق�������Ȃ̂ŗp�ӁB
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			// ����Ȃ���ł�����..
			return (int)(X + Y);
		//	return base.GetHashCode();
		}

		/// <summary>
		/// ����
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public float Dot(Point a)
		{
			return this.X * a.X + this.Y * a.Y;
		}

		/// <summary>
		/// �O��
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public float Cross(Point a)
		{
			return this.X * a.Y - this.Y * a.X;
		}

		/// <summary>
		/// �P���}�C�i�X
		/// </summary>
		/// <returns></returns>
		public static Point operator -(Point a)
		{
			return new Point(-a.X, -a.Y);
		}

		/// <summary>
		/// �m����(�傫����2��)
		/// </summary>
		/// <returns></returns>
		public float Norm()
		{
			return X * X + Y * Y;
		}

		/// <summary>
		/// ���_����̋��������߂�B
		/// �x�N�g���Ƃ݂Ȃ����Ƃ��̑傫���ł���A�m������sqrt�ł���B
		/// </summary>
		/// <returns></returns>
		public float Abs()
		{
			return (float)global::System.Math.Sqrt(Norm());
		}

		/// <summary>
		/// Debug�p�ɕ����񉻂���
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("({0},{1})", X, Y);
		}
	}

}
