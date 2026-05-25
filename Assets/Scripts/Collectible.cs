using UnityEngine;

public enum ItemType { Gem, Cherry }

public class Collectible : MonoBehaviour
{
    [Header("Effects")]
    public GameObject feedbackEffectPrefab;

    public ItemType itemType;
    public int scoreValue = 1;
    public AudioClip collectSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Update Score based on item type
            if (itemType == ItemType.Gem)
            {
                GameManager.instance.AddGems(scoreValue);
            }
            else if (itemType == ItemType.Cherry)
            {
                GameManager.instance.AddCherries(scoreValue);
            }

            // Play Sound (using a temporary audio source at the location)
            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }

            if (feedbackEffectPrefab != null)
            {
                Instantiate(feedbackEffectPrefab, transform.position, Quaternion.identity);
            }

            // Destroy the collectible
            Destroy(gameObject);
        }
    }
}
