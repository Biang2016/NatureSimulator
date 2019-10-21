using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeoGroup : PoolObject
{
    public List<GeoElement> AllGeos = new List<GeoElement>();
    public GeoGroupInfo GeoGroupInfo = new GeoGroupInfo();

    public override void PoolRecycle()
    {
        base.PoolRecycle();
        foreach (GeoElement ge in AllGeos)
        {
            ge.PoolRecycle();
        }

        AllGeos.Clear();
    }

    public void Initialize(GeoGroupInfo geoGroupInfo)
    {
        GeoGroupInfo = geoGroupInfo;
        GeoGroupInfo.ResetCenterAndSortingOrder();
        foreach (GeoInfo gi in GeoGroupInfo.GeoInfos)
        {
            GeoElement ge = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.GeoElement].AllocateGameObject<GeoElement>(transform);
            ge.OnSelected = false;
            ge.Initialize(gi.GeoType, gi.Size, gi.Color, gi.SortingOrder);
            AllGeos.Add(ge);
        }
    }

    void Update()
    {
    }
}