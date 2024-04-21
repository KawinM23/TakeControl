using Assets.Scripts.Combat;
using UnityEngine;

public class EnemyHealthBar : BaseHealthBar
{
    protected override void Awake()
    {
        base.Awake();
        _robot = transform.parent.parent.gameObject;
        _health = _robot.GetComponent<Health>();
    }

    protected override void HandlePlayerChanged(GameObject player)
    {
        gameObject.SetActive(_robot != player);
    }
}
