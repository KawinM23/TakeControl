using Assets.Scripts.Combat;
using UnityEngine;

public class SavePoint : Interactable
{
    private string _sceneName;

    private void Awake()
    {
        _sceneName = gameObject.scene.name;
    }

    public override void Interact()
    {
        Debug.Log("SAVE POINT " + _sceneName);

        SaveManager.Instance.PersistSave();
        if (PlayerManager.Instance.Player.TryGetComponent(out Health health)) health.ResetHealth();
    }
}
