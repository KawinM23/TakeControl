using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HackButtonUI : MonoBehaviour
{
    public bool Clicked;

    private Image _image;
    private Button _button;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _button = GetComponent<Button>();
    }

    public void SetButton(bool clicked)
    {
        if (!Clicked && clicked)
        {
            HackEventManager.Instance.ButtonAmount -= 1;
        }
        Clicked = clicked;
        if (clicked)
        {
            SoundManager.Instance.PlayDing(HackEventManager.Instance.ButtonAmount, HackEventManager.Instance.TotalButtonAmount);
            _image.color = _button.colors.pressedColor;
        }
        else
        {
            _image.color = _button.colors.normalColor;
        }

    }
}
