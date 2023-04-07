using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginAPI;
using PluginAPI.Core.Attributes;
using PluginAPI.Core;
using PluginAPI.Helpers;
using Microsoft.SqlServer.Server;
using System.IO;

namespace SLRealism
{
    public class SLRealism
    {
        public const string PluginName = "SLRealism";
        public const string PluginVersion = "1.0.0";
        public const string PluginDesc = "A plugin (test) adding most likely unbalanced but funny \"realistic\" changes to SL.";

        public static SLRealism instance;
        [PluginConfig(PluginName)]
        public Config config = new Config();
        public EventHandler eventHandler;

        public string AudioPath = Path.Combine($"{Paths.Plugins}", "Sounds");

        [PluginEntryPoint(PluginName, PluginVersion, PluginDesc, "btelnyy#8395")]
        public void LoadPlugin()
        {
            if(!config.PluginEnabled)
            {
                return;
            }
            instance = this;
            PluginAPI.Events.EventManager.RegisterEvents<EventHandler>(this);
            Log.Debug("SLRealism v" + PluginVersion + " loaded.");
        }

        [PluginUnload()]
        public void Unload()
        {
            config = null;
            eventHandler = null;
        }
    }
}
