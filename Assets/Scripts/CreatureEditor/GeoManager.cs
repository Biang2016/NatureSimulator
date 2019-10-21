using System;
using UnityEngine;
using System.Collections.Generic;

public class GeoManager : MonoSingleton<GeoManager>
{
    public Sprite[] DefaultGeoSprites;
    public Sprite[] BloomGeoSprites;
    public Sprite[] DrawGeoSprites;
    public Sprite[] DrawGeoBorderSprites;

    void Update()
    {
    }

    [Serializable]
    public class GeoDrawingSetting
    {
        public GeoTypes GeoType;
        public Vector2 PivotOffset = Vector2.zero;
        public float SizeRatio = 1.0f;
        public float DefaultRotation = 0f;
    }

    public List<GeoDrawingSetting> GeoDrawingSettings = new List<GeoDrawingSetting>();
    internal Dictionary<GeoTypes, GeoDrawingSetting> GeoDrawingSettingsDict = new Dictionary<GeoTypes, GeoDrawingSetting>();

    void Start()
    {
        foreach (GeoDrawingSetting gds in GeoDrawingSettings)
        {
            GeoDrawingSettingsDict.Add(gds.GeoType, gds);
        }
    }
}