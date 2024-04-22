using Assets.Scripts.Combat;
using UnityEngine;

public class LaserShooter : MonoBehaviour
{
    [SerializeField] private bool _active = true;

    [SerializeField] private int _damagePerTick;
    [Tooltip("Time till first laser shoot")]
    [SerializeField] private float _firstDelay;
    [Tooltip("Time till laser shoot again")]
    [SerializeField] private float _delay;
    [Tooltip("Time the laser stay on")]
    [SerializeField] private float _duration;
    private float _timer;

    [SerializeField] private Collider2D _collider;

    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] Color _normalColor;
    [SerializeField] Color _shootColor;

    private bool _autoDeactive = false;

    private void Start()
    {
        _timer = _firstDelay;
        _collider.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_active)
        {
            _timer -= Time.deltaTime;
            if (_collider.gameObject.activeSelf)
            {
                if (_timer <= 0)
                {
                    _collider.gameObject.SetActive(false);
                    _timer = _delay;
                    if (_autoDeactive)
                    {
                        SetActive(false);
                        _spriteRenderer.color = Color.Lerp(_normalColor, _shootColor, 0);
                    }
                }
            }
            else
            {
                _spriteRenderer.color = Color.Lerp(_normalColor, _shootColor, (_delay - _timer) / _delay);
                if (_timer <= 0)
                {
                    _collider.gameObject.SetActive(true);
                    _timer = _duration;
                    SoundManager.Instance.PlayLaser();
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.gameObject.TryGetComponent(out Health health))
        {
            health.TakeDamage(_damagePerTick, collision.transform.position - transform.position, 0.6f);
        }
    }

    public bool IsActive()
    {
        return _active;
    }

    public void SetActive(bool active)
    {
        _active = active;
    }

    public void SetDelay(float delay)
    {
        _delay = delay;
    }

    public void SetDuration(float duration)
    {
        _duration = duration;
    }

    public void SetTimer(float timer)
    {
        _timer = timer;
    }

    public void SetAutoDeactive(bool autoDeactive)
    {
        _autoDeactive = autoDeactive;
    }
}
