using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private CollectibleManager collectibleManager; // Reference to the CollectibleManager
    [SerializeField] private int coinValue = 1; // Value of the coin (optional)

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collider belongs to the player
        if (collision.gameObject == PlayerManager.Instance.playerGameObject)
        {
            // Add coin to the CollectibleManager
            collectibleManager.AddCollectible(CollectibleData.Coin, coinValue);
            // Optional: Play sound effect, show visual feedback, etc.

            // Destroy the coin object after collection
            Destroy(gameObject);
        }
    }
}
