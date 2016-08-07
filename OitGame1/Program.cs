using System;
using System.Windows.Forms;
using Yanesdk.Ytl;
using Yanesdk.Draw;
using Yanesdk.Timer;
using Yanesdk.System;

namespace OitGame1
{
    static class Program
    {
        static void Main(string[] args)
        {
            var playerCount = GetPlayerCount(args);
            var result = MessageBox.Show("フルスクリーンで起動しますか？", "OitGame1", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            var fullscreen = result == DialogResult.Yes;
            try
            {
                using (var app = new OitGame1Application(fullscreen, playerCount))
                {
                    app.Run();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static int GetPlayerCount(string[] args)
        {
            if (args.Length > 0)
            {
                try
                {
                    var playerCount = int.Parse(args[0]);
                    if (playerCount < 1) return 1;
                    else if (playerCount > 8) return 8;
                    else return playerCount;
                }
                catch
                {
                    return 4;
                }
            }
            return 4;
        }
    }
}
