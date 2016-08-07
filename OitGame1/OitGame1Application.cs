using System;
using Yanesdk.Ytl;
using Yanesdk.Draw;
using Yanesdk.Timer;
using Yanesdk.System;

namespace OitGame1
{
    public class OitGame1Application : IDisposable
    {
        private readonly SDLWindow window;
        private readonly SdlGraphics graphics;
        private readonly SdlAudio audio;
        private readonly SdlInput input;

        private readonly GameWorld world;

        private FpsTimer timer;

        public OitGame1Application(bool fullscreen, int playerCount)
        {
            window = new SDLWindow();
            window.SetCaption("OitGame1");
            var bpp = fullscreen ? 32 : 0;
            window.BeginScreenTest();
            window.TestVideoMode(Setting.ScreenWidth, Setting.ScreenHeight, bpp);
            window.EndScreenTest();
            graphics = new SdlGraphics(window);
            audio = new SdlAudio();
            input = new SdlInput(fullscreen, playerCount);
            world = new GameWorld(playerCount);
            world.Audio = audio;
        }

        public void Run()
        {
            audio.StartMusic();
            timer = new FpsTimer();
            timer.Fps = 60;
            while (SDLFrame.PollEvent() == YanesdkResult.NoError)
            {
                input.Update();
                world.Update(input.Current);
                if (!timer.ToBeSkip)
                {
                    world.Draw(graphics);
                }
                if (input.Quit())
                {
                    return;
                }
                timer.WaitFrame();
            }
        }

        public void Dispose()
        {
            //Console.WriteLine("OitGame1Application.Dispose");
            input.Dispose();
            audio.Dispose();
            graphics.Dispose();
            window.Dispose();
        }
    }
}
