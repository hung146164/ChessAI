using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonColorChange : MonoBehaviour
{
    Image buttonImage;
    public ButtonEffectConfig buttonEffectConfig;

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
    }
    private void Start()
    {
        buttonImage.sprite = buttonEffectConfig.normalSprite;
        buttonImage.color = buttonEffectConfig.normalColor;
    }
    public void OnHoverEnter()
    {
        buttonImage.sprite = buttonEffectConfig.hoverSprite;
        buttonImage.color = buttonEffectConfig.hoverColor;
        
    }
    public void OnHoverExit()
    {
        buttonImage.sprite = buttonEffectConfig.normalSprite;
        buttonImage.color = buttonEffectConfig.normalColor;
    }
    public void OnButtonPressed()
    {
        buttonImage.sprite = buttonEffectConfig.pressedSprite;
        buttonImage.color = buttonEffectConfig.pressedColor;
    }


}
