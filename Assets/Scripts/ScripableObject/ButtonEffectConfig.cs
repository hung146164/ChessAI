using UnityEngine;

[CreateAssetMenu(fileName = "ButtonEffectConfig", menuName = "UI/ButtonEffectConfig")]
public class ButtonEffectConfig : ScriptableObject
{

    public Sprite normalSprite;
    public Sprite hoverSprite;
    public Sprite pressedSprite;
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;
    public Color pressedColor = Color.gray;
}
