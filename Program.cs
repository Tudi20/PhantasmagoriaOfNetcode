using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PhantasmagoriaOfNetcode
{
    class Program
    {
        static void Main(string[] args)
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
            Console.WriteLine("ZeroTier Oublic Identity: {0}", status.PublicIdentity);
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
            if (args[0] is not null)
            {
                //Client Stuff
            } else
            {
               
            }
            
        }
    }
}
