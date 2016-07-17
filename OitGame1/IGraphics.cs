using System;

namespace OitGame1
{
    public interface IGraphics
    {
        void Begin();
        void End();
        void SetColor(int a, int r, int g, int b);
        void DrawRectangle(int x, int y, int width, int height);
        void Test(int x, int y);
    }
}
