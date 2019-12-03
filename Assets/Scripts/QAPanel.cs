using System.Collections.Generic;
using UnityEngine;

// HOW PLAYER CAN UNDERSTAND THE UNDERLINE MODEL OF YOUR GAME!!!!!!!

public class QAPanel : BaseUIForm
{
    [SerializeField] private Transform TextContainer;
    private List<TextBubble> TextBubbles = new List<TextBubble>();

    void Awake()
    {
        UIType = new UIType();
        UIType.IsClearStack = false;
        UIType.IsESCClose = true;
        UIType.IsClickElsewhereClose = false;
        UIType.UIForms_Type = UIFormTypes.Fixed;
        UIType.UIForms_ShowMode = UIFormShowModes.Normal;
        UIType.UIForm_LucencyType = UIFormLucencyTypes.Penetrable;
    }

    public void Reset()
    {
        foreach (TextBubble textBubble in TextBubbles)
        {
            textBubble.PoolRecycle();
        }

        TextBubbles.Clear();
    }

    public void GenerateText(string text)
    {
        TextBubble tb = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.TextBubble].AllocateGameObject<TextBubble>(TextContainer);
        tb.Init(text);
        TextBubbles.Add(tb);
    }
}