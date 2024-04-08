using Assets.Scripts.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentUIManager : MonoBehaviour
{
    public static EquipmentUIManager Instance { get; private set; }
    [SerializeField] private EquipmentSlot _swordSlot;
    [SerializeField] private EquipmentSlot _gunSlot;

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
        CheckEquipment();
        PlayerManager.OnPlayerChanged += (player) => CheckEquipment();
    }

    public void CheckEquipment()
    {
        GameObject player = PlayerManager.Instance.Player;
        _swordSlot.gameObject.SetActive(player.TryGetComponent(out Sword sword));
        _gunSlot.gameObject.SetActive(player.TryGetComponent(out Gun gun));
        if (sword && sword.enabled)
        {
            Select(sword);
        }
        if (gun && gun.enabled)
        {
            Select(gun);
        }
    }

    public void Select(MonoBehaviour mono)
    {
        if (mono is Sword)
        {
            _swordSlot.Select();
            _gunSlot.Deselect();
        }
        if (mono is Gun)
        {
            _swordSlot.Deselect();
            _gunSlot.Select();
        }
    }
}
