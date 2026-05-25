using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Membuat background berulang (seamless scroll) yang bergerak dari kanan ke kiri.
/// Pasang script ini pada objek RawImage di dalam Canvas.
/// </summary>
public class ScrollingBackground : MonoBehaviour
{
    [Header("Scroll Settings")]
    [Tooltip("Kecepatan gerak background. Nilai positif = gerak ke kiri.")]
    public float scrollSpeed = 0.05f;

    [Header("Tiling (Pengulangan Gambar)")]
    [Tooltip("Berapa kali gambar diulang secara horizontal. Naikkan jika gambar terlalu kecil.")]
    public float tileX = 5f;

    [Tooltip("Berapa kali gambar diulang secara vertikal.")]
    public float tileY = 1f;

    private RawImage rawImage;
    private float offset;

    void Awake()
    {
        rawImage = GetComponent<RawImage>();
        if (rawImage == null)
        {
            Debug.LogError("ScrollingBackground: RawImage component tidak ditemukan!");
            return;
        }

        // Set ukuran tiling awal
        rawImage.uvRect = new Rect(0f, 0f, tileX, tileY);
    }

    void Update()
    {
        if (rawImage == null) return;

        // Geser nilai UV offset terus menerus
        offset += scrollSpeed * Time.deltaTime;
        if (offset >= 1f) offset -= 1f;

        // Terapkan offset + tiling ke UV rect
        rawImage.uvRect = new Rect(offset, 0f, tileX, tileY);
    }
}
