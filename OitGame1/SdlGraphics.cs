using System;
using Yanesdk.Ytl;
using Yanesdk.Draw;
using Yanesdk.Timer;
using Yanesdk.System;

namespace OitGame1
{
    public class SdlGraphics : IGameGraphics, IDisposable
    {
        private readonly SDLWindow window;
        private readonly IScreen screen;

        private ITexture[] textures;

        public SdlGraphics(SDLWindow window)
        {
            this.window = window;
            screen = window.Screen;
            Begin();
            screen.SetClearColor(128, 128, 128);
            screen.Clear();
            End();
            LoadTextures();
        }

        private void LoadTextures()
        {
            var textureCount = Enum.GetValues(typeof(GameImage)).Length;
            textures = new ITexture[textureCount];
            for (var i = 0; i < textureCount; i++)
            {
                var path = "images/" + Enum.GetName(typeof(GameImage), i) + ".png";
                Console.WriteLine(path);
                textures[i] = GetTexture(path);
            }
        }

        public void Begin()
        {
            screen.Select();
            screen.Blend = true;
            screen.BlendSrcAlpha();
        }

        public void End()
        {
            screen.Update();
        }

        public void SetColor(int a, int r, int g, int b)
        {
            screen.SetColor(r, g, b, a);
        }

        public void DrawRectangle(int x, int y, int width, int height)
        {
            var x1 = x;
            var y1 = y;
            var x2 = x + width;
            var y2 = y;
            var x3 = x + width;
            var y3 = y + height;
            var x4 = x;
            var y4 = y + height;
            screen.DrawPolygon(x1, y1, x2, y2, x3, y3, x4, y4);
        }

        public void DrawImage(GameImage image, int x, int y)
        {
            var texture = textures[(int)image];
            screen.Blt(texture, x, y);
        }

        public void DrawImage(GameImage image, int width, int height, int row, int col, int x, int y)
        {
            var left = width * col;
            var top = height * row;
            var right = left + width;
            var bottom = top + height;
            var rect = new Rect(left, top, right, bottom);
            var texture = textures[(int)image];
            screen.Blt(texture, x, y, rect);
        }

        public void Test(int x, int y)
        {
            screen.Blt(textures[0], x, y);
        }

        private static ITexture GetTexture(string path)
        {
            var texture = new GlTexture();
            texture.LocalOption.Smooth = false;
            var result = texture.Load(path);
            if (result == YanesdkResult.NoError)
            {
                return texture;
            }
            else
            {
                throw new Exception("画像 '" + path + "' の読み込みに失敗しました。");
            }
        }

        public void Dispose()
        {
            Console.WriteLine("SdlGraphics.Dispose");
            foreach (var texture in textures)
            {
                texture.Dispose();
            }
        }
    }
}
