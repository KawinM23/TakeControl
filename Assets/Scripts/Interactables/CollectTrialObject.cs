using UnityEngine;

public class CollectTrialObject : MonoBehaviour
{
    [SerializeField] private CollectTrial _collectTrial;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.CompareTag("Player"))
        {
            _collectTrial.Collect();
            gameObject.SetActive(false);
        }
    }
}
