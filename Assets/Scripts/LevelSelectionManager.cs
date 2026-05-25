using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Membuat tombol level secara otomatis di dalam Grid.
/// Level terkunci jika level sebelumnya belum diselesaikan.
/// </summary>
public class LevelSelectionManager : MonoBehaviour
{
    [Header("Tombol Level")]
    [Tooltip("Prefab tombol level.")]
    public GameObject levelButtonPrefab;

    [Tooltip("Jumlah total level.")]
    public int totalLevels = 6;

    [Header("Tampilan Terkunci")]
    [Tooltip("Warna overlay gelap untuk tombol yang terkunci (gunakan alpha sekitar 150).")]
    public Color lockedOverlayColor = new Color(0f, 0f, 0f, 0.6f);
    
    [Tooltip("Prefab atau GameObject icon gembok (Padlock) yang akan muncul di tombol terkunci. (Opsional)")]
    public GameObject lockedIconPrefab;

    void Start()
    {
        if (levelButtonPrefab == null)
        {
            Debug.LogError("LevelSelectionManager: levelButtonPrefab belum diisi!");
            return;
        }

        GenerateLevelButtons();
    }

    void GenerateLevelButtons()
    {
        // Hapus tombol lama jika ada
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        int highestUnlocked = GetHighestUnlockedLevel();

        for (int i = 1; i <= totalLevels; i++)
        {
            GameObject btn = Instantiate(levelButtonPrefab, transform);
            btn.name = "LevelButton_" + i;

            bool isUnlocked = i <= highestUnlocked;

            // ✅ Update teks angka level di tombol
            // Coba TextMeshPro dulu
            TextMeshProUGUI tmpLabel = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (tmpLabel != null)
            {
                tmpLabel.text = i.ToString();
            }
            else
            {
                // Fallback ke Legacy Text
                Text legacyLabel = btn.GetComponentInChildren<Text>();
                if (legacyLabel != null)
                    legacyLabel.text = i.ToString();
            }

            // Atur interactable
            Button button = btn.GetComponent<Button>();
            if (button != null)
            {
                button.interactable = isUnlocked;

                if (isUnlocked)
                {
                    int levelIndex = i;
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => LoadLevel(levelIndex));
                }
            }

            // Jika terkunci, tambahkan overlay gelap di atas tombol
            if (!isUnlocked)
            {
                AddLockOverlay(btn);
            }
        }

    }

    /// <summary>
    /// Tambahkan lapisan gelap di atas tombol yang terkunci.
    /// </summary>
    void AddLockOverlay(GameObject btn)
    {
        // Buat Overlay Gelap
        GameObject overlay = new GameObject("LockOverlay");
        overlay.transform.SetParent(btn.transform, false);

        Image overlayImg = overlay.AddComponent<Image>();
        overlayImg.color = lockedOverlayColor;

        // Stretch overlay agar menutupi seluruh tombol
        RectTransform rt = overlay.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        // Tambahkan Icon Padlock jika ada
        if (lockedIconPrefab != null)
        {
            GameObject padlock = Instantiate(lockedIconPrefab, btn.transform);
            padlock.name = "PadlockIcon";
            // Pastikan posisinya di tengah
            RectTransform padlockRT = padlock.GetComponent<RectTransform>();
            if (padlockRT != null)
            {
                padlockRT.anchoredPosition = Vector2.zero;
            }
            
            // Sembunyikan text angka jika terkunci (opsional)
            TextMeshProUGUI tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null) tmp.enabled = false;
            else
            {
                Text txt = btn.GetComponentInChildren<Text>();
                if (txt != null) txt.enabled = false;
            }
        }
    }

    /// <summary>
    /// Load scene level berdasarkan nomornya.
    /// Pastikan scene sudah ditambahkan ke Build Settings dengan urutan:
    /// Index 0 = MainMenu, Index 1 = Level1, Index 2 = Level2, dst.
    /// </summary>
    void LoadLevel(int levelNumber)
    {
        int buildIndex = levelNumber; // Level1 = index 1, Level2 = index 2, dst.
        int totalScenes = SceneManager.sceneCountInBuildSettings;

        Debug.Log("Mencoba load Level " + levelNumber + " (Build Index: " + buildIndex + "), Total scenes: " + totalScenes);

        if (buildIndex < totalScenes)
        {
            SceneManager.LoadScene(buildIndex);
        }
        else
        {
            Debug.LogWarning("Level " + levelNumber + " belum ada di Build Settings! " +
                             "Buka File > Build Settings dan tambahkan scene Level" + levelNumber);
        }
    }

    public static int GetHighestUnlockedLevel()
    {
        return PlayerPrefs.GetInt("HighestUnlockedLevel", 1);
    }

    public static void UnlockNextLevel(int completedLevel)
    {
        int current = PlayerPrefs.GetInt("HighestUnlockedLevel", 1);
        if (completedLevel >= current)
        {
            PlayerPrefs.SetInt("HighestUnlockedLevel", completedLevel + 1);
            PlayerPrefs.Save();
            Debug.Log("Level " + (completedLevel + 1) + " telah terbuka!");
        }
    }

    public static void ResetProgress()
    {
        PlayerPrefs.SetInt("HighestUnlockedLevel", 1);
        PlayerPrefs.Save();
        Debug.Log("Progress direset ke Level 1.");
    }
}
