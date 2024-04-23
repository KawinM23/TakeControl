using UnityEngine;

public class CollectTrialObject : MonoBehaviour
{
    [SerializeField] private CollectTrial _collectTrial;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.CompareTag("Player"))
        {
            _collectTrial.Collect();
            SoundManager.Instance.PlayDing(Random.Range(1,4),Random.Range(5,6));
            gameObject.SetActive(false);
        }
    }
}
