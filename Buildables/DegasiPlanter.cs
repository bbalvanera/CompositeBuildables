using System.Collections;
using System.IO;
using System.Reflection;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Utility;
using UnityEngine;
using UWE;

namespace CompositeBuildables;

public static class DegasiPlanter
{
    public static PrefabInfo Info { get; } = PrefabInfo
        .WithTechType("DegasiPlanter", "Degasi Planter", "Bart Torgal's planter from Degasi Base 1-a.")
        .WithIcon(ImageUtils.LoadSpriteFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets", "DegasiPlanter.png")));

    public static bool registered = false;

    // called on obj as obj is instantiated from this prefab
    public static IEnumerator ModifyPrefabAsync(GameObject obj)
    {

        // Control loading distance
        PrefabUtils.AddBasicComponents(obj, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Near);

        // Wait for the PrefabHelper to complete asynchronous initialization
        yield return CoroutineHost.StartCoroutine(PrefabHelper.EnsureInitialized());

        // Find the GameObject that holds the model for the object being instantiated
        var planterModel = obj.transform.Find("model").gameObject; // Holds model called "Base_Interior_Planter_Tray_01"
        planterModel.transform.Rotate(new Vector3(0, 180, 0), Space.World); // rotate so that the "Front" faces the player while placing

        // Lantern Tree is special - we need to steal its model AND deep copy its FruitPlant component
        var lanternTreeObj = PrefabHelper.InstantiatePrefabInactive("8fa4a413-57fa-47a3-828d-de2255dbce4f");
        var modelName = PrefabHelper.GetModelName("8fa4a413-57fa-47a3-828d-de2255dbce4f");

        // Steal and re-position its model
        var lanternTreeModel = lanternTreeObj.transform.Find(modelName);
        lanternTreeModel.parent = planterModel.transform;
        lanternTreeModel.localPosition = new Vector3(0, 0.3757f, 0);

        // Fruits 15--20 cause problems in the Cyclops: their SphereColliders are too high up and send it flying. 
        // The solution is to remove the SphereColliders for these 5 fruits. It makes them un-pickable, but ~1/2 of them are out of reach anyway because this is an overworld-sized Lantern Tree
        Object.Destroy(lanternTreeModel.Find("Fruit_15").GetComponent<SphereCollider>());
        Object.Destroy(lanternTreeModel.Find("Fruit_16").GetComponent<SphereCollider>());
        Object.Destroy(lanternTreeModel.Find("Fruit_17").GetComponent<SphereCollider>());
        Object.Destroy(lanternTreeModel.Find("Fruit_18").GetComponent<SphereCollider>());
        Object.Destroy(lanternTreeModel.Find("Fruit_19").GetComponent<SphereCollider>());
        Object.Destroy(lanternTreeModel.Find("Fruit_20").GetComponent<SphereCollider>());

        // See FruitPlantClone for why the built-in FruitPlant can't be used here
        FruitPlantClone fruitPlant = obj.AddComponent<FruitPlantClone>();

        var oldFruitPlant = lanternTreeObj.GetComponent<FruitPlant>();
        fruitPlant.fruits = new PickPrefab[oldFruitPlant.fruits.Length];

        // spawn interval is handled by FruitPlantClone
        for (int i = 0; i < fruitPlant.fruits.Length; i++)
        {
            fruitPlant.fruits[i] = oldFruitPlant.fruits[i];
        }

        Object.Destroy(lanternTreeObj);

        // Medium Fern Palm
        Transform model = PrefabHelper.AttachModelFromPrefabTo("1d6d89dd-3e49-48b7-90e4-b521fbc3d36f", planterModel.transform);
        model.Rotate(new Vector3(0, 180, 0), Space.World);
        model.localPosition = new Vector3((float)0.185, (float)0.3757, (float)-1);

        // Fern Palm
        model = PrefabHelper.AttachModelFromPrefabTo("523879d5-3241-4a94-8588-cb3b38945119", planterModel.transform);
        model.Rotate(new Vector3(0, 180, 0), Space.World);
        model.localPosition = new Vector3((float)-0.71, (float)0.3757, (float)-0.481);

        // Spindly Tall Plant
        model = PrefabHelper.AttachModelFromPrefabTo("154a88c1-6c7f-44e4-974e-c52d2f48fa28", planterModel.transform);
        model.localScale = new Vector3((float)0.3503, (float)0.3503, (float)0.3503);
        model.Rotate(new Vector3(0, 180, 0), Space.World);
        model.localPosition = new Vector3((float)0.509, (float)0.3757, (float)0.611);

        // Vine Fill
        model = PrefabHelper.AttachModelFromPrefabTo("75ab087f-9934-4e2a-b025-02fc333a5c99", planterModel.transform);
        model.localScale = new Vector3((float)0.3122, (float)0.3122, (float)0.3122);
        model.Rotate(new Vector3(0, 180, 0), Space.World);
        model.localPosition = new Vector3((float)0.053, (float)0.3757, (float)-0.009);

        // Voxel Shrub
        model = PrefabHelper.AttachModelFromPrefabTo("28ec1137-da13-44f3-b76d-bac12ab766d1", planterModel.transform);
        model.localPosition = new Vector3((float)-0.4871, (float)0.3757, (float)0.35);
        model.Rotate(new Vector3(0, 180, 0), Space.World);
        model.GetComponent<Renderer>().material.SetColor("_Scale", new Color(0f, 0f, 0f, 0f));

        // Jaffa Cup
        model = PrefabHelper.AttachModelFromPrefabTo("35056c71-5da7-4e73-be60-3c22c5c9e75c", planterModel.transform);
        model.localScale = new Vector3((float)0.4366, (float)0.4366, (float)0.4366);
        model.localPosition = new Vector3((float)1.0079, (float)0.2805, (float)0.627);
        model.Rotate(new Vector3(0, 180, 0), Space.World);

        // Grub Basket
        model = PrefabHelper.AttachModelFromPrefabTo("28c73640-a713-424a-91c6-2f5d4672aaea", planterModel.transform);
        model.localScale = new Vector3((float)0.4366, (float)0.4366, (float)0.4366);
        model.localPosition = new Vector3((float)0.578, (float)0.3757, (float)-1.028);
        model.Rotate(new Vector3(0, 180, 0), Space.World);
        model.GetComponent<Renderer>().material.SetColor("_Scale", new Color(0f, 0f, 0f, 0f));

        // Red-Tipped Fern
        model = PrefabHelper.AttachModelFromPrefabTo("559fe0c7-1754-40f5-9453-b537900b3ac4", planterModel.transform);
        model.localScale = new Vector3((float)0.6, (float)0.6, (float)0.6);
        model.localPosition = new Vector3((float)-0.578, (float)0.2805, (float)-0.9);
        model.Rotate(new Vector3(0, 180, 0), Space.World);
        model.GetComponent<Renderer>().material.SetColor("_Scale", new Color(0f, 0f, 0f, 0f));

        // Speckled Rattler
        model = PrefabHelper.AttachModelFromPrefabTo("98be0944-e0b3-4fba-8f08-ca5d322c22f6", planterModel.transform);
        model.localScale = new Vector3((float)0.02, (float)0.02, (float)0.028);
        model.localPosition = new Vector3((float)-0.4871, (float)0.271, (float)-0.05);
        model.Rotate(new Vector3(0, 180, 0), Space.World);
        model.GetComponent<Renderer>().material.SetColor("_Scale", new Color(0f, 0f, 0f, 0f));

        // Pink Cap
        model = PrefabHelper.AttachModelFromPrefabTo("c7faff7e-d9ff-41b4-9782-98d2e09d29c1", planterModel.transform);
        model.localPosition = new Vector3((float)0.6871, (float)0.471, (float)-0.05);
        model.Rotate(new Vector3(0, 180, 0), Space.World);
        model.GetComponent<Renderer>().material.SetColor("_Scale", new Color(0f, 0f, 0f, 0f));

        // Make skyApplier act on all of the added models
        var skyApplier = planterModel.EnsureComponent<SkyApplier>();
        skyApplier.anchorSky = Skies.Auto;
        skyApplier.renderers = planterModel.GetAllComponentsInChildren<Renderer>();

        // Configure placement rules
        var constructableFlags = ConstructableFlags.Inside | ConstructableFlags.Rotatable | ConstructableFlags.Ground;

        // Add all components necessary for it to be built:
        PrefabUtils.AddConstructable(obj, Info.TechType, constructableFlags, planterModel);

        // Remove the Planter component so that seeds can't be planted/grow in the composite object
        Object.Destroy(obj.GetComponent<Planter>());

        // Remove the StorageContainer component as it is unnecessary
        Object.Destroy(obj.GetComponent<StorageContainer>());
        Object.Destroy(obj.transform.Find("slots").gameObject);
        Object.Destroy(obj.transform.Find("slots_big").gameObject);
        Object.Destroy(obj.transform.Find("StorageRoot").gameObject);
        Object.Destroy(obj.transform.Find("grownPlants").gameObject);

        yield return obj;
    }

    public static void UpdateRecipe()
    {
        if (!registered)
            return;

        switch (Plugin.config.RecipeComplexity)
        {
            case RecipeComplexityEnum.Simple:
                CraftDataHandler.SetRecipeData(Info.TechType, new RecipeData(
                  new Ingredient(TechType.Titanium, 4)
                )); // Planter Box
                break;
            case RecipeComplexityEnum.Standard:
                CraftDataHandler.SetRecipeData(Info.TechType, new RecipeData(
                  new Ingredient(TechType.Titanium, 4),
                  new Ingredient(TechType.HangingFruit, 1)
                )); // Planter Box plus Lantern Tree
                break;
            case RecipeComplexityEnum.Complex:
                CraftDataHandler.SetRecipeData(Info.TechType, new RecipeData(
                  new Ingredient(TechType.Titanium, 4), // Planter Box
                  new Ingredient(TechType.HangingFruit, 1), // Lantern Tree
                  new Ingredient(TechType.FernPalmSeed, 2), // 2 Fern Palms
                  new Ingredient(TechType.OrangePetalsPlantSeed, 1), // Grub Basket
                  new Ingredient(TechType.OrangeMushroomSpore, 1), // Jaffa Cup
                  new Ingredient(TechType.PinkMushroomSpore, 1), // Pink Cap
                  new Ingredient(TechType.PurpleRattleSpore, 1), // Speckled Rattler
                  new Ingredient(TechType.PinkFlowerSeed, 1) // Voxel Shrub
                ));
                break;
        }
    }

    public static void Register()
    {
        // create prefab:
        CustomPrefab planterPrefab = new CustomPrefab(Info);
        // Initializes planterPrefab.Info to the argument Info

        CloneTemplate planterClone = new CloneTemplate(Info, "87f5d3e6-e00b-4cf3-be39-0a9c7e951b84"); // model is stored in object called "model"
                                                                                                      // CloneTemplate(PrefabInfo Info, string classIdToClone) calls CloneTemplate(Info, TechType.None, classIDToClone, null)
                                                                                                      // CloneTemplate(PrefabInfo info, TechType techTypeToClone, string classIdToClone, AssetReferenceGameObject prefabToClone) calls PrefabTemplate(info) and then sets _techTypeToClone through _spawnType
                                                                                                      // PrefabTemplate(info) sets the internal variable PrefabInfo info to argument info

        // modify the cloned model:
        planterClone.ModifyPrefabAsync += ModifyPrefabAsync;

        // assign the created clone to the prefab itself:
        planterPrefab.SetGameObject(planterClone);
        // Sets the internal variable GameObject planterPrefab._prefab to planterClone
        // Sets the internal variable PrefabHelperAsync planterPrefab.Prefab to a function taking an argument obj and returning SyncPrefab(obj, prefab) where
        // IEnumerator SyncPrefab(IOut<GameObject> obj, GameObject prefab) {
        //  obj.Set(prefab);
        //  yield break; // indicates that the iterable IOut has no more elements
        // }

        // assign it to the correct tab in the builder tool:
        planterPrefab.SetPdaGroupCategory(TechGroup.InteriorModules, TechCategory.InteriorModule);

        planterPrefab.SetUnlock(TechType.PlanterBox);

        // Register into the game:
        planterPrefab.Register();
        registered = true;

        // Set Recipe Data
        UpdateRecipe();
    }
}