using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DrawFunctionButton : MonoBehaviour
{
    [SerializeField] private Image BG;
    [SerializeField] private Image Icon;
    [SerializeField] private Button Button;
    [SerializeField] private Sprite[] Sprites;

    private bool isSelected;
    internal FunctionTypes MyFunctionType;

    public bool IsSelected
    {
        get => isSelected;
        set
        {
            isSelected = value;
            BG.enabled = isSelected;
        }
    }

    public void Initialize(FunctionTypes ft, UnityAction<DrawFunctionButton> onclick)
    {
        Icon.sprite = Sprites[(int) ft];
        MyFunctionType = ft;
        Button.onClick.AddListener(delegate { onclick(this); });
    }

    public enum FunctionTypes
    {
        Select,
        Delete
    }

    public static Dictionary<FunctionTypes, EditArea.States> TarStateDict = new Dictionary<FunctionTypes, EditArea.States>
    {
        {FunctionTypes.Select, EditArea.States.Select},
        {FunctionTypes.Delete, EditArea.States.Delete},
    };
}