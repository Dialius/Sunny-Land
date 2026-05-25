using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Karakter berjalan otomatis di dalam Canvas (UI Image).
/// Pasang script ini pada objek Image di dalam Canvas.
/// </summary>
public class MenuWalkingCharacterUI : MonoBehaviour
{
    [Header("Sprite Animasi Lari")]
    [Tooltip("Masukkan sprite player-run-1 sampai player-run-6 di sini, urut.")]
    public Sprite[] runFrames;

    [Tooltip("Berapa frame per detik animasi berjalan.")]
    public float frameRate = 10f;

    [Header("Gerakan")]
    [Tooltip("Kecepatan gerak karakter dalam pixel per detik.")]
    public float walkSpeed = 200f;

    [Tooltip("Posisi X awal (dari luar batas kiri layar, misal -200).")]
    public float startX = -200f;

    [Tooltip("Posisi X akhir sebelum karakter kembali ke kiri (misal 2200 untuk layar 1920px).")]
    public float endX = 2200f;

    private Image characterImage;
    private RectTransform rectTransform;
    private float frameTimer;
    private int currentFrame;

    void Awake()
    {
        characterImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();

        if (characterImage == null)
        {
            Debug.LogError("MenuWalkingCharacterUI: Tidak ada komponen Image! Pasang script ini pada UI Image.");
            return;
        }

        if (runFrames == null || runFrames.Length == 0)
        {
            Debug.LogWarning("MenuWalkingCharacterUI: runFrames kosong! Isi dengan sprite animasi lari di Inspector.");
        }

        // Taruh karakter di posisi awal
        Vector2 pos = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition = new Vector2(startX, pos.y);
    }

    void Update()
    {
        if (rectTransform == null) return;

        // --- ANIMASI FRAME ---
        if (runFrames != null && runFrames.Length > 0)
        {
            frameTimer += Time.deltaTime;
            if (frameTimer >= 1f / frameRate)
            {
                frameTimer = 0f;
                currentFrame = (currentFrame + 1) % runFrames.Length;
                characterImage.sprite = runFrames[currentFrame];
            }
        }

        // --- GERAKAN ---
        Vector2 currentPos = rectTransform.anchoredPosition;
        currentPos.x += walkSpeed * Time.deltaTime;
        rectTransform.anchoredPosition = currentPos;

        // Jika sudah keluar batas kanan, kembali ke kiri
        if (currentPos.x >= endX)
        {
            rectTransform.anchoredPosition = new Vector2(startX, currentPos.y);
        }
    }
}
