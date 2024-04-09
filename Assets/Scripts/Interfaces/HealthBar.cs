using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Combat;
public class HealthBar : MonoBehaviour
{
    private Slider _slider;
    public Gradient gradient;
    public Image fill;

    private Health _health;

    void Awake()
    {
        _slider = GetComponent<Slider>();
        PlayerManager.OnPlayerChanged += (player) => { if (player.TryGetComponent(out Health health)) { _health = health; } };
    }

    private void Start()
    {
        PlayerManager.Instance.Player.TryGetComponent(out _health);
    }

    private void Update()
    {
        if (!_health)
        {
            if (PlayerManager.Instance.Player) PlayerManager.Instance.Player.TryGetComponent(out _health);
        }
        else
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
