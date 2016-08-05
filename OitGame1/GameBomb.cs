using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OitGame1
{
    public class GameBomb : GameObject
    {
        private static readonly double gravityAcceleration = 1.0 / 64;
        private static readonly double maxFallingSpeed = 4.0;

        private double vy;

        private bool vanishing;
        private int vanishingCount;

        private bool deleted;

        public GameBomb(GameWorld world, double x)
            : base(world)
        {
            CenterX = x;
            Bottom = 0;
            vy = 0;
            vanishing = false;
            vanishingCount = 0;
            deleted = false;
        }

        public void Update()
        {
            vy = Utility.AddClampMax(vy, gravityAcceleration, maxFallingSpeed);
            Y += vy;
            if (Bottom > World.FloorY)
            {
                Bottom = World.FloorY;
                vy = -vy / 8;
                vanishing = true;
            }
            if (vanishing)
            {
                vanishingCount++;
                if (vanishingCount >= 600)
                {
                    Delete(false);
                }
            }
        }

        public void Delete(bool explode)
        {
            if (!explode)
            {
                World.AddParticle(new SmallExplosion(World, CenterX, CenterY));
            }
            else
            {
                World.AddParticle(new Explosion(World, CenterX, CenterY));
            }
            deleted = true;
        }

        public void Draw(IGameGraphics graphics)
        {
            if (Right <= World.CameraLeft || Left >= World.CameraRight) return;
            var drawX = (int)Math.Round(X) - World.CameraLeft;
            var drawY = (int)Math.Round(Y);
            graphics.DrawImage(GameImage.Bomb, drawX, drawY);
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
