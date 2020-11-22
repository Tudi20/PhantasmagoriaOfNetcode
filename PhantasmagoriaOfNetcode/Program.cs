using System;
using Discord.GameSDK;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Discord.GameSDK.Activities;
using LibZeroTier;
using Activity = Discord.GameSDK.Activities.Activity;

// Don't forget to change the Working Directory in Project Properties > Build > Working Directory to your Touhou 9 folder! 

namespace PhantasmagoriaOfNetcode
{
    class Program
    {
        public static Discord.GameSDK.Discord Discord;
        public static APIHandler Zerotier;

        public enum ConnectionType
        {
            Host = 0,
            Client,
            Watch
        }
        static async Task Main(string[] args)
        {
            Console.Title = "Phantasmagoria of Netcode";
            Console.WriteLine("Grabbing ZeroTier Data...");
            var status = new ZeroTierStatus();
            try
            {
                Zerotier = new APIHandler();
                status = Zerotier.GetStatus();
            } catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed to grab ZeroTier data!\nMake sure it's installed properly and running!");
                Console.WriteLine("Exception: {0}", ex.Message);
                Environment.Exit(1);
            }
            #if DEBUG
            Console.WriteLine("======== ZEROTIER STATUS ========");
            #endif
            if (status != null)
            {
                #if DEBUG
                Console.WriteLine($"Version: {status.VersionMajor}.{status.VersionMinor}.{status.VersionRev}");
                Console.WriteLine("Online: {0}", status.Online);
                Console.WriteLine("ZeroTier Address: {0}", status.Address);
                Console.WriteLine("ZeroTier Public Identity: {0}", status.PublicIdentity);
                Console.WriteLine("Clock: {0}", status.Clock);
                #endif
                while (!status.Online)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("ZeroTier is offline! Connect to the appropriate network, then let's try again!");
                    Console.ResetColor();
                    Console.WriteLine("Press any key to try again... or CTRL+C to terminate.");
                    Console.ReadKey();
                }

                Console.WriteLine("Initializing Discord...");
                Discord =
                    new Discord.GameSDK.Discord(777469694357143552, (ulong) CreateFlags.Default);
                Discord.Init();
                var activityManager = Discord.GetActivityManager();
                Discord.SetLogHook(LogLevel.Debug, LogProblemsFunction);

                var activity = new Activity
                {
                    Party = new ActivityParty
                    {
                        Id = status.Address,
                        Size = new PartySize
                        {
                            CurrentSize = 1,
                            MaxSize = 2
                        }
                    },
                    Instance = true,
                    State = "Initializing..."
                };
                activityManager.UpdateActivity(activity, WriteCallbackResponse);
                var cts = new CancellationTokenSource();
#pragma warning disable 4014
                UpdateDiscord(Discord, cts.Token);
#pragma warning restore 4014
                cts.Token.Register(Discord.Dispose);
                Console.WriteLine("Init Done");
                if (args.Length > 0)
                {
                    //Client Stuff
                }
                else
                {
                    Console.WriteLine("Welcome to Phantasmagoria of Netplay!");
                    Console.WriteLine(
                        "If you're seeing this message, that means everything should be set up correctly, except with maybe the typical adonis stuff.");
                    WriteInputKey('d');
                    Console.WriteLine(
                        "If a problem arises with adonis don't be afraid to ask in the PoFV Netplay Discord Server.");
                    WriteInputKey('h');
                    Console.WriteLine(
                        "You can select to start hosting and afterward Invite people trough Discord or other people can Ask to Join.");
                    WriteInputKey('c');
                    Console.WriteLine(
                        "You can also select Client mode, which just puts this application readying to throw you into the game, once your Ask to Join request gets accepted or you click on an Invite in Discord.");
                    WriteInputKey('e');
                    Console.WriteLine("You can also just exit.");
                    ConsoleKeyInfo key;
                    do
                    {
                        key = Console.ReadKey();
                        switch (key.Key)
                        {
                            case ConsoleKey.H or ConsoleKey.C or ConsoleKey.E:
                                continue;
                            case ConsoleKey.D:
                            {
                                //Simplistic protection method against invite scrapers on Github if they exist.
                                var proc = new Process
                                {
                                    StartInfo = {
                                        UseShellExecute = true,
                                        FileName = "https://discord.gg/" + "2QPPPpE"
                                    }
                                };
                                proc.Start();
                                continue;
                            }
                        }

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid key press! Your only options are d,h,c,e.");
                        Console.ResetColor();
                    } while (key.Key is not ConsoleKey.H and not ConsoleKey.C and not ConsoleKey.E);

                    switch (key.Key)
                    {
                        case ConsoleKey.H:
                        {
                            Server.StartAdonis(activity);
                            break;
                        }
                        case ConsoleKey.C:
                        {
                            break;
                        }
                    }
                }

                Console.WriteLine("Press any key to close");
                await Task.Run(Console.ReadKey, cts.Token);
                cts.Cancel();
            }
        }
        public static void LogProblemsFunction(LogLevel level, string message)
        {
            Console.WriteLine("Discord:{0} - {1}", level, message);
        }

        public static async Task UpdateDiscord(Discord.GameSDK.Discord discord, CancellationToken cancellationToken)
        {
            while(true)
            {
                try
                {
                    discord.RunCallbacks();
                } catch (ResultException re)
                {
                    if (re.Result == Result.NotRunning)
                    {
                        Console.WriteLine("Discord is not running!");
                    }
                }
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                await Task.Yield();
            }
        }

        private static void WriteInputKey(char c)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write('[');
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(c);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("] ");
            Console.ResetColor();
        }

        public static void WriteCallbackResponse(Result result)
        {
            #if DEBUG
            Console.WriteLine("Result: {0}", result);
             #endif
        }
    }
    internal static class NativeMethods
    {
        [DllImport("User32.dll")]
        internal static extern int SetForegroundWindow(IntPtr point);
    }
}
