using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OitGame1
{
    public class GameCoin : GameObject
    {
        private static readonly double gravityAcceleration = 1.0 / 64;
        private static readonly double maxFallingSpeed = 4.0;

        private static readonly double fastGravityAcceleration = 1.0 / 4;
        private static readonly double fastMaxFallingSpeed = 8.0;

        private double vx;
        private double vy;

        private bool fast;
        private GamePlayer parent;

        private int animation;

        private bool vanishing;
        private int vanishingCount;

        private bool deleted;

        public GameCoin(GameWorld world, double x)
            : base(world)
        {
            CenterX = x;
            Bottom = 0;
            vx = 0;
            vy = 0;
            fast = false;
            parent = null;
            animation = world.Random.Next(60);
            vanishing = false;
            vanishingCount = 0;
            deleted = false;
        }

        public GameCoin(GameWorld world, double x, double y, double vy, double vx, GamePlayer parent)
            : base(world)
        {
            CenterX = x;
            CenterY = y;
            this.vx = vx;
            this.vy = vy;
            fast = true;
            this.parent = parent;
            animation = world.Random.Next(60);
        }

        public void Update()
        {
            if (!fast)
            {
                vy = Utility.AddClampMax(vy, gravityAcceleration, maxFallingSpeed);
            }
            else
            {
                vy = Utility.AddClampMax(vy, fastGravityAcceleration, fastMaxFallingSpeed);
            }
            Y += vy;
            X += vx;
            animation = (animation + 1) % 60;
            if (Bottom > World.FloorY)
            {
                Bottom = World.FloorY;
                if (!fast)
                {
                    vy = -vy / 4;
                }
                else
                {
                    vy = -vy / 2;
                }
                vx *= 1.0 - 1.0 / 16;
                vanishing = true;
            }
            if (vanishing)
            {
                vanishingCount++;
                if (vanishingCount >= 180)
                {
                    Delete(false);
                }
            }
        }

        public void Delete(bool withStar)
        {
            if (withStar)
            {
                var n = 10;
                var phase = 2 * Math.PI * World.Random.NextDouble();
                for (var i = 0; i < n; i++)
                {
                    var theta = 2 * Math.PI * i / n + phase;
                    var cos = Math.Cos(theta);
                    var sin = -Math.Sin(theta);
                    var x = CenterX + 8 * cos;
                    var y = CenterY + 8 * sin;
                    var tx = CenterX + 64 * cos;
                    var ty = CenterY + 64 * sin;
                    World.AddParticle(new Star(World, x, y, tx, ty));
                }
            }
            else
            {
                World.AddParticle(new SmallExplosion(World, CenterX, CenterY));
            }
            deleted = true;
        }

        public void Draw(IGameGraphics graphics)
        {
            if (Right <= World.CameraLeft || Left >= World.CameraRight) return;
            var drawX = (int)Math.Round(X) - World.CameraLeft;
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

        public GamePlayer Parent
        {
            get
            {
                return parent;
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
