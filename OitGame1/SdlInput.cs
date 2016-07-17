using System;
using Yanesdk.Draw;
using Yanesdk.Ytl;
using Yanesdk.Input;

namespace OitGame1
{
    public class SdlInput : IInput, IDisposable
    {
        private bool fullscreen;
        private int inputDeviceCount;
        private KeyBoardInput keyBoardInput;
        private MouseInput mouseInput;

        private GameCommand[] commands;

        public SdlInput(bool fullscreen, int inputDeviceCount)
        {
            this.fullscreen = fullscreen;
            this.inputDeviceCount = inputDeviceCount;
            keyBoardInput = new KeyBoardInput();
            mouseInput = new MouseInput();
            if (fullscreen)
            {
                mouseInput.Hide();
            }
            commands = new GameCommand[inputDeviceCount];
        }

        public void Update()
        {
            keyBoardInput.Update();
            mouseInput.Update();
            for (var i = 0; i < inputDeviceCount; i++)
            {
                var left = keyBoardInput.IsPress(KeyCode.LEFT);
                var right = keyBoardInput.IsPress(KeyCode.RIGHT);
                commands[i] = new GameCommand(left, right, false, false);
            }
        }

        public GameCommand[] GetCurrent()
        {
            return commands;
        }

        public bool Quit()
        {
            return keyBoardInput.IsPress(KeyCode.ESCAPE);
        }

        public void Dispose()
        {
            Console.WriteLine("SdlInput.Dispose");
            if (keyBoardInput != null)
            {
                keyBoardInput.Dispose();
                keyBoardInput = null;
            }
            if (mouseInput != null)
            {
                mouseInput.Dispose();
                mouseInput = null;
            }
        }
    }
}
