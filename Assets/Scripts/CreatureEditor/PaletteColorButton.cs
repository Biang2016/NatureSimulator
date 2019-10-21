using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PaletteColorButton : PoolObject
{
    internal Color Color;
    [SerializeField] private Button Button;
    [SerializeField] private Image Image;
    [SerializeField] private Image BorderImage;

    Vector2 A = new Vector2(0, -1);
    Vector2 B = new Vector2(0.866f, -0.5f);

    public void Initialize(int posA, int posB, float sizeRatio, Color color, UnityAction<PaletteColorButton> onClick)
    {
        transform.localPosition = A * posA * sizeRatio + B * posB * sizeRatio;
        Image.color = color;
        Color = color;
        BorderImage.color = new Color(255 - color.r, 255 - color.g, 255 - color.b);
        transform.localScale = Vector3.one * sizeRatio / 70f;
        Button.onClick.AddListener(delegate { onClick(this); });
    }

    private bool isSelected;

    public bool IsSelected
    {
        get { return isSelected; }
        set
        {
            isSelected = value;
            BorderImage.enabled = isSelected;
        }
    }

    public void OnMouseHover()
    {
        if (!IsSelected)
        {
            BorderImage.enabled = true;
            transform.SetAsLastSibling();
        }
    }

    public void OnMouseLeave()
    {
        if (!IsSelected)
        {
            BorderImage.enabled = false;
        }
    }
}