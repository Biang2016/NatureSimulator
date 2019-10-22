using UnityEngine;
using System.Collections;
using System.Net.Http.Headers;

public class GeoElement : PoolObject
{
    [SerializeField] private SpriteRenderer SpriteRenderer;
    [SerializeField] private SpriteRenderer BorderSR;
    public BoxCollider2D Collider;

    public int MySortingOrder => SpriteRenderer.sortingOrder;

    public override void PoolRecycle()
    {
        Collider.enabled = true;
        OnSelected = false;
        base.PoolRecycle();
    }

    private GeoTypes geoType;

    public void Initialize(GeoTypes geoTypes, Vector2 size, Color color, int sortingOrder = -1)
    {
        geoType = geoTypes;
        SpriteRenderer.sprite = GeoManager.Instance.DrawGeoSprites[(int) geoTypes];
        BorderSR.sprite = GeoManager.Instance.DrawGeoBorderSprites[(int) geoTypes];
        SpriteRenderer.size = size;
        BorderSR.size = size;
        Collider.size = size;
        SpriteRenderer.color = color;
        if (sortingOrder != -1)
        {
            SpriteRenderer.sortingOrder = sortingOrder;
            BorderSR.sortingOrder = sortingOrder + 1;
        }
    }

    private bool onSelected = false;

    public bool OnSelected
    {
        get => onSelected;
        set
        {
            onSelected = value;
            BorderSR.color = value ? Color.yellow : Color.white;
        }
    }

    public void ChangeColor(Color color)
    {
        SpriteRenderer.color = color;
    }

    void Start()
    {
    }

    void Update()
    {
    }

    public GeoInfo ExportGeoInfo()
    {
        GeoInfo gi = new GeoInfo();
        gi.Color = SpriteRenderer.color;
        gi.GeoType = geoType;
        gi.Position = transform.localPosition;
        gi.Rotation = transform.rotation;
        gi.Size = SpriteRenderer.size / GameManager.Instance.ScaleFactor / GameManager.Instance.ScaleFactor;
        gi.SortingOrder = SpriteRenderer.sortingOrder;
        return gi;
    }
}