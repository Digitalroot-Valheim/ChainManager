using BepInEx;
using BepInEx.Configuration;
using Digitalroot.Valheim.Common;
using JetBrains.Annotations;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Digitalroot.Valheim.ChainManager
{
  [BepInPlugin(Guid, Name, Version)]
  [BepInDependency(Jotunn.Main.ModGuid, "2.12.4")]
  [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  public partial class Main : BaseUnityPlugin, ITraceableLogging
  {
    [UsedImplicitly] public static ConfigEntry<int> NexusId;
    public static Main Instance;

    public Main()
    {
      Instance = this;
#if DEBUG
      EnableTrace = true;
      Log.RegisterSource(Instance);
#else
      EnableTrace = false;
#endif
      Log.Trace(Instance, $"{Namespace}.{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}");
    }

    [UsedImplicitly]
    private void Awake()
    {
      try
      {
        Log.Trace(Instance, $"{Namespace}.{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}");
        // NexusId = Config.Bind("General", "NexusID", 0000, new ConfigDescription("Nexus mod ID for updates", null, new ConfigurationManagerAttributes { Browsable = false, ReadOnly = true }));
        PrefabManager.OnVanillaPrefabsAvailable += OnVanillaPrefabsAvailable;
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
    }

    private void OnVanillaPrefabsAvailable()
    {
      PrefabManager.OnVanillaPrefabsAvailable -= OnVanillaPrefabsAvailable;

      // Create a custom recipe with a RecipeConfig
      var recipeConfig = new RecipeConfig
      {
        CraftingStation = Common.Names.Vanilla.CraftingStationNames.Forge
        , Name = "CraftableChainRecipe"
        , Item = Common.Names.Vanilla.ItemDropNames.Chain
        , Amount = 1
        , Enabled = true
        , MinStationLevel = 0
        , Requirements = new RequirementConfig[]
        {
          new(Common.Names.Vanilla.ItemDropNames.Iron, 2)
        }
      };
      
      ItemManager.Instance.AddRecipe(new CustomRecipe(recipeConfig));
    }

    #region Implementation of ITraceableLogging

    /// <inheritdoc />
    public string Source => Namespace;

    /// <inheritdoc />
    public bool EnableTrace { get; }

    #endregion
  }
}
