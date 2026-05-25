using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject levelSelectionPanel;

    [Header("Settings UI")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Toggle musicToggle;
    public Toggle sfxToggle;

    private void Start()
    {
        // Menampilkan Main Menu saat pertama kali jalan, menyembunyikan panel lain
        ShowMainMenu();

        // Inisialisasi nilai volume (bisa menggunakan PlayerPrefs jika ingin menyimpan pengaturan)
        if (musicSlider != null) musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        if (sfxSlider != null) sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        if (musicToggle != null) musicToggle.isOn = PlayerPrefs.GetInt("MusicToggle", 1) == 1;
        if (sfxToggle != null) sfxToggle.isOn = PlayerPrefs.GetInt("SFXToggle", 1) == 1;
    }

    // --- MAIN MENU BUTTONS ---

    public void OnPlayButtonClicked()
    {
        // Pindah dari Main Menu ke Level Selection Menu
        mainMenuPanel.SetActive(false);
        levelSelectionPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void OnSettingsButtonClicked()
    {
        // Pindah dari Main Menu ke Settings Menu
        mainMenuPanel.SetActive(false);
        levelSelectionPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void OnQuitButtonClicked()
    {
        Debug.Log("Quit Game!");
        Application.Quit();
    }

    public void OnBackButtonClicked()
    {
        // Tombol kembali untuk kembali ke Main Menu dari panel mana pun
        ShowMainMenu();
    }

    private void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        levelSelectionPanel.SetActive(false);
    }

    // --- LEVEL SELECTION ---

    public void LoadLevel(int levelNumber)
    {
        // Build Index:
        // 0 = MainMenu
        // 1 = Level1, 2 = Level2, ... 6 = Level6
        // levelNumber 1-6, jadi build index = levelNumber + 0
        // (sesuaikan jika urutan di Build Settings berbeda)

        int buildIndex = levelNumber; // Level1 = index 1, Level2 = index 2, dst.

        if (buildIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log("Loading Level: " + levelNumber + " (Build Index: " + buildIndex + ")");
            SceneManager.LoadScene(buildIndex);
        }
        else
        {
            Debug.LogWarning("Level " + levelNumber + " belum ditambahkan ke Build Settings!");
        }
    }

    // --- SETTINGS ---

    public void SetMusicVolume(float volume)
    {
        // Jika Anda punya AudioMixer, Anda bisa menghubungkannya di sini
        // AudioManager.instance.SetMusicVolume(volume);
        Debug.Log("Music Volume Set To: " + volume);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        // AudioManager.instance.SetSFXVolume(volume);
        Debug.Log("SFX Volume Set To: " + volume);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void ToggleMusic(bool isMuted)
    {
        Debug.Log("Music Toggle: " + isMuted);
        PlayerPrefs.SetInt("MusicToggle", isMuted ? 1 : 0);
        // Implementasikan logika mute di sini
    }

    public void ToggleSFX(bool isMuted)
    {
        Debug.Log("SFX Toggle: " + isMuted);
        PlayerPrefs.SetInt("SFXToggle", isMuted ? 1 : 0);
        // Implementasikan logika mute di sini
    }
}
