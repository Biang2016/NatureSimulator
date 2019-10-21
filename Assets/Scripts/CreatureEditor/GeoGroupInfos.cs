using System.Collections.Generic;
using UnityEngine;

public class GeoGroupInfo
{
    public List<GeoInfo> GeoInfos = new List<GeoInfo>();
    public string Name;

    public void ResetCenterAndSortingOrder()
    {
        float center_X = 0;
        float center_Y = 0;
        float G = 0;
        foreach (GeoInfo geoInfo in GeoInfos)
        {
            center_X += geoInfo.Position.x * geoInfo.Size.x * geoInfo.Size.x;
            center_Y += geoInfo.Position.y * geoInfo.Size.x * geoInfo.Size.x;
            G += geoInfo.Size.x;
        }

        center_X /= G;
        center_Y /= G;

        foreach (GeoInfo gi in GeoInfos)
        {
            gi.Position -= new Vector2(center_X, center_Y);
        }

        int minSortingOrder = 999000;

        foreach (GeoInfo gi in GeoInfos)
        {
            minSortingOrder = Mathf.Min(minSortingOrder, gi.SortingOrder);
        }

        foreach (GeoInfo gi in GeoInfos)
        {
            gi.SortingOrder -= minSortingOrder;
        }
    }
}