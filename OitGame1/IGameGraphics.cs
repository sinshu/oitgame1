using System;

namespace OitGame1
{
    public interface IGameGraphics
    {
        void Begin();
        void End();
        void SetColor(int a, int r, int g, int b);
        void DrawRectangle(int x, int y, int width, int height);
        void DrawImage(GameImage image, int x, int y);
        void DrawImage(GameImage image, int width, int height, int row, int col, int x, int y);
        void DrawImage(GameImage image, int width, int height, int row, int col, int x, int y, int rotation);
        void Test(int x, int y);
    }
}
