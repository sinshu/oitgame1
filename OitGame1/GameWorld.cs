using System;

namespace OitGame1
{
    public class GameWorld
    {
        private int inputDeviceCount;

        private int x;
        private int y;

        public GameWorld(int inputDeviceCount)
        {
            this.inputDeviceCount = inputDeviceCount;
            x = 100;
            y = 300;
        }

        public void Update(GameCommand[] command)
        {
            if (command[0].Left)
            {
                x--;
            }
            if (command[0].Right)
            {
                x++;
            }
        }

        public void Draw(IGraphics graphics)
        {
            graphics.Begin();
            graphics.SetColor(255, 128, 128, 192);
            graphics.DrawRectangle(0, 0, Setting.ScreenWidth, Setting.ScreenHeight);
            graphics.SetColor(255, 255, 255, 255);
            graphics.Test(x, y);
            graphics.End();
        }
    }
}
