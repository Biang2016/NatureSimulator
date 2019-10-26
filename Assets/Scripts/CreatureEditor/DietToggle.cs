using UnityEngine;
using UnityEngine.UI;

public class DietToggle : PoolObject
{
    [SerializeField] private Toggle Toggle;
    [SerializeField] private Text SpeciesName;

    public void Initialize(GeoGroupInfo hostGGI, GeoGroupInfo targetGGI, Types type)
    {
        SpeciesName.text = targetGGI.Name;

        Toggle.onValueChanged.RemoveAllListeners();
        if (type == Types.TargetIsPrey)
        {
            Toggle.isOn = hostGGI.Diets.Contains(targetGGI.Name);
        }

        else if (type == Types.TargetIsPredator)
        {
            Toggle.isOn = hostGGI.Predators.Contains(targetGGI.Name);
        }

        Toggle.onValueChanged.AddListener(delegate(bool toggleBool)
        {
            if (type == Types.TargetIsPrey)
            {
                if (toggleBool)
                {
                    hostGGI.Diets.Add(targetGGI.Name);
                    targetGGI.Predators.Add(hostGGI.Name);
                }
                else
                {
                    hostGGI.Diets.Remove(targetGGI.Name);
                    targetGGI.Predators.Remove(hostGGI.Name);
                }
            }
            else if (type == Types.TargetIsPredator)
            {
                if (toggleBool)
                {
                    hostGGI.Predators.Add(targetGGI.Name);
                    targetGGI.Diets.Add(hostGGI.Name);
                }
                else
                {
                    hostGGI.Predators.Remove(targetGGI.Name);
                    targetGGI.Diets.Remove(hostGGI.Name);
                }
            }
        });
    }

    public enum Types
    {
        TargetIsPrey,
        TargetIsPredator
    }
}