using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers; // CraftDataHandler
using Nautilus.Extensions;
using UnityEngine;
using UnityEditor;

using BepInEx;
using Nautilus.Utility;

using UWE;

using System.Collections; // IEnumerator
using System.IO; // Path
using System.Reflection; // Assembly

namespace CompositeBuildables;

public static class ScienceBench1
{
    public static PrefabInfo Info { get; } = PrefabInfo
        .WithTechType("ScienceBench1", "Science Bench 1", "Bench with microscope.")
        .WithIcon(ImageUtils.LoadSpriteFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets", "ScienceBench1.png")));
        
    public static bool registered = false;
    
    public static IEnumerator ModifyPrefabAsync(GameObject obj) { // called on obj as obj is instantiated from this prefab
      
      // Wait for the PrefabHelper to complete asynchronous initialization
      
        yield return CoroutineHost.StartCoroutine(PrefabHelper.EnsureInitialized());

      // Configure placement rules
      
        ConstructableFlags constructableFlags = ConstructableFlags.Inside | ConstructableFlags.Rotatable | ConstructableFlags.Ground;

      // Find the GameObject that holds the model for the object being instantiated
      
        GameObject counterModel = obj.transform.Find("biodome_lab_counter_01").gameObject; 
        
      // Add Colliders
      
        BoxCollider box = obj.AddComponent<BoxCollider>();
        //box.center = new Vector3(0f,0.51f,0f);
        box.size = new Vector3(2.2433f,1.02f,0.8673f);
        
        ConstructableBounds cb = obj.AddComponent<ConstructableBounds>(); // This one is used for the pre-construction hologram
        cb.bounds.position = new Vector3(0f,0.52f,0f);
        cb.bounds.size = new Vector3(2.2433f,1.02f,0.8673f);
      //----------------------------------------------------------------------------------------------------------------
      // Use PrefabHelper to add models. All prefabs referenced must be listed in PrefabHelper.prefabIdModelNameList
      //----------------------------------------------------------------------------------------------------------------
      
        // Microscope
        Transform model = PrefabHelper.AttachModelFromPrefabTo("2cee55bc-6136-47c5-a1ed-14c8f3203856", counterModel.transform);
        model.position = counterModel.transform.position + new Vector3((float)0,(float)1.0203,(float)-0.1);
        
        // Cylindrical Test Tube
        model = PrefabHelper.AttachModelFromPrefabTo("7f601dd4-0645-414d-bb62-5b0b62985836", counterModel.transform); 
        model.rotation = Quaternion.Euler(0, -45, 0);
        model.position = counterModel.transform.position + new Vector3((float)-0.58,(float)1.0203,(float)0.1);
        
        // Large Square Jar with Lid
        model = PrefabHelper.AttachModelFromPrefabTo("e7f9c5e7-3906-4efd-b239-28783bce17a5", counterModel.transform);
        model.rotation = Quaternion.Euler(0, -40, 0);
        model.position = counterModel.transform.position + new Vector3((float)-0.77,(float)1.0203,(float)-0.1);
        
        // Small Square Jar with Lid
        model = PrefabHelper.AttachModelFromPrefabTo("e3e00261-92fc-4f52-bad2-4f0e5802a43d", counterModel.transform);
        model.rotation = Quaternion.Euler(0, -40, 0);
        model.position = counterModel.transform.position + new Vector3((float)-0.97,(float)1.0203,(float)0.12);
        
        // Clipboard
        model = PrefabHelper.AttachModelFromPrefabTo("a7519acf-6dec-429e-82ed-bbcf7a616c50", counterModel.transform);
        model.rotation = Quaternion.Euler(0, 235, 0) * Quaternion.Euler(-90, 0, 0);
        model.position = counterModel.transform.position + new Vector3((float)0.75,(float)1.0203,(float)0.1);
        
        // Hatching Enzymes
        model = PrefabHelper.AttachModelFromPrefabTo("fab9bc63-1916-4434-a9c6-231f421ffbb5", counterModel.transform); 
        model.rotation = Quaternion.Euler(0, -110, 0) * Quaternion.Euler(0, -20, 0);
        model.position = counterModel.transform.position + new Vector3((float)0.5,(float)1.0203,(float)-0.25);
      
      // Make skyApplier act on all of the added models
      
        var skyApplier = counterModel.EnsureComponent<SkyApplier>();
        skyApplier.anchorSky = Skies.Auto;
        skyApplier.renderers = counterModel.GetAllComponentsInChildren<Renderer>();

      // Add all components necessary for it to be built:
      
        PrefabUtils.AddConstructable(obj, Info.TechType, constructableFlags, counterModel);
      
      // Return
      
        yield return obj;
    }
    
    public static void UpdateRecipe()
    {
      if(!registered) return;
      
      switch (Plugin.config.RecipeComplexity) {
        case RecipeComplexityEnum.Simple:
          CraftDataHandler.SetRecipeData(Info.TechType, new RecipeData(
            new Ingredient(TechType.Titanium, 2)
          )); 
          break;
        case RecipeComplexityEnum.Standard:
          CraftDataHandler.SetRecipeData(Info.TechType, new RecipeData(
            new Ingredient(TechType.Titanium, 3), 
            new Ingredient(TechType.Glass, 1)
          )); // Planter Box plus Lantern Tree
          break;
        case RecipeComplexityEnum.Complex:
          CraftDataHandler.SetRecipeData(Info.TechType, new RecipeData(
            new Ingredient(TechType.Titanium, 4), // Bench + Microscope + Clipboard
            new Ingredient(TechType.Glass, 5), // Microscope + Cylindrical Test Tube + Square Jar with Lid + Small Square Jar with Lid
            new Ingredient(TechType.HatchingEnzymes, 1)
          )); 
          break;
      }
    }
    
    public static void Register()
    {
        // create prefab:
        CustomPrefab planterPrefab = new CustomPrefab(Info);
          // Initializes planterPrefab.Info to the argument Info
        
        CloneTemplate counterClone = new CloneTemplate(Info, "42eae67f-f31a-45a0-95bf-27e189de65a0"); // model is stored in object called "biodome_lab_counter_01"
          // CloneTemplate(PrefabInfo Info, string classIdToClone) calls CloneTemplate(Info, TechType.None, classIDToClone, null)
          // CloneTemplate(PrefabInfo info, TechType techTypeToClone, string classIdToClone, AssetReferenceGameObject prefabToClone) calls PrefabTemplate(info) and then sets _techTypeToClone through _spawnType
          // PrefabTemplate(info) sets the internal variable PrefabInfo info to argument info
          
        // modify the cloned model:
        counterClone.ModifyPrefabAsync += ModifyPrefabAsync;

        // assign the created clone model to the prefab itself:
        planterPrefab.SetGameObject(counterClone);
          // Sets the internal variable GameObject planterPrefab._prefab to counterClone
          // Sets the internal variable PrefabHelperAsync planterPrefab.Prefab to a function taking an argument obj and returning SyncPrefab(obj, prefab) where
          // IEnumerator SyncPrefab(IOut<GameObject> obj, GameObject prefab) {
          //  obj.Set(prefab);
          //  yield break; // indicates that the iterable IOut has no more elements
          // }

        // assign it to the correct tab in the builder tool:
        planterPrefab.SetPdaGroupCategory(TechGroup.Miscellaneous , TechCategory.Misc);
        
        planterPrefab.SetUnlock(TechType.StarshipDesk);

        // finally, register it into the game:
        planterPrefab.Register();
        registered = true;
        
        // Set Recipe Data
        UpdateRecipe();
    }
}