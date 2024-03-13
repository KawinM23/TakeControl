using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    [SerializeField]  private Assets.Scripts.Combat.Health health;

    public void Awake()
    {
        if(!health)
        {
            health = transform.parent.GetComponent<Assets.Scripts.Combat.Health>();
        }
    }

    public void SetHealthBar()
    {
        slider.maxValue = health.GetMaxHealth();
        slider.value = health.GetCurrentHealth();
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
