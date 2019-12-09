﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

// HOW PLAYER CAN UNDERSTAND THE UNDERLINE MODEL OF YOUR GAME!!!!!!!

public class QAPanel : BaseUIForm
{
    [SerializeField] private Transform TextContainer;
    [SerializeField] private ScrollRect ScrollView;
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

    void Start()
    {
        GenerateText("I'm your Geometry World assistant. If you have any question about this world or the species, you can ask me here.", TextBubble.Alignment.Left);
    }

    public void Reset()
    {
        foreach (TextBubble textBubble in TextBubbles)
        {
            textBubble.PoolRecycle();
        }

        TextBubbles.Clear();
    }

    public void GenerateText(string text, TextBubble.Alignment align)
    {
        TextBubble tb = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.TextBubble].AllocateGameObject<TextBubble>(TextContainer);
        tb.Init(text, align);
        TextBubbles.Add(tb);
        ScrollView.ScrollToBottom();
    }
}