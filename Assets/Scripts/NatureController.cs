using System;
using System.Collections;
using System.Collections.Generic;
using instinctai.usr.behaviours;
using UnityEngine;
using Random = System.Random;

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

    public int Blue_NumberEachSpecies = 10;
    public int Green_NumberEachSpecies = 10;
    public int Red_NumberEachSpecies = 10;
    public int Yellow_NumberEachSpecies = 10;
    public float NutritionRatio = 0.6f;
    public int SpeciesCountUpperLimit = 50;

    void Start()
    {
        //GetCirclePoints(540f, 128);
        foreach (string enumName in Enum.GetNames(typeof(Species.SpeciesTypes)))
        {
            Species species = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.Species].AllocateGameObject<Species>(transform);
            Species.SpeciesTypes sp = (Species.SpeciesTypes) Enum.Parse(typeof(Species.SpeciesTypes), enumName);
            species.Init(sp);
            AllSpecies.Add(sp, species);
            species.name = enumName + "_Species";
        }

        AllSpecies[Species.SpeciesTypes.Black].SpawnDots(Blue_NumberEachSpecies);
        AllSpecies[Species.SpeciesTypes.Green].SpawnDots(Green_NumberEachSpecies);
        AllSpecies[Species.SpeciesTypes.Red].SpawnDots(Red_NumberEachSpecies);
        AllSpecies[Species.SpeciesTypes.Yellow].SpawnDots(Yellow_NumberEachSpecies);
        //foreach (KeyValuePair<Species.SpeciesTypes, Species> kv in AllSpecies)
        //{
        //    kv.Value.SpawnDots(NumberEachSpecies);
        //}
    }

    public void GetCirclePoints(float radius, int numberCount)
    {
        string result = "";
        float angle = 360f / numberCount;
        for (int i = 0; i < numberCount; i++)
        {
            float x = radius * Mathf.Sin(Mathf.Deg2Rad * i * angle);
            float y = radius * Mathf.Cos(Mathf.Deg2Rad * i * angle);
            result += "  - {x: " + x + ", y: " + y + "}\n";
        }

        Debug.Log(result);
    }

    public static Vector2 GetRandomPos(float radius = 0f)
    {
        float distance = UnityEngine.Random.Range(0, 540f - radius);
        float angle = UnityEngine.Random.Range(0, 360f);
        float x = Mathf.Sin(angle * Mathf.Deg2Rad) * distance;
        float y = Mathf.Cos(angle * Mathf.Deg2Rad) * distance;
        return new Vector2(x, y);
    }

    public Color[] ColorSet;

    public Dictionary<Species.SpeciesTypes, Species> AllSpecies = new Dictionary<Species.SpeciesTypes, Species>();

    public void DestroyDot(Dot dot)
    {
        dot.Destroyed = true;
        foreach (KeyValuePair<Species.SpeciesTypes, Species> kv in AllSpecies)
        {
            kv.Value.Dots.Remove(dot);
        }

        dot.transform.position = Vector2.one * -2000;
        dot.Collider2D.enabled = false;
        dot.ResetTree();
        dot.Valid(false);
        dot.Rigidbody2D.simulated = false;
        dot.Rigidbody2D.velocity = Vector2.zero;
        StartCoroutine(Co_Destroy(dot));
    }

    IEnumerator Co_Destroy(Dot dot)
    {
        yield return new WaitForSeconds(1f);
        try
        {
            DestroyImmediate(dot.gameObject);
        }
        catch
        {
        }
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