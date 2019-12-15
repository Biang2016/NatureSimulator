using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

// HOW PLAYER CAN UNDERSTAND THE UNDERLINE MODEL OF YOUR GAME!!!!!!!

public class QAPanel : BaseUIForm
{
    [SerializeField] private Transform TextContainer;
    [SerializeField] private GameObject Panel;
    [SerializeField] private ScrollRect ScrollView;
    [SerializeField] private Button ShowButton;
    [SerializeField] private Button HideButton;
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

        ShowButton.onClick.AddListener(ShowPanel);
        HideButton.onClick.AddListener(HidePanel);
    }

    void Start()
    {
        GenerateText("I'm your Geometry World assistant. If you have any question about this world or the species, you can ask me here.", TextBubble.Alignment.Left);
    }

    public void ShowPanel()
    {
        Panel.SetActive(true);
        HideButton.gameObject.SetActive(true);
        ShowButton.gameObject.SetActive(false);
    }

    public void HidePanel()
    {
        Panel.SetActive(false);
        HideButton.gameObject.SetActive(false);
        ShowButton.gameObject.SetActive(true);
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