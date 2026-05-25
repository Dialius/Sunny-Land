using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int gems = 0;
    public int cherries = 0;

    public UIManager uiManager;

    [Header("Audio Settings")]
    public AudioClip backgroundMusicClip;
    private AudioSource bgmSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keep across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Setup BGM
        if (backgroundMusicClip != null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.clip = backgroundMusicClip;
            bgmSource.loop = true;
            bgmSource.playOnAwake = true;
            bgmSource.volume = 0.5f; // Set default volume
            bgmSource.Play();
        }

        // Find UIManager if not assigned
        if (uiManager == null)
            uiManager = FindFirstObjectByType<UIManager>();
            
        UpdateUI();
    }

    public void AddGems(int amount)
    {
        gems += amount;
        UpdateUI();
    }

    public void AddCherries(int amount)
    {
        cherries += amount;
        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        // Integration with HealthHeartSystem
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.TakeDamage(damage);

            if (uiManager == null) uiManager = FindFirstObjectByType<UIManager>();

            // Update tampilan nyawa di UI (Health adalah float, di-cast ke int)
            if (uiManager != null)
                uiManager.UpdateLivesText((int)PlayerStats.Instance.Health);
            
            if (PlayerStats.Instance.Health <= 0)
            {
                GameOver();
            }
        }
        else 
        {
            Debug.LogWarning("[GameManager] PlayerStats tidak ditemukan! Pastikan script PlayerStats ada di Player.");
        }
    }

    void UpdateUI()
    {
        if (uiManager == null) uiManager = FindFirstObjectByType<UIManager>();

        if (uiManager != null)
        {
            uiManager.UpdateGemText(gems);
            uiManager.UpdateCherryText(cherries);
            
            // Note: UIManager no longer handles hearts text here, HealthBarController does it automatically 
            // via PlayerStats events.
        }
    }

    void GameOver()
    {
        Debug.Log("Game Over!");

        if (uiManager == null) uiManager = FindFirstObjectByType<UIManager>();

        // Tampilkan panel Game Over
        if (uiManager != null)
        {
            uiManager.ShowGameOver();
        }
        else
        {
            Debug.LogWarning("[GameManager] UIManager tidak ditemukan! Pastikan UIManager ada di scene.");
        }

        // Hentikan waktu agar game berhenti saat Game Over
        Time.timeScale = 0f;
    }
}
