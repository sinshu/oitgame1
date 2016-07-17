using System;
using System.Collections.Generic;
using System.Text;

namespace Yanesdk.Draw
{
	/// <summary>
	/// �T�C�Y��\���Ă���\���́B
	/// </summary>
	public class Size
	{
		/// <summary>
		/// ���B
		/// </summary>
		public float Cx;

		/// <summary>
		/// �����B
		/// </summary>
		public float Cy;

		/// <summary>
		/// �R���X�g���N�^�B
		/// </summary>
		/// <param name="cx_"></param>
		/// <param name="cy_"></param>
		public Size(float cx_, float cy_)
		{
			Cx = cx_;
			Cy = cy_;
		}

		/// <summary>
		/// �e�f�[�^��ݒ肷��B
		/// </summary>
		/// <param name="Cx"></param>
		/// <param name="Cy"></param>
		public void SetSize(float cx, float cy)
		{
			this.Cx = cx; this.Cy = cy;
		}
	}
}
