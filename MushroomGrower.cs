using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers; // CraftDataHandler
using Nautilus.Extensions;
using Nautilus.Json;
using UnityEngine;
using UnityEditor;

using BepInEx;
using Nautilus.Utility;

using UWE;

using System.Collections; // IEnumerator
using System.Collections.Generic; // List
using System.Linq; // Where
using System.IO; // Path
using System.Reflection; // Assembly

using ProtoBuf;

namespace CompositeBuildables;

public class MushroomGrower : HandTarget, IHandTarget//, IProtoEventListener
{
  private MushroomGrowerSaveData _saveData = new();
  public MushroomGrowerSaveData SaveData { get => _saveData; }
  
  public StorageContainer storageContainer;
  
  public float speedFactor = 1f; // 2f for Large Mushroom Terrariums
  
  private const int maxPinkCap = 8;
  private const int maxRattler = 6;
  private const int maxJaffa = 4;
  
  // Rates configured so that overflow is enough to power exactly one bioreactor
  private const float pinkTime = 336f;
  private const float rattlerTime = 448f;
  private const float jaffaTime = 672f;
  
  private float timeRemainingPink = -1f;
  private float timeRemainingRattler = -1f;
  private float timeRemainingJaffa = -1f;
  
  public void TryGrowPink() {
    if(timeRemainingPink < 0f && (storageContainer.container.GetCount(TechType.PinkMushroom) < maxPinkCap || Plugin.config.MushroomsToReactors)) {
      timeRemainingPink = pinkTime/Plugin.config.MushroomProductionSpeedFactor/speedFactor;
    }
  }
  
  public void TryGrowRattler() {
    if(timeRemainingRattler < 0f  && (storageContainer.container.GetCount(TechType.PurpleRattle) < maxRattler || Plugin.config.MushroomsToReactors)) {
      timeRemainingRattler = rattlerTime/Plugin.config.MushroomProductionSpeedFactor/speedFactor;
    }
  }
  
  public void TryGrowJaffa() {
    if(timeRemainingJaffa < 0f  && (storageContainer.container.GetCount(TechType.OrangeMushroomSpore) < maxJaffa || Plugin.config.MushroomsToReactors)) { 
      timeRemainingJaffa = jaffaTime/Plugin.config.MushroomProductionSpeedFactor/speedFactor;
    }
  }
  
  public void OnHandHover(GUIHand hand) {
    if(!base.enabled) return;
    Constructable c = base.gameObject.GetComponent<Constructable>();
    if(!(bool)c || c.constructed) {
      string subscript = "Stores Filled";
    
      if (timeRemainingPink >= 0f || timeRemainingRattler >= 0f || timeRemainingJaffa >= 0f)
      {
        float pinkFrac = 1f - Mathf.Clamp01(timeRemainingPink / (pinkTime/Plugin.config.MushroomProductionSpeedFactor/speedFactor));
        float rattlerFrac = 1f - Mathf.Clamp01(timeRemainingRattler / (rattlerTime/Plugin.config.MushroomProductionSpeedFactor/speedFactor));
        float jaffaFrac = 1f - Mathf.Clamp01(timeRemainingJaffa / (jaffaTime/Plugin.config.MushroomProductionSpeedFactor/speedFactor));
        subscript = "Pink Cap "+(pinkFrac*100f).ToString("0\\%")+", Speckled Rattler "+(rattlerFrac*100f).ToString("0\\%")+", Jaffa Cup "+(jaffaFrac*100f).ToString("0\\%");
      }
      HandReticle.main.SetText(HandReticle.TextType.Hand, "Use Mushroom Terrarium", translate: false, GameInput.Button.LeftHand);
      HandReticle.main.SetText(HandReticle.TextType.HandSubscript, "Production Progress: "+subscript, translate: false);
      HandReticle.main.SetIcon(HandReticle.IconType.Interact);
    }
  }
  
  public void OnHandClick(GUIHand hand) {
    Constructable c = base.gameObject.GetComponent<Constructable>();
    if(!(bool)c || c.constructed) {
      storageContainer.Open();
    }
  }
  
  public void UpdateGrowth() {
    float dt = 1f * DayNightCycle.main.dayNightSpeed; // 1f is the invokation interval specified in InvokeRepeating
    if(dt > 0f) {
      if(timeRemainingPink > 0f) {
        timeRemainingPink = Mathf.Max(0f, timeRemainingPink - dt); 
      }
      if(timeRemainingRattler > 0f) {
        timeRemainingRattler = Mathf.Max(0f, timeRemainingRattler - dt); 
      }
      if(timeRemainingJaffa > 0f) {
        timeRemainingJaffa = Mathf.Max(0f, timeRemainingJaffa - dt);
      }
    }
    if(timeRemainingPink == 0f)
    {
      if(storageContainer.container.GetCount(TechType.PinkMushroom) < maxPinkCap && storageContainer.container.HasRoomFor(1,1)) {
        Pickupable component = Object.Instantiate(PrefabHelper.GetPrefabGameObject("7f9a765d-0b4e-4b3f-81b9-38b38beedf55")).GetComponent<Pickupable>();
        component.Pickup(events: false);
        InventoryItem item = new InventoryItem(component);
        storageContainer.container.UnsafeAdd(item);
        timeRemainingPink = -1f;
        TryGrowPink();
      } else if (Plugin.config.MushroomsToReactors) {
        // Adapted from WaterPark_Patches.cs in BetterACU mod by MrPurple6411 https://github.com/MrPurple6411/My-Subnautica-Mods/blob/main/BetterACU/Patches/WaterPark_Patches.cs
        var baseBioReactors =
					GetComponentInParent<SubRoot>()?.gameObject.GetComponentsInChildren<BaseBioReactor>()
							?.Where(baseBioReactor => baseBioReactor.container.HasRoomFor(1,1))
							.ToList() ?? new List<BaseBioReactor>();
            
        if(baseBioReactors.Count > 0)
        {
          baseBioReactors.Shuffle();
          var baseBioReactor = baseBioReactors.First();
          Pickupable component = Object.Instantiate(PrefabHelper.GetPrefabGameObject("7f9a765d-0b4e-4b3f-81b9-38b38beedf55")).GetComponent<Pickupable>();
          component.Pickup(false);
          baseBioReactor.container.AddItem(component);
          timeRemainingPink = -1f;
          TryGrowPink();
        } 
      }
    }
    if(timeRemainingRattler == 0f)
    {
      if(storageContainer.container.GetCount(TechType.PurpleRattle) < maxRattler && storageContainer.container.HasRoomFor(1,1)) {
        Pickupable component = Object.Instantiate(PrefabHelper.GetPrefabGameObject("28818d8a-5e50-41f0-8e14-44cb89a0b611")).GetComponent<Pickupable>();
        component.Pickup(events: false);
        InventoryItem item = new InventoryItem(component);
        storageContainer.container.UnsafeAdd(item);
        timeRemainingRattler = -1f;
        TryGrowRattler();
      } else if (Plugin.config.MushroomsToReactors) {
        // Adapted from WaterPark_Patches.cs in BetterACU mod by MrPurple6411 https://github.com/MrPurple6411/My-Subnautica-Mods/blob/main/BetterACU/Patches/WaterPark_Patches.cs
        var baseBioReactors =
					GetComponentInParent<SubRoot>()?.gameObject.GetComponentsInChildren<BaseBioReactor>()
							?.Where(baseBioReactor => baseBioReactor.container.HasRoomFor(1,1))
							.ToList() ?? new List<BaseBioReactor>();
            
        if(baseBioReactors.Count > 0)
        {
          baseBioReactors.Shuffle();
          var baseBioReactor = baseBioReactors.First();
          Pickupable component = Object.Instantiate(PrefabHelper.GetPrefabGameObject("28818d8a-5e50-41f0-8e14-44cb89a0b611")).GetComponent<Pickupable>();
          component.Pickup(false);
          baseBioReactor.container.AddItem(component);
          timeRemainingRattler = -1f;
          TryGrowRattler();
        } 
      }
    }
    if(timeRemainingJaffa == 0f)
    {
      if(storageContainer.container.GetCount(TechType.OrangeMushroomSpore) < maxJaffa && storageContainer.container.HasRoomFor(2,2)) {
        Pickupable component = Object.Instantiate(PrefabHelper.GetPrefabGameObject("ff727b98-8d85-416a-9ee7-4beda86d2ba2")).GetComponent<Pickupable>();
        component.Pickup(events: false);
        InventoryItem item = new InventoryItem(component);
        storageContainer.container.UnsafeAdd(item);
        timeRemainingJaffa = -1f;
        TryGrowJaffa();
      } else if (Plugin.config.MushroomsToReactors) {
        // Adapted from WaterPark_Patches.cs in BetterACU mod by MrPurple6411 https://github.com/MrPurple6411/My-Subnautica-Mods/blob/main/BetterACU/Patches/WaterPark_Patches.cs
        var baseBioReactors =
					GetComponentInParent<SubRoot>()?.gameObject.GetComponentsInChildren<BaseBioReactor>()
							?.Where(baseBioReactor => baseBioReactor.container.HasRoomFor(2,2))
							.ToList() ?? new List<BaseBioReactor>();
            
        if(baseBioReactors.Count > 0)
        {
          baseBioReactors.Shuffle();
          var baseBioReactor = baseBioReactors.First();
          Pickupable component = Object.Instantiate(PrefabHelper.GetPrefabGameObject("ff727b98-8d85-416a-9ee7-4beda86d2ba2")).GetComponent<Pickupable>();
          component.Pickup(false);
          baseBioReactor.container.AddItem(component);
          timeRemainingJaffa = -1f;
          TryGrowJaffa();
        } 
      }
    }
  }
  
  private bool IsAllowedToAdd(Pickupable pickupable, bool verbose) {
    if(pickupable.GetTechType() == TechType.PinkMushroom && storageContainer.container.GetCount(TechType.PinkMushroom) < maxPinkCap && storageContainer.container.HasRoomFor(1,1)) {
      return true;
    } else if (pickupable.GetTechType() == TechType.PurpleRattle && storageContainer.container.GetCount(TechType.PurpleRattle) < maxRattler && storageContainer.container.HasRoomFor(1,1)) {
      return true;
    } else if(pickupable.GetTechType() == TechType.OrangeMushroomSpore && storageContainer.container.GetCount(TechType.OrangeMushroomSpore) < maxJaffa && storageContainer.container.HasRoomFor(2,2)) {
      return true;
    }
    return false;
  }
  
  private void Start() { // invoked at the start of the next frame after AddComponent<MushroomGrower>
    // Try loading save data
    
    string id = GetComponent<PrefabIdentifier>().Id;
    if (!Plugin.SaveCache.mushroomGrowerSaves.TryGetValue(id, out var data))
    {
      // Start() has been called due to PLACING a MushroomGrower
      Plugin.Logger.LogDebug("MushroomGrower " + id + " was placed");
    } else {
      // Start() has been called due to LOADING a MushroomGrower
      _saveData = data;
      Plugin.Logger.LogDebug("MushroomGrower " + id + " was loaded");
    }
    
    timeRemainingPink = _saveData.timeRemainingPink;
    timeRemainingRattler = _saveData.timeRemainingRattler;
    timeRemainingJaffa = _saveData.timeRemainingJaffa;
    
    TryGrowPink();
    TryGrowRattler();
    TryGrowJaffa();
    InvokeRepeating("UpdateGrowth", 1f, 1f);
  }
  
  public void OnEnable() {
    //string id = GetComponent<PrefabIdentifier>().Id;
    //Plugin.Logger.LogDebug("MushroomGrower::OnEnable() called with id = " + id);
    Plugin.SaveCache.OnStartedSaving += OnBeforeSave;
    if (storageContainer == null) return;
    storageContainer.enabled = true;
    if (storageContainer.container == null) {
      storageContainer.CreateContainer();
    }
    storageContainer.container.onRemoveItem += RemoveItem;
    storageContainer.container.isAllowedToAdd = IsAllowedToAdd;
  }
  
  private void OnDisable() {
    //string id = GetComponent<PrefabIdentifier>().Id;
    //Plugin.Logger.LogDebug("MushroomGrower::OnDisable() called with id = " + id);
    Plugin.SaveCache.OnStartedSaving -= OnBeforeSave;
    if (storageContainer == null) return;
    storageContainer.container.onRemoveItem -= RemoveItem;
    storageContainer.container.isAllowedToAdd = null;
    storageContainer.enabled = false;
  }
  
  private void OnDestroy() {
    string id = GetComponent<PrefabIdentifier>().Id;
    if (Plugin.SaveCache.mushroomGrowerSaves.TryGetValue(id, out var data))
    {
      Plugin.SaveCache.mushroomGrowerSaves.Remove(id);
    } 
  }
  
  private void OnBeforeSave(object _, JsonFileEventArgs __) {
    _saveData.timeRemainingPink = timeRemainingPink;
    _saveData.timeRemainingRattler = timeRemainingRattler;
    _saveData.timeRemainingJaffa = timeRemainingJaffa;
    Plugin.SaveCache.mushroomGrowerSaves[GetComponent<PrefabIdentifier>().Id] = SaveData;
  }
  
  private void RemoveItem(InventoryItem item) {
    TryGrowPink();
    TryGrowRattler();
    TryGrowJaffa();
  }
}