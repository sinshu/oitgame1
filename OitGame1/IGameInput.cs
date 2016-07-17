using System;
using System.Collections.Generic;

namespace OitGame1
{
    public interface IGameInput
    {
        void Update();
        IList<GameCommand> Current { get; }
        bool Quit();
    }
}
