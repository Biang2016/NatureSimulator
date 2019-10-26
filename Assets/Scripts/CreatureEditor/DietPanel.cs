using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DietPanel : MonoBehaviour
{
    [SerializeField] private Transform DietContainer;
    [SerializeField] private Dictionary<string, DietToggle> DietToggles = new Dictionary<string, DietToggle>();
    [SerializeField] private Button CloseButton;

    public void OnCloseButtonClick()
    {
        gameObject.SetActive(false);
    }

    public void Refresh(GeoGroupInfo hostGGI)
    {
        foreach (KeyValuePair<string, DietToggle> kv in DietToggles)
        {
            kv.Value.PoolRecycle();
        }

        DietToggles.Clear();

        foreach (KeyValuePair<string, DietToggle> kv in PredatorToggles)
        {
            kv.Value.PoolRecycle();
        }

        PredatorToggles.Clear();

        foreach (string dietName in NatureController.Instance.AllGeoGroupInfo.Keys)
        {
            if (dietName == hostGGI.Name) continue;
            DietToggle dt = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.DietToggle].AllocateGameObject<DietToggle>(DietContainer);
            DietToggles.Add(dietName, dt);
            dt.Initialize(hostGGI, NatureController.Instance.AllGeoGroupInfo[dietName], DietToggle.Types.TargetIsPrey);
        }

        foreach (string predatorName in NatureController.Instance.AllGeoGroupInfo.Keys)
        {
            if (predatorName == hostGGI.Name) continue;
            DietToggle dt = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.DietToggle].AllocateGameObject<DietToggle>(PredatorContainer);
            PredatorToggles.Add(predatorName, dt);
            dt.Initialize(hostGGI, NatureController.Instance.AllGeoGroupInfo[predatorName], DietToggle.Types.TargetIsPredator);
        }
    }

    [SerializeField] private Transform PredatorContainer;
    [SerializeField] private Dictionary<string, DietToggle> PredatorToggles = new Dictionary<string, DietToggle>();
}