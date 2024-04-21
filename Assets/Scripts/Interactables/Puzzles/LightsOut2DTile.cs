using UnityEngine;
public class LightsOut2DTile : MonoBehaviour
{
    [SerializeField] private GameObject _puzzle; // belongs to which puzzle
    [SerializeField] private int _index; // index in the puzzle grid
    [SerializeField] private Collider2D _tileCollider2D;

    void Start()
    {
        // Ensure the tile prefab has a Collider2D component
        _tileCollider2D = GetComponent<Collider2D>();
        if (_tileCollider2D == null)
        {
            Debug.LogError("Tile prefab is missing a Collider2D component! Please attach one.");
            return; // Exit if no collider found
        }
    }

    public void SetIndex(int index)
    {
        _index = index;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the colliding object is the player using tags
        if (collision.gameObject.CompareTag("Player"))
        {
            // Handle collision with the player here
            Debug.Log("Player collided with tile!");

            if (_puzzle.TryGetComponent(out LightsOut2D puzzle))
            {
                puzzle.ToggleLights(_index);
            }
            else
            {
                Debug.LogError("Tile is not part of a LightsOut2D puzzle!");
            }

        }
    }
}