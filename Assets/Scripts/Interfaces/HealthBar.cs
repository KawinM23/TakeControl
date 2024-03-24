using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Combat;
public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    [SerializeField] private Health _health;

    void Awake()
    {
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
        slider.maxValue = _health.GetMaxHealth();
        slider.value = _health.GetCurrentHealth();
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
