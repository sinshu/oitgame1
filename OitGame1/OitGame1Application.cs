using System;
using Yanesdk.Ytl;
using Yanesdk.Draw;
using Yanesdk.Timer;
using Yanesdk.System;

namespace OitGame1
{
    public class OitGame1Application : IDisposable
    {
        private SDLWindow window;
        private SdlGraphics graphics;
        private SdlInput input;

        private GameWorld world;

        private FpsTimer timer;

        public OitGame1Application(bool fullscreen, int inputDeviceCount)
        {
            window = new SDLWindow();
            window.SetCaption("OitGame1");
            var bpp = fullscreen ? 32 : 0;
            window.BeginScreenTest();
            window.TestVideoMode(Setting.ScreenWidth, Setting.ScreenHeight, bpp);
            window.EndScreenTest();
            graphics = new SdlGraphics(window);
            input = new SdlInput(fullscreen, inputDeviceCount);
            world = new GameWorld(inputDeviceCount);
        }

        public void Run()
        {
            timer = new FpsTimer();
            timer.Fps = 60;
            while (SDLFrame.PollEvent() == YanesdkResult.NoError)
            {
                input.Update();
                world.Update(input.GetCurrent());
                world.Draw(graphics);
                if (input.Quit())
                {
                    return;
                }
                timer.WaitFrame();
            }
        }

        public void Dispose()
        {
            Console.WriteLine("OitGame1Application.Dispose");
            if (input != null)
            {
                input.Dispose();
                input = null;
            }
            if (graphics != null)
            {
                graphics.Dispose();
                graphics = null;
            }
            if (window != null)
            {
                window.Dispose();
                window = null;
            }
        }
    }
}
