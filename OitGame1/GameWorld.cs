using System;
using System.Collections.Generic;

namespace OitGame1
{
    public class GameWorld
    {
        private static readonly int fieldWidth = 1024;
        private static readonly int floorY = Setting.ScreenHeight - 64;

        private readonly int playerCount;
        private readonly GamePlayer[] players;

        private double cameraRealX;
        private int cameraIntX;

        private State state;
        private IEnumerator<int> stateCoroutine;
        private int sleepCount;
        private int countDown;

        private Random random;
        private List<GameCoin> coins;
        private int coinGenDuration;

        private int elapsedTime;
        private int remainingTime;

        public GameWorld(int playerCount)
        {
            this.playerCount = playerCount;
            players = new GamePlayer[playerCount];
            for (var i = 0; i < playerCount; i++)
            {
                players[i] = new GamePlayer(this, i, 100 * i);
            }
            SetCameraCenterX(GetAveragePlayerX());

            state = State.Waiting;
            stateCoroutine = StateCoroutine().GetEnumerator();
            sleepCount = 0;
            countDown = 0;

            random = new Random();
            coins = new List<GameCoin>();
            coinGenDuration = 0;

            elapsedTime = 0;
            remainingTime = 60 * 60;
        }

        public void Update(IList<GameCommand> command)
        {
            for (var i = 0; i < playerCount; i++)
            {
                players[i].Update1(command[i]);
            }
            for (var i = 0; i < playerCount; i++)
            {
                players[i].Update2();
            }
            SetCameraCenterX(GetAveragePlayerX());

            if (state == State.Playing)
            {
                if (coinGenDuration == 0)
                {
                    var x = fieldWidth * random.NextDouble();
                    coins.Add(new GameCoin(this, x));
                    coinGenDuration = 15 + random.Next(15);
                }
                else
                {
                    coinGenDuration--;
                }
            }
            foreach (var coin in coins)
            {
                coin.Update();
            }

            CheckCoinCollision();

            UpdateCoroutine();

            if (state == State.Playing)
            {
                elapsedTime++;
                if (remainingTime > 0)
                {
                    remainingTime--;
                }
            }
        }

        public void CheckCoinCollision()
        {
            foreach (var player in players)
            {
                foreach (var coin in coins)
                {
                    if (player.IsOverlappedWith(coin))
                    {
                        player.GetCoin(coin);
                        coin.Delete();
                    }
                }
            }
            coins.RemoveAll(coin => coin.Deleted);
        }

        public void UpdateCoroutine()
        {
            sleepCount--;
            if (sleepCount <= 0)
            {
                stateCoroutine.MoveNext();
                sleepCount = stateCoroutine.Current;
            }
        }

        private bool AllPlayersAreReady()
        {
            foreach (var player in players)
            {
                if (!player.Ready)
                {
                    return false;
                }
            }
            return true;
        }

        private IEnumerable<int> StateCoroutine()
        {
            while (true)
            {
                switch (state)
                {
                    case State.Waiting:
                        if (AllPlayersAreReady())
                        {
                            yield return 60;
                            state = State.Ready;
                        }
                        else
                        {
                            yield return 1;
                        }
                        break;
                    case State.Ready:
                        for (var i = 0; i < 3; i++)
                        {
                            yield return 60;
                            countDown++;
                        }
                        state = State.Playing;
                        yield return 1;
                        break;
                    case State.Playing:
                        yield return 1;
                        break;
                    default:
                        throw new Exception("＼(^o^)／");
                }
            }
        }

        public void Draw(IGameGraphics graphics)
        {
            graphics.Begin();
            graphics.SetColor(255, 128, 128, 192);
            graphics.DrawRectangle(0, 0, Setting.ScreenWidth, Setting.ScreenHeight);
            graphics.SetColor(255, 255, 255, 255);
            graphics.DrawImage(GameImage.Field, -cameraIntX, Setting.ScreenHeight - 128);
            graphics.SetColor(255, 255, 255, 255);
            foreach (var coin in coins)
            {
                coin.Draw(graphics);
            }
            foreach (var player in players)
            {
                player.Draw(graphics);
            }
            if (state == State.Waiting)
            {
                foreach (var player in players)
                {
                    player.DrawReady(graphics);
                }
            }
            else if (state == State.Ready)
            {
                if (countDown < 3)
                {
                    var drawX = (Setting.ScreenWidth - 128) / 2;
                    graphics.DrawImage(GameImage.Message, 128, 128, 0, countDown, drawX, 64);
                }
            }
            else if (state == State.Playing)
            {
                if (elapsedTime < 60)
                {
                    var drawX = (Setting.ScreenWidth - 256) / 2;
                    graphics.DrawImage(GameImage.Message, 256, 128, 1, 0, drawX, 64);
                }
            }
            graphics.End();
        }

        private void SetCameraCenterX(double x)
        {
            cameraRealX = x - Setting.ScreenWidth / 2;
            if (cameraRealX < 0)
            {
                cameraRealX = 0;
            }
            if (cameraRealX > fieldWidth - Setting.ScreenWidth)
            {
                cameraRealX = fieldWidth - Setting.ScreenWidth;
            }
            cameraIntX = (int)Math.Round(cameraRealX);
        }

        private double GetAveragePlayerX()
        {
            var min = double.MaxValue;
            var max = double.MinValue;
            foreach (var player in players)
            {
                if (player.Left < min)
                {
                    min = player.Left;
                }
                if (player.Right > max)
                {
                    max = player.Right;
                }
            }
            return (min + max) / 2;
        }

        public int CameraLeft
        {
            get
            {
                return cameraIntX;
            }
        }

        public int CameraRight
        {
            get
            {
                return cameraIntX + Setting.ScreenWidth;
            }
        }

        public IList<GamePlayer> Players
        {
            get
            {
                return players;
            }
        }

        public Random Random
        {
            get
            {
                return random;
            }
        }

        public int FieldWidth
        {
            get
            {
                return fieldWidth;
            }
        }

        public int FloorY
        {
            get
            {
                return floorY;
            }
        }

        private enum State
        {
            Waiting,
            Ready,
            Playing
        }
    }
}
