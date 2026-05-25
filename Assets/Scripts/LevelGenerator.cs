using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{
    [Header("Terrain Settings")]
    public int levelWidth = 100;
    public int minHeight = 2;
    public int maxHeight = 6;
    public float smoothness = 10f; // Higher = smoother hills, Lower = spiky
    public float seed; // Randomizer

    [Header("Tiles")]
    public Tilemap groundTilemap;
    public TileBase groundTopTile;   // Grass
    public TileBase groundMidTile;   // Dirt
    public TileBase platformTile;    // Floating Platform

    [Header("Decorations (Drag Sprites/Prefabs Here)")]
    public GameObject[] decorations; // Bush, Rock, Tree (As GameObjects)
    public float decorationChance = 0.2f; // 20% chance per tile

    void Start()
    {
        seed = Random.Range(0, 1000f);
        GenerateLevel();
    }

    void GenerateLevel()
    {
        // Clear previous generic run (if any)
        groundTilemap.ClearAllTiles();
        
        // Delete old props if re-generating
        foreach (Transform child in transform) { Destroy(child.gameObject); }

        for (int x = -5; x < levelWidth; x++)
        {
            // 1. Calculate Perlin Noise Height
            int height = Mathf.RoundToInt(Mathf.PerlinNoise((x + seed) / smoothness, seed) * (maxHeight - minHeight)) + minHeight;

            for (int y = -5; y < height; y++)
            {
                if (y == height - 1)
                {
                    // Top Layer (Grass)
                    groundTilemap.SetTile(new Vector3Int(x, y, 0), groundTopTile);

                    // 2. Spawn Decoration on top?
                    if (x > 2 && x < levelWidth - 5 && Random.value < decorationChance)
                    {
                         SpawnDecoration(x, y + 1);
                    }
                }
                else
                {
                    // Middle Layer (Dirt)
                    groundTilemap.SetTile(new Vector3Int(x, y, 0), groundMidTile);
                }
            }

            // 3. Floating Platforms (Higher up)
            if (x > 5 && x < levelWidth - 5)
            {
                // Simple chance for platform above head height
                if (Random.value < 0.1f)
                {
                     int platY = height + Random.Range(3, 5);
                     groundTilemap.SetTile(new Vector3Int(x, platY, 0), platformTile);
                     groundTilemap.SetTile(new Vector3Int(x + 1, platY, 0), platformTile);
                }
            }
        }
    }

    void SpawnDecoration(int x, int y)
    {
        if (decorations.Length == 0) return;

        GameObject prefab = decorations[Random.Range(0, decorations.Length)];
        // Spawn as child of this generator to keep hierarchy clean
        GameObject instance = Instantiate(prefab, new Vector3(x + 0.5f, y, 0), Quaternion.identity, transform);
        
        // Randomize size slightly for variety
        float scale = Random.Range(0.8f, 1.2f);
        instance.transform.localScale = new Vector3(scale, scale, 1);
    }
}
