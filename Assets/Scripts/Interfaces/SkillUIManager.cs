using Assets.Scripts.Capabilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUIManager : MonoBehaviour
{
    public static SkillUIManager Instance { get; private set; }
    [SerializeField] private EquipmentSlot _doubleJumpSlot;
    [SerializeField] private EquipmentSlot _dashSlot;

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        CheckSkill();
        PlayerManager.OnPlayerChanged += (player) => CheckSkill();
    }

    public void CheckSkill()
    {
        GameObject player = PlayerManager.Instance.Player;
        _doubleJumpSlot.gameObject.SetActive(player.TryGetComponent(out Jump jump) && jump.GetMaxAirJump()==1);
        _dashSlot.gameObject.SetActive(player.TryGetComponent(out Move move) && move._hasDash);
    }
}
