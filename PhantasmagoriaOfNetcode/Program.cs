using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Discord.GameSDK;
using System.Threading.Tasks;
using System.Threading;

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
            activityManager.UpdateActivity(activity, (result) =>
            {
                #if DEBUG
                Console.WriteLine("Updating Activity: {0}", result);
                #endif
                });
            CancellationTokenSource cts = new CancellationTokenSource();
            Task task = UpdateDiscord(discord, cts.Token);
            cts.Token.Register(discord.Dispose);
            Console.WriteLine("Init Done");
            if (args is not null)
            {
                //Client Stuff
            } else
            {
               
            }
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
    }
}
