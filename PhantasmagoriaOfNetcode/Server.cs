using System;
using System.Diagnostics;
using System.Threading;
using Discord.GameSDK.Activities;
using WindowsInput.Native;

namespace PhantasmagoriaOfNetcode
{
    class Server
    {
        private static readonly WindowsInput.InputSimulator InputSimulator = new WindowsInput.InputSimulator();


        public static void StartAdonis(Discord.GameSDK.Activities.Activity activity)
        {
            var activityManager = Program.Discord.GetActivityManager();
            activity.State = "Starting up server";
            activityManager.UpdateActivity(activity, Program.WriteCallbackResponse);
            var adonis = Process.Start(Environment.OSVersion.Platform == PlatformID.Win32NT ? "adonis.exe" : "wine adonis.exe");
            while (adonis != null && !adonis.Responding) Thread.Sleep(1);
            Thread.Sleep(100);
            if (adonis != null) _ = NativeMethods.SetForegroundWindow(adonis.MainWindowHandle);
            InputSimulator.Keyboard.TextEntry("s");
            InputSimulator.Keyboard.KeyPress(VirtualKeyCode.RETURN); // Select server option
            InputSimulator.Keyboard.TextEntry("17723");
            InputSimulator.Keyboard.KeyPress(VirtualKeyCode.RETURN); //Select port
            //Waiting for connection...
            activity.State = "Waiting for someone to join them";
            activity.Secrets = new ActivitySecrets
            {
                Join = Program.Zerotier.NodeAddress(),
                Spectate = Program.Zerotier.NodeAddress(),
                Match = DateTime.UtcNow.ToString("yyyyMMddTHH:mm:ssZ")
            };
            activityManager.UpdateActivity(activity, Program.WriteCallbackResponse);
            activityManager.OnActivityJoinRequest += OnActivityJoinRequest;
            // TODO: Wait for connection
            InputSimulator.Keyboard.KeyPress(VirtualKeyCode.RETURN); //Use recommended latency
            InputSimulator.Keyboard.KeyPress(VirtualKeyCode.RETURN); //Use default side
            Touhou.HandleRunningTouhou(Program.ConnectionType.Host);
        }

        private static void OnActivityJoinRequest(ref Discord.GameSDK.Users.User user)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Note: Currently these request handling are blocking you from playing!");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{user.Username}#{user.Discriminator} with ID {user.Id} would like to join you.");
            Console.WriteLine("Press [y] to accept and [n] to refuse!");
            ConsoleKeyInfo key;
            bool b;
            do
            {
                key = Console.ReadKey();
                b = key.Key is not ConsoleKey.Y and not ConsoleKey.N;
                if (!b) continue;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid key press! Your only options are y or n.");
                Console.ResetColor();
            } while (b);
            switch (key.Key)
            {
                case ConsoleKey.Y:
                {
                    Program.Discord.GetActivityManager().SendRequestReply(user.Id, ActivityJoinRequestReply.Yes,  Program.WriteCallbackResponse);
                    break;
                }
                case ConsoleKey.N:
                {
                    Program.Discord.GetActivityManager().SendRequestReply(user.Id, ActivityJoinRequestReply.No, Program.WriteCallbackResponse);
                        break;
                }
                default:
                {
                    Program.Discord.GetActivityManager().SendRequestReply(user.Id, ActivityJoinRequestReply.Ignore, Program.WriteCallbackResponse);
                    break;
                }
            }
            Console.ResetColor();
        }
    }
}