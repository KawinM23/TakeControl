using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float _upVelocity;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.TryGetComponent(out Rigidbody2D rb))
            {
                rb.velocity = new Vector2(rb.velocity.x, _upVelocity);
                SoundManager.Instance.PlayJumpPad();
            }
        }
    }
}
