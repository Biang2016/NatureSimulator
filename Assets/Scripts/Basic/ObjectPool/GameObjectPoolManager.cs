using System.Collections.Generic;
using UnityEngine;

public class GameObjectPoolManager : MonoSingleton<GameObjectPoolManager>
{
    private GameObjectPoolManager()
    {
    }

    public enum PrefabNames
    {
        BlackDot,
        YellowDot,
        RedDot,
        GreenDot,
        Species,
        GeoButton,
        GeoElement,
        GeoGroup,
        ButtonOfSpecies,
        PaletteColorButton,
    }
    

    public Dictionary<PrefabNames, int> PoolConfigs = new Dictionary<PrefabNames, int>
    {
        {PrefabNames.Species, 4},
        {PrefabNames.GeoButton, 5},
        {PrefabNames.GeoElement, 5},
        {PrefabNames.GeoGroup, 5},
        {PrefabNames.ButtonOfSpecies, 5},
        {PrefabNames.PaletteColorButton, 5},
    };

    public Dictionary<PrefabNames, int> PoolWarmUpDict = new Dictionary<PrefabNames, int>
    {
    };

    public Dictionary<PrefabNames, GameObjectPool> PoolDict = new Dictionary<PrefabNames, GameObjectPool>();

    void Awake()
    {
        PrefabManager.Instance.LoadPrefabs_Editor();
        
        foreach (KeyValuePair<PrefabNames, int> kv in PoolConfigs)
        {
            string prefabName = kv.Key.ToString();
            GameObject go = new GameObject("Pool_" + prefabName);
            GameObjectPool pool = go.AddComponent<GameObjectPool>();
            PoolDict.Add(kv.Key, pool);
            GameObject go_Prefab = PrefabManager.Instance.GetPrefab(prefabName);
            PoolObject po = go_Prefab.GetComponent<PoolObject>();
            pool.Initiate(po, kv.Value);
            pool.transform.SetParent(transform);
        }
    }

    public void OptimizeAllGameObjectPools()
    {
        foreach (KeyValuePair<PrefabNames, GameObjectPool> kv in PoolDict)
        {
            kv.Value.OptimizePool();
        }
    }
}