using System.Collections.Generic;
using instinctai.usr.behaviours;
using UnityEngine;

public class GeoGroupInfo
{
    public List<GeoInfo> GeoInfos = new List<GeoInfo>();
    public string Name;

    public void ResetCenterAndSortingOrder()
    {
        //float center_X = 0;
        //float center_Y = 0;
        //float G = 0;
        //foreach (GeoInfo geoInfo in GeoInfos)
        //{
        //    center_X += geoInfo.Position.x * geoInfo.Size.x * geoInfo.Size.x;
        //    center_Y += geoInfo.Position.y * geoInfo.Size.x * geoInfo.Size.x;
        //    G += geoInfo.Size.x * geoInfo.Size.x;
        //}

        //center_X /= G;
        //center_Y /= G;

        //foreach (GeoInfo gi in GeoInfos)
        //{
        //    gi.Position -= new Vector2(center_X, center_Y);
        //}

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

    public Creature.CreatureInfo GetCreatureInfo()
    {
        Creature.CreatureInfo ci = new Creature.CreatureInfo();
        float XMin = 0;
        float XMax = 0;
        float YMin = 0;
        float YMax = 0;
        foreach (GeoInfo gi in GeoInfos)
        {
            XMin = Mathf.Min(XMin, gi.Position.x);
            XMax = Mathf.Max(XMax, gi.Position.x);
            YMin = Mathf.Min(YMin, gi.Position.y);
            YMax = Mathf.Max(YMax, gi.Position.y);
            float area = gi.Size.x * gi.Size.x * 3000;
            ci.Mass += area;
            switch (gi.GeoType)
            {
                case GeoTypes.Circle:
                {
                    ci.Life -= 3 * area;
                    ci.Speed -= 10 * area;
                    ci.Damage -= 5 * area;
                    ci.Vision += 20 * area;
                    break;
                }
                case GeoTypes.Square:
                {
                    ci.Life += 5 * area;
                    ci.Speed += 10 * area;
                    ci.Damage -= 5 * area;
                    ci.Vision -= 10 * area;
                    break;
                }
                case GeoTypes.Triangle:
                {
                    ci.Life -= 5 * area;
                    ci.Speed -= 3 * area;
                    ci.Damage += 10 * area;
                    ci.Vision -= 2 * area;
                    break;
                }
                case GeoTypes.Hexagon:
                {
                    ci.Life += 20 * area;
                    ci.Speed -= 10 * area;
                    ci.Damage -= 2 * area;
                    ci.Vision -= 3 * area;
                    break;
                }
            }

        }

        ci.Life = Mathf.Max(5f, ci.Life);
        ci.Speed = Mathf.Max(5, ci.Speed);
        ci.Damage = Mathf.Max(5, ci.Damage);
        ci.Vision = Mathf.Max(5, ci.Vision);

        ci.GeneralSize = Mathf.Sqrt((XMax - XMin) * (XMax - XMin) + (YMax - YMin) * (YMax - YMin));

        return ci;
    }
}