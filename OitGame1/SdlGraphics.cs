using System;
using Yanesdk.Ytl;
using Yanesdk.Draw;
using Yanesdk.Timer;
using Yanesdk.System;

namespace OitGame1
{
    public class SdlGraphics : IGraphics, IDisposable
    {
        private SDLWindow window;
        private IScreen screen;
        private GlTexture test;

        public SdlGraphics(SDLWindow window)
        {
            this.window = window;
            screen = window.Screen;
            Begin();
            screen.SetClearColor(128, 128, 128);
            screen.Clear();
            End();
            test = new GlTexture();
            var result = test.Load("images/test.png");
            Console.WriteLine(result);
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

        public void Test(int x, int y)
        {
            screen.Blt(test, x, y);
        }

        public void Dispose()
        {
            Console.WriteLine("SdlGraphics.Dispose");
            if (test != null)
            {
                test.Dispose();
                test = null;
            }
        }
    }
}
