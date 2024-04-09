using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    }
}
