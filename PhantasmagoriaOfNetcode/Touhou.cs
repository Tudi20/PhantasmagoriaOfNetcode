using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static PhantasmagoriaOfNetcode.Program;

namespace PhantasmagoriaOfNetcode
{
    class Touhou
    {
        internal static void HandleRunningTouhou(ConnectionType ct)
        {
            Process process = FindTouhou();
            if (process == null)
                Console.WriteLine("Game Not Found");
            else
            {

                while (!WaitTillTouhouExists(process).IsCompleted)
                {
                    #if DEBUG
                    Console.WriteLine("Playing the game");
                    #endif
                    switch (ct)
                    {
                        case ConnectionType.Host:
                        case ConnectionType.Client:

                            break;
                        case ConnectionType.Watch:

                            break;
                        default:
                            throw new ArgumentOutOfRangeException("ct");
                    }
                }
                #if DEBUG
                Console.WriteLine("Game exited");
                #endif
            }

        }

        private static Process FindTouhou()
        {
            Process gameProcess = null;
            var sw = new Stopwatch();
            sw.Start();
            do
            {
                try
                {
                    gameProcess = Process.GetProcessesByName("th09")[0];
                    #if DEBUG
                    Console.WriteLine("Found Game Process with PID " + gameProcess.Id);
                    #endif
                    if (Process.GetProcessesByName("th09").Length > 1)
                    {
                        Console.WriteLine("!! Multiple games running found, killing all, please retry");
                        foreach (var item in Process.GetProcessesByName("th09")) item.Kill();

                    }
                }
                catch { Thread.Sleep(10); }
                if (sw.Elapsed.CompareTo(new TimeSpan(0, 1, 0)) > 0)
                {
                    Console.WriteLine("Finding game timed out");
                    sw.Stop();
                    return null;
                }
            } while (gameProcess == null); 
            sw.Stop();
            return gameProcess;
        }
        private static Task WaitTillTouhouExists(Process pr)
        {
            return Task.Run(() => pr.WaitForExit());
        }
    }
}
