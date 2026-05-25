using UnityEngine;

public class DestroyAfterAnimation : MonoBehaviour
{
    public float delay = 0.5f;

    void Start()
    {
        // Memastikan efek ini muncul di depan musuh
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = 100; 
        }

        Destroy(gameObject, delay);
    }
}
