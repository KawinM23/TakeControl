using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Combat;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private GameObject _robot;
    private Slider _slider;
    public Gradient gradient;
    public Image fill;

    private Health _health;

    void Awake()
    {
        _health = _robot.GetComponent<Health>();
        _slider = GetComponent<Slider>();
        PlayerManager.OnPlayerChanged += (player) => { gameObject.SetActive(_robot != player); };
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
