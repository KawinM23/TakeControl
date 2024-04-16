using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HackEventManager : MonoBehaviour
{
    public static HackEventManager Instance { get; private set; }

    [SerializeField] private GameObject _hackEventCanvas;
    [SerializeField] private GameObject _buttonPrefab;
    [SerializeField] private Slider _slider;
    [SerializeField] private Image _sliderFillImage;

    public bool IsHacking = false;
    public List<HackButtonUI> HackButtons;
    private float _hackDuration;
    public float HackTimer;
    public int ButtonAmount;
    public int TotalButtonAmount;

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

                _slider.value = (HackTimer) / _hackDuration;
                _sliderFillImage.color = Color.Lerp(_slider.colors.pressedColor, _slider.colors.normalColor, _slider.value);
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
        _hackDuration = hackDuration;
        ButtonAmount = amount;
        TotalButtonAmount = amount;
        Time.timeScale = 0.01f;
        Vector2 size = _buttonPrefab.GetComponent<RectTransform>().sizeDelta;
        for (int i = 0; i < amount; i++)
        {
            Vector3 pos = new Vector3(Random.Range(0 + size.x / 2, Screen.width - size.x / 2), Random.Range(0 + size.y / 2, Screen.height - size.y / 2), 0);
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
        return ButtonAmount == 0;
    }

    public bool HackQuickTimeEvent()
    {
        return !IsHacking;
    }


}
