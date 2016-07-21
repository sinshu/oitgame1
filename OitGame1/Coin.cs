using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OitGame1
{
    public class GameCoin : GameObject
    {
        private GameWorld world;
        private int animation;

        public GameCoin(GameWorld world, double x, double y, int animation)
            : base(world)
        {
            CenterX = x;
            CenterY = y;
            this.world = world;
            this.animation = animation;
        }

        public void Update()
        {
            Y += 2;
            animation = (animation + 1) % 60;
            if (Top >= Setting.ScreenHeight)
            {
                Bottom = 0;
            }
        }

        public void Draw(IGameGraphics graphics)
        {
            if (Right <= World.CameraLeft || Left >= World.CameraRight) return;
            var drawX = (int)Math.Round(X - world.CameraLeft);
            var drawY = (int)Math.Round(Y);
            var row = animation / 2 / 8;
            var col = animation / 2 % 8;
            graphics.DrawImage(GameImage.Coin, 32, 32, row, col, drawX, drawY);
        }

        public override double Width
        {
            get
            {
                return 32;
            }
        }

        public override double Height
        {
            get
            {
                return 32;
            }
        }
    }
}
