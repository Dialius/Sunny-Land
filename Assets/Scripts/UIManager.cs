using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public Text gemText;
    public Text cherryText;
    public Text livesText;

    // Array of Heart Images (Jika ingin pakai deretan hati, opsional)
    public Image[] healthHearts; 

    [Header("Panels")]
    public GameObject gameOverPanel;
    public GameObject levelCompletePanel;

    private void Start()
    {
        // Fitur Auto-Find: Jika lupa di-assign di Inspector, script akan mencoba mencarinya otomatis
        if (gemText == null) gemText = FindText("GemText");
        if (cherryText == null) cherryText = FindText("CherryText");
        if (livesText == null) livesText = FindText("LivesText");
        
        // Cari panel (meskipun sedang mati/inactive)
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas != null)
        {
            if (gameOverPanel == null)
            {
                Transform t = canvas.transform.Find("GameOverPanel");
                if (t != null) gameOverPanel = t.gameObject;
            }
            if (levelCompletePanel == null)
            {
                Transform t = canvas.transform.Find("LevelCompletePanel");
                if (t != null) levelCompletePanel = t.gameObject;
            }
        }
    }

    private Text FindText(string nameContains)
    {
        Text[] allTexts = Object.FindObjectsByType<Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Text t in allTexts)
        {
            if (t.gameObject.name.Contains(nameContains)) return t;
        }
        return null;
    }

    public void UpdateGemText(int gems)
    {
        if (gemText != null)
            gemText.text = gems.ToString(); 
    }

    public void UpdateCherryText(int cherries)
    {
        if (cherryText != null)
            cherryText.text = cherries.ToString(); 
    }

    public void UpdateLivesText(int lives)
    {
        if (livesText != null)
            // Menampilkan tulisan "x 3" di sebelah icon hati
            livesText.text = "x " + lives;

        // Opsional: Jika menggunakan deretan sprite hati (seperti game Zelda)
        if (healthHearts != null && healthHearts.Length > 0)
        {
            for (int i = 0; i < healthHearts.Length; i++)
            {
                if (i < lives)
                    healthHearts[i].enabled = true; // Munculkan hati
                else
                    healthHearts[i].enabled = false; // Sembunyikan hati yang hilang
            }
        }
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void ShowLevelComplete()
    {
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(true);
    }

    // Fungsi untuk Tombol di UI
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); // Asumsi index 0 adalah Main Menu
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        FinishPoint fp = Object.FindFirstObjectByType<FinishPoint>();
        if (fp != null && !string.IsNullOrEmpty(fp.nextLevelName))
        {
            SceneManager.LoadScene(fp.nextLevelName);
        }
        else
        {
            Debug.LogWarning("Next level belum diatur di FinishPoint atau ini level terakhir.");
            GoToMainMenu();
        }
    }
}
