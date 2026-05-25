using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Membuat deretan tile tanah otomatis yang memenuhi lebar layar.
/// Pasang script ini pada GameObject kosong di dalam Canvas.
/// PENTING: Set RectTransform GroundTiler agar stretch full width di bagian bawah Canvas.
/// </summary>
public class MenuGroundTiler : MonoBehaviour
{
    [Header("Tile Settings")]
    [Tooltip("Sprite tile tanah yang ingin digunakan.")]
    public Sprite groundSprite;

    [Tooltip("Ukuran tiap tile dalam pixel.")]
    public float tileSize = 64f;

    [Tooltip("Berapa baris tile tanah (1 = satu baris saja).")]
    public int rows = 2;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        if (groundSprite == null)
        {
            Debug.LogWarning("MenuGroundTiler: groundSprite belum diisi di Inspector!");
            return;
        }

        BuildGround();
    }

    void BuildGround()
    {
        // Gunakan lebar RectTransform GroundTiler itu sendiri
        float containerWidth = rectTransform.rect.width;
        float containerHeight = rectTransform.rect.height;

        // Jika lebar masih 0 (belum dihitung), pakai Screen.width sebagai fallback
        if (containerWidth <= 0)
            containerWidth = Screen.width;

        // Hitung jumlah tile yang dibutuhkan (tambah 1 ekstra agar tidak ada celah)
        int tileCount = Mathf.CeilToInt(containerWidth / tileSize) + 1;

        Debug.Log($"MenuGroundTiler: Membuat {tileCount} tile, lebar container: {containerWidth}");

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < tileCount; col++)
            {
                // Buat GameObject tile
                GameObject tile = new GameObject($"Tile_{row}_{col}");
                tile.transform.SetParent(transform, false);

                // Tambahkan Image
                Image img = tile.AddComponent<Image>();
                img.sprite = groundSprite;

                // Atur RectTransform tile
                RectTransform rt = tile.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(tileSize, tileSize);

                // Anchor pojok kiri bawah dari parent
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.zero;
                rt.pivot = new Vector2(0f, 0f);

                // Posisi: dari kiri ke kanan, dari bawah ke atas
                float posX = col * tileSize;
                float posY = row * tileSize;

                rt.anchoredPosition = new Vector2(posX, posY);
            }
        }
    }
}
