using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GeoButton : PoolObject
{
    [SerializeField] private Text DescText;
    [SerializeField] private Image Image;
    [SerializeField] private Button Button;
    [SerializeField] private GeoTypes MyGeoType;

    public void Initialize(GeoTypes geoType, UnityAction<GeoTypes> clickAction)
    {
        MyGeoType = geoType;
        Image.sprite = GeoManager.Instance.DefaultGeoSprites[(int) MyGeoType];
        DescText.text = Geo.GeoDescDict[geoType];
        if (clickAction != null)
            Button.onClick.AddListener(delegate { clickAction(geoType); });
    }

    private bool isSelected;

    public bool IsSelected
    {
        get { return isSelected; }
        set
        {
            isSelected = value;
            if (isSelected)
            {
                Image.sprite = GeoManager.Instance.BloomGeoSprites[(int) MyGeoType];
                DescText.enabled = true;
            }
            else
            {
                Image.sprite = GeoManager.Instance.DefaultGeoSprites[(int) MyGeoType];
                DescText.enabled = false;
            }
        }
    }

    public void OnMouseHover()
    {
        if (!isSelected)
        {
            Image.sprite = GeoManager.Instance.BloomGeoSprites[(int) MyGeoType];
            DescText.enabled = true;
        }
    }

    public void OnMouseExit()
    {
        if (!isSelected)
        {
            Image.sprite = GeoManager.Instance.DefaultGeoSprites[(int) MyGeoType];
            DescText.enabled = false;
        }
    }
}