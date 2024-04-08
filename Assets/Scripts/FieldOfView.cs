using System.Linq;
using UnityEngine;

#nullable enable

public class FieldOfView : MonoBehaviour
{
    // https://www.youtube.com/watch?v=j1-OyLo77ss
    // Pooh: thank you @Mifuu for the "original" implementation
    public Vector2 facingDir;
    public float radius;
    public float spanAngle;
    public LayerMask targetMask;
    public LayerMask obstructionMask;

    private GameObject _playerRef;

    private bool _canSeePlayer;
    public bool CanSeePlayer => _canSeePlayer;

    private SpriteRenderer _spriteRenderer;
    private bool _prevFlipX = false;


    void Start()
    {
        _playerRef = PlayerManager.Instance.Player;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        _prevFlipX = _spriteRenderer.flipX;

        // StartCoroutine(FOVRoutine());
    }

    void FixedUpdate()
    {
        if (_spriteRenderer.flipX != _prevFlipX)
        {
            facingDir = -facingDir;
            _prevFlipX = _spriteRenderer.flipX;
        }
    }

    // IEnumerator FOVRoutine()
    // {
    //     WaitForSeconds wait = new WaitForSeconds(0.2f);

    //     while (!_canSeePlayer)
    //     {
    //         yield return wait;
    //         FieldOfViewCheck();
    //     }
    // }


    public bool FieldOfViewCheck()
    {
        Collider2D[] rangeChecks = Physics2D.OverlapCircleAll(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform? target = rangeChecks.FirstOrDefault(c => c.transform != transform)?.transform;
            if (target == null)
            {
                _canSeePlayer = false;
                return _canSeePlayer;
            }

            Vector2 directionToTarget = (target.position - transform.position).normalized;

            if (Vector2.Angle(facingDir, directionToTarget) < spanAngle / 2)
            {
                float distanceToTarget = Vector2.Distance(transform.position, target.position);

                if (!Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    _canSeePlayer = true;
                }
                else
                {
                    _canSeePlayer = false;
                }
            }
            else
            {
                _canSeePlayer = false;
            }
        }
        else
        {
            _canSeePlayer = false;
        }
        return _canSeePlayer;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, PosFromAngle((spanAngle / 2) + Vector2.SignedAngle(Vector2.right, facingDir)));
        Gizmos.DrawLine(transform.position, PosFromAngle(-(spanAngle / 2) + Vector2.SignedAngle(Vector2.right, facingDir)));
        if (_canSeePlayer && _playerRef != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _playerRef.transform.position);
        }
    }

    Vector2 PosFromAngle(float deg)
    {
        Vector2 output = new Vector2();
        output.x = transform.position.x + radius * Mathf.Cos(deg * Mathf.Deg2Rad);
        output.y = transform.position.y + radius * Mathf.Sin(deg * Mathf.Deg2Rad);
        return output;
    }
}
