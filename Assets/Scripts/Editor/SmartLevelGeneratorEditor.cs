using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[CustomEditor(typeof(SmartLevelGenerator))]
public class SmartLevelGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SmartLevelGenerator generator = (SmartLevelGenerator)target;

        GUILayout.Space(10);
        GUILayout.Label("Automation Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("Auto-Assign Assets (USE EVERYTHING)"))
        {
            AutoAssignAssets(generator);
        }

        if (GUILayout.Button("Generate Level"))
        {
            generator.GenerateLevel();
        }

        if (GUILayout.Button("Clear Level"))
        {
            generator.ClearLevel();
        }
    }

    void AutoAssignAssets(SmartLevelGenerator generator)
    {
        Undo.RecordObject(generator, "Auto-Assign Assets");
        
        // 0. Ensure Grid and Tilemap exist
        if (generator.groundTilemap == null)
        {
            generator.groundTilemap = generator.GetComponentInChildren<Tilemap>();
            if (generator.groundTilemap == null)
            {
                 GameObject gridGo = new GameObject("Grid");
                 gridGo.transform.SetParent(generator.transform);
                 gridGo.transform.localPosition = Vector3.zero;
                 Grid grid = gridGo.AddComponent<Grid>();
                 grid.cellSize = new Vector3(1, 1, 0);

                 GameObject tilemapGo = new GameObject("Tilemap");
                 tilemapGo.transform.SetParent(gridGo.transform);
                 tilemapGo.transform.localPosition = Vector3.zero;
                 Tilemap tm = tilemapGo.AddComponent<Tilemap>();
                 tilemapGo.AddComponent<TilemapRenderer>();
                 generator.groundTilemap = tm;
            }
        }

        // 1. Initialize Themes List
        generator.themes.Clear();
        
        // create a default "SunnyLand" theme
        SmartLevelGenerator.LevelTheme mainTheme = new SmartLevelGenerator.LevelTheme();
        mainTheme.themeName = "SunnyLand Woods";
        
        // 2. Scan Assets
        string[] guids = AssetDatabase.FindAssets("", new[] { "Assets/SunnyLand Artwork/Environment" });
        
        List<GameObject> allDecorations = new List<GameObject>();
        List<GameObject> allObstacles = new List<GameObject>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string name = System.IO.Path.GetFileNameWithoutExtension(path).ToLower();
            if (System.IO.Directory.Exists(path)) continue;

            Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);
            if (obj == null) continue;

            // TILES
            if (obj is TileBase tile)
            {
                if (name.Contains("tileset-sliced_0")) mainTheme.groundLeft = tile;
                else if (name.Contains("tileset-sliced_2")) mainTheme.groundRight = tile;
                else if (name.Contains("tileset-sliced_1")) mainTheme.groundMid = tile; // Top
                // else if (name.Contains("tileset-sliced_13")) // Dirt Mid - Logic simplified to use Top for now or generic dirt if we expand
                
                // Platform Tiles (Standard 3-piece set if available, otherwise reuse ground or specific platform tiles)
                // SunnyLand often has distinct floating platform tiles.
                // Analysis showed 120-127 are platform like.
                // Let's try to find them by name if they are named "tileset-sliced_120" etc. or usage.
                
                // fallback logic: if we find explicit platform tiles
                if (name.Contains("platform") && !name.Contains("floating")) 
                {
                   // manual approximation for now or simplistic assignment
                   mainTheme.platformMid = tile; 
                   // Ideally we find Left/Right versions too
                }
            }
            // GAME OBJECTS
            else if (obj is GameObject go) 
            {
                if (name.Contains("back") || name.Contains("middle"))
                {
                    mainTheme.background = go; // Takes the last one found, or list if we wanted randomized BGs per theme
                }
                else if (name.Contains("tree") || name.Contains("bush") || name.Contains("house") || name.Contains("sign") || name.Contains("shroom"))
                {
                    mainTheme.decorations.Add(go);
                    allDecorations.Add(go);
                }
                else if (name.Contains("rock") || name.Contains("crate") || name.Contains("spike"))
                {
                    mainTheme.obstacles.Add(go);
                    allObstacles.Add(go);
                }
            }
        }
        
        // Fallback for missing Left/Right platform tiles -> use Mid
        if (mainTheme.platformLeft == null) mainTheme.platformLeft = mainTheme.platformMid;
        if (mainTheme.platformRight == null) mainTheme.platformRight = mainTheme.platformMid;

        generator.themes.Add(mainTheme);
        
        // If we wanted to split themes (e.g. "Stone Theme"), we would filter `allDecorations` 
        // by "rock" vs "tree" and create new LevelTheme objects.
        // For now, SunnyLand is mostly one cohesive style, so one big theme is safer than splitting arbitrarily.
        
        Debug.Log($"Auto-Assigned Theme: {mainTheme.themeName}");
        EditorUtility.SetDirty(generator);
    }
}
