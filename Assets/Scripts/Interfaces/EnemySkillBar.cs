using Assets.Scripts.Capabilities;
using Assets.Scripts.Combat;
using UnityEngine;

public class EnemySkillBar : MonoBehaviour
{
    private GameObject _robot;

    [SerializeField] private EquipmentSlot _swordSlot;
    [SerializeField] private EquipmentSlot _gunSlot;

    [SerializeField] private EquipmentSlot _doubleJumpSlot;
    [SerializeField] private EquipmentSlot _dashSlot;

    private void Awake()
    {
        _robot = transform.parent.parent.gameObject;
        _swordSlot.gameObject.SetActive(_robot.GetComponent<Sword>() != null);
        _gunSlot.gameObject.SetActive(_robot.GetComponent<Gun>() != null);

        _doubleJumpSlot.gameObject.SetActive(_robot.TryGetComponent(out Jump jump) && jump.GetMaxAirJump() == 1);
        _dashSlot.gameObject.SetActive(_robot.TryGetComponent(out Move move) && move._hasDash);

    }

    private void Start()
    {
        PlayerManager.OnPlayerChanged += HandlePlayerChanged;
    }

    private void OnDisable()
    {
        PlayerManager.OnPlayerChanged -= HandlePlayerChanged;
    }

    private void HandlePlayerChanged(GameObject player)
    {
        gameObject.SetActive(_robot != player);
    }
}
