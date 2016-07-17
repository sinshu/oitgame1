using System;
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
            using (var app = new OitGame1Application(false, 3))
            {
                app.Run();
            }
        }
    }
}
