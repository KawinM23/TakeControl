using Assets.Scripts.Combat;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    private GameObject _robot;
    private Slider _slider;
    public Gradient gradient;
    public Image fill;

    private Health _health;

    void Awake()
    {
        _robot = transform.parent.parent.gameObject;
        _health = _robot.GetComponent<Health>();
        _slider = GetComponent<Slider>();
    }

    void Start()
    {
        PlayerManager.OnPlayerChanged += HandlePlayerChanged;
    }

    void OnDisable()
    {
        PlayerManager.OnPlayerChanged -= HandlePlayerChanged;
    }

    void HandlePlayerChanged(GameObject player)
    {
        gameObject.SetActive(_robot != player);
    }

    private void Update()
    {
        if (_health)
        {
            SetHealthBar();
        }
    }

    public void SetHealthBar()
    {
        _slider.maxValue = _health.GetMaxHealth();
        _slider.value = _health.GetCurrentHealth();
        fill.color = gradient.Evaluate(_slider.normalizedValue);
    }
}
