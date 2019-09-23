using System;
using System.Collections.Generic;
using instinctai.usr.behaviours;
using UnityEngine;

public class NatureController : MonoSingleton<NatureController>
{
    public float EatSizeThreshold = 0.5f;
    public float EscapingDistanceThreshold = 400f;
    public float ChasingPreyDistanceThreshold = 450f;
    public float NormalSpeed = 100f;
    public float EscapingSpeedFactor = 1.2f;
    public float ChasingSpeedFactor = 1.1f;
    public float WanderingSpeedFactor = 0.5f;
    public float FindingMateSpeedFactor = 1.0f;

    public float SizeRangeUp = 100f;
    public float SizeRangeLow = 20f;
    public int NumberEachSpecies = 10;
    public float NutritionRatio = 0.6f;

    void Start()
    {
        foreach (string enumName in Enum.GetNames(typeof(Species.SpeciesTypes)))
        {
            Species species = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.Species].AllocateGameObject<Species>(transform);
            Species.SpeciesTypes sp = (Species.SpeciesTypes) Enum.Parse(typeof(Species.SpeciesTypes), enumName);
            species.Init(sp);
            AllSpecies.Add(sp, species);
            species.name = enumName + "_Species";
        }

        foreach (KeyValuePair<Species.SpeciesTypes, Species> kv in AllSpecies)
        {
            kv.Value.SpawnDots(NumberEachSpecies);
        }
    }

    public Color[] ColorSet;

    public Dictionary<Species.SpeciesTypes, Species> AllSpecies = new Dictionary<Species.SpeciesTypes, Species>();

    public void DestoryDot(Dot dot)
    {
        AllSpecies[dot.M_SpeciesType].Dots.Remove(dot);
        dot.transform.position = Vector2.one * -3000;
        dot.Collider2D.enabled = false;
        Destroy(dot);
    }

    public Dot FindNearestPredator(Dot callingDot)
    {
        Dot nearDot = null;
        float distance = 9999f;
        foreach (KeyValuePair<Species.SpeciesTypes, Species> kv in AllSpecies)
        {
            if (kv.Value.M_SpeciesType != callingDot.M_SpeciesType)
            {
                foreach (Dot dot in kv.Value.Dots)
                {
                    if (dot.IsPredatorOf(callingDot))
                    {
                        float tempDist = Vector2.Distance(dot.transform.position, callingDot.transform.position);
                        if (tempDist < distance)
                        {
                            distance = tempDist;
                            nearDot = dot;
                        }
                    }
                }
            }
        }

        if (distance > EscapingDistanceThreshold)
        {
            return null;
        }
        else
        {
            return nearDot;
        }
    }

    public Dot FindNearestPrey(Dot callingDot)
    {
        Dot nearDot = null;
        float distance = 9999f;
        foreach (KeyValuePair<Species.SpeciesTypes, Species> kv in AllSpecies)
        {
            if (kv.Value.M_SpeciesType != callingDot.M_SpeciesType)
            {
                foreach (Dot dot in kv.Value.Dots)
                {
                    if (dot.IsPreyOf(callingDot))
                    {
                        float tempDist = Vector2.Distance(dot.transform.position, callingDot.transform.position);
                        if (tempDist < distance)
                        {
                            distance = tempDist;
                            nearDot = dot;
                        }
                    }
                }
            }
        }

        if (distance > ChasingPreyDistanceThreshold)
        {
            return null;
        }
        else
        {
            return nearDot;
        }
    }
}