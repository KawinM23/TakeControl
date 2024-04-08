using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackEventManager : MonoBehaviour
{
    public static HackEventManager Instance { get; private set; }

    [SerializeField] private GameObject _hackEventCanvas;
    [SerializeField] private GameObject _buttonPrefab;

    public bool IsHacking = false;
    public List<HackButtonUI> HackButtons;
    public float HackTimer;
    public int ButtonAmount;

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
        _hackEventCanvas.SetActive(false);
    }

    private void Update()
    {
        if (IsHacking)
        {
            if (HackTimer >= 0)
            {
                HackTimer -= Time.unscaledDeltaTime;
                if (ButtonAmount == 0)
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
        _hackEventCanvas.SetActive(true);
        IsHacking = true;
        HackTimer = hackDuration;
        ButtonAmount = amount;
        Time.timeScale = 0.01f;
        for (int i = 0; i < amount; i++)
        {
            Vector3 pos = new Vector3(Random.Range(0, Screen.width), Random.Range(0, Screen.height), 0);
            GameObject button = Instantiate(_buttonPrefab, pos, new Quaternion(), _hackEventCanvas.transform);
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
        _hackEventCanvas.SetActive(false);
        return ButtonAmount==0;
    }

    public bool HackQuickTimeEvent()
    {
        return !IsHacking;
    }


}
