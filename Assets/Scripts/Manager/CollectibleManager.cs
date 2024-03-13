using System.Collections.Generic;
using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    [SerializeField] private List<CollectibleData> allCollectibles; // List of all collectible types
    [SerializeField] private Dictionary<CollectibleData, int> currentCollectibles; // Dictionary to store current counts for each collectible
    public static CollectibleManager Instance { get; private set; } // Singleton instance

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(this.gameObject);

        // Initialize collectibles
        foreach (CollectibleData cd in Enum.GetValues(typeof(CollectibleData)))
        {

        }

        // TODO: save and load system in the future

        currentCollectibles = new Dictionary<CollectibleData, int>();
        foreach (CollectibleData collectible in allCollectibles)
        {
            currentCollectibles.Add(CollectibleData.Coin, 0); // Initialize all counts to 0
        }
        Debug.Log("CollectibleManager: Initialized");
    }


    public void AddCollectible(CollectibleData collectibleName, int amount = 1)
    {
        if (!currentCollectibles.ContainsKey(collectibleName))
        {
            Debug.LogError("CollectibleManager: Collectible '" + collectibleName + "' not found!");
            return;
        }

        currentCollectibles[collectibleName] += amount;
        Debug.Log("Collected " + amount + " '" + collectibleName + "'");
    }

    public bool RemoveCollectible(CollectibleData collectibleName, int amount = 1)
    {
        if (!currentCollectibles.ContainsKey(collectibleName))
        {
            Debug.LogError("CollectibleManager: Collectible '" + collectibleName + "' not found!");
            return false;
        }

        if (currentCollectibles[collectibleName] < amount)
        {
            Debug.LogError("CollectibleManager: Insufficient '" + collectibleName + "' to remove!");
            return false;
        }

        currentCollectibles[collectibleName] -= amount;
        Debug.Log("Used " + amount + " '" + collectibleName + "'");
        return true;
    }

    public int GetCurrentCount(CollectibleData collectibleName)
    {
        if (!currentCollectibles.ContainsKey(collectibleName))
        {
            Debug.LogError("CollectibleManager: Collectible '" + collectibleName + "' not found!");
            return 0;
        }

        return currentCollectibles[collectibleName];
    }
}

// Collectible data class (can be extended with additional properties)
public enum CollectibleData
{
    Coin,
    // Add more collectible types here
}
