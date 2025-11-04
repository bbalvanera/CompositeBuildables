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

namespace CompositeBuildables;

public static class TubularShelfSmall
{
    public static PrefabInfo Info { get; } = PrefabInfo
        .WithTechType("TubularshelfSmall", "Tubular Shelf (Small)", "A tubular shelf holding chemicals. Sized for standard rooms.");
        // set the icon to that of the vanilla locker:
        //.WithIcon(SpriteManager.Get(TechType.PlanterBox));
        
    public static bool registered = false;
    
    public static IEnumerator ModifyPrefabAsync(GameObject obj) { // called on obj as obj is instantiated from this prefab
      
      // Wait for the PrefabHelper to complete asynchronous initialization
      
        yield return CoroutineHost.StartCoroutine(PrefabHelper.EnsureInitialized());

      // Configure placement rules
      
        ConstructableFlags constructableFlags = ConstructableFlags.Inside | ConstructableFlags.Rotatable | ConstructableFlags.Ground;

      // Find the GameObject that holds the model for the object being instantiated
      
        GameObject tubeShelfModel = obj.transform.Find("biodome_lab_tube_01").gameObject; 
        // Scale the collider and the model
        obj.transform.Find("Capsule").localScale = new Vector3((float)0.6,(float)0.55,(float)0.6);
        tubeShelfModel.transform.localScale = new Vector3((float)0.6,(float)0.55,(float)0.6);
        
      // Add and set a collider for use for the pre-construction hologram
      
        ConstructableBounds cb = obj.AddComponent<ConstructableBounds>();
        cb.bounds.position = new Vector3(0f,1.5f,0f);
        cb.bounds.extents = new Vector3(
          0.3f, 
          0.7f, 
          0.3f
        );
        
      
      //----------------------------------------------------------------------------------------------------------------
      // Use PrefabHelper to add models. All prefabs referenced must be listed in PrefabHelper.prefabIdModelNameList
      //----------------------------------------------------------------------------------------------------------------
      
        // Benzene
        /*Transform model = PrefabHelper.AttachModelFromPrefabTo("986b31ea-3c9d-498c-9f38-2af8ffe86ed7", tubeShelfModel);
        model.transform.localScale = new Vector3((float)2,(float)2,(float)2);
        model.position = tubeShelfModel.transform.position + new Vector3(0f,(float)1.0203,(float)-0.1);*/
        
        // For each Shelf
        for(int i = 0; i < 4; i++) {
          //Debug.Log("Iteration "+i.ToString());
          float yPos;
          float dy;
          float theta;
          string modelID;
          switch(i) {
            case 0:
              modelID = "7e164f67-f4e7-41fc-98a5-7a84ccaa1d09"; // Polyaniline.  On shelf index 0 this should be at y = 3.65. On shelf 1 at 2.58
              dy = 0.01f;
              yPos = 3.64f;
              theta = 0f;
              break;
            case 1:
              modelID = "96b1b863-2ff7-451b-aa38-8b3a06e72d63"; // Bleach. On shelf index 1 this should be at y = 2.86. = 0.28 higher than Polyaniline
              dy = 0.29f;
              yPos = 2.57f;
              theta = 90f;
              break;
            case 2:
              modelID = "74912c22-a383-48c7-8e9e-34b515c6aebb"; // Hydrochloric Acid. On shelf index 2 this should be at y = 1.54. On shelf 1 at 2.75
              dy = 0f;
              yPos = 1.54f;
              theta = 180f;
              break;
            case 3: default:
              modelID = "986b31ea-3c9d-498c-9f38-2af8ffe86ed7"; // Benzene. On shelf index 3 this should be at 0.53. shelf 1 at 2.57
              dy = 0f;
              yPos = 0.53f;
              theta = 270f;
              break;
          }
          Quaternion rot=Quaternion.AngleAxis(theta, Vector3.up);
          
          // Place Supply 1: 
          Transform model = PrefabHelper.AttachModelFromPrefabTo(modelID, tubeShelfModel.transform);
          model.localPosition = rot * new Vector3(0.25f,yPos+dy,0.25f);
          model.localScale = new Vector3(2f, 2f, 2f);
          
          // Place Supply 2:
          /*model = PrefabHelper.AttachModelFromPrefabTo("98be0944-e0b3-4fba-8f08-ca5d322c22f6", tubeShelfModel.transform);
          model.localPosition = rot * new Vector3(0.25f,yPos+0.125f,-0.25f);
          model.localScale = new Vector3(0.025f, 0.025f, 0.03f);
          model.GetComponent<Renderer>().material.SetColor("_Scale", new Color(0f, 0f, 0f, 0f)); // disable blowing in wind
          
          // Place Supply 3:
          model = PrefabHelper.AttachModelFromPrefabTo("c7faff7e-d9ff-41b4-9782-98d2e09d29c1", tubeShelfModel.transform);
          model.localPosition = rot * new Vector3(-0.25f,yPos+0.125f,-0.25f);
          model.localScale = new Vector3(1f, 1f, 1f);
          model.GetComponent<Renderer>().material.SetColor("_Scale", new Color(0f, 0f, 0f, 0f)); // disable blowing in wind
          
          // Place Supply 4:
          model = PrefabHelper.AttachModelFromPrefabTo("35056c71-5da7-4e73-be60-3c22c5c9e75c", tubeShelfModel.transform);
          model.localPosition = rot * new Vector3(-0.25f,yPos+0.125f,0.25f);
          model.localScale = new Vector3(0.3f, 0.3f, 0.3f);
          model.GetComponent<Renderer>().material.SetColor("_Scale", new Color(0f, 0f, 0f, 0f)); // disable blowing in wind*/
        }
      
      // Make skyApplier act on all of the added models
      
        var skyApplier = tubeShelfModel.EnsureComponent<SkyApplier>();
        skyApplier.anchorSky = Skies.Auto;
        skyApplier.renderers = tubeShelfModel.GetAllComponentsInChildren<Renderer>();

      // Add all components necessary for it to be built:
      
        PrefabUtils.AddConstructable(obj, Info.TechType, constructableFlags, tubeShelfModel);
      
      // Return
      
        yield return obj;
    }
    
    public static void UpdateRecipe()
    {
      if(!registered) return;
      
      switch (Plugin.config.RecipeComplexity) {
        case RecipeComplexityEnum.Simple:
          CraftDataHandler.SetRecipeData(Info.TechType, new RecipeData(
            new Ingredient(TechType.Titanium, 2),
            new Ingredient(TechType.Glass, 1)
          )); 
          break;
        case RecipeComplexityEnum.Standard:
          CraftDataHandler.SetRecipeData(Info.TechType, new RecipeData(
            new Ingredient(TechType.Titanium, 2),
            new Ingredient(TechType.Glass, 1) // TO BE COMPLETED
          )); 
          break;
        case RecipeComplexityEnum.Complex:
          CraftDataHandler.SetRecipeData(Info.TechType, new RecipeData(
            new Ingredient(TechType.Titanium, 2), 
            new Ingredient(TechType.Glass, 1) // TO BE COMPLETED
          )); 
          break;
      }
    }
    
    public static void Register(Config config)
    {
        // create prefab:
        CustomPrefab tubeShelfPrefab = new CustomPrefab(Info);
          // Initializes tubeShelfPrefab.Info to the argument Info
        
        CloneTemplate tubeShelfClone = new CloneTemplate(Info, "a36047b0-1533-4718-8879-d6ba9229c978"); // model is stored in object called "biodome_lab_counter_01"
          // CloneTemplate(PrefabInfo Info, string classIdToClone) calls CloneTemplate(Info, TechType.None, classIDToClone, null)
          // CloneTemplate(PrefabInfo info, TechType techTypeToClone, string classIdToClone, AssetReferenceGameObject prefabToClone) calls PrefabTemplate(info) and then sets _techTypeToClone through _spawnType
          // PrefabTemplate(info) sets the internal variable PrefabInfo info to argument info
          
        // modify the cloned model:
        tubeShelfClone.ModifyPrefabAsync += ModifyPrefabAsync;

        // assign the created clone model to the prefab itself:
        tubeShelfPrefab.SetGameObject(tubeShelfClone);
          // Sets the internal variable GameObject tubeShelfPrefab._prefab to tubeShelfClone
          // Sets the internal variable PrefabHelperAsync tubeShelfPrefab.Prefab to a function taking an argument obj and returning SyncPrefab(obj, prefab) where
          // IEnumerator SyncPrefab(IOut<GameObject> obj, GameObject prefab) {
          //  obj.Set(prefab);
          //  yield break; // indicates that the iterable IOut has no more elements
          // }

        // assign it to the correct tab in the builder tool:
        tubeShelfPrefab.SetPdaGroupCategory(TechGroup.Miscellaneous , TechCategory.Misc);
        
        tubeShelfPrefab.SetUnlock(TechType.PlanterBox);

        // Register it into the game:
        tubeShelfPrefab.Register();
        registered = true;
        
        // Set Recipe
        UpdateRecipe();
    }
}