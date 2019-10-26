﻿using System.Collections;
using System.Collections.Generic;
using instinctai.usr.behaviours;
using UnityEngine;

public class Species : PoolObject
{
    public List<Creature> Creatures = new List<Creature>();

    public GeoGroupInfo MyGeoGroupInfo;

    public void SpawnCreatures()
    {
        StartCoroutine(Co_SpawnDots());
    }

    IEnumerator Co_SpawnDots()
    {
        for (int i = 0; i < MyGeoGroupInfo.StartNumber; i++)
        {
            SpawnCreatures(0, NatureController.GetRandomPos(), true);
            yield return new WaitForEndOfFrame();
        }
    }

    public void SpawnCreatures(float size, Vector2 pos, bool randomSize = false)
    {
        GameObject prefab = (GameObject) Resources.Load("Creature");
        GameObject CreatureGO = Instantiate(prefab);
        CreatureGO.transform.SetParent(transform);
        Creature creature = CreatureGO.GetComponent<Creature>();
        creature.Init(this, MyGeoGroupInfo, size, randomSize);
        creature.transform.position = pos;
        Creatures.Add(creature);
    }

    public Creature FindNearestMate(Creature callingCreature)
    {
        float distance = 99999f;
        Creature mateCreature = null;
        foreach (Creature creature in Creatures)
        {
            if (creature != callingCreature && creature.IsMateOf(callingCreature))
            {
                float tempDic = Vector2.Distance(creature.transform.position, callingCreature.transform.position);
                if (tempDic < distance)
                {
                    mateCreature = creature;
                    distance = tempDic;
                }
            }
        }

        return mateCreature;
    }

    public float WholeMassInSpecies = 0;

    void Update()
    {
        WholeMassInSpecies = 0;
        foreach (Creature c in Creatures)
        {
            WholeMassInSpecies += c.MyGeoGroupInfo.Mass;
        }
    }
}