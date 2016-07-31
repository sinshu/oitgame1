using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OitGame1
{
    public class Explosion : Particle
    {
        private int animation;

        public Explosion(GameWorld world, double x, double y)
            : base(world, x, y)
        {
            animation = 0;
        }

        public override void Update()
        {
            animation++;
            if (animation >= 8)
            {
                Delete();
            }
        }

        public override void Draw(IGameGraphics graphics)
        {
            if (Right <= World.CameraLeft || Left >= World.CameraRight) return;
            var drawX = (int)Math.Round(X - World.CameraLeft);
            var drawY = (int)Math.Round(Y);
            var anim = animation / 2;
            graphics.DrawImage(GameImage.Explosion, 128, 128, 0, anim, drawX, drawY);
        }

        public override double Width
        {
            get
            {
                return 128;
            }
        }

        public override double Height
        {
            get
            {
                return 128;
            }
        }
    }
}
