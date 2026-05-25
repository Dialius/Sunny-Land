using UnityEngine;

/// <summary>
/// Membuat karakter 2D berjalan otomatis dari kiri ke kanan di layar Menu.
/// Saat sudah melewati batas kanan, karakter akan muncul lagi dari kiri.
/// Pasang script ini pada GameObject yang memiliki SpriteRenderer.
/// </summary>
public class MenuWalkingCharacter : MonoBehaviour
{
    [Header("Gerakan")]
    [Tooltip("Kecepatan karakter berjalan (unit/detik).")]
    public float walkSpeed = 2f;

    [Tooltip("Posisi X di mana karakter muncul kembali dari kiri (biasanya di luar batas kiri layar).")]
    public float spawnX = -12f;

    [Tooltip("Posisi X di mana karakter dianggap sudah keluar layar (batas kanan).")]
    public float exitX = 12f;

    [Header("Posisi Y Karakter")]
    [Tooltip("Posisi Y karakter di layar (sesuaikan dengan posisi 'tanah' di menu kamu).")]
    public float groundY = -3.5f;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Pastikan karakter menghadap ke kanan
        if (spriteRenderer != null)
            spriteRenderer.flipX = false;

        // Tempatkan karakter di posisi awal (dari luar batas kiri)
        transform.position = new Vector3(spawnX, groundY, transform.position.z);
    }

    void Update()
    {
        // Gerakkan karakter ke kanan
        transform.Translate(Vector3.right * walkSpeed * Time.deltaTime);

        // Jika karakter sudah melewati batas kanan, kembalikan ke posisi kiri
        if (transform.position.x >= exitX)
        {
            transform.position = new Vector3(spawnX, groundY, transform.position.z);
        }
    }
}
