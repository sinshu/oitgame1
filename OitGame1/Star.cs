using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OitGame1
{
    public class Star : Particle
    {
        private static readonly double alpha = 1.0 - 1.0 / 8;

        private double tx;
        private double ty;

        private int animation;

        public Star(GameWorld world, double x, double y, double tx, double ty)
            : base(world, x, y)
        {
            this.tx = tx;
            this.ty = ty;
            animation = 0;
        }

        public override void Update()
        {
            CenterX = alpha * CenterX + (1 - alpha) * tx;
            CenterY = alpha * CenterY + (1 - alpha) * ty;
            animation++;
            if (animation >= 32)
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
            var row = anim / 4;
            var col = anim % 4;
            graphics.DrawImage(GameImage.Star, 16, 16, row, col, drawX, drawY);
        }

        public override double Width
        {
            get
            {
                return 16;
            }
        }

        public override double Height
        {
            get
            {
                return 16;
            }
        }
    }
}
