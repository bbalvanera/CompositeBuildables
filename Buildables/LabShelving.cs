using System.Collections; // IEnumerator
using System.IO; // Path
using System.Reflection; // Assembly
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers; // CraftDataHandler
using Nautilus.Utility;
using UnityEngine;
using UWE;

namespace CompositeBuildables;

public static class LabShelving
{
    public static PrefabInfo Info { get; } = PrefabInfo
        .WithTechType("LabShelving", "Lab Shelving Unit", "Standard shelving unit with chemicals.")
        .WithIcon(ImageUtils.LoadSpriteFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets", "LabShelving.png")));

    public static bool registered = false;

    public static IEnumerator ModifyPrefabAsync(GameObject obj)
    { // called on obj as obj is instantiated from this prefab

        // Wait for the PrefabHelper to complete asynchronous initialization

        yield return CoroutineHost.StartCoroutine(PrefabHelper.EnsureInitialized());

        // Configure placement rules

        ConstructableFlags constructableFlags = ConstructableFlags.Inside | ConstructableFlags.Rotatable | ConstructableFlags.Ground;

        // Find the GameObject that holds the model for the object being instantiated

        GameObject mainModel = obj.transform.Find("biodome_lab_shelf_01").gameObject;
        mainModel.transform.localScale = new Vector3(0.42f, 0.42f, 0.42f); // copied from Decorations Mod
        mainModel.name = "MainShelf";

        // Scale the Cube as well for proper collision

        GameObject cubeModel = obj.transform.Find("Cube").gameObject;
        cubeModel.transform.localScale = new Vector3(0.42f, 0.42f, 0.42f);

        // Add Colliders

        BoxCollider box = obj.AddComponent<BoxCollider>();
        box.size = new Vector3(0.8f, 0.9f, 0.5f);

        ConstructableBounds cb = obj.AddComponent<ConstructableBounds>(); // This one is used for the pre-construction hologram
        cb.bounds.position = new Vector3(0f, 0.46f, 0f);
        cb.bounds.size = new Vector3(0.8f, 0.9f, 0.5f);

        //----------------------------------------------------------------------------------------------------------------
        // Use PrefabHelper to add models. All prefabs referenced must be listed in PrefabHelper.prefabIdModelNameList
        //----------------------------------------------------------------------------------------------------------------

        // Second Shelving Unit
        Transform model = PrefabHelper.AttachModelFromPrefabTo("33acd899-72fe-4a98-85f9-b6811974fbeb", mainModel.transform);
        model.localScale = new Vector3(0.25f, 1f, 1f);
        Object.Destroy(model.Find("biodome_lab_shelf_01_thing").gameObject);
        Object.Destroy(model.Find("biodome_lab_shelf_01_thing_glass").gameObject);
        model.rotation = Quaternion.Euler(0, -90, 0);
        model.position = mainModel.transform.position + new Vector3(0.785f, 0.001f, 0.25f);
        model.name = "SecondShelf";

        // Third Shelving Unit
        model = PrefabHelper.AttachModelFromPrefabTo("33acd899-72fe-4a98-85f9-b6811974fbeb", mainModel.transform);
        model.localScale = new Vector3(0.25f, 1f, 1f);
        Object.Destroy(model.Find("biodome_lab_shelf_01_thing").gameObject);
        Object.Destroy(model.Find("biodome_lab_shelf_01_thing_glass").gameObject);
        model.rotation = Quaternion.Euler(0, 90, 0);
        model.position = mainModel.transform.position + new Vector3(-0.785f, 0.001f, 0.25f);
        model.name = "ThirdShelf";

        // Sample Analyzers
        for (int i = 0; i < 2; i++)
        {
            model = PrefabHelper.AttachModelFromPrefabTo("3fd9050b-4baf-4a78-a883-e774c648887c", mainModel.transform);
            model.localScale = new Vector3(2.05f, 2.05f, 2.05f);
            model.localPosition = new Vector3(1f - i * 2f, 2.45f, 0.75f);
            // RENAME to avoid name conflict issue
            model.name = "SampleAnalyzer" + i;
        }

        // Polyaniline
        model = PrefabHelper.AttachModelFromPrefabTo("7e164f67-f4e7-41fc-98a5-7a84ccaa1d09", mainModel.transform);
        model.localScale = new Vector3(2f, 2f, 2f);
        model.localPosition = new Vector3(0f, 2.45f, 0.75f);
        model.name = "Polyaniline1";

        // Cylindrical Test Tubes
        for (int i = 0; i < 2; i++)
        {
            model = PrefabHelper.AttachModelFromPrefabTo("7f601dd4-0645-414d-bb62-5b0b62985836", mainModel.transform);
            model.localScale = new Vector3(2f, 2f, 2f);
            model.Rotate(new Vector3(0, 90, 0), Space.World);
            model.localPosition = new Vector3(1.4f - i * 0.4f, 3.45f, 0.75f);
            model.name = "TestTube" + i;
        }

        // Cylindrical Bell Jars
        for (int i = 0; i < 3; i++)
        {
            model = PrefabHelper.AttachModelFromPrefabTo("a227d6b6-d64c-4bf0-b919-2db02d67d037", mainModel.transform);
            model.localScale = new Vector3(2f, 2f, 2f);
            model.localPosition = new Vector3(-1.4f + i * 0.4f, 3.45f, 0.75f);
            model.name = "BellJar" + i;
        }

        // Benzene
        for (int i = 0; i < 3; i++)
        {
            model = PrefabHelper.AttachModelFromPrefabTo("986b31ea-3c9d-498c-9f38-2af8ffe86ed7", mainModel.transform);
            model.localScale = new Vector3(2.3f, 2.3f, 2.3f);
            model.localPosition = new Vector3(1.4f - i * 0.4f, 1.45f, 0.75f);
            model.name = "Benzene" + i;
        }

        // Hydrochloric Acid
        for (int i = 0; i < 2; i++)
        {
            model = PrefabHelper.AttachModelFromPrefabTo("74912c22-a383-48c7-8e9e-34b515c6aebb", mainModel.transform);
            model.localScale = new Vector3(2f, 2f, 2f);
            model.localPosition = new Vector3(-1.4f + i * 0.4f, 1.45f, 0.75f);
            model.name = "HydrochloricAcid" + i;
        }

        // Lubricant
        for (int i = 0; i < 3; i++)
        {
            model = PrefabHelper.AttachModelFromPrefabTo("96b1b863-2ff7-451b-aa38-8b3a06e72d63", mainModel.transform);
            model.localScale = new Vector3(2f, 2f, 2f);
            model.Rotate(new Vector3(0, 65, 0), Space.World);
            model.localPosition = new Vector3(1.4f - i * 0.4f, 0.65f, 0.75f);
            model.name = "Lubricant" + i;
        }

        // Bleach
        for (int i = 0; i < 3; i++)
        {
            model = PrefabHelper.AttachModelFromPrefabTo("fbfacd7b-32a8-4065-8c25-b0a703f2683b", mainModel.transform);
            model.localScale = new Vector3(2f, 2f, 2f);
            model.Rotate(new Vector3(0, 135, 0), Space.World);
            model.localPosition = new Vector3(-1.4f + i * 0.4f, 0.65f, 0.75f);
            model.name = "Bleach" + i;
        }

        // Cylindrical Test Tube
        /*model = PrefabHelper.AttachModelFromPrefabTo("7f601dd4-0645-414d-bb62-5b0b62985836", mainModel.transform); 
        model.rotation = Quaternion.Euler(0, -45, 0);
        model.position = mainModel.transform.position + new Vector3((float)-0.58,(float)1.0203,(float)0.1);
        
        // Large Square Jar with Lid
        model = PrefabHelper.AttachModelFromPrefabTo("e7f9c5e7-3906-4efd-b239-28783bce17a5", mainModel.transform);
        model.rotation = Quaternion.Euler(0, -40, 0);
        model.position = mainModel.transform.position + new Vector3((float)-0.77,(float)1.0203,(float)-0.1);
        
        // Small Square Jar with Lid
        model = PrefabHelper.AttachModelFromPrefabTo("e3e00261-92fc-4f52-bad2-4f0e5802a43d", mainModel.transform);
        model.rotation = Quaternion.Euler(0, -40, 0);
        model.position = mainModel.transform.position + new Vector3((float)-0.97,(float)1.0203,(float)0.12);
        
        // Clipboard
        model = PrefabHelper.AttachModelFromPrefabTo("a7519acf-6dec-429e-82ed-bbcf7a616c50", mainModel.transform);
        model.rotation = Quaternion.Euler(0, 235, 0) * Quaternion.Euler(-90, 0, 0);
        model.position = mainModel.transform.position + new Vector3((float)0.75,(float)1.0203,(float)0.1);
        
        // Hatching Enzymes
        model = PrefabHelper.AttachModelFromPrefabTo("fab9bc63-1916-4434-a9c6-231f421ffbb5", mainModel.transform); 
        model.rotation = Quaternion.Euler(0, -110, 0) * Quaternion.Euler(0, -20, 0);
        model.position = mainModel.transform.position + new Vector3((float)0.5,(float)1.0203,(float)-0.25);*/

        // Make skyApplier act on all of the added models

        var skyApplier = mainModel.EnsureComponent<SkyApplier>();
        skyApplier.anchorSky = Skies.Auto;
        skyApplier.renderers = mainModel.GetAllComponentsInChildren<Renderer>();

        // Add all components necessary for it to be built:

        PrefabUtils.AddConstructable(obj, Info.TechType, constructableFlags, mainModel);

        // Return

        yield return obj;
    }

    public static void UpdateRecipe()
    {
        if (!registered) return;

        switch (Plugin.config.RecipeComplexity)
        {
            case RecipeComplexityEnum.Simple:
                CraftDataHandler.SetRecipeData(Info.TechType, new RecipeData(
                  new Ingredient(TechType.Titanium, 2)
                )); // Main Shelf
                break;
            case RecipeComplexityEnum.Standard:
                CraftDataHandler.SetRecipeData(Info.TechType, new RecipeData(
                  new Ingredient(TechType.Titanium, 3), // Main Shelf + Side Shelves
                  new Ingredient(TechType.Glass, 1),
                  new Ingredient(TechType.Polyaniline, 1),
                  new Ingredient(TechType.Benzene, 1),
                  new Ingredient(TechType.HydrochloricAcid, 1),
                  new Ingredient(TechType.Lubricant, 1),
                  new Ingredient(TechType.Bleach, 1)
                ));
                break;
            case RecipeComplexityEnum.Complex:
                CraftDataHandler.SetRecipeData(Info.TechType, new RecipeData(
                  new Ingredient(TechType.Titanium, 8), // Shelves + 2 Sample Analyzer + 3 Bell Jars
                  new Ingredient(TechType.Glass, 7), // 2 Sample Analyzer + 2 Cylindrical Test Tubes + 3 Bell Jars
                  new Ingredient(TechType.Polyaniline, 1),
                  new Ingredient(TechType.Benzene, 3),
                  new Ingredient(TechType.HydrochloricAcid, 2),
                  new Ingredient(TechType.Lubricant, 3),
                  new Ingredient(TechType.Bleach, 3)
                ));
                break;
        }
    }

    public static void Register()
    {
        // create prefab:

        CustomPrefab mainPrefab = new CustomPrefab(Info);
        // Initializes mainPrefab.Info to the argument Info

        CloneTemplate mainClone = new CloneTemplate(Info, "33acd899-72fe-4a98-85f9-b6811974fbeb"); // model is stored in object called "biodome_lab_shelf_01"
                                                                                                   // CloneTemplate(PrefabInfo Info, string classIdToClone) calls CloneTemplate(Info, TechType.None, classIDToClone, null)
                                                                                                   // CloneTemplate(PrefabInfo info, TechType techTypeToClone, string classIdToClone, AssetReferenceGameObject prefabToClone) calls PrefabTemplate(info) and then sets _techTypeToClone through _spawnType
                                                                                                   // PrefabTemplate(info) sets the internal variable PrefabInfo info to argument info

        // modify the cloned model:
        mainClone.ModifyPrefabAsync += ModifyPrefabAsync;

        // assign the created clone model to the prefab itself:
        mainPrefab.SetGameObject(mainClone);
        // Sets the internal variable GameObject mainPrefab._prefab to mainClone
        // Sets the internal variable PrefabHelperAsync mainPrefab.Prefab to a function taking an argument obj and returning SyncPrefab(obj, prefab) where
        // IEnumerator SyncPrefab(IOut<GameObject> obj, GameObject prefab) {
        //  obj.Set(prefab);
        //  yield break; // indicates that the iterable IOut has no more elements
        // }

        // assign it to the correct tab in the builder tool:
        mainPrefab.SetPdaGroupCategory(TechGroup.Miscellaneous, TechCategory.Misc);

        mainPrefab.SetUnlock(TechType.StarshipDesk);

        // Register it into the game:
        mainPrefab.Register();
        registered = true;

        // Set Recipe
        UpdateRecipe();
    }
}