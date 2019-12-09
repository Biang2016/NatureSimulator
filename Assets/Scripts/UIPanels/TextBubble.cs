using UnityEngine;
using UnityEngine.UI;

public class TextBubble : PoolObject
{
    [SerializeField] private RectTransform SelfRect;
    [SerializeField] private Image BG;
    [SerializeField] private Text Text;
    [SerializeField] private Color SelfColor;
    [SerializeField] private Color OtherSideColor;

    public enum Alignment
    {
        Left,
        Right
    }

    public void Init(string text, Alignment align)
    {
        Text.text = text;
        if (align == Alignment.Left)
        {
            BG.rectTransform.SetLeft(0f);
            BG.rectTransform.SetRight(50f);
            BG.color = OtherSideColor;
        }
        else
        {
            BG.rectTransform.SetLeft(50);
            BG.rectTransform.SetRight(0f);
            BG.color = SelfColor;
        }
    }

    void Update()
    {
        SelfRect.sizeDelta = new Vector2(SelfRect.sizeDelta.x, Text.rectTransform.sizeDelta.y + 10f);
    }
}