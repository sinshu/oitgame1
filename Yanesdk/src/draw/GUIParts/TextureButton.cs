using System;
using System.Collections.Generic;
using System.Text;

using Yanesdk.Draw;
using Yanesdk.Input;

namespace Yanesdk.GUIParts
{
	/// <summary>
	/// ボタンの描画コントロール
	/// </summary>
	public class TextureButton : ITextureGUI , IDisposable
	{
		/// <summary>
		/// TextureLoaderでボタン素材として読み込む定義ファイルを指定すること。
		/// 
		/// 定義ファイルの
		///    0番…　ボタン通常時
		///    1番…　ボタン押し下げ時
		///    2番…　ボタンhover時
		///    3番…  disable状態のボタン(この3番目のものは、LoaderOffset propertyで変更できる。
		///			defaultが3というだけ)
		/// を指定すること。
		/// 
		/// hover時も同じ画像で良いなら、0と2番に同じファイルを指定してください。
		/// </summary>
		/// <remarks>
		/// 一番基本的なパーツなので、自分でGUIPartsを書くときの参考になるだろう。
		/// </remarks>
		/// <param name="defFile"></param>
		public TextureButton(string defFile)
		{
			this.defFile = defFile;
			isDefFileRelative = false;
		}
		
		/// <summary>
		/// isDefFileRelative = trueにすれば
		/// 定義ファイル相対で、定義ファイルに書かれているファイルを
		/// 読み込むことが出来る。
		/// </summary>
		/// <param name="defFile"></param>
		/// <param name="isDefFileRelative"></param>
		public TextureButton(string defFile,bool isDefFileRelative)
		{
			this.defFile = defFile;
			this.isDefFileRelative = isDefFileRelative;
		}
		bool isDefFileRelative;

		private string defFile;

		public virtual void OnInit(ControlContext cc)
		{
			loader = cc.SmartTextureLoader.LoadDefFile(defFile,isDefFileRelative);

			/*
			// これ、ここでやってしまうと、コンテキスト選択されてないとおかしゅうなる。

			ITexture t = loader.GetTexture(0);
			// ボタンサイズを取得しておき、これをマウスの判定矩形として利用する
			if (t != null)
			{
				width = (int)t.Width;
				height = (int)t.Height;
			}
			 */
		}

		private int width, height;

		/// <summary>
		/// ボタンの色を設定/取得する
		/// (半透明にしたいときなどに設定して)
		/// </summary>
		public Color4ub? Color
		{
			get { return color; }
			set { color = value; }
		}
		private Color4ub? color;

		public virtual void OnPaint(IScreen scr , ControlContext cc , int x , int y)
		{
			if (!visible) return ;

			if (width == 0 || height == 0)
			{
				ITexture t0 = loader.GetTexture(0);
				// ボタンサイズを取得しておき、これをマウスの判定矩形として利用する
				if (t0 != null)
				{
					width = (int)t0.Width;
					height = (int)t0.Height;
				}
			}

			int mx, my;
			if (cc.MouseInput != null)
				cc.MouseInput.GetPos(out mx, out my);
			else
				mx = my = 0;

			bool isHover =
				(x <= mx) && (mx < x + width) &&
				(y <= my) && (my < y + height);

			bool isDown =
				(cc.MouseInput != null) ? cc.MouseInput.IsPress(MouseInput.Button.Left) : false;

			// 前フレームで、
			// isHover状態でかつisDown
			// 今回のフレームで
			// isHover状態かつ、!isDown
			// ならば、ボタンが押されたとしてボタンイベント発生。

			isOnDown = false;
			if (enable)
			{
				if (lastIsHover && lastIsDown && isHover && !isDown)
				{
					if ( OnClick != null )
					{
						OnClick(scr , cc);
					}
					isOnDown = true;
				}
				if (lastIsHover && !lastIsDown && isHover && isDown)
				{
					if (OnDown != null)
						OnDown(scr, cc);
				}
			}
			lastIsDown = isDown;
			lastIsHover = isHover;

			int pattern;
			if (!enable)
				pattern = 3;
			else if (!isHover)
				pattern = 0;
			else if (isDown)
				pattern = 1;
			else
				pattern = 2;

			ITexture tex = loader.GetTexture(pattern + loaderOffset);
			if ( tex != null )
			{
				if ( color != null )
				{
					scr.SetColor(color.Value);
					scr.Blt(tex , x , y);
					scr.ResetColor();
				}
				else
				{
					scr.Blt(tex , x , y);
				}
			}
		}

		public delegate void MouseEvent(IScreen scr , ControlContext cc);

		//　前フレームの結果
		private bool lastIsDown;
		private bool lastIsHover;

		/// <summary>
		/// 現在hoverされているのか
		/// </summary>
		/// <remarks>
		/// Enable == falseでもhoverの判定は行なう
		/// </remarks>
		public bool IsHover
		{
			get { return lastIsHover; }
		}

		/// <summary>
		/// このボタンが現在クリックされたところか
		/// (OnClickハンドラを書くのが面倒なときに使ってください)
		/// </summary>
		/// <remarks>
		/// Enable == falseならば常にfalseを返す
		/// </remarks>
		public bool IsDown
		{
			get { return isOnDown && enable; }
		}
		private bool isOnDown;

		/// <summary>
		/// クリックされたときのイベント
		/// </summary>
		public MouseEvent OnClick;

		/// <summary>
		/// 押し下げられた瞬間に発生するイベント
		/// </summary>
		public MouseEvent OnDown;

		private TextureLoader loader;

		/// <summary>
		/// このボタンは使用可能か不可能か。
		/// Enable = falseにすると、ボタンイメージ番号は3番で
		/// 使用不可能なボタンが表示され、その場合には、
		/// ボタンのイベントハンドラは呼び出されない。
		/// 
		/// defaultではEnable。
		/// </summary>
		public bool Enable
		{
			get { return enable; }
			set { enable = value; }
		}
		private bool enable = true;

		/// <summary>
		/// loaderに、表示するときのoffsetを指定できる。
		/// defaultでは0。たとえば、この数値として5を指定すれば、
		/// Disable時に描画されるテクスチャ番号は、本来の3から、3+5 = 8になる。
		/// </summary>
		public int LoaderOffset
		{
			get { return loaderOffset; }
			set { loaderOffset = value; }
		}
		private int loaderOffset;


		/// <summary>
		/// このボタンは表示されているか？
		/// Visible = falseにすると、何も表示されない。
		/// 
		/// defaultでは、true
		/// </summary>
		public bool Visible
		{
			get { return visible; }
			set { visible = value; }
		}
		private bool visible = true;

		public void Dispose()
		{
		//	loader.Dispose();
		// cacheしているのであえて解放しない
		}
	}
}
