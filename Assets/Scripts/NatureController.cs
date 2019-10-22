using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using instinctai.usr.behaviours;
using UnityEngine;
using UnityEngine.UI;

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

    public float NutritionRatio = 0.6f;
    public int SpeciesCountUpperLimit = 50;

    public int UpdateNFrames = 10;

    public void RecreateAllSpecies()
    {
        ClearAll();

        foreach (KeyValuePair<string, GeoGroupInfo> kv in AllGeoGroupInfo)
        {
            Species species = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.Species].AllocateGameObject<Species>(transform);
            species.name = kv.Key + "_Species";
            species.MyGeoGroupInfo = kv.Value;
            AllSpecies.Add(kv.Key, species);
        }
    }

    public void ClearAll()
    {
        foreach (KeyValuePair<string, Species> kv in AllSpecies)
        {
            foreach (Creature c in kv.Value.Creatures.ToList())
            {
                DestroyCreature(c);
            }

            kv.Value.Creatures.Clear();
            kv.Value.PoolRecycle();
        }

        AllSpecies.Clear();
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
        float distance = UnityEngine.Random.Range(0, 5.2f - radius / 1000f);
        float angle = UnityEngine.Random.Range(0, 360f);
        float x = Mathf.Sin(angle * Mathf.Deg2Rad) * distance;
        float y = Mathf.Cos(angle * Mathf.Deg2Rad) * distance;
        return new Vector2(x, y);
    }

    public Color[] ColorSet;

    public Dictionary<string, Species> AllSpecies = new Dictionary<string, Species>();
    public Dictionary<string, GeoGroupInfo> AllGeoGroupInfo = new Dictionary<string, GeoGroupInfo>();

    public void DestroyCreature(Creature creature)
    {
        creature.Destroyed = true;
        foreach (KeyValuePair<string, Species> kv in AllSpecies)
        {
            kv.Value.Creatures.Remove(creature);
        }

        creature.transform.position = Vector2.one * -2000;
        creature.ResetTree();
        creature.Valid(false);
        creature.Rigidbody2D.simulated = false;
        creature.Rigidbody2D.velocity = Vector2.zero;
        StartCoroutine(Co_Destroy(creature));
    }

    IEnumerator Co_Destroy(Creature creature)
    {
        yield return new WaitForSeconds(1f);
        try
        {
            DestroyImmediate(creature.gameObject);
        }
        catch
        {
        }
    }

    public Creature FindNearestPredator(Creature callingDot)
    {
        Creature nearCreature = null;
        float distance = 9999f;
        foreach (KeyValuePair<string, Species> kv in AllSpecies)
        {
            if (kv.Key.Equals(callingDot.M_SpeciesName))
            {
                foreach (Creature creature in kv.Value.Creatures)
                {
                    if (callingDot.IsPredatorOf(callingDot))
                    {
                        float tempDist = Vector2.Distance(callingDot.transform.position, callingDot.transform.position);
                        if (tempDist < distance)
                        {
                            distance = tempDist;
                            nearCreature = callingDot;
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
            return nearCreature;
        }
    }

    public Creature FindNearestPrey(Creature callingCreature)
    {
        Creature nearCreature = null;
        float distance = 9999f;
        foreach (KeyValuePair<string, Species> kv in AllSpecies)
        {
            if (kv.Key != callingCreature.M_SpeciesName)
            {
                foreach (Creature creature in kv.Value.Creatures)
                {
                    if (creature.IsPreyOf(callingCreature))
                    {
                        float tempDist = Vector2.Distance(creature.transform.position, callingCreature.transform.position);
                        if (tempDist < distance)
                        {
                            distance = tempDist;
                            nearCreature = creature;
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
            return nearCreature;
        }
    }

    public void RestartSimulate()
    {
        RecreateAllSpecies();
        foreach (KeyValuePair<string, Species> kv in AllSpecies)
        {
            kv.Value.SpawnCreatures(10);
        }
    }
}