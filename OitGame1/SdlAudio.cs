using System;
using System.Collections.Generic;
using Yanesdk.Sound;
using Yanesdk.Ytl;

namespace OitGame1
{
    public class SdlAudio : IGameAudio, IDisposable
    {
        private Sound[] sounds;
        private Sound music;

        public SdlAudio()
        {
            LoadSounds();
            Sound.SoundConfig.AudioBuffers = 1024;
            Sound.SoundConfig.Update();
        }

        private void LoadSounds()
        {
            var soundCount = Enum.GetValues(typeof(GameSound)).Length;
            sounds = new Sound[soundCount];
            for (var i = 0; i < soundCount; i++)
            {
                var path = "sounds/" + Enum.GetName(typeof(GameSound), i) + ".wav";
                sounds[i] = LoadSound(path, 0, i < 2 ? 0.3f : 0.8f);
            }
            music = LoadSound("sounds/Bgm.ogg", -1, 0.5f);
        }

        private Sound LoadSound(string path, int channel, float gain)
        {
            Console.WriteLine(path);
            var sound = new Yanesdk.Sound.Sound();
            var result = sound.Load(path, channel);
            if (result == YanesdkResult.NoError)
            {
                sound.Volume = gain;
                return sound;
            }
            else
            {
                throw new Exception("効果音 '" + path + "' の読み込みに失敗しました。");
            }
        }

        public void PlaySound(GameSound sound)
        {
            sounds[(int)sound].Play();
        }

        public void StartMusic()
        {
            music.Loop = -1;
            music.Play();
        }

        public void StopMusic()
        {
            music.Stop();
        }

        public void Dispose()
        {
            Console.WriteLine("SdlAudio.Dispose");
            foreach (var sound in sounds)
            {
                sound.Stop();
                sound.Dispose();
            }
        }
    }
}
