using UnityEngine;
using UnityEngine.UI;

public class TextBubble : PoolObject
{
    [SerializeField] private Text Text;

    public void Init(string text)
    {
        Text.text = text;
    }
}