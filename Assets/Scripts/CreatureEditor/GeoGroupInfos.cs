using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

[Serializable]
public class GeoGroupInfo
{
    public List<GeoInfo> GeoInfos = new List<GeoInfo>();
    public string Name;

    public float Life;
    public float Speed;
    public float Damage;
    public float Vision;
    public float GeneralSize;
    public int FertilityRate = 100;
    public int OffspringSizePercent = 50;
    public int MatureSizePercent = 70;
    public int MinSizePercent = 35;
    public int MaxSizePercent = 130;
    public int StartNumber = 10;
    public int MaxNumber = 100;
    public float GrowUpRate = 0.001f;
    public float Mass;

    public float MaxSize => MaxSizePercent / 100f * GeneralSize;
    public float MinSize => MinSizePercent / 100f * GeneralSize;
    public float MateMatureSizeThreshold => MatureSizePercent / 100f * GeneralSize;
    public float ColliderRadius => GeneralSize / ScaleOfAllCreatures / 50000f;

    public Vector2 Center;
    public HashSet<string> Diets = new HashSet<string>();
    public HashSet<string> Predators = new HashSet<string>();

    public void ResetCenterAndSortingOrder()
    {
        int minSortingOrder = 999000;

        foreach (GeoInfo gi in GeoInfos)
        {
            minSortingOrder = Mathf.Min(minSortingOrder, gi.SortingOrder);
        }

        minSortingOrder -= 2;
        foreach (GeoInfo gi in GeoInfos)
        {
            gi.SortingOrder -= minSortingOrder;
        }
    }

    public void RefreshInfo()
    {
        Life = 0;
        Speed = 0;
        Damage = 0;
        Vision = 0;
        Mass = 0;
        GeneralSize = 0;
        Center = Vector3.zero;
        if (GeoInfos.Count == 0)
        {
            return;
        }

        float XMin = 9999;
        float XMax = -9999;
        float YMin = 9999;
        float YMax = -9999;
        foreach (GeoInfo gi in GeoInfos)
        {
            XMin = Mathf.Min(XMin, gi.Position.x - gi.Size.x * 10000f);
            XMax = Mathf.Max(XMax, gi.Position.x + gi.Size.x * 10000f);
            YMin = Mathf.Min(YMin, gi.Position.y - gi.Size.y * 10000f);
            YMax = Mathf.Max(YMax, gi.Position.y + gi.Size.y * 10000f);
            float area = gi.Size.x * gi.Size.x * 3000;
            Mass += area;
            switch (gi.GeoType)
            {
                case GeoTypes.Circle:
                {
                    Life -= 3 * area;
                    Speed -= 3 * area;
                    Damage -= 5 * area;
                    Vision += 20;
                    break;
                }
                case GeoTypes.Square:
                {
                    Life += 5 * area;
                    Speed += Mathf.Max(5, 10 * area);
                    Damage -= 5 * area;
                    Vision -= 3 * area;
                    break;
                }
                case GeoTypes.Triangle:
                {
                    Life -= 5 * area;
                    Speed -= 1 * area;
                    Damage += 10;
                    Vision -= 2 * area;
                    break;
                }
                case GeoTypes.Hexagon:
                {
                    Life += 20 * area;
                    Speed -= 4 * area;
                    Damage -= 2 * area;
                    Vision -= 3 * area;
                    break;
                }
            }
        }

        Life = Mathf.Max(5f, Life);
        Speed = Mathf.Max(5, Speed);
        Damage = Mathf.Max(5, Damage);
        Vision = Mathf.Max(30, Vision);

        GeneralSize = Mathf.Sqrt((XMax - XMin) * (XMax - XMin) + (YMax - YMin) * (YMax - YMin)) * ScaleOfAllCreatures;
        Center = new Vector3((XMax + XMin) / 2f, (YMax + YMin) / 2f);
    }

    public const float ScaleOfAllCreatures = 0.5f;

    public void ExportToXML(XmlElement allCreatureElement)
    {
        XmlDocument doc = allCreatureElement.OwnerDocument;

        //XmlElement old_node = null;
        //foreach (XmlElement creature_node in allCreatureElement.ChildNodes)
        //{
        //    if (creature_node.Attributes["name"].Value.Equals(Name))
        //    {
        //        old_node = creature_node;
        //    }
        //}

        //if (old_node != null)
        //{
        //    allCreatureElement.RemoveChild(old_node);
        //}

        XmlElement creature_ele = doc.CreateElement("Creature");
        allCreatureElement.AppendChild(creature_ele);
        creature_ele.SetAttribute("name", Name);

        XmlElement baseInfo_ele = doc.CreateElement("CreatureInfo");
        creature_ele.AppendChild(baseInfo_ele);
        baseInfo_ele.SetAttribute("FertilityRate", FertilityRate.ToString());
        baseInfo_ele.SetAttribute("OffspringSizePercent", OffspringSizePercent.ToString());
        baseInfo_ele.SetAttribute("MatureSizePercent", MatureSizePercent.ToString());
        baseInfo_ele.SetAttribute("MinSizePercent", MinSizePercent.ToString());
        baseInfo_ele.SetAttribute("MaxSizePercent", MaxSizePercent.ToString());
        baseInfo_ele.SetAttribute("GrowUpRate", GrowUpRate.ToString());
        baseInfo_ele.SetAttribute("StartNumber", StartNumber.ToString());
        baseInfo_ele.SetAttribute("MaxNumber", MaxNumber.ToString());

        List<string> removeDiets = new List<string>();
        foreach (string d in Diets)
        {
            if (!NatureController.Instance.AllGeoGroupInfo.ContainsKey(d))
            {
                removeDiets.Add(d);
            }
        }

        foreach (string d in removeDiets)
        {
            Diets.Remove(d);
        }

        List<string> removePredators = new List<string>();
        foreach (string p in Predators)
        {
            if (!NatureController.Instance.AllGeoGroupInfo.ContainsKey(p))
            {
                removePredators.Add(p);
            }
        }

        foreach (string d in removePredators)
        {
            Predators.Remove(d);
        }

        string dietString = string.Join(",", Diets);
        baseInfo_ele.SetAttribute("Diets", dietString);
        string predatorString = string.Join(",", Predators);
        baseInfo_ele.SetAttribute("Predators", predatorString);

        XmlElement geoInfos_ele = doc.CreateElement("GeoInfos");
        creature_ele.AppendChild(geoInfos_ele);
        foreach (GeoInfo gi in GeoInfos)
        {
            XmlElement geoInfo_ele = doc.CreateElement("GeoInfo");
            geoInfos_ele.AppendChild(geoInfo_ele);
            gi.ExportToXML(geoInfo_ele);
        }
    }

    public static GeoGroupInfo GenerateGeoGroupInfoFromXML(XmlNode creatureElement)
    {
        GeoGroupInfo ggi = new GeoGroupInfo();
        ggi.Name = creatureElement.Attributes["name"].Value;

        XmlNode creatureInfo = creatureElement.ChildNodes[0];

        ggi.FertilityRate = int.Parse(creatureInfo.Attributes["FertilityRate"].Value);
        ggi.OffspringSizePercent = int.Parse(creatureInfo.Attributes["OffspringSizePercent"].Value);
        ggi.MatureSizePercent = int.Parse(creatureInfo.Attributes["MatureSizePercent"].Value);
        ggi.MinSizePercent = int.Parse(creatureInfo.Attributes["MinSizePercent"].Value);
        ggi.MaxSizePercent = int.Parse(creatureInfo.Attributes["MaxSizePercent"].Value);
        ggi.GrowUpRate = float.Parse(creatureInfo.Attributes["GrowUpRate"].Value);
        ggi.StartNumber = int.Parse(creatureInfo.Attributes["StartNumber"].Value);
        ggi.MaxNumber = int.Parse(creatureInfo.Attributes["MaxNumber"].Value);

        List<string> diets = creatureInfo.Attributes["Diets"].Value.Split(',').ToList();
        List<string> predators = creatureInfo.Attributes["Predators"].Value.Split(',').ToList();

        foreach (string diet in diets)
        {
            ggi.Diets.Add(diet);
        }

        foreach (string p in predators)
        {
            ggi.Predators.Add(p);
        }

        XmlNode geoInfos_element = creatureElement.ChildNodes[1];
        for (int i = 0; i < geoInfos_element.ChildNodes.Count; i++)
        {
            XmlNode geoInfo_element = geoInfos_element.ChildNodes[i];
            GeoInfo gi = GeoInfo.GenerateGeoInfoFromXML(geoInfo_element);
            ggi.GeoInfos.Add(gi);
        }

        return ggi;
    }
}