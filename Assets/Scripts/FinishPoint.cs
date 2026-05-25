using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishPoint : MonoBehaviour
{
    [Header("Pengaturan Finish")]
    [Tooltip("Nama scene/level yang akan diload setelah menyentuh finish")]
    public string nextLevelName;

    [Tooltip("Nomor level ini (1 untuk Level1, 2 untuk Level2, dst). Dipakai untuk unlock level berikutnya.")]
    public int currentLevelNumber = 1;

    [Tooltip("Waktu tunggu sebelum pindah level setelah menyentuh finish")]
    public float loadDelay = 1f;

    private bool isFinished = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isFinished)
        {
            isFinished = true;
            Debug.Log("Player menyentuh garis finish!");
            
            // Sembunyikan sprite player
            SpriteRenderer[] renderers = collision.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sr in renderers)
                sr.enabled = false;

            // Matikan collider player
            Collider2D[] colliders = collision.GetComponents<Collider2D>();
            foreach (Collider2D col in colliders)
                col.enabled = false;
            
            // Hentikan fisika player
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.simulated = false;
            }

            // ✅ Unlock level berikutnya
            LevelSelectionManager.UnlockNextLevel(currentLevelNumber);

            // Tampilkan UI Level Complete
            UIManager uiManager = Object.FindFirstObjectByType<UIManager>();

            // Cek apakah Level Complete Panel sudah dibuat dan di-assign
            if (uiManager != null && uiManager.levelCompletePanel != null)
            {
                uiManager.ShowLevelComplete();
                // Hentikan waktu agar player bisa klik tombol
                Time.timeScale = 0f;
            }
            else
            {
                // Fallback: Level Complete Panel belum dibuat, langsung pindah scene
                Debug.Log("[FinishPoint] levelCompletePanel belum diatur, langsung pindah scene.");
                Invoke("LevelComplete", loadDelay);
            }

        }
    }

    private void LevelComplete()
    {
        Time.timeScale = 1f; // Kembalikan waktu normal
        if (!string.IsNullOrEmpty(nextLevelName))
        {
            SceneManager.LoadScene(nextLevelName);
        }
        else
        {
            // Jika tidak ada level berikutnya, kembali ke Main Menu
            Debug.Log("Level terakhir selesai! Kembali ke Main Menu.");
            SceneManager.LoadScene(0); // Build index 0 = MainMenu
        }
    }
}
