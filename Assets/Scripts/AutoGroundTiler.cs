using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class AutoGroundTiler : MonoBehaviour
{
    [Header("Top Ground Tiles (Masukkan 3 tile 'tileset-sliced' di sini)")]
    public TileBase[] randomTopTiles;

    [Header("Deep Ground Tile (Tile tanah bawah, opsional)")]
    public TileBase deepGroundTile;

    [Tooltip("Jika dicentang, akan otomatis di-generate saat game dimulai (Play).")]
    public bool generateOnStart = false;

    void Start()
    {
        if (generateOnStart)
        {
            GenerateGroundFromBaseLine();
        }
    }

    [ContextMenu("Generate Ground From Base Line")]
    public void GenerateGroundFromBaseLine()
    {
        Tilemap tilemap = GetComponent<Tilemap>();
        BoundsInt bounds = tilemap.cellBounds;

        // Kita scan semua tile dari grid
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                TileBase currentTile = tilemap.GetTile(pos);

                // Jika ada tile yang sudah Anda gambar (base line)
                if (currentTile != null)
                {
                    // Cek apakah di atasnya kosong (berarti dia adalah permukaan/top tile)
                    Vector3Int posAbove = new Vector3Int(x, y + 1, 0);
                    TileBase tileAbove = tilemap.GetTile(posAbove);

                    if (tileAbove == null)
                    {
                        // Ini adalah permukaan! Ganti jadi salah satu dari 3 tile grass secara acak
                        if (randomTopTiles.Length > 0)
                        {
                            TileBase randomlyChosenTile = randomTopTiles[Random.Range(0, randomTopTiles.Length)];
                            tilemap.SetTile(pos, randomlyChosenTile);
                        }
                    }
                    else
                    {
                        // Di atasnya ada tile lain, berarti ini tanah bagian dalam (bawah).
                        if (deepGroundTile != null)
                        {
                            tilemap.SetTile(pos, deepGroundTile);
                        }
                    }
                }
            }
        }
        
        Debug.Log("Tile berhasil di-generate dan diacak!");
    }
}
