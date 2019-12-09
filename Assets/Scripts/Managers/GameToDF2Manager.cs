using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Syrus.Plugins.DFV2Client;

public class GameToDF2Manager : MonoSingleton<GameToDF2Manager>
{
    public void OnAddSpeciesToEntity(List<string> speciesNames)
    {
        DF2Entity[] entities = new DF2Entity[speciesNames.Count];
        int i = 0;
        foreach (string s in speciesNames)
        {
            entities[i] = new DF2Entity(s, s);
            i++;
        }

        DF2EntityType names = new DF2EntityType("UserCreatedSpecies", DF2EntityType.DF2EntityOverrideMode.ENTITY_OVERRIDE_MODE_SUPPLEMENT, entities);
        DF2Client.Instance.Client.AddEntityType(names, DF2Client.Instance.SessionName);
    }
}