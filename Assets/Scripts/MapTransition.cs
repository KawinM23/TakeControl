using UnityEngine;
using UnityEngine.SceneManagement;

public class MapTransition : MonoBehaviour
{
    public enum Direction
    {
        None,
        Left,
        Right,
        Up,
        Down
    }

    [SerializeField] private string _toScene;
    private Scene _fromScene;
    [SerializeField] private Direction _direction;

    private Collider2D _collider;
    private Transform spawnPoint;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _fromScene = gameObject.scene;
        spawnPoint = transform.GetChild(0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.TryGetComponent(out Controller controller);
        if (controller && controller.Input is PlayerController && MapManager.Instance.CanChangeScene)
        {
            float distanceFromSpawn = 0f;
            /*if (_direction == Direction.Left || _direction == Direction.Right)
            {
                distanceFromSpawn = collision.transform.position.y - spawnPoint.position.y;
            }
            else if (_direction == Direction.Up || _direction == Direction.Down)
            {
                distanceFromSpawn = collision.transform.position.x - spawnPoint.position.x;
            }*/
            Debug.Log("From " + _fromScene.name + " to " + _toScene + " with dis " + distanceFromSpawn);
            MapManager.Instance.ChangeScene(_fromScene.name, _toScene, _direction, distanceFromSpawn);
        }
    }

    public static Vector2 FindDestinationPosition(string fromScene, string toScene, float distanceFromSpawn)
    {
        MapTransition[] mta = FindObjectsByType<MapTransition>(FindObjectsSortMode.None);
        foreach (MapTransition mt in mta)
        {
            if (mt._fromScene.name.Equals(toScene) && mt._toScene.Equals(fromScene))
            {
                Vector2 destination = mt.spawnPoint.position;
                if (mt._direction == Direction.Left || mt._direction == Direction.Right)
                {

                    destination.y += distanceFromSpawn;
                }
                else if (mt._direction == Direction.Up || mt._direction == Direction.Down)
                {
                    destination.x += distanceFromSpawn;
                }
                return destination;
            }
        }
        return Vector2.zero;
    }
}
