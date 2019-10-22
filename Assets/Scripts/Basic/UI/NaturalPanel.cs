using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class NaturalPanel : BaseUIForm
{
    [SerializeField] private Transform LeftPanel;
    public List<ButtonOfSpecies> ButtonOfSpeciesList = new List<ButtonOfSpecies>();

    void Awake()
    {
        SimulateSpeed = 1.0f;
        UIType = new UIType();
        UIType.IsClickElsewhereClose = false;
        UIType.IsESCClose = false;
    }

    public void Initialize()
    {
        foreach (ButtonOfSpecies b in ButtonOfSpeciesList)
        {
            b.PoolRecycle();
        }

        ButtonOfSpeciesList.Clear();
        foreach (KeyValuePair<string, GeoGroupInfo> kv in NatureController.Instance.AllGeoGroupInfo)
        {
            ButtonOfSpecies bos = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ButtonOfSpecies].AllocateGameObject<ButtonOfSpecies>(LeftPanel);
            ButtonOfSpeciesList.Add(bos);
            bos.Initialize(kv.Value);
        }
    }

    public void CreateNewButtonClick()
    {
        NatureController.Instance.ClearAll();
        UIManager.Instance.GetBaseUIForm<NaturalPanel>().isSimulationStart = false;
        GameManager.Instance.BG.SetActive(true);
        UIManager.Instance.ShowUIForms<CreatureEditorPanel>().Initialize(new GeoGroupInfo());
        CloseUIForm();
    }

    [SerializeField] private Slider SimulateSpeedSlider;

    void Start()
    {
        SimulateSpeedSlider.value = 0.5f;
        SimulateSpeedSlider.maxValue = 5f;
        SimulateSpeedSlider.minValue = 0f;
        SimulateSpeedSlider.onValueChanged.AddListener(delegate(float value)
        {
            SimulateSpeed = value;
            Time.timeScale = value;
        });
    }

    public bool isSimulationStart = false;

    public void RestartSimulateButtonClick()
    {
        isSimulationStart = true;
        NatureController.Instance.RestartSimulate();
    }

    public void ResumeSimulate()
    {
        if (!isSimulationStart)
        {
            RestartSimulateButtonClick();
        }

        Time.timeScale = SimulateSpeed;
    }

    public void StopSimulate()
    {
        Time.timeScale = 0;
    }

    public float SimulateSpeed = 1.0f;
}