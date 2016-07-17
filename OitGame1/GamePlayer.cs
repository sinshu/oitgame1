using System;
using System.Collections.Generic;

namespace OitGame1
{
    public class GamePlayer : GameObject
    {
        private static readonly double floorY = Setting.ScreenHeight - 64;
        private static readonly double gravityAcceleration = 0.5;
        private static readonly double maxFallingSpeed = 8.0;

        private static readonly double accelerationOnGround = 1.0;
        private static readonly double accelerationInAir = 0.5;
        private static readonly double maxMovingSpeed = 4.0;

        private readonly GameWorld world;
        private readonly int playerIndex;

        private double vx;
        private double vy;

        public GamePlayer(GameWorld world, int playerIndex, double x)
        {
            this.world = world;
            this.playerIndex = playerIndex;
            CenterX = x;
        }

        public void Update(GameCommand command)
        {
            vy = Utility.AddClampMax(vy, gravityAcceleration, maxFallingSpeed);
            Bottom = Utility.AddClampMax(Bottom, vy, floorY);
            
            if (command.Left == command.Right)
            {
                vx = Utility.DecreaseAbs(vx, 0.5);
            }
            else if (command.Left)
            {
                vx = Utility.AddClampMin(vx, -accelerationOnGround, -maxMovingSpeed);
            }
            else if (command.Right)
            {
                vx = Utility.AddClampMax(vx, accelerationOnGround, maxMovingSpeed);
            }
            X += vx;

            if (command.Jump) vy = -12;
            if (Left < world.CameraLeft)
            {
                Left = world.CameraLeft;
            }
            if (Right > world.CameraRight)
            {
                Right = world.CameraRight;
            }
        }

        public void Draw(IGameGraphics graphics)
        {
            var drawX = (int)Math.Round(X - world.CameraLeft);
            var drawY = (int)Math.Round(Y);
            graphics.Test(drawX, drawY);
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
