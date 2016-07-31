using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OitGame1
{
    public abstract class Particle : GameObject
    {
        private bool deleted;

        public Particle(GameWorld world, double x, double y)
            : base(world)
        {
            CenterX = x;
            CenterY = y;
            deleted = false;
        }

        public abstract void Update();

        public abstract void Draw(IGameGraphics graphics);

        public void Delete()
        {
            deleted = true;
        }

        public bool Deleted
        {
            get
            {
                return deleted;
            }
        }
    }
}
