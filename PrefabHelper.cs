using System;
using System.Collections.Generic;
using UnityEngine;
using UWE;

namespace CompositeBuildables;

internal static class PrefabHelper
{
    private class PrefabInfo
    {
        public string name { get; init; }
        public GameObject prefab { get; init; }
    }

    private class PrefabActiveGuard : IDisposable
    {
        private readonly GameObject prefab;
        private readonly bool activeState;

        public PrefabActiveGuard(GameObject prefab)
        {
            this.prefab = prefab;
            activeState = prefab.activeSelf;

            prefab.SetActive(false);
        }

        public void Dispose()
        {
            prefab.SetActive(activeState);
        }
    }

    private static bool isInit = false;
    private static readonly Dictionary<string, PrefabInfo> prefabs = new();

    private static readonly Dictionary<string, string> prefabClassIDs = new()
    {
        { "42eae67f-f31a-45a0-95bf-27e189de65a0", "biodome_lab_counter_01" },
        { "a36047b0-1533-4718-8879-d6ba9229c978", "biodome_lab_tube_01" },
        { "33acd899-72fe-4a98-85f9-b6811974fbeb", "biodome_lab_shelf_01" },
        { "68e7dcd8-fe09-4dac-b966-85463c3c58af", "biodome_Robot_Arm" },
        { "2cee55bc-6136-47c5-a1ed-14c8f3203856", "discovery_lab_props_01" },
        { "9c5f22de-5049-48bb-ad1e-0d78c894210e", "discovery_lab_props_02" },
        { "3fd9050b-4baf-4a78-a883-e774c648887c", "discovery_lab_props_03" },
        { "7f601dd4-0645-414d-bb62-5b0b62985836", "biodome_lab_containers_tube_01" },
        { "a227d6b6-d64c-4bf0-b919-2db02d67d037", "biodome_lab_containers_tube_02" },
        { "e7f9c5e7-3906-4efd-b239-28783bce17a5", "biodome_lab_containers_close_01" },
        { "e3e00261-92fc-4f52-bad2-4f0e5802a43d", "biodome_lab_containers_close_02" },
        { "a7519acf-6dec-429e-82ed-bbcf7a616c50", "docking_clerical_clipboard1" },
        { "fab9bc63-1916-4434-a9c6-231f421ffbb5", "model" },
        { "7e164f67-f4e7-41fc-98a5-7a84ccaa1d09", "Polyaniline" },
        { "fbfacd7b-32a8-4065-8c25-b0a703f2683b", "model" },
        { "74912c22-a383-48c7-8e9e-34b515c6aebb", "Hydrochloric_acid" },
        { "986b31ea-3c9d-498c-9f38-2af8ffe86ed7", "Benzene" },
        { "96b1b863-2ff7-451b-aa38-8b3a06e72d63", "model" },
        { "c59c1abc-4e00-4480-8d8d-f337a81ba2d6", "model" },
        { "5c8cb04b-9f30-49e7-8687-0cbb338fc7fa", "model" },
        { "0fbf203a-a940-4b6e-ac63-0fe2737d84c2", "model" },
        { "8fa4a413-57fa-47a3-828d-de2255dbce4f", "farming_plant_03" },
        { "6d13066f-95c8-491b-965b-79ac3c67e6aa", "land_plant_middle_03_03" },
        { "1d6d89dd-3e49-48b7-90e4-b521fbc3d36f", "land_plant_middle_03_02" },
        { "523879d5-3241-4a94-8588-cb3b38945119", "land_plant_middle_03_01" },
        { "35056c71-5da7-4e73-be60-3c22c5c9e75c", "land_plant_middle_05_01" },
        { "08c1c77c-6ca3-49d1-9e4f-608e87d6f90c", "land_plant_middle_05_02" },
        { "8e4e640e-4c04-4168-a0cc-4ec86b709345", "land_plant_middle_05_03" },
        { "ff727b98-8d85-416a-9ee7-4beda86d2ba2", "" },
        { "28c73640-a713-424a-91c6-2f5d4672aaea", "land_plant_middle_02" },
        { "28818d8a-5e50-41f0-8e14-44cb89a0b611", "land_plant_small_02_01" },
        { "98be0944-e0b3-4fba-8f08-ca5d322c22f6", "land_plant_small_02_02" },
        { "7f9a765d-0b4e-4b3f-81b9-38b38beedf55", "land_plant_small_03_01" },
        { "e88e7a23-2a99-41c5-aed9-a2bfaca3619d", "land_plant_small_03_02" },
        { "a7aef01f-0dc0-4d03-913d-d47d8d2ba407", "land_plant_small_03_03" },
        { "c7faff7e-d9ff-41b4-9782-98d2e09d29c1", "land_plant_small_03_04" },
        { "b715508e-a7e4-47f0-a55b-bf6f65d24ac2", "land_plant_small_03_05" },
        { "e97c72ec-4999-48fa-b8b2-6d3f8791a7e8", "land_plant_small_01_03" },
        { "2cab613d-2fc0-4012-ae6e-99f42d4262fd", "land_plant_small_01_02" },
        { "28ec1137-da13-44f3-b76d-bac12ab766d1", "land_plant_small_01_01" },
        { "559fe0c7-1754-40f5-9453-b537900b3ac4", "land_plant_middle_06_01_LOD1" },
        { "154a88c1-6c7f-44e4-974e-c52d2f48fa28", "Tropical_plant_6b" },
        { "75ab087f-9934-4e2a-b025-02fc333a5c99", "Tropical_Plant_10a" },
        { "1cc51be0-8ea9-4730-936f-23b562a9256f", "Land_tree_01_LOD0" },
        { "dfabc84e-c4c5-45d9-8b01-ca0eaeeb8e65", "model" },
        { "c0d320d2-537e-4128-90ec-ab1466cfbbc3", "starship_souvenir" }
    };

    public static IEnumerator<IPrefabRequest> EnsureInitialized()
    {
        if (isInit)
        {
            var ids = new List<string>(prefabs.Keys);
            foreach (var id in ids)
            {
                var newPrefab = PrefabDatabase.GetPrefabAsync(id);

                yield return newPrefab;

                newPrefab.TryGetPrefab(out GameObject prefab);
                prefabs[id] = new PrefabInfo { name = prefabs[id].name, prefab = prefab };
            }

            yield break;
        }

        foreach (var (id, name) in prefabClassIDs)
        {
            var prefabRqst = PrefabDatabase.GetPrefabAsync(id);
            yield return prefabRqst;

            if (!prefabRqst.TryGetPrefab(out GameObject prefab))
            {
                Plugin.Logger.LogDebug($"TryGetPrefab failed for {id}");
                continue;
            }

            prefabs.Add(id, new PrefabInfo { name = name, prefab = prefab });
        }

        isInit = true;
    }

    public static string GetModelName(string classID)
    {
        if (!prefabs.ContainsKey(classID))
        {
            Plugin.Logger.LogError($"Prefab with ClassID {classID} not found in PrefabHelper.");
            return null;
        }

        return prefabs[classID].name;
    }

    public static GameObject GetPrefabGameObject(string classID)
    {
        if (!prefabs.ContainsKey(classID))
        {
            Plugin.Logger.LogError($"Prefab with ClassID {classID} not found in PrefabHelper.");
            return null;
        }

        return prefabs[classID].prefab;
    }

    public static GameObject InstantiatePrefabInactive(string classID)
    {
        if (!prefabs.ContainsKey(classID))
        {
            Plugin.Logger.LogError($"Prefab with ClassID {classID} not found in PrefabHelper.");
            return null;
        }

        var prefab = prefabs[classID].prefab;
        using (new PrefabActiveGuard(prefab))
        {
            var instance = GameObject.Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
            return instance;
        }
    }

    public static Transform AttachModelFromPrefabTo(string classID, Transform target)
    {
        if (!prefabs.ContainsKey(classID))
        {
            Plugin.Logger.LogError($"Prefab with ClassID {classID} not found in PrefabHelper.");
            return null;
        }

        var modelName = prefabs[classID].name;
        var instance = InstantiatePrefabInactive(classID);
        var model = instance.transform.Find(modelName);

        model.parent = target;

        UnityEngine.Object.Destroy(instance);

        return model;
    }

    public static Transform AttachAllModelsFromPrefabTo(string classID, Transform target)
    {
        if (!prefabs.ContainsKey(classID))
        {
            Plugin.Logger.LogError($"Prefab with ClassID {classID} not found in PrefabHelper.");
            return null;
        }

        var modelName = prefabs[classID].name;
        var instance = InstantiatePrefabInactive(classID);
        var model = instance.transform.Find(modelName);

        foreach (Transform child in instance.transform)
        {
            child.parent = target;
        }

        UnityEngine.Object.Destroy(instance);
        return model;
    }
}