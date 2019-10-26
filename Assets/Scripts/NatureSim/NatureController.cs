﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using instinctai.usr.behaviours;
using UnityEngine;

public class NatureController : MonoSingleton<NatureController>
{
    public float EatSizeThreshold = 0.5f;
    public float EscapingDistanceFactor = 0.01f;
    public float ChasingPreyDistanceFactor = 0.015f;
    public float NormalSpeed = 100f;
    public float EscapingSpeedFactor = 1.2f;
    public float ChasingSpeedFactor = 1.1f;
    public float WanderingSpeedFactor = 0.5f;
    public float FindingMateSpeedFactor = 1.0f;
    public float MateTimeInterval = 2f;

    public float NutritionRatio = 0.6f;

    public int UpdateNFrames = 10;

    public void RecreateAllSpecies()
    {
        ClearAll();
        NaturalPanel np = UIManager.Instance.GetBaseUIForm<NaturalPanel>();
        np.RefreshButtonSelected();
        foreach (string ggiName in AllSelectedGeoGroupInfoNames)
        {
            GeoGroupInfo ggi = AllGeoGroupInfo[ggiName];
            Species species = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.Species].AllocateGameObject<Species>(transform);
            species.name = ggi.Name + "_Species";
            species.MyGeoGroupInfo = ggi;
            AllSpecies.Add(ggi.Name, species);
        }

        np.Initialize();
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
        AllSelectedGeoGroupInfoNames.Clear();
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
        float distance = UnityEngine.Random.Range(0, 5f - radius / 1000f);
        float angle = UnityEngine.Random.Range(0, 360f);
        float x = Mathf.Sin(angle * Mathf.Deg2Rad) * distance;
        float y = Mathf.Cos(angle * Mathf.Deg2Rad) * distance;
        return new Vector2(x, y);
    }

    public Dictionary<string, Species> AllSpecies = new Dictionary<string, Species>();
    public Dictionary<string, GeoGroupInfo> AllGeoGroupInfo = new Dictionary<string, GeoGroupInfo>();
    public HashSet<string> AllSelectedGeoGroupInfoNames = new HashSet<string>();

    public float WholeMassInNature;

    void Update()
    {
        WholeMassInNature = 0;
        foreach (KeyValuePair<string, Species> kv in AllSpecies)
        {
            WholeMassInNature += kv.Value.WholeMassInSpecies;
        }
    }

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

    public Creature FindNearestPredator(Creature callingCreature)
    {
        Creature nearCreature = null;
        float distance = 9999f;
        foreach (KeyValuePair<string, Species> kv in AllSpecies)
        {
            if (!kv.Key.Equals(callingCreature.M_SpeciesName))
            {
                foreach (Creature creature in kv.Value.Creatures)
                {
                    if (creature.IsPredatorOf(callingCreature))
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

        if (distance > callingCreature.MyGeoGroupInfo.Vision * EscapingDistanceFactor)
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
            if (!kv.Key.Equals(callingCreature.M_SpeciesName))
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

        if (distance > callingCreature.MyGeoGroupInfo.Vision * ChasingPreyDistanceFactor)
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
            kv.Value.SpawnCreatures();
        }
    }

    private string XMLPath = Application.streamingAssetsPath + "/CreatureLibrary.xml";

    public void LoadAllSpeciesFromXML()
    {
        AllGeoGroupInfo.Clear();
        AllSelectedGeoGroupInfoNames.Clear();
        UIManager.Instance.GetBaseUIForm<NaturalPanel>().Initialize();

        string text;
        using (StreamReader sr = new StreamReader(XMLPath))
        {
            text = sr.ReadToEnd();
        }

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(text);
        XmlElement allCreaturesEle = doc.DocumentElement;

        for (int i = 0; i < allCreaturesEle.ChildNodes.Count; i++)
        {
            XmlNode creature_ele = allCreaturesEle.ChildNodes[i];
            GeoGroupInfo ggi = GeoGroupInfo.GenerateGeoGroupInfoFromXML(creature_ele);
            AllGeoGroupInfo.Add(ggi.Name, ggi);
            ggi.RefreshInfo();
        }

        RecreateAllSpecies();
    }

    public void OnSaveAllCreatures()
    {
        string text;
        using (StreamReader sr = new StreamReader(XMLPath))
        {
            text = sr.ReadToEnd();
        }

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(text);

        XmlElement allCreaturesEle = doc.DocumentElement;
        allCreaturesEle.RemoveAll();

        foreach (KeyValuePair<string, GeoGroupInfo> kv in AllGeoGroupInfo)
        {
            kv.Value.ExportToXML(allCreaturesEle);
        }

        using (StreamWriter sw = new StreamWriter(XMLPath))
        {
            doc.Save(sw);
        }
    }
}