using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OitGame1
{
    public class Smoke : Particle
    {
        private int animation;
        private int rotation;

        public Smoke(GameWorld world, double x, double y)
            : base(world, x, y)
        {
            animation = 0;
            rotation = world.Random.Next(0, 512);
        }

        public override void Update()
        {
            Y -= 0.5 * World.Random.NextDouble() + 0.5;
            animation += World.Random.Next(0, 2) + 1;
            if (animation >= 128)
            {
                Delete();
            }
        }

        public override void Draw(IGameGraphics graphics)
        {
            if (Right <= World.CameraLeft || Left >= World.CameraRight) return;
            var drawX = (int)Math.Round(X) - World.CameraLeft;
            var drawY = (int)Math.Round(Y);
            var anim = animation / 4;
            var row = anim / 4;
            var col = anim % 4;
            graphics.DrawImage(GameImage.Smoke, 32, 32, row, col, drawX, drawY, rotation);
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
