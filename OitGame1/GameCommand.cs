using System;

namespace OitGame1
{
    public class GameCommand
    {
        private bool left;
        private bool right;
        private bool jump;
        private bool start;

        public GameCommand(bool left, bool right, bool jump, bool start)
        {
            this.left = left;
            this.right = right;
            this.jump = jump;
            this.start = start;
        }

        public bool Left
        {
            get
            {
                return left;
            }
        }

        public bool Right
        {
            get
            {
                return right;
            }
        }

        public bool Jump
        {
            get
            {
                return jump;
            }
        }

        public bool Start
        {
            get
            {
                return start;
            }
        }
    }
}
