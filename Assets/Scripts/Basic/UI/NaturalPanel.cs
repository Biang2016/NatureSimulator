using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NaturalPanel : BaseUIForm
{
    private List<ButtonOfSpecies> ButtonOfSpeciesList = new List<ButtonOfSpecies>();

    void Awake()
    {
        UIType = new UIType();
        UIType.IsClickElsewhereClose = false;
        UIType.IsESCClose = false;
    }

    public void Initialize()
    {
        foreach (ButtonOfSpecies b in ButtonOfSpeciesList)
        {
            b.PoolRecycle();
        }

        ButtonOfSpeciesList.Clear();
        foreach (KeyValuePair<string, GeoGroupInfo> kv in NatureController.Instance.AllGeoGroupInfo)
        {
            ButtonOfSpecies bos = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ButtonOfSpecies].AllocateGameObject<ButtonOfSpecies>(transform);
            ButtonOfSpeciesList.Add(bos);
            bos.Initialize(kv.Value);
        }
    }
}