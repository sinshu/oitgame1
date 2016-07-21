using System;
using System.Collections.Generic;

namespace OitGame1
{
    public class GameWorld
    {
        private static readonly int fieldWidth = 1024;

        private readonly int playerCount;
        private readonly GamePlayer[] players;

        private double cameraRealX;
        private int cameraIntX;

        private List<GameCoin> coins;

        public GameWorld(int playerCount)
        {
            this.playerCount = playerCount;
            players = new GamePlayer[playerCount];
            for (var i = 0; i < playerCount; i++)
            {
                players[i] = new GamePlayer(this, i, 100 * i);
            }
            coins = new List<GameCoin>();
            var rand = new Random();
            for (var i = 0; i < 10; i++)
            {
                coins.Add(new GameCoin(this, rand.Next(0, 640), rand.Next(0, 480), rand.Next(0, 60)));
            }
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
            foreach (var coin in coins)
            {
                coin.Update();
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
            foreach (var player in players)
            {
                player.Draw(graphics);
            }
            foreach (var coin in coins)
            {
                coin.Draw(graphics);
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
    }
}
