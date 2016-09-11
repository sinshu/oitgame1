using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OitGame1
{
    public class GamePlayer : GameObject
    {
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

        private bool ready;

        private int coinCount;

        private int trophyCount;
        private int unkoCount;

        public GamePlayer(GameWorld world, int playerIndex, double x)
            : base(world)
        {
            CenterX = x;
            Bottom = world.FloorY;
            this.playerIndex = playerIndex;
            vx = 0;
            vy = 0;
            direction = Direction.Right;
            state = State.OnGround;
            jumpUpDuration = 0;
            canJump = true;
            Reset();
            ResetCoinCount();
            trophyCount = 0;
            unkoCount = 0;
        }

        public void Reset()
        {
            damageDuration = 0;
            canMove = true;
            walkingDistance = 0;
            ready = false;
        }

        public void ResetCoinCount()
        {
            coinCount = 0;
        }

        public void Update1(GameCommand command)
        {
            UpdateX(command);
            UpdateY(command);
            if (!ready && command.Start)
            {
                ready = true;
                World.PlaySound(GameSound.Ready);
            }
            if (!canMove)
            {
                CreateSmoke();
                if (damageDuration > 0)
                {
                    damageDuration--;
                }
                else
                {
                    canMove = true;
                }
            }
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
            if (Left < World.CameraLeft)
            {
                Left = World.CameraLeft;
                vx = 0;
            }
            if (Right > World.CameraRight)
            {
                Right = World.CameraRight;
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
                    World.PlaySound(GameSound.Jump);
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
            if (Bottom >= World.FloorY)
            {
                Bottom = World.FloorY;
                vy = 0;
                state = State.OnGround;
            }
            if (!command.Jump)
            {
                canJump = true;
            }
        }

        private void CreateSmoke()
        {
            int n, p;
            if (damageDuration > 170)
            {
                n = 3;
                p = 1;
            }
            else if (damageDuration > 150)
            {
                n = 1;
                p = 1;
            }
            else if (damageDuration > 120)
            {
                n = 1;
                p = 4;
            }
            else if (damageDuration > 90)
            {
                n = 1;
                p = 8;
            }
            else
            {
                n = 1;
                p = 16;
            }
            for (var i = 0; i < n; i++)
            {
                var offsetX = 16 * World.Random.NextDouble() - 8;
                var offsetY = 16 * World.Random.NextDouble() - 8;
                if (World.Random.Next(0, p) == 0)
                {
                    World.AddParticle(new Smoke(World, CenterX + offsetX, CenterY + offsetY));
                }
            }
        }

        private void ProcessCollision()
        {
            foreach (var other in World.Players)
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

        public void GetCoin(GameCoin coin)
        {
            coinCount++;
            World.PlaySound(GameSound.Coin);
        }

        public void GetBomb(GameBomb bomb)
        {
            var dx = CenterX - bomb.CenterX;
            var dy = CenterY - bomb.CenterY;
            var bunbo = Math.Abs(dx) + Math.Abs(dy);
            if (bunbo >= 0.000000001)
            {
                vx += maxSpeed * dx;
                vy += maxSpeed * dy;
            }
            canMove = false;
            damageDuration = 180;
            var coinPenalty = (int)Math.Ceiling((double)coinCount / 5);
            if (coinCount < coinPenalty)
            {
                coinPenalty = coinCount;
            }
            coinCount -= coinPenalty;
            if (coinPenalty > 0)
            {
                var phase = 2 * Math.PI * World.Random.NextDouble();
                for (var i = 0; i < coinPenalty; i++)
                {
                    var theta = 2 * Math.PI * i / coinPenalty;
                    var cvx = 8 * Math.Cos(theta + phase) + World.Random.NextDouble() - 0.5;
                    var cvy = 8 * -Math.Sin(theta + phase) + World.Random.NextDouble() - 0.5;
                    World.AddCoin(new GameCoin(World, CenterX, CenterY, cvx, cvy, this));
                }
            }
            World.PlaySound(GameSound.Bomb);
        }

        public void Draw(IGameGraphics graphics)
        {
            if (playerIndex >= 4)
            {
                graphics.SetColor(255, 0, 255, 0);
            }
            var drawOffset = (int)((32 - Width) / 2);
            var drawX = (int)Math.Round(X) - World.CameraLeft - drawOffset;
            var drawY = (int)Math.Round(Y);
            var rowOffset = 2 * (playerIndex % 4);
            if (canMove)
            {
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
            else
            {
                var anim = 3;
                if (direction == Direction.Right)
                {
                    anim += 4;
                }
                graphics.DrawImage(GameImage.Player, 32, 32, rowOffset + 1, anim, drawX, drawY);
            }
            graphics.SetColor(255, 255, 255, 255);
        }

        public void DrawState(IGameGraphics graphics, int rank)
        {
            if (!ready)
            {
                if (rank >= 0)
                {
                    var r = rank;
                    if (r > 3) r = 3;
                    var drawX = (int)Math.Round(CenterX - World.CameraLeft - 32);
                    var drawY = (int)Math.Round(Top - 64);
                    graphics.DrawImage(GameImage.Rank, 64, 64, 0, r, drawX, drawY);
                }
            }
            else
            {
                var drawX = (int)Math.Round(CenterX - World.CameraLeft - 32);
                var drawY = (int)Math.Round(Top - 32);
                graphics.DrawImage(GameImage.Ready, drawX, drawY);
            }
        }

        public void AddTrophy()
        {
            trophyCount++;
        }

        public void AddUnko()
        {
            unkoCount++;
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

        public bool Ready
        {
            get
            {
                return ready;
            }
        }

        public bool CanMove
        {
            get
            {
                return canMove;
            }
        }

        public int PlayerIndex
        {
            get
            {
                return playerIndex;
            }
        }

        public int CoinCount
        {
            get
            {
                return coinCount;
            }
        }

        public int TrophyCount
        {
            get
            {
                return trophyCount;
            }
        }

        public int UnkoCount
        {
            get
            {
                return unkoCount;
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
