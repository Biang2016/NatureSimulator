using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ButtonOfSpecies : PoolObject
{
    [SerializeField] private Button Button;
    public Toggle Toggle;
    [SerializeField] private Text Text;
    [SerializeField] private Text CountText;
    [SerializeField] private Text RatioText;
    [SerializeField] private Slider RatioSlider;

    public GeoGroupInfo GGI;

    public void Initialize(GeoGroupInfo ggi)
    {
        Button.image.color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        GGI = ggi;
        Text.text = ggi.Name;
        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(delegate
        {
            NatureController.Instance.ClearAll();
            CreatureEditorPanel cep = UIManager.Instance.ShowUIForms<CreatureEditorPanel>();
            cep.Initialize(GGI, true);
            UIManager.Instance.CloseUIForm<NaturalPanel>();
            UIManager.Instance.GetBaseUIForm<NaturalPanel>().isSimulationStart = false;
        });
    }

    public void OnRightClick()
    {
        NaturalPanel np = UIManager.Instance.GetBaseUIForm<NaturalPanel>();
        np.ButtonOfSpeciesList.Remove(this);
        NatureController.Instance.ClearAll();
        NatureController.Instance.AllGeoGroupInfo.Remove(Text.text);
        NatureController.Instance.AllSelectedGeoGroupInfoNames.Remove(Text.text);
        NatureController.Instance.AllSpecies.Remove(Text.text);
        foreach (KeyValuePair<string, GeoGroupInfo> kv in NatureController.Instance.AllGeoGroupInfo)
        {
            kv.Value.Diets.Remove(Text.text);
            kv.Value.Predators.Remove(Text.text);
        }

        UIManager.Instance.GetBaseUIForm<NaturalPanel>().RestartSimulateButtonClick();
        PoolRecycle();
    }

    void Update()
    {
        if (UIManager.Instance.GetBaseUIForm<NaturalPanel>().isSimulationStart && NatureController.Instance.AllSelectedGeoGroupInfoNames.Contains(GGI.Name))
        {
            float ratio = NatureController.Instance.AllSpecies[GGI.Name].WholeMassInSpecies / NatureController.Instance.WholeMassInNature;
            CountText.text = "x" + NatureController.Instance.AllSpecies[GGI.Name].Creatures.Count.ToString();
            RatioText.text = Math.Round(ratio * 100f, 1) + "%";
            RatioSlider.value = ratio;
        }
        else
        {
            CountText.text = "";
            RatioText.text = "";
            RatioSlider.value = 0f;
        }
    }
}