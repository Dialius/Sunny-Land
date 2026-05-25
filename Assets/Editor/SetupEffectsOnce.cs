using UnityEngine;
using UnityEditor;
using System.IO;

[InitializeOnLoad]
public class SetupEffectsOnce
{
    static SetupEffectsOnce()
    {
        EditorApplication.delayCall += DoSetup;
    }

    static void DoSetup()
    {
        if (EditorPrefs.GetBool("EffectsSetupDone", false))
            return;

        Debug.Log("Starting automatic effect setup...");

        // 1. Create FX Prefabs
        string deathControllerPath = "Assets/SunnyLand Artwork/Sprites/Fx/enemy-death/enemy-death-1.controller";
        string feedbackControllerPath = "Assets/SunnyLand Artwork/Sprites/Fx/item-feedback/item-feedback-1.controller";

        GameObject deathFX = CreateFXPrefab("FX_EnemyDeath", deathControllerPath);
        GameObject feedbackFX = CreateFXPrefab("FX_ItemFeedback", feedbackControllerPath);

        // 2. Assign to Enemies
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
        bool changesMade = false;

        foreach(string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
            {
                EnemyAI enemyAI = prefab.GetComponent<EnemyAI>();
                if (enemyAI != null && deathFX != null)
                {
                    enemyAI.deathEffectPrefab = deathFX;
                    EditorUtility.SetDirty(prefab);
                    Debug.Log($"Assigned death effect to {prefab.name}");
                    changesMade = true;
                }

                Collectible collectible = prefab.GetComponent<Collectible>();
                if (collectible != null && feedbackFX != null)
                {
                    collectible.feedbackEffectPrefab = feedbackFX;
                    EditorUtility.SetDirty(prefab);
                    Debug.Log($"Assigned feedback effect to {prefab.name}");
                    changesMade = true;
                }
            }
        }

        if (changesMade)
        {
            AssetDatabase.SaveAssets();
        }

        EditorPrefs.SetBool("EffectsSetupDone", true);
        Debug.Log("Automatic effect setup completed!");
    }

    static GameObject CreateFXPrefab(string name, string controllerPath)
    {
        string prefabPath = $"Assets/Prefabs/{name}.prefab";

        // Check if exists
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (existing != null) return existing;

        RuntimeAnimatorController controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(controllerPath);
        if (controller == null)
        {
            Debug.LogError($"Could not find animator controller at {controllerPath}");
            return null;
        }

        GameObject go = new GameObject(name);
        go.AddComponent<SpriteRenderer>();
        Animator anim = go.AddComponent<Animator>();
        anim.runtimeAnimatorController = controller;
        
        DestroyAfterAnimation daa = go.AddComponent<DestroyAfterAnimation>();
        daa.delay = 0.5f; // Most particle/feedback fx are around 0.5s

        if (!Directory.Exists("Assets/Prefabs"))
        {
            Directory.CreateDirectory("Assets/Prefabs");
        }

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
        GameObject.DestroyImmediate(go);
        return prefab;
    }

    [MenuItem("Tools/Force Effects Setup")]
    public static void ResetSetup()
    {
        EditorPrefs.SetBool("EffectsSetupDone", false);
        DoSetup();
    }
}
