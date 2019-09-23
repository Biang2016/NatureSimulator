using System.Collections.Generic;
using instinctai.usr.behaviours;
using UnityEngine;

public class Species : PoolObject
{
    public List<Dot> Dots = new List<Dot>();

    public void Init(SpeciesTypes speciesType)
    {
        M_SpeciesType = speciesType;
    }

    public SpeciesTypes M_SpeciesType;

    public void SpawnDots(int number)
    {
        for (int i = 0; i < number; i++)
        {
            SpawnDot(Random.Range(NatureController.Instance.SizeRangeLow, NatureController.Instance.SizeRangeUp), new Vector2(Random.Range(-860f, 860), Random.Range(-440f, 440)));
        }
    }

    public void SpawnDot(float size, Vector2 pos)
    {
        GameObject prefab = (GameObject) Resources.Load(PrefabNamesForSpecies[M_SpeciesType].ToString());
        GameObject dotGO = Instantiate(prefab);
        dotGO.transform.SetParent(transform);
        Dot dot = dotGO.GetComponent<Dot>();
        dot.Init(size, this);
        dot.transform.position = pos;
        Dots.Add(dot);
    }

    public enum SpeciesTypes
    {
        Black = 0,
        Red = 1,
        Yellow = 2,
        Green = 3,
    }

    public static Dictionary<Species.SpeciesTypes, GameObjectPoolManager.PrefabNames> PrefabNamesForSpecies = new Dictionary<SpeciesTypes, GameObjectPoolManager.PrefabNames>
    {
        {SpeciesTypes.Black, GameObjectPoolManager.PrefabNames.BlackDot},
        {SpeciesTypes.Red, GameObjectPoolManager.PrefabNames.RedDot},
        {SpeciesTypes.Yellow, GameObjectPoolManager.PrefabNames.YellowDot},
        {SpeciesTypes.Green, GameObjectPoolManager.PrefabNames.GreenDot},
    };

    public Dot FindNearestMate(Dot callingDot)
    {
        float distance = 99999f;
        Dot mateDot = null;
        foreach (Dot dot in Dots)
        {
            if (dot != callingDot && dot.IsMateOf(callingDot))
            {
                float tempDic = Vector2.Distance(dot.transform.position, callingDot.transform.position);
                if (distance < tempDic)
                {
                    mateDot = dot;
                    distance = tempDic;
                }
            }
        }

        return mateDot;
    }
}