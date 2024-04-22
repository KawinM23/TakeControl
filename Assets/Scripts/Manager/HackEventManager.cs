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
        Debug.Log("Start Hack " + amount + " buttons");
        _hackEventCanvas.SetActive(true);
        IsHacking = true;
        HackTimer = hackDuration;
        _hackDuration = hackDuration;
        ButtonAmount = amount;
        TotalButtonAmount = amount;
        Time.timeScale = 0.01f;

        Vector2 size = _buttonPrefab.GetComponent<RectTransform>().sizeDelta;
        List<Vector3> validPositions = new List<Vector3>();  // Store valid positions for buttons

        // Try to place buttons within screen bounds without overlap
        for (int i = 0; i < amount; i++)
        {
            int attempts = 1000; // Maximum attempts to find a valid position
            bool positionFound = false;

            while (attempts > 0 && !positionFound)
            {
                Vector3 pos = new Vector3(Random.Range(0 + size.x / 2, Screen.width - size.x / 2),
                                            Random.Range(0 + size.y / 2, Screen.height - size.y / 2), 0);

                // Check for overlap with existing buttons and screen edges
                bool isValid = true;
                foreach (Vector3 validPos in validPositions)
                {
                    if (Vector3.Distance(pos, validPos) < size.x + size.y) // Check for minimum distance between buttons
                    {
                        isValid = false;
                        break;
                    }
                }

                if (isValid && pos.x > size.x / 2 && pos.x < Screen.width - size.x / 2 &&
                    pos.y > size.y / 2 && pos.y < Screen.height - size.y / 2) // Check screen edges
                {
                    validPositions.Add(pos);
                    positionFound = true;
                }

                attempts--;
            }

            // If no valid position found after attempts, reduce amount by 1
            if (!positionFound)
            {
                amount--;
                Debug.LogWarning($"Failed to find position for button {i + 1}. Reducing total buttons to {amount}");
            }
        }

        // If any valid positions found, instantiate buttons
        if (validPositions.Count > 0)
        {
            for (int i = 0; i < validPositions.Count; i++)
            {
                GameObject button = Instantiate(_buttonPrefab, validPositions[i], new Quaternion(), _hackEventCanvas.transform);
                HackButtons.Add(button.GetComponent<HackButtonUI>());
            }
        }
        else
        {
            Debug.LogError("No valid positions found for buttons. Hack event may not function properly.");
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
