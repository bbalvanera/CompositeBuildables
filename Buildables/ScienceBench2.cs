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

public static class ScienceBench2
{
    public static PrefabInfo Info { get; } = PrefabInfo
        .WithTechType("ScienceBench2", "Science Bench 2", "Bench with sample analyzer.")
        .WithIcon(ImageUtils.LoadSpriteFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets", "ScienceBench2.png")));

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
        box.size = new Vector3(2.2433f,1.02f,0.8673f);
        
        ConstructableBounds cb = obj.AddComponent<ConstructableBounds>(); // This one is used for the pre-construction hologram
        cb.bounds.position = new Vector3(0f,0.52f,0f);
        cb.bounds.size = new Vector3(2.2433f,1.02f,0.8673f);
      
      //----------------------------------------------------------------------------------------------------------------
      // Use PrefabHelper to add models. All prefabs referenced must be listed in PrefabHelper.prefabIdModelNameList
      //----------------------------------------------------------------------------------------------------------------
      
        // Sample Analyzer
        Transform model = PrefabHelper.AttachModelFromPrefabTo("3fd9050b-4baf-4a78-a883-e774c648887c", counterModel.transform);
        model.position = counterModel.transform.position + new Vector3((float)0.5,(float)1.0203,(float)-0.1);
        
        // Cylindrical Test Tube
        model = PrefabHelper.AttachModelFromPrefabTo("7f601dd4-0645-414d-bb62-5b0b62985836", counterModel.transform); 
        model.rotation = Quaternion.Euler(0, -45, 0);
        model.position = counterModel.transform.position + new Vector3((float)-0.28,(float)1.0203,(float)0);
        
        // Cylindrical Test Tube 2
        model = PrefabHelper.AttachModelFromPrefabTo("7f601dd4-0645-414d-bb62-5b0b62985836", counterModel.transform);
        model.rotation = Quaternion.Euler(0, -40, 0);
        model.position = counterModel.transform.position + new Vector3((float)-0.17,(float)1.0203,(float)-0.2);
        
        // Clipboard
        model = PrefabHelper.AttachModelFromPrefabTo("a7519acf-6dec-429e-82ed-bbcf7a616c50", counterModel.transform);
        model.rotation = Quaternion.Euler(0, 280, 0) * Quaternion.Euler(-90, 0, 0);
        model.position = counterModel.transform.position + new Vector3((float)-0.75,(float)1.0203,(float)0.1);
      
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
          )); 
          break;
        case RecipeComplexityEnum.Complex:
          CraftDataHandler.SetRecipeData(Info.TechType, new RecipeData(
            new Ingredient(TechType.Titanium, 4), // Bench + Sample Analyzer + Clipboard
            new Ingredient(TechType.Glass, 3) // Sample Analyzeri + 2 Cylindrical Test Tube 
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