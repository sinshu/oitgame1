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

        public GameWorld(int playerCount)
        {
            this.playerCount = playerCount;
            players = new GamePlayer[playerCount];
            for (var i = 0; i < playerCount; i++)
            {
                players[i] = new GamePlayer(this, i, 100 * i);
            }
        }

        public void Update(IList<GameCommand> command)
        {
            for (var i = 0; i < playerCount; i++)
            {
                players[i].Update(command[i]);
            }
            SetCameraCenterX(GetAveragePlayerX());
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
    }
}
