using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OitGame1
{
    public class GameCoin : GameObject
    {
        private static readonly double gravityAcceleration = 1.0 / 64;
        private static readonly double maxFallingSpeed = 4.0;

        private double vx;
        private double vy;

        private int animation;

        private bool deleted;

        public GameCoin(GameWorld world, double x)
            : base(world)
        {
            CenterX = x;
            Bottom = 0;
            vx = 0;
            vy = 0;
            animation = world.Random.Next(60);
            deleted = false;
        }

        public GameCoin(GameWorld world, double x, double y, double vy, double vx)
            : base(world)
        {
            CenterX = x;
            CenterY = y;
            this.vx = vx;
            this.vy = vy;
            animation = world.Random.Next(60);
        }

        public void Update()
        {
            vy = Utility.AddClampMax(vy, gravityAcceleration, maxFallingSpeed);
            Y += vy;
            animation = (animation + 1) % 60;
            if (Bottom > World.FloorY)
            {
                Bottom = World.FloorY;
                vy = -vy / 4;
            }
        }

        public void Delete()
        {
            deleted = true;
        }

        public void Draw(IGameGraphics graphics)
        {
            if (Right <= World.CameraLeft || Left >= World.CameraRight) return;
            var drawX = (int)Math.Round(X - World.CameraLeft);
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

        public bool Deleted
        {
            get
            {
                return deleted;
            }
        }
    }
}
