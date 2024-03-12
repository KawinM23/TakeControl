using Assets.Scripts.Combat;
using UnityEngine;

public class HealCollectible : MonoBehaviour
{
    [SerializeField] private int healAmount = 20; // Amount of health to heal the player
    [SerializeField] private AudioClip healSound; // Optional sound effect for healing

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("HealCollectible: OnTriggerEnter2D");
        Debug.Log("Collide with:", collision.gameObject);
        Debug.Log("Compare to:", PlayerManager.Instance.playerGameObject);

        // Check if the collider belongs to the player
        if (collision.gameObject == PlayerManager.Instance.playerGameObject)
        {
            // Get the player health component (replace "PlayerHealth" with your actual script name)
            Health playerHealth = collision.gameObject.GetComponent<Health>();

            if (playerHealth != null)
            {
                // Heal the player
                playerHealth.Heal(healAmount);

                // Play sound effect (if available)
                if (healSound != null)
                {
                    AudioSource audioSource = GetComponent<AudioSource>();
                    if (audioSource == null)
                    {
                        audioSource = gameObject.AddComponent<AudioSource>();
                    }
                    audioSource.clip = healSound;
                    audioSource.Play();
                }

                // Destroy the collectible after use
                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("HealCollectible: PlayerHealth component not found on player!");
            }
        }
    }
}
