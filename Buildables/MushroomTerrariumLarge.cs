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

public static class MushroomTerrariumLarge
{
    public static PrefabInfo Info { get; } = PrefabInfo
        .WithTechType("MushroomTerrariumLarge", "Mushroom Terrarium (Large)", "A tubular terrarium for cultivating mushrooms. Sized for domed rooms.")
        .WithIcon(ImageUtils.LoadSpriteFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets", "MushroomTerrariumLarge.png")));
        
    public static bool registered = false;
    
    public static IEnumerator ModifyPrefabAsync(GameObject obj) { // called on obj as obj is instantiated from this prefab
      
      // Wait for the PrefabHelper to complete asynchronous initialization
      
        yield return CoroutineHost.StartCoroutine(PrefabHelper.EnsureInitialized());
        
        // Add MushroomGrower component
        
          MushroomGrower mg = obj.AddComponent<MushroomGrower>();
          mg.storageContainer = PrefabUtils.AddStorageContainer(obj, "StorageRoot", "MushroomTerrariumSmall", 6, 5, false);
          mg.storageContainer.storageLabel = "MUSHROOM TERRARIUM";
          mg.speedFactor = 2f;
          mg.OnEnable();
        
      // Place storage container 
        
        StorageContainer container = PrefabUtils.AddStorageContainer(obj, "StorageRoot", "MushroomTerrariumSmall", 6, 8, false);

      // Configure placement rules
      
        ConstructableFlags constructableFlags = ConstructableFlags.Inside | ConstructableFlags.Rotatable | ConstructableFlags.Ground;

      // Find the GameObject that holds the model for the object being instantiated
      
        GameObject tubeShelfModel = obj.transform.Find("biodome_lab_tube_01").gameObject; 
        // Scale the collider and the model
        obj.transform.Find("Capsule").localScale = new Vector3((float)1,(float)1.065,(float)1);
        tubeShelfModel.transform.localScale = new Vector3((float)1,(float)1.065,(float)1);
        
      // Add and set a collider for use for the pre-construction hologram
        
        ConstructableBounds cb = obj.AddComponent<ConstructableBounds>();
        cb.bounds.position = new Vector3(0f,2.0f,0f);
        cb.bounds.extents = new Vector3(
          0.5f, 
          0.85f, 
          0.5f
        );
      
      //----------------------------------------------------------------------------------------------------------------
      // Use PrefabHelper to add models. All prefabs referenced must be listed in PrefabHelper.prefabIdModelNameList
      //----------------------------------------------------------------------------------------------------------------
      
        // For each Shelf
        for(int i = 0; i < 4; i++) {
          // Place a Composite Pot on each shelf
          Transform planterModel = PrefabHelper.AttachModelFromPrefabTo("5c8cb04b-9f30-49e7-8687-0cbb338fc7fa", tubeShelfModel.transform);
          planterModel.localScale = new Vector3(1.8f, 0.6f, 1.8f);
          planterModel.Find("Base_interior_Planter_Pot_02").Find("pot_generic_plant_02").localScale = new Vector3(2f, 6f, 2f);
          float yPos;
          float theta;
          switch(i) {
            case 0:
              yPos = 3.65f;
              theta = 0f;
              break;
            case 1:
              yPos = 2.55f;
              theta = 90f;
              break;
            case 2:
              yPos = 1.55f;
              theta = 180f;
              break;
            case 3: default:
              yPos = 0.55f;
              theta = 270f;
              break;
          }
          Quaternion rot=Quaternion.AngleAxis(theta, Vector3.up);
          
          //yPos = yPos - 0.05f; // this causes the trays to overlap the shelves
          planterModel.localPosition = new Vector3(0f,yPos,0f);
          planterModel.name = planterModel.name + i.ToString(); // rename model so that when we instantate another model with the same name we don't have a name collision
          
          // Activate vine fill built in to planters
          Transform model = planterModel.transform.Find("Tropical_Plant_10a");
          model.gameObject.SetActive(true);
          model.parent = tubeShelfModel.transform;
          //Transform model = PrefabHelper.AttachModelFromPrefabTo("75ab087f-9934-4e2a-b025-02fc333a5c99", tubeShelfModel.transform);
          model.localPosition = new Vector3(0f,yPos+0.225f,0f);
          model.localScale = new Vector3(0.1f,0.1f, 0.06f);
          model.parent = planterModel;
          
          // Place Mushroom 1: Qunituple Pink Cap
          model = PrefabHelper.AttachModelFromPrefabTo("c7faff7e-d9ff-41b4-9782-98d2e09d29c1", tubeShelfModel.transform);
          model.localPosition = rot * new Vector3(0.25f,yPos+0.125f,0.25f);
          model.localScale = new Vector3(1f, 1f, 1f);
          //model.rotation = Quaternion.Euler(theta+180, 0, 0);
          model.GetComponent<Renderer>().material.SetColor("_Scale", new Color(0f, 0f, 0f, 0f)); // disable blowing in wind
          model.parent = planterModel;
          
          // Place Mushroom 2: Triple Speckled Rattler
          model = PrefabHelper.AttachModelFromPrefabTo("98be0944-e0b3-4fba-8f08-ca5d322c22f6", tubeShelfModel.transform);
          model.localPosition = rot * new Vector3(0.25f,yPos+0.125f,-0.25f);
          model.localScale = new Vector3(0.025f, 0.025f, 0.03f);
          //model.rotation = Quaternion.Euler(theta-30, 0, 0);
          model.GetComponent<Renderer>().material.SetColor("_Scale", new Color(0f, 0f, 0f, 0f)); // disable blowing in wind
          model.parent = planterModel;
          
          // Place Mushroom 3: Qunituple Pink Cap
          model = PrefabHelper.AttachModelFromPrefabTo("c7faff7e-d9ff-41b4-9782-98d2e09d29c1", tubeShelfModel.transform);
          model.localPosition = rot * new Vector3(-0.25f,yPos+0.125f,-0.25f);
          model.localScale = new Vector3(1f, 1f, 1f);
          //model.rotation = Quaternion.Euler(theta, 0, 0);
          model.GetComponent<Renderer>().material.SetColor("_Scale", new Color(0f, 0f, 0f, 0f)); // disable blowing in wind
          model.parent = planterModel;
          
          // Place Mushroom 4: Jaffa Cup
          model = PrefabHelper.AttachModelFromPrefabTo("35056c71-5da7-4e73-be60-3c22c5c9e75c", tubeShelfModel.transform);
          model.localPosition = rot * new Vector3(-0.25f,yPos+0.125f,0.25f);
          model.localScale = new Vector3(0.3f, 0.3f, 0.3f);
          model.GetComponent<Renderer>().material.SetColor("_Scale", new Color(0f, 0f, 0f, 0f)); // disable blowing in wind
          model.parent = planterModel;
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
            new Ingredient(TechType.Titanium, 4),
            new Ingredient(TechType.Glass, 2)
          )); 
          break;
        case RecipeComplexityEnum.Standard:
          CraftDataHandler.SetRecipeData(Info.TechType, new RecipeData(
            new Ingredient(TechType.Titanium, 4),
            new Ingredient(TechType.Glass, 2),
            new Ingredient(TechType.OrangePetalsPlantSeed, 1), // Grub Basket
            new Ingredient(TechType.OrangeMushroomSpore, 1), // Jaffa Cup
            new Ingredient(TechType.PinkMushroomSpore, 2), // Pink Cap
            new Ingredient(TechType.PurpleRattleSpore, 1) // Speckled Rattler
          )); 
          break;
        case RecipeComplexityEnum.Complex:
          CraftDataHandler.SetRecipeData(Info.TechType, new RecipeData(
            new Ingredient(TechType.Titanium, 4), 
            new Ingredient(TechType.Glass, 2),
            new Ingredient(TechType.OrangePetalsPlantSeed, 4), // Grub Basket
            new Ingredient(TechType.OrangeMushroomSpore, 4), // Jaffa Cup
            new Ingredient(TechType.PinkMushroomSpore, 8), // Pink Cap
            new Ingredient(TechType.PurpleRattleSpore, 4) // Speckled Rattler
          )); 
          break;
      }
    }
    
    public static void Register()
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
        
        // Set Recipe Data
        UpdateRecipe();
    }
}