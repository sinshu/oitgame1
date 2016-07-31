using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OitGame1
{
    public class SmallExplosion : Particle
    {
        private int animation;

        public SmallExplosion(GameWorld world, double x, double y)
            : base(world, x, y)
        {
            animation = 0;
        }

        public override void Update()
        {
            animation++;
            if (animation >= 16)
            {
                Delete();
            }
        }

        public override void Draw(IGameGraphics graphics)
        {
            if (Right <= World.CameraLeft || Left >= World.CameraRight) return;
            var drawX = (int)Math.Round(X - World.CameraLeft);
            var drawY = (int)Math.Round(Y);
            var anim = animation;
            var row = anim / 4;
            var col = anim % 4;
            graphics.DrawImage(GameImage.SmallExplosion, 32, 32, row, col, drawX, drawY);
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
