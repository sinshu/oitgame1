using System;

namespace OitGame1
{
    public interface IInput
    {
        void Update();
        GameCommand[] GetCurrent();
        bool Quit();
    }
}
