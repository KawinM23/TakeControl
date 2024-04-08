using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackEventManager : MonoBehaviour
{
    public static HackEventManager Instance { get; private set; }
    [SerializeField] private GameObject HackEventCanvas;
    [SerializeField] private GameObject ButtonPrefab;
    public List<HackButtonUI> HackButtons;

    public bool IsHacking = false;
    public float HackTimer;
    public int buttonAmount = 9999;

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
        IsHacking = false;
        HackEventCanvas.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (IsHacking)
        {
            if (HackTimer >= 0)
            {
                HackTimer -= Time.fixedUnscaledDeltaTime;
                if (buttonAmount == 0)
                {
                    IsHacking = false;
                }
            }
            else
            {
                IsHacking = false;
            }
        }
    }

    public void StartHack(float hackDuration, int amount)
    {
        HackEventCanvas.SetActive(true);
        IsHacking = true;
        HackTimer = hackDuration;
        buttonAmount = amount;
        Time.timeScale = 0.01f;
        for (int i = 0; i < amount; i++)
        {
            Vector3 pos = new Vector3(Random.Range(0, Screen.width), Random.Range(0, Screen.height), 0);
            GameObject button = Instantiate(ButtonPrefab, pos, new Quaternion(), HackEventCanvas.transform);
            HackButtons.Add(button.GetComponent<HackButtonUI>());
        }
        
    }

    public bool EndHack()
    {
        foreach (HackButtonUI hackButton in HackButtons)
        {
            Destroy(hackButton.gameObject);
        }
        HackButtons.Clear(); 

        IsHacking = false;
        Time.timeScale = 1f;
        HackEventCanvas.SetActive(false);
        return buttonAmount==0;
    }

    public bool HackQuickTimeEvent()
    {
        return !IsHacking;
    }


}
