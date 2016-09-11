using System;
using System.Collections.Generic;

namespace OitGame1
{
    public class GameWorld
    {
        private static readonly int fieldWidth = 1024;
        private static readonly int floorY = Setting.ScreenHeight - 64;

        private static readonly double cameraSpeedFactor = 1.0 / 16.0;

        private static readonly int initCoinGenPeriod = 20;
        private static readonly int minCoinGenPeriod = 5;
        private static readonly int initBombGenPeriod = 120;
        private static readonly int minBombGenPeriod = 20;

        private static readonly int oneGameDuration = 100 * 60;

        private readonly int playerCount;
        private readonly GamePlayer[] players;

        private double cameraTargetX;
        private double cameraX;
        private int cameraIntX;

        private Random random;

        private State state;
        private IEnumerator<int> stateCoroutine;
        private int sleepCount;

        private List<Particle> particles;

        private int countDown;

        private List<GameCoin> coins;
        private List<GameCoin> newCoins;
        private int coinGenPeriod;
        private int coinGenCount;

        private List<GameBomb> bombs;
        private int bombGenPeriod;
        private int bombGenCount;

        private int elapsedTime;
        private int remainingTime;

        private int[] playerRank;

        private IGameAudio audio;

        public GameWorld(int playerCount)
        {
            this.playerCount = playerCount;
            players = new GamePlayer[playerCount];
            var cx = fieldWidth / 2;
            var left = cx - Setting.ScreenWidth / 2 + (double)Setting.ScreenWidth / (playerCount + 1);
            var stride = (double)Setting.ScreenWidth / (playerCount + 1);
            for (var i = 0; i < playerCount; i++)
            {
                players[i] = new GamePlayer(this, i, left + i * stride);
            }
            InitCameraCenterX(GetAveragePlayerX());

            random = new Random();

            state = State.Waiting;
            stateCoroutine = StateCoroutine().GetEnumerator();
            sleepCount = 0;

            particles = new List<Particle>();

            Reset();
        }

        private void Reset()
        {
            countDown = 0;

            coins = new List<GameCoin>();
            newCoins = new List<GameCoin>();
            coinGenPeriod = initCoinGenPeriod;
            coinGenCount = 0;

            bombs = new List<GameBomb>();
            bombGenPeriod = initBombGenPeriod;
            bombGenCount = bombGenPeriod + random.Next(bombGenPeriod);

            elapsedTime = 0;
            remainingTime = oneGameDuration;
        }

        public void Update(IList<GameCommand> command)
        {
            UpdateParticles();

            for (var i = 0; i < playerCount; i++)
            {
                players[i].Update1(command[i]);
            }
            for (var i = 0; i < playerCount; i++)
            {
                players[i].Update2();
            }
            SetCameraCenterX(GetAveragePlayerX());

            UpdateItems();
            CheckItemCollision();
            DeleteItems();
            AddNewCoins();

            UpdateCoroutine();

            if (state == State.Playing)
            {
                elapsedTime++;
                if (remainingTime > 0)
                {
                    if (remainingTime <= 600 && remainingTime % 60 == 0)
                    {
                        PlaySound(GameSound.CountDown);
                    }
                    remainingTime--;
                }
            }


            var coeff = (double)remainingTime / oneGameDuration;
            coeff *= coeff;
            coeff *= coeff;
            coinGenPeriod = (int)(minCoinGenPeriod + (initCoinGenPeriod - minCoinGenPeriod) * coeff);
            bombGenPeriod = (int)(minBombGenPeriod + (initBombGenPeriod - minBombGenPeriod) * coeff);
        }

        private void UpdateParticles()
        {
            foreach (var particle in particles)
            {
                particle.Update();
            }
            particles.RemoveAll(particle => particle.Deleted);
        }

        private void UpdateItems()
        {
            if (state == State.Playing)
            {
                if (coinGenCount == 0)
                {
                    var x = fieldWidth * random.NextDouble();
                    coins.Add(new GameCoin(this, x));
                    coinGenCount = coinGenPeriod + random.Next(coinGenPeriod);
                }
                else
                {
                    coinGenCount--;
                }
                if (bombGenCount == 0)
                {
                    var x = fieldWidth * random.NextDouble();
                    bombs.Add(new GameBomb(this, x));
                    bombGenCount = bombGenPeriod + random.Next(bombGenPeriod);
                }
                else
                {
                    bombGenCount--;
                }
            }

            foreach (var coin in coins)
            {
                coin.Update();
            }

            foreach (var bomb in bombs)
            {
                bomb.Update();
            }
        }

        private void CheckItemCollision()
        {
            foreach (var player in players)
            {
                foreach (var coin in coins)
                {
                    if (coin.Deleted) continue;
                    if (!player.CanMove && coin.Parent == player) continue;
                    if (player.IsOverlappedWith(coin))
                    {
                        player.GetCoin(coin);
                        coin.Delete(true);
                    }
                }
            }
            foreach (var player in players)
            {
                foreach (var bomb in bombs)
                {
                    if (bomb.Deleted) continue;
                    if (player.IsOverlappedWith(bomb))
                    {
                        player.GetBomb(bomb);
                        bomb.Delete(true);
                    }
                }
            }
        }

        private void DeleteItems()
        {
            coins.RemoveAll(coin => coin.Deleted);
            bombs.RemoveAll(bomb => bomb.Deleted);
        }

        public void AddCoin(GameCoin coin)
        {
            newCoins.Add(coin);
        }

        private void AddNewCoins()
        {
            foreach (var coin in newCoins)
            {
                coins.Add(coin);
            }
            newCoins.Clear();
        }

        private void ClearItems()
        {
            foreach (var coin in coins)
            {
                coin.Delete(false);
            }
            foreach (var bomb in bombs)
            {
                bomb.Delete(false);
            }
        }

        public void AddParticle(Particle particle)
        {
            particles.Add(particle);
        }

        private void UpdateCoroutine()
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
                        Reset();
                        foreach (var player in players)
                        {
                            player.ResetCoinCount();
                        }
                        for (var i = 0; i < 3; i++)
                        {
                            PlaySound(GameSound.CountDown);
                            yield return 60;
                            countDown++;
                        }
                        state = State.Playing;
                        PlaySound(GameSound.Start);
                        yield return 1;
                        break;
                    case State.Playing:
                        if (remainingTime == 0)
                        {
                            foreach (var player in players)
                            {
                                player.Reset();
                            }
                            ClearItems();
                            playerRank = GetPlayerRank();
                            GiveTrophy();
                            state = State.Waiting;
                            PlaySound(GameSound.Start);
                        }
                        yield return 1;
                        break;
                    default:
                        throw new Exception("＼(^o^)／");
                }
            }
        }

        private int[] GetPlayerRank()
        {
            var rank = new int[playerCount];
            for (var i = 0; i < playerCount; i++)
            {
                var r = 0;
                for (var j = 0; j < playerCount; j++)
                {
                    if (i == j) continue;
                    if (players[j].CoinCount > players[i].CoinCount)
                    {
                        r++;
                    }
                }
                rank[i] = r;
            }
            return rank;
        }

        private void GiveTrophy()
        {
            var min = int.MaxValue;
            var max = int.MinValue;
            foreach (var rank in playerRank)
            {
                if (rank < min) min = rank;
                if (rank > max) max = rank;
            }
            for (var i = 0; i < playerCount; i++)
            {
                if (playerRank[i] == min) players[i].AddTrophy();
                if (playerRank[i] == max) players[i].AddUnko();
            }
        }

        public void Draw(IGameGraphics graphics)
        {
            graphics.Begin();
            var sky = (double)elapsedTime / oneGameDuration;
            graphics.SetColor(255, 255, 255, 255);
            {
                var dx = cameraIntX + Setting.ScreenWidth / 2 - fieldWidth / 2;
                var drawX = (int)Math.Round((Setting.ScreenWidth - 1024) / 2 - dx / 4.0);
                graphics.DrawImage(GameImage.BlueSky, drawX, 0);
            }
            {
                var dx = cameraIntX + Setting.ScreenWidth / 2 - fieldWidth / 2;
                var drawX = (int)Math.Round((Setting.ScreenWidth - 1024) / 2 - dx / 2.0);
                graphics.DrawImage(GameImage.Trees, drawX, 256);
            }
            graphics.DrawImage(GameImage.Field, -cameraIntX, Setting.ScreenHeight - 128);
            graphics.SetColor(255, 255, 255, 255);
            foreach (var particle in particles)
            {
                particle.Draw(graphics);
            }
            foreach (var coin in coins)
            {
                coin.Draw(graphics);
            }
            foreach (var bomb in bombs)
            {
                bomb.Draw(graphics);
            }
            foreach (var player in players)
            {
                player.Draw(graphics);
            }
            if (state == State.Waiting)
            {
                DrawPlayerState(graphics);
                if (playerRank != null)
                {
                    var drawX = (Setting.ScreenWidth - 256) / 2;
                    graphics.DrawImage(GameImage.Message, 256, 128, 1, 1, drawX, 64);
                }
                else
                {
                    var drawX = (Setting.ScreenWidth - 512) / 2;
                    graphics.DrawImage(GameImage.Title, drawX, 64);
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
                DrawTime(graphics);
            }
            DrawHuds(graphics);
            graphics.End();
        }

        private void DrawPlayerState(IGameGraphics graphics)
        {
            for (var i = 0; i < players.Length; i++)
            {
                var rank = -1;
                if (playerRank != null)
                {
                    rank = playerRank[i];
                }
                players[i].DrawState(graphics, rank);
            }
        }

        private void DrawTime(IGameGraphics graphics)
        {
            var sec = (int)(Math.Ceiling(remainingTime / 60.0));
            var digits = 0;
            {
                var n = sec;
                while (n > 0)
                {
                    digits++;
                    n /= 10;
                }
            }
            var drawOffsetX = (Setting.ScreenWidth - 32 * digits) / 2;
            var drawOffsetY = 16;
            {
                var n = sec;
                for (var i = 0; i < digits; i++)
                {
                    var d = n % 10;
                    var row = d / 4;
                    var col = d % 4;
                    var drawX = drawOffsetX + 32 * (digits - i - 1);
                    graphics.DrawImage(GameImage.Time, 32, 32, row, col, drawX, drawOffsetY);
                    n /= 10;
                }
            }
        }

        private void DrawHuds(IGameGraphics graphics)
        {
            var hudSpace = 96 * players.Length;
            var emptySpace = Setting.ScreenWidth - hudSpace;
            var left = emptySpace / (players.Length + 1);
            var stride = left + 96;
            var drawX = left;
            foreach (var player in Players)
            {
                DrawHud(graphics, player, drawX, Setting.ScreenHeight - 48);
                drawX += stride;
            }
        }

        private void DrawHud(IGameGraphics graphics, GamePlayer player, int x, int y)
        {
            graphics.DrawImage(GameImage.Hud, 96, 32, 3, 0, x, y);

            var ammo = player.CoinCount;
            if (ammo / 100 > 0)
            {
                DrawNumber(graphics, ammo / 100 % 10, x + 32, y);
            }
            if (ammo / 10 > 0)
            {
                DrawNumber(graphics, ammo / 10 % 10, x + 32 + 19, y);
            }
            if (ammo >= 0)
            {
                DrawNumber(graphics, ammo % 10, x + 32 + 19 + 19, y);
            }

            var pi = player.PlayerIndex % 4;
            if (player.PlayerIndex >= 4) graphics.SetColor(255, 0, 255, 0);
            graphics.DrawImage(GameImage.Player, 32, 32, 2 * pi, 4, x, y);
            if (player.PlayerIndex >= 4) graphics.SetColor(255, 255, 255, 255);

            for (var i = 0; i < Math.Min(player.TrophyCount, 6); i++)
            {
                graphics.DrawImage(GameImage.Trophy, 16, 16, 0, 0, x + 16 * i, y - 16);
            }
            for (var i = 0; i < Math.Min(player.UnkoCount, 6); i++)
            {
                graphics.DrawImage(GameImage.Trophy, 16, 16, 0, 1, x + 16 * i, y + 32);
            }
        }

        private void DrawNumber(IGameGraphics graphics, int n, int x, int y)
        {
            graphics.DrawImage(GameImage.Hud, 32, 32, 4 + n / 8, n % 8, x, y);
        }

        private void InitCameraCenterX(double x)
        {
            cameraTargetX = x - Setting.ScreenWidth / 2;
            cameraX = cameraTargetX;
            cameraIntX = (int)Math.Round(cameraX);
        }

        private void SetCameraCenterX(double x)
        {
            cameraTargetX = x - Setting.ScreenWidth / 2;
            if (cameraTargetX < 0)
            {
                cameraTargetX = 0;
            }
            if (cameraTargetX > fieldWidth - Setting.ScreenWidth)
            {
                cameraTargetX = fieldWidth - Setting.ScreenWidth;
            }
            cameraX = cameraSpeedFactor * cameraTargetX + (1.0 - cameraSpeedFactor) * cameraX;
            cameraIntX = (int)Math.Round(cameraX);
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

        public void PlaySound(GameSound sound)
        {
            if (audio != null)
            {
                audio.PlaySound(sound);
            }
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

        public IGameAudio Audio
        {
            get
            {
                return audio;
            }

            set
            {
                audio = value;
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
