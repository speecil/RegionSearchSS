using HarmonyLib;
using IPA;
using System.Reflection;
using IPALogger = IPA.Logging.Logger;
namespace SearchOCE
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }

        public static Harmony harmony;

        [Init]
        public void Init(IPALogger logger)
        {
            Instance = this;
            Log = logger;
            harmony = new Harmony("Speecil.BeatSaber.SearchOCE");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
