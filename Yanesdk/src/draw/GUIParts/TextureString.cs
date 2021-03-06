using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Yanesdk.Draw;

namespace Yanesdk.GUIParts
{
	/// <summary>
	/// Textureに書かれた文字列(文字以外にGraphicsで描けるものは何でも描けるが)
	/// 
	/// openglはHDCをとれないので文字列を描画したければ
	/// 事前にBitmapに描いて、Textureを作成し、そこに転送する必要がある。
	/// </summary>
	/// <remarks>
	/// ここで生成するTextureは動的に生成するTextureなので、
	/// cache systemでcacheする価値はないと考えられる。
	/// 
	/// (cache systemで自動的に破棄されても再構築するときのにcallbackがかかるのは
	/// 非常に扱いづらい)
	/// 
	/// よって、ここで動的に生成したTextureは、cache systemに対してpressureを
	/// かけるにとどめるべきである。
	/// 
	/// しかし、通例、文字は微量だし、次のシーンに行くときに解体されるので
	/// (TextureControl.Disposeが次のシーン行くときに呼び出されるならば)
	/// あえてpressureをかけるほどでもない、と考えることが出来る。
	/// 
	/// ゆえに、ここで生成されるTextureに関しては、完全にcache systemの管轄外とする。
	/// 
	/// </remarks>
	public class TextureString : ITextureGUI , IDisposable
	{
		public TextureString() { }

		/// <summary>
		/// SetDrawHandlerに渡すパラメータをコンストラクタで設定できる
		/// </summary>
		/// <param name="drawDelegate"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public TextureString(int width , int height,OnDrawDelegate drawDelegate)
		{
			SetDrawHandler(width , height , drawDelegate);
		}

		public TextureString(int width, int height, OnDrawDelegateHelper drawDelegateHelper)
		{
			SetDrawHandler(width, height, drawDelegateHelper);
		}

		/// <summary>
		/// 描画用のハンドラとサーフェースのサイズを指定する
		/// </summary>
		/// <param name="drawDelegate"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public void SetDrawHandler(int width , int height,OnDrawDelegate drawDelegate)
		{
			this.drawDelegate = drawDelegate;
			this.width = width;
			this.height = height;
		}

		/// <summary>
		/// 描画用のハンドラとサーフェースのサイズを指定する
		/// </summary>
		/// <param name="drawDelegate"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public void SetDrawHandler(int width, int height, OnDrawDelegateHelper drawDelegateHelper)
		{
			this.drawDelegateHelper = drawDelegateHelper;
			this.width = width;
			this.height = height;
		}


		public void OnInit(ControlContext cc)
		{
			texture = cc.SmartTextureLoader.Factory() as ITexture;
		//	Update();
			// ここでUpdateすると、OnInitの間にテクスチャーの生成が行なわれることになって、
			// そのためには Screen.Select～Unselectがなされていることが条件となるので
			// 好ましくない。
		}

		public void OnPaint(IScreen scr , ControlContext cc , int x , int y)
		{
			if ( visible )
			{
				Update();
				scr.BlendSrcAlpha(); // 背景が描画されてはいけないので…。
				scr.Blt(texture , x , y);
			}
		}

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
			texture.Dispose();
		}

		/// <summary>
		/// 描画すべき文字
		/// 
		/// 設定しておけば次回のOnPaintで自動的にOnDrawDelegateが
		/// 呼び出されて描画される。
		/// </summary>
		public string DrawString
		{
			get { return drawString; }
			set {
				if ( drawString != value )
				{
					drawString = value;
					isDirty = true;
				}
			}
		}
		private string drawString;

		/// <summary>
		/// 文字列に変更があったかのフラグ
		/// </summary>
		public bool isDirty = true;
		
		/// <summary>
		/// 描画用のdelegate
		/// 
		/// gに対して、文字列drawStringを書き殴ってください。
		/// 
		/// </summary>
		public delegate void OnDrawDelegate(Graphics g,string drawString);
		public delegate void OnDrawDelegateHelper(Graphics g, string drawString, DrawStringHelperDelegate d);
		public delegate void DrawStringHelperDelegate( Graphics g, global::System.Drawing.Font font, Brush brush, HPosition hPos, VPosition vPos );

		private OnDrawDelegate drawDelegate;
		private OnDrawDelegateHelper drawDelegateHelper;

		/// <summary>
		/// 生成するtextureの幅と高さ
		/// </summary>
		private int width , height;

		/// <summary>
		/// 汚れフラグが立っているならupdateする
		/// </summary>
		public void Update()
		{
			if ( isDirty )
			{
				if (drawDelegate != null || drawDelegateHelper != null)
				{
					Bitmap bitmap = new Bitmap(width , height);

					if (drawString!=null )
					{
						Graphics g = Graphics.FromImage(bitmap);
						g.TextRenderingHint = global::System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
						// ↑どうせ一回しか描画しないので、綺麗に描画しておこう。


						if (drawDelegate != null)
							drawDelegate(g, drawString);
						else if (drawDelegateHelper != null)
						{
							drawDelegateHelper(g, drawString, new DrawStringHelperDelegate(this.DrawStringHelper));
						}
						g.Dispose();
					}

					texture.SetBitmap(bitmap);
					bitmap.Dispose();
				}
				isDirty = false;
			}
		}

		/*	
		// 描画文字列のサイズを取得するコード例
                    // 文字列のサイズを取得する。
                    SizeF size = g.MeasureString(text, font);
                    // 文字列のサイズをキャプションに設定する。
                    Text = String.Format("W = {0}, H = {1}", size.Width, size.Height);
		*/

		#region drawDelegate内で使うと便利なヘルパ関数

		/// <summary>
		///		縦位置
		/// </summary>
		public enum VPosition
		{ 
			Top,		// 上寄せ
			Middle,		// 中心
			Bottom,		// 下寄せ
		}

		/// <summary>
		///		横位置
		/// </summary>
		public enum HPosition
		{
			Left,		// 左寄せ
			Center,		// センタリング
			Right,		// 右寄せ
		}

		/// <summary>
		///		縦横位置を指定して矩形領域内に文字を描画する。
		/// </summary>
		public void DrawStringHelper(Graphics g, global::System.Drawing.Font font, Brush brush, HPosition hPos, VPosition vPos)
		{
			PointF drawpos = new PointF(0,0);

			if( hPos == HPosition.Left && vPos == VPosition.Top )
			{
				g.DrawString(drawString, font, global::System.Drawing.Brushes.Black, drawpos );
				return;
			}

			SizeF strsize =	g.MeasureString( drawString, font );

			if( hPos == HPosition.Right )
				drawpos.X = width - strsize.Width -1;
			else if( hPos == HPosition.Center )
				drawpos.X = (width - strsize.Width) / 2;

			if( vPos == VPosition.Bottom )
				drawpos.Y = height - strsize.Height -1;
			else if( vPos == VPosition.Middle )
				drawpos.Y = (height - strsize.Height) / 2;

			g.DrawString(drawString, font, brush, drawpos);
		
		}


		#endregion

		/// <summary>
		/// 描画すべき文字列が入ったテクスチャ
		/// </summary>
		private ITexture texture;
	}
}
