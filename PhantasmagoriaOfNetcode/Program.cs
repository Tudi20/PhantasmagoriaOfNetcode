using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Discord.GameSDK;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace PhantasmagoriaOfNetcode
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Phantasmagoria of Netcode";
            Console.WriteLine("Grabbing ZeroTier Data...");
            LibZeroTier.APIHandler zerotier;
            LibZeroTier.ZeroTierStatus status = new LibZeroTier.ZeroTierStatus();
            try {
            zerotier = new LibZeroTier.APIHandler();
            status = zerotier.GetStatus();
            } catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed to grab ZeroTier data!\nMake sure it's installed properly and running!");
                Console.WriteLine("Exception: {0}", ex.Message);
                Environment.Exit(1);
            }
            #if DEBUG
            Console.WriteLine("======== ZEROTIER STATUS ========");
            Console.WriteLine($"Version: {status?.VersionMajor}.{status.VersionMinor}.{status.VersionRev}");
            Console.WriteLine("Online: {0}", status.Online);
            Console.WriteLine("ZeroTier Address: {0}", status.Address);
            Console.WriteLine("ZeroTier Public Identity: {0}", status.PublicIdentity);
            Console.WriteLine("Clock: {0}", status.Clock);
            #endif
            while (!status.Online)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("ZeroTier is offline! Connect to the appropiate network, then let's try again!");
                Console.ResetColor();
                Console.WriteLine("Press any key to try again... or CTRL+C to terminate.");
                Console.ReadKey();
            }
            Console.WriteLine("Initialising Discord...");
            Discord.GameSDK.Discord discord = new Discord.GameSDK.Discord(777469694357143552, (ulong)CreateFlags.Default);
            discord.Init();
            Discord.GameSDK.Activities.ActivityManager activityManager = discord.GetActivityManager();
            discord.SetLogHook(LogLevel.Debug, LogProblemsFunction);
            Discord.GameSDK.Activities.Activity activity = new Discord.GameSDK.Activities.Activity{
                Party = new Discord.GameSDK.Activities.ActivityParty
                {
                    Id = status.Address,
                    Size = new Discord.GameSDK.Activities.PartySize{
                        CurrentSize = 1,
                        MaxSize = 2
                    }
                },
                Instance = true,
                State = "Initializing..."
            };
            activityManager.UpdateActivity(activity, WriteActivityResponse);
            CancellationTokenSource cts = new CancellationTokenSource();
            Task task = UpdateDiscord(discord, cts.Token);
            cts.Token.Register(discord.Dispose);
            Console.WriteLine("Init Done");
            if (args is not null)
            {
                //Client Stuff
            } else
            {   
                Console.WriteLine("Welcome to Phantasmgaoria of Netplay!");
                Console.WriteLine("If you're seeing this message, that means everything should be set up correctly, except with maybe the typical adonis stuff.");
                WriteInputKey('d');
                Console.WriteLine("If a problem arises with adonis don't be afraid to ask in the PoFV Netplay Discord Server.");
                WriteInputKey('h');
                Console.WriteLine("You can select to start hosting and afterward Invite people trough Discord or other people can Ask to Join.");
                WriteInputKey('c');
                Console.WriteLine("You can also select Client mode, which just puts this application readying to throw you into the game, once your Ask to Join request gets accepted or you click on an Invite in Discord.");
                WriteInputKey('e');
                Console.WriteLine("You can also just exit.");
                ConsoleKeyInfo key;
                bool b = false;
                do
                {
                    key = Console.ReadKey();
                    b = !(key.Key is ConsoleKey.D or ConsoleKey.H or ConsoleKey.C or ConsoleKey.E);
                    if (b)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid key press! Your only options are d,h,c,e.");
                        Console.ResetColor();
                    }
                } while(b);
                switch (key.Key)
                {
                    case ConsoleKey.D:
                    {
                        //Simplistic protection method against invite scrapers on Github if they exist.
                        Process.Start("https://" + "discord.gg" + "/2QPPPpE");
                        break;
                    }
                    case ConsoleKey.H:
                    {
                        break;
                    }
                    case ConsoleKey.C:
                    {
                        break;
                    }
                    default: break;
                }
            }
            Console.WriteLine("Press any key to close");
            await Task.Run(Console.ReadKey);
            cts.Cancel();
        }
        public static void LogProblemsFunction(LogLevel level, string message)
        {
            Console.WriteLine("Discord:{0} - {1}", level, message);
        }

        public static async Task UpdateDiscord(Discord.GameSDK.Discord discord, CancellationToken cancellationToken)
        {
            while(true)
            {
                discord.RunCallbacks();
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

        public static void WriteActivityResponse(Result result)
        {
             #if DEBUG
                Console.WriteLine("Updating Activity: {0}", result);
             #endif
        }
    }
}
