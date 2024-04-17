using UnityEngine;
using Assets.Scripts.Combat;

public class HealthBar : BaseHealthBar
{
    protected override void Start()
    {
        base.Start();
        PlayerManager.Instance.Player.TryGetComponent(out _health);
    }

    protected override void HandlePlayerChanged(GameObject player)
    {
        if (player.TryGetComponent(out Health health))
        {
            _health = health;
        }
    }

    protected override void Update()
    {
        if (!_health && PlayerManager.Instance.Player)
        {
            PlayerManager.Instance.Player.TryGetComponent(out _health);
        }
        base.Update();
    }
}
