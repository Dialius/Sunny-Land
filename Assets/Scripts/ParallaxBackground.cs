using System.Collections.Generic;
using UnityEngine;
public class ParallaxBackground : MonoBehaviour
{
    [Header("Movement Settings")]
    public float parallaxSpeedX = 0.5f; // 0 = diam, 1 = ikut kamera
    public float parallaxSpeedY = 0f;
    public bool lockYToCamera = false;

    [Header("Looping Settings")]
    public bool repeatBackground = true;
    public float customSpacingX = 0f; // Jarak ekstra antar background
    public int clonesOnEachSide = 1; // Jumlah clone di tiap sisi (Kiri dan Kanan)

    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    private float startOffsetY;
    
    // Looping variables
    private float textureUnitSizeX;
    private List<Transform> backgroundClones = new List<Transform>();

    void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
        startOffsetY = transform.position.y - cameraTransform.position.y;

        // Setup Looping
        if (repeatBackground)
        {
            Sprite sprite = GetComponent<SpriteRenderer>().sprite;
            Texture2D texture = sprite.texture;
            // Hitung lebar gambar dalam satuan unit Unity (ditambah spacing custom)
            textureUnitSizeX = (texture.width / sprite.pixelsPerUnit) * transform.localScale.x + customSpacingX;

            // Tambahkan background asli sebagai clone pertama
            backgroundClones.Add(transform);

            // Buat clone tambahan: sebanyak 'clonesOnEachSide' di kiri dan kanan
            for (int i = 1; i <= clonesOnEachSide; i++)
            {
                CreateClone(-i); // Kiri
                CreateClone(i);  // Kanan
            }
        }
    }

    void CreateClone(int directionMultiplier)
    {
        // Copy GameObject ini sendiri tapi hapus script Parallax-nya agar tidak jalan 2x
        GameObject clone = Instantiate(gameObject, transform.parent);
        Destroy(clone.GetComponent<ParallaxBackground>());
        
        // Posisikan clone di sebelah asli
        clone.transform.position = new Vector3(
            transform.position.x + (textureUnitSizeX * directionMultiplier),
            transform.position.y,
            transform.position.z
        );
        
        backgroundClones.Add(clone.transform);
    }

    void LateUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        
        // Gerakkan semua clone
        float moveX = deltaMovement.x * parallaxSpeedX;
        float moveY = lockYToCamera ? 0 : (deltaMovement.y * parallaxSpeedY);
        float newY = lockYToCamera ? cameraTransform.position.y + startOffsetY : backgroundClones[0].position.y + moveY;

        foreach (Transform clone in backgroundClones)
        {
            clone.position = new Vector3(clone.position.x + moveX, newY, clone.position.z);
        }

        // Looping logic
        if (repeatBackground)
        {
            float relativeDistanceX = cameraTransform.position.x * (1 - parallaxSpeedX);

            // Jika kamera bergerak jauh ke kanan
            if (relativeDistanceX > transform.position.x + (textureUnitSizeX * 0.5f))
            {
                transform.position = new Vector3(transform.position.x + textureUnitSizeX, transform.position.y, transform.position.z);
                
                // Rapihkan kembali posisi semua clone
                RepositionClones();
            }
            // Jika kamera bergerak jauh ke kiri
            else if (relativeDistanceX < transform.position.x - (textureUnitSizeX * 0.5f))
            {
                transform.position = new Vector3(transform.position.x - textureUnitSizeX, transform.position.y, transform.position.z);
                
                // Rapihkan kembali posisi semua clone
                RepositionClones();
            }
        }
        
        lastCameraPosition = cameraTransform.position;
    }

    void RepositionClones()
    {
        // backgroundClones[0] adalah transform utama
        int cloneIndex = 1;
        for (int i = 1; i <= clonesOnEachSide; i++)
        {
            // Kiri
            if (cloneIndex < backgroundClones.Count)
            {
                backgroundClones[cloneIndex].position = new Vector3(transform.position.x - (textureUnitSizeX * i), transform.position.y, transform.position.z);
                cloneIndex++;
            }
            
            // Kanan
            if (cloneIndex < backgroundClones.Count)
            {
                backgroundClones[cloneIndex].position = new Vector3(transform.position.x + (textureUnitSizeX * i), transform.position.y, transform.position.z);
                cloneIndex++;
            }
        }
    }
}
