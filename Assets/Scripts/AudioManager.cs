using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Background Music")]
    public AudioClip bgmClip;       // Tarik file musik ke sini di Inspector
    private AudioSource bgmSource;

    // Nama parameter harus SAMA PERSIS dengan yang kamu tulis di Exposed Parameters
    private const string MUSIC_PARAM = "MusicVolume";
    private const string SFX_PARAM   = "SFXVolume";

    void Awake()
    {
        // Singleton agar AudioManager tidak terduplikasi saat pindah scene
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Buat AudioSource untuk BGM secara otomatis
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
    }

    void Start()
    {
        // Putar BGM jika sudah ada clipnya
        if (bgmClip != null)
        {
            bgmSource.clip = bgmClip;
            bgmSource.Play();
        }

        // Muat pengaturan volume terakhir yang tersimpan (default 1.0 = 100%)
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float savedSFX   = PlayerPrefs.GetFloat("SFXVolume", 0.5f);

        SetMusicVolume(savedMusic);
        SetSFXVolume(savedSFX);
    }

    /// <summary>
    /// Mengganti BGM (misalnya saat masuk scene baru)
    /// </summary>
    public void PlayBGM(AudioClip newClip)
    {
        if (bgmSource.clip == newClip) return; // Jangan restart jika musik sama
        bgmSource.clip = newClip;
        bgmSource.Play();
    }

    /// <summary>
    /// Dipanggil oleh Slider Music. Nilai slider: 0.0001 sampai 1.0
    /// </summary>
    public void SetMusicVolume(float sliderValue)
    {
        // Konversi nilai slider linear (0-1) ke decibel (-80 dB hingga 0 dB)
        // Logaritma dipakai agar perubahan volume terasa natural di telinga
        if (audioMixer != null)
        {
            float dB = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20f;
            audioMixer.SetFloat(MUSIC_PARAM, dB);
        }

        // Fallback langsung ke AudioSource (berfungsi meski belum pakai mixer)
        if (bgmSource != null)
            bgmSource.volume = sliderValue;

        PlayerPrefs.SetFloat("MusicVolume", sliderValue);
    }

    /// <summary>
    /// Dipanggil oleh Slider SFX. Nilai slider: 0.0001 sampai 1.0
    /// </summary>
    public void SetSFXVolume(float sliderValue)
    {
        if (audioMixer != null)
        {
            float dB = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20f;
            audioMixer.SetFloat(SFX_PARAM, dB);
        }

        PlayerPrefs.SetFloat("SFXVolume", sliderValue);
    }

    /// <summary>
    /// Memutar SFX sekali. Bisa dipanggil dari script manapun.
    /// Contoh: AudioManager.Instance.PlaySFX(clickSoundClip);
    /// </summary>
    public void PlaySFX(AudioClip clip, AudioSource source)
    {
        if (clip != null && source != null)
            source.PlayOneShot(clip);
    }
}
