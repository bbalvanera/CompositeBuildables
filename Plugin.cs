using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

using Nautilus.Handlers; // OptionsPanelHandler

namespace CompositeBuildables;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus")]
public class Plugin : BaseUnityPlugin
{
    public new static ManualLogSource Logger { get; private set; }
    public static Config config;

    private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

    internal static SaveCache SaveCache { get; } = SaveDataHandler.RegisterSaveDataCache<SaveCache>();

    private void Awake()
    {
        // set project-scoped logger instance
        Logger = base.Logger;

        // Initialize Mod Config
        config = OptionsPanelHandler.RegisterModOptions<Config>();

        Logger.LogDebug("Initializing Buildables...");

        // Initialize custom buildables
        InitializeBuildables();

        // register harmony patches, if there are any
        Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_GUID}");
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void InitializeBuildables()
    {
        Logger.LogDebug("Starting Coroutine for Prefab Initialization");
        StartCoroutine(PrefabHelper.EnsureInitialized());

        Logger.LogDebug("Registering Buildables...");
        DegasiPlanter.Register();
        DegasiPlanterRound.Register();
        //DegasiPlanter2.Register(); 

        ScienceBench1.Register();
        ScienceBench2.Register();

        LabShelving.Register();

        MushroomTerrariumSmall.Register();
        MushroomTerrariumLarge.Register();

        //FlowerTerrariumSmall.Register(); 

        //TubularShelfSmall.Register();
    }
}