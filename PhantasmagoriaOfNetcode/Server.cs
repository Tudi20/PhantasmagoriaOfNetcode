using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.GameSDK.Activities;

namespace PhantasmagoriaOfNetcode
{
    class Server
    {
        private readonly WindowsInput.InputSimulator _inputSimulator = new WindowsInput.InputSimulator();
        private readonly Discord.GameSDK.Discord _discord;
        private readonly LibZeroTier.APIHandler _zerotier;

        public Server(Discord.GameSDK.Discord discord, LibZeroTier.APIHandler zerotier)
        {
            _discord = discord;
            _zerotier = zerotier;
        }

        public void StartAdonis(Discord.GameSDK.Activities.Activity activity)
        {
            var activityManager = _discord.GetActivityManager();
            activity.State = "Starting up server...";
            activityManager.UpdateActivity(activity, Program.WriteActivityResponse);
            Process adonis;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                adonis = Process.Start("adonis.exe");
            } else
            {
                adonis = Process.Start("wine adonis.exe");
            }
            _zerotier.GetPeers()
        }
    }
}