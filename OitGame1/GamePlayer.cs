using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OitGame1
{
    public class GamePlayer : GameObject
    {
        private static readonly double floorY = Setting.ScreenHeight - 64;
        private static readonly double gravityAcceleration = 0.5;
        private static readonly double maxFallingSpeed = 8.0;

        private static readonly double accelerationOnGround = 1.0;
        private static readonly double accelerationInAir = 0.5;
        private static readonly double maxMovingSpeed = 6.0;

        private static readonly double jumpUpSpeed = 8.0;
        private static readonly int initJumpUpDuration = 16;

        private static readonly double penaltyFactor = 1.0 / 32.0;
        private static readonly double maxPenalty = 4.0;
        private static readonly double maxSpeed = 12.0;

        private static readonly int animationCount = 4;
        private static readonly double distancePerAnimation = 24.0;

        private readonly GameWorld world;
        private readonly int playerIndex;

        private double vx;
        private double vy;

        private Direction direction;
        private State state;

        private int jumpUpDuration;
        private bool canJump;

        private int damageDuration;
        private bool canMove;

        private double walkingDistance;

        public GamePlayer(GameWorld world, int playerIndex, double x)
        {
            CenterX = x;
            Bottom = floorY;
            this.world = world;
            this.playerIndex = playerIndex;
            vx = 0;
            vy = 0;
            direction = Direction.Left;
            state = State.OnGround;
            jumpUpDuration = 0;
            canJump = true;
            damageDuration = 0;
            canMove = true;
            walkingDistance = 0;
        }

        public void Update1(GameCommand command)
        {
            UpdateX(command);
            UpdateY(command);
        }

        public void Update2()
        {
            ProcessCollision();
        }

        private void UpdateX(GameCommand command)
        {
            var acceleration = state == State.OnGround ? accelerationOnGround : accelerationInAir;
            if (command.Left == command.Right || !canMove)
            {
                vx = Utility.DecreaseAbs(vx, acceleration / 2);
                if (vx == 0) walkingDistance = 0;
            }
            else
            {
                if (command.Left)
                {
                    if (vx == 0)
                    {
                        walkingDistance = distancePerAnimation;
                    }
                    vx = Utility.AddClampMin(vx, -acceleration, -maxMovingSpeed);
                    direction = Direction.Left;
                    if (vx < 0)
                    {
                        walkingDistance += Math.Abs(vx);
                    }
                    else
                    {
                        walkingDistance = 0;
                    }
                }
                else if (command.Right)
                {
                    if (vx == 0)
                    {
                        walkingDistance = distancePerAnimation;
                    }
                    vx = Utility.AddClampMax(vx, acceleration, maxMovingSpeed);
                    direction = Direction.Right;
                    if (vx > 0)
                    {
                        walkingDistance += Math.Abs(vx);
                    }
                    else
                    {
                        walkingDistance = 0;
                    }
                }
                else
                {
                    Debug.Assert(false);
                }
            }
            vx = Utility.ClampAbs(vx, maxSpeed);
            X += vx;
            if (Left < world.CameraLeft)
            {
                Left = world.CameraLeft;
                vx = 0;
            }
            if (Right > world.CameraRight)
            {
                Right = world.CameraRight;
                vx = 0;
            }
            if (walkingDistance > animationCount * distancePerAnimation)
            {
                walkingDistance -= animationCount * distancePerAnimation;
            }
        }

        private void UpdateY(GameCommand command)
        {
            if (state == State.InAir)
            {
                vy = Utility.AddClampMax(vy, gravityAcceleration, maxFallingSpeed);
            }
            if (command.Jump && canMove)
            {
                if (state == State.OnGround && canJump)
                {
                    vy = Math.Min(-jumpUpSpeed, vy);
                    state = State.InAir;
                    jumpUpDuration = initJumpUpDuration;
                    canJump = false;
                }
                else if (state == State.InAir)
                {
                    if (jumpUpDuration > 0)
                    {
                        vy = Math.Min(-jumpUpSpeed, vy);
                        jumpUpDuration--;
                    }
                }
                else
                {
                    Debug.Assert(false);
                }
            }
            else
            {
                jumpUpDuration = 0;
            }
            vy = Utility.ClampAbs(vy, maxSpeed);
            Y += vy;
            state = State.InAir;
            if (Bottom >= floorY)
            {
                Bottom = floorY;
                vy = 0;
                state = State.OnGround;
            }
            if (!command.Jump)
            {
                canJump = true;
            }
        }

        private void ProcessCollision()
        {
            foreach (var other in world.Players)
            {
                if (other == this) continue;
                if (IsOverlappedWith(other))
                {
                    var dx = CenterX - other.CenterX;
                    var dy = CenterY - other.CenterY;
                    double overlapX;
                    double overlapY;
                    if (dx < 0)
                    {
                        overlapX = Right - other.Left;
                    }
                    else
                    {
                        overlapX = other.Right - Left;
                    }
                    if (dy < 0)
                    {
                        overlapY = Bottom - other.Top;
                    }
                    else
                    {
                        overlapY = other.Bottom - Top;
                    }
                    var penalty = penaltyFactor * overlapX * overlapY;
                    var bunbo = Math.Abs(dx) + Math.Abs(dy);
                    if (bunbo < 0.000000001) continue;
                    var penaltyX = Utility.ClampAbs(dx / bunbo * penalty, maxPenalty);
                    var penaltyY = Utility.ClampAbs(dy / bunbo * penalty, maxPenalty);
                    vx += penaltyX;
                    vy += penaltyY;
                }
            }
        }

        public void Draw(IGameGraphics graphics)
        {
            var drawOffset = (32 - Width) / 2;
            var drawX = (int)Math.Round(X - world.CameraLeft - drawOffset);
            var drawY = (int)Math.Round(Y);
            var rowOffset = 2 * (playerIndex % 4);
            if (state == State.OnGround)
            {
                var anim = (int)(walkingDistance / distancePerAnimation);
                if (direction == Direction.Right)
                {
                    anim += 4;
                }
                graphics.DrawImage(GameImage.Player, 32, 32, rowOffset, anim, drawX, drawY);
            }
            else if (state == State.InAir)
            {
                var anim = 0;
                if (vy > 0)
                {
                    if (vy < maxFallingSpeed)
                    {
                        anim = 1;
                    }
                    else
                    {
                        anim = 2;
                    }
                }
                if (direction == Direction.Right)
                {
                    anim += 4;
                }
                graphics.DrawImage(GameImage.Player, 32, 32, rowOffset + 1, anim, drawX, drawY);
            }
            else
            {
                Debug.Assert(false);
            }
        }

        public override double Width
        {
            get
            {
                return 24;
            }
        }

        public override double Height
        {
            get
            {
                return 32;
            }
        }

        private enum Direction
        {
            Left,
            Right
        }

        private enum State
        {
            OnGround,
            InAir
        }
    }
}
