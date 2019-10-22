using System.Collections.Generic;
using instinctai.usr.behaviours;
using UnityEngine;

public class GeoGroupInfo
{
    public List<GeoInfo> GeoInfos = new List<GeoInfo>();
    public string Name;

    public float Life;
    public float Speed;
    public float Damage;
    public float Vision;
    public float GeneralSize;
    public int FertilityRate;
    public int OffspringSizeRatio;
    public int MatureSizeRatio;
    public int MinSizeRatio;
    public int MaxSizeRatio;
    public float GrowUpRate => 0.5f / Speed;
    public List<string> Diet;
    public float Mass;

    public Vector2 Center;

    public void ResetCenterAndSortingOrder()
    {
        int minSortingOrder = 999000;

        foreach (GeoInfo gi in GeoInfos)
        {
            minSortingOrder = Mathf.Min(minSortingOrder, gi.SortingOrder);
        }

        minSortingOrder -= 2;
        foreach (GeoInfo gi in GeoInfos)
        {
            gi.SortingOrder -= minSortingOrder;
        }
    }

    public void RefreshInfo()
    {
        float XMin = 9999;
        float XMax = -9999;
        float YMin = 9999;
        float YMax = -9999;
        foreach (GeoInfo gi in GeoInfos)
        {
            XMin = Mathf.Min(XMin, gi.Position.x - gi.Size.x * 10000f);
            XMax = Mathf.Max(XMax, gi.Position.x + gi.Size.x * 10000f);
            YMin = Mathf.Min(YMin, gi.Position.y - gi.Size.y * 10000f);
            YMax = Mathf.Max(YMax, gi.Position.y + gi.Size.y * 10000f);
            float area = gi.Size.x * gi.Size.x * 3000;
            Mass += area;
            switch (gi.GeoType)
            {
                case GeoTypes.Circle:
                {
                    Life -= 3 * area;
                    Speed -= 10 * area;
                    Damage -= 5 * area;
                    Vision += 20 * area;
                    break;
                }
                case GeoTypes.Square:
                {
                    Life += 5 * area;
                    Speed += 10 * area;
                    Damage -= 5 * area;
                    Vision -= 10 * area;
                    break;
                }
                case GeoTypes.Triangle:
                {
                    Life -= 5 * area;
                    Speed -= 3 * area;
                    Damage += 10 * area;
                    Vision -= 2 * area;
                    break;
                }
                case GeoTypes.Hexagon:
                {
                    Life += 20 * area;
                    Speed -= 10 * area;
                    Damage -= 2 * area;
                    Vision -= 3 * area;
                    break;
                }
            }
        }

        Life = Mathf.Max(5f, Life);
        Speed = Mathf.Max(5, Speed);
        Damage = Mathf.Max(5, Damage);
        Vision = Mathf.Max(5, Vision);

        GeneralSize = Mathf.Sqrt((XMax - XMin) * (XMax - XMin) + (YMax - YMin) * (YMax - YMin));
        Center = new Vector3((XMax + XMin) / 2f, (YMax + YMin) / 2f);
    }
}