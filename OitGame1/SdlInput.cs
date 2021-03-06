﻿using System;
using System.Collections.Generic;
using Yanesdk.Draw;
using Yanesdk.Ytl;
using Yanesdk.Input;

namespace OitGame1
{
    public class SdlInput : IGameInput, IDisposable
    {
        private static readonly PlayerKeySetting[] playerKeySettings;

        private readonly bool fullscreen;
        private readonly int playerCount;
        private readonly KeyBoardInput keyBoardInput;
        private readonly MouseInput mouseInput;
        private readonly JoyStick[] joySticks;

        private GameCommand[] commands;
        private GameCommand[] emptyCommands;

        static SdlInput()
        {
            playerKeySettings = new PlayerKeySetting[4];
            playerKeySettings[0] = new PlayerKeySetting(KeyCode.LEFT, KeyCode.RIGHT, KeyCode.UP, KeyCode.DOWN);
            playerKeySettings[1] = new PlayerKeySetting(KeyCode.a, KeyCode.d, KeyCode.w, KeyCode.s);
            playerKeySettings[2] = new PlayerKeySetting(KeyCode.f, KeyCode.h, KeyCode.t, KeyCode.g);
            playerKeySettings[3] = new PlayerKeySetting(KeyCode.j, KeyCode.l, KeyCode.i, KeyCode.k);
        }

        public SdlInput(bool fullscreen, int playerCount)
        {
            this.fullscreen = fullscreen;
            this.playerCount = playerCount;
            keyBoardInput = new KeyBoardInput();
            mouseInput = new MouseInput();
            if (fullscreen)
            {
                mouseInput.Hide();
            }
            joySticks = new JoyStick[playerCount];
            for (var i = 0; i < playerCount; i++)
            {
                joySticks[i] = new JoyStick(i);
            }
            commands = new GameCommand[playerCount];
            emptyCommands = new GameCommand[playerCount];
            for (var i = 0; i < playerCount; i++)
            {
                commands[i] = new GameCommand(false, false, false, false);
                emptyCommands[i] = new GameCommand(false, false, false, false);
            }
        }

        public void Update()
        {
            keyBoardInput.Update();
            mouseInput.Update();
            for (var i = 0; i < playerCount; i++)
            {
                var keySetting = GetKeySetting(i);
                var joyStick = joySticks[i];
                joyStick.Update();
                var left = keyBoardInput.IsPress(keySetting.left) || joyStick.IsPress(2);
                var right = keyBoardInput.IsPress(keySetting.right) || joyStick.IsPress(3);
                var jump = keyBoardInput.IsPress(keySetting.jump);
                for (var j = 4; j < 8; j++)
                {
                    jump = jump || joyStick.IsPress(j);
                }
                var start = keyBoardInput.IsPress(keySetting.start) || joyStick.IsPress(11);
                commands[i] = new GameCommand(left, right, jump, start);
            }
        }

        public IList<GameCommand> Current
        {
            get
            {
                if (SDLFrame.IsActive)
                {
                    return commands;
                }
                else
                {
                    return emptyCommands;
                }
            }
        }

        public bool Quit()
        {
            return keyBoardInput.IsPress(KeyCode.ESCAPE);
        }

        public void Dispose()
        {
            //Console.WriteLine("SdlInput.Dispose");
            keyBoardInput.Dispose();
            mouseInput.Dispose();
        }

        private static PlayerKeySetting GetKeySetting(int playerIndex)
        {
            return playerKeySettings[playerIndex % playerKeySettings.Length];
        }

        private class PlayerKeySetting
        {
            public readonly KeyCode left;
            public readonly KeyCode right;
            public readonly KeyCode jump;
            public readonly KeyCode start;

            public PlayerKeySetting(KeyCode left, KeyCode right, KeyCode jump, KeyCode start)
            {
                this.left = left;
                this.right = right;
                this.jump = jump;
                this.start = start;
            }
        }
    }
}
