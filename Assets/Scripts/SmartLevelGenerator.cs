using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class SmartLevelGenerator : MonoBehaviour
{
    [System.Serializable]
    public class LevelTheme
    {
        public string themeName;
        public TileBase groundLeft;
        public TileBase groundMid;
        public TileBase groundRight;
        public TileBase platformLeft;
        public TileBase platformMid;
        public TileBase platformRight;
        public GameObject background;
        public List<GameObject> decorations = new List<GameObject>();
        public List<GameObject> obstacles = new List<GameObject>();
    }

    [Header("Terrain Settings")]
    public int levelWidth = 100;
    public int minHeight = 2;
    public int maxHeight = 6;
    public float smoothness = 10f;
    public float seed;

    [Header("Assets (Auto-Assigned)")]
    public Tilemap groundTilemap;
    
    // THEMES: Group assets so they match visual style
    public List<LevelTheme> themes = new List<LevelTheme>();
    
    public float decorationChance = 0.2f;
    public float obstacleChance = 0.05f;

    public void GenerateLevel()
    {
        if (groundTilemap == null)
        {
            Debug.LogError("No Tilemap assigned! Please assign a Tilemap in the Inspector or click 'Auto-Assign Assets'.");
            return;
        }

        if (seed == 0) seed = Random.Range(0, 1000f);
        
        ClearLevel();

        // 1. Pick a Random Theme for the whole level (Visual Consistency)
        if (themes.Count == 0)
        {
            Debug.LogError("No Themes found! Please click 'Auto-Assign Assets'.");
            return;
        }
        LevelTheme currentTheme = themes[Random.Range(0, themes.Count)];

        // Setup Background
        if (currentTheme.background != null)
        {
            GameObject bg = Instantiate(currentTheme.background, transform);
            bg.transform.localPosition = new Vector3(levelWidth / 2f, maxHeight / 2f, 10);
            bg.transform.localScale = new Vector3(levelWidth / 10f, 1, 1); 
        }

        // Generate Terrain
        for (int x = -5; x < levelWidth; x++)
        {
            // Start Area (0-10) and End Area (Width-10 to Width) are flat
            bool isStartArea = x >= 0 && x < 10;
            bool isEndArea = x > levelWidth - 10 && x < levelWidth;
            
            int height = isStartArea || isEndArea ? minHeight + 2 :
                         Mathf.RoundToInt(Mathf.PerlinNoise((x + seed) / smoothness, seed) * (maxHeight - minHeight)) + minHeight;

            int prevHeight = x > -5 ? GetHeightAt(x - 1) : height;
            int nextHeight = x < levelWidth - 1 ? GetHeightAt(x + 1) : height;

            for (int y = -5; y < height; y++)
            {
                if (y == height - 1)
                {
                    // Top Layer Logic: Determine if this is a Left, Right, or Mid tile
                    // based on neighbors to ensure valid connection.
                    
                    TileBase tileToPlace = currentTheme.groundMid; // Default Mid

                    if (height > prevHeight) // Step Up -> Needs Left Edge? No, typically standard.
                    {
                        // Simplified 2D side-scroller logic:
                        // Real "Left" edge is when there is NO ground to the left at this height.
                        // Real "Right" edge is when there is NO ground to the right at this height.
                        
                        bool emptyLeft = x == -5 || GetHeightAt(x - 1) < height;
                        bool emptyRight = x == levelWidth - 1 || GetHeightAt(x + 1) < height;

                        if (emptyLeft) tileToPlace = currentTheme.groundLeft;
                        else if (emptyRight) tileToPlace = currentTheme.groundRight;
                    }

                    if (tileToPlace != null)
                        groundTilemap.SetTile(new Vector3Int(x, y, 0), tileToPlace);

                    // Decorations (Only on Flat Mid Ground)
                    if (!isStartArea && !isEndArea && tileToPlace == currentTheme.groundMid)
                    {
                        if (Random.value < decorationChance) SpawnProp(currentTheme.decorations, x, y + 1);
                        else if (Random.value < obstacleChance) SpawnProp(currentTheme.obstacles, x, y + 1);
                    }
                }
                else
                {
                    // Fill underneath
                    if (currentTheme.groundMid != null)
                        groundTilemap.SetTile(new Vector3Int(x, y, 0), currentTheme.groundMid); 
                        // Note: ideally "groundCenter" (dirt) distinct from "groundTopMid" (grass)
                        // But for now reusing mid logic or we can add "DeepGround" later.
                }
            }

            // Floating Platforms (Strict 3-part construction: Left-Mid-Right)
            if (!isStartArea && !isEndArea && x > 5 && x < levelWidth - 5)
            {
                if (Random.value < 0.1f)
                {
                    int platY = height + Random.Range(3, 5);
                    // Determine Length (e.g., 3 units for simplicity: Left, Mid, Right)
                    if (x + 2 < levelWidth - 5)
                    {
                        groundTilemap.SetTile(new Vector3Int(x, platY, 0), currentTheme.platformLeft);
                        groundTilemap.SetTile(new Vector3Int(x + 1, platY, 0), currentTheme.platformMid);
                        groundTilemap.SetTile(new Vector3Int(x + 2, platY, 0), currentTheme.platformRight);
                    }
                }
            }
        }
    }

    int GetHeightAt(int x)
    {
         bool isStart = x >= 0 && x < 10;
         bool isEnd = x > levelWidth - 10 && x < levelWidth;
         if (isStart || isEnd) return minHeight + 2;
         return Mathf.RoundToInt(Mathf.PerlinNoise((x + seed) / smoothness, seed) * (maxHeight - minHeight)) + minHeight;
    }

    void SpawnProp(List<GameObject> props, int x, int y)
    {
         if (props == null || props.Count == 0) return;
         GameObject prefab = props[Random.Range(0, props.Count)];
         if (prefab != null)
         {
             Instantiate(prefab, new Vector3(x + 0.5f, y, 0), Quaternion.identity, transform);
         }
    }

    public void ClearLevel()
    {
        if (groundTilemap != null) groundTilemap.ClearAllTiles();
        int childCount = transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (groundTilemap != null && (child == groundTilemap.transform || (groundTilemap.transform.parent != null && child == groundTilemap.transform.parent))) continue;
            DestroyImmediate(child.gameObject);
        }
    }
}
