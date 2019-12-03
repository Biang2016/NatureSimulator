using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NaturalPanel : BaseUIForm
{
    [SerializeField] private Transform LeftPanel;
    [SerializeField] private GameObject CoverImage;
    public List<ButtonOfSpecies> ButtonOfSpeciesList = new List<ButtonOfSpecies>();

    void Awake()
    {
        CoverImage.SetActive(true);
        RestartButton.gameObject.SetActive(false);
        PauseButton.gameObject.SetActive(false);
        SimulateSpeed = 1.0f;
        UIType = new UIType();
        UIType.IsClickElsewhereClose = false;
        UIType.IsESCClose = false;
        StopSimulate();
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

    public void RefreshButtonSelected()
    {
        foreach (ButtonOfSpecies bos in ButtonOfSpeciesList)
        {
            if (bos.Toggle.isOn)
            {
                NatureController.Instance.AllSelectedGeoGroupInfoNames.Add(bos.GGI.Name);
            }
            else
            {
                NatureController.Instance.AllSelectedGeoGroupInfoNames.Remove(bos.GGI.Name);
            }
        }
    }

    public void CreateNewButtonClick()
    {
        NatureController.Instance.ClearAll();
        UIManager.Instance.GetBaseUIForm<NaturalPanel>().isSimulationStart = false;
        UIManager.Instance.ShowUIForms<CreatureEditorPanel>().Initialize(new GeoGroupInfo(), false);
        CloseUIForm();
    }

    [SerializeField] private Slider SimulateSpeedSlider;
    [SerializeField] private Text SpeedText;

    void Start()
    {
        SimulateSpeedSlider.maxValue = 5f;
        SimulateSpeedSlider.minValue = 0f;
        SimulateSpeedSlider.onValueChanged.AddListener(delegate(float value)
        {
            SimulateSpeed = value;
            Time.timeScale = value;
            SpeedText.text = Math.Round(value, 1) + "x";
        });
        SimulateSpeedSlider.value = 1f;
        NatureController.Instance.LoadAllSpeciesFromXML();
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
        Time.timeScale = 0f;
    }

    public float SimulateSpeed = 1.0f;

    [SerializeField] private Button StartButton;
    [SerializeField] private Button PauseButton;
    [SerializeField] private Button RestartButton;

    public override void Hide()
    {
        StartButton.gameObject.SetActive(true);
        PauseButton.gameObject.SetActive(false);
        RestartButton.gameObject.SetActive(false);
        base.Hide();
    }

    public void OnSave()
    {
        ConfirmPanel cp = UIManager.Instance.ShowUIForms<ConfirmPanel>();
        cp.Initialize("If you export these species as presets, existing presets would be cleared. Continue?", "Continue", "Cancel", delegate
        {
            NatureController.Instance.OnSaveAllCreatures();
            cp.CloseUIForm();
        }, delegate { cp.CloseUIForm(); });
    }

    public void OnLoad()
    {
        ConfirmPanel cp = UIManager.Instance.ShowUIForms<ConfirmPanel>();
        cp.Initialize("If the presets are loaded, all species here would be cleared. Continue?", "Continue", "Cancel", delegate
        {
            NatureController.Instance.LoadAllSpeciesFromXML();
            cp.CloseUIForm();
        }, delegate { cp.CloseUIForm(); });
    }

    public void OnQuit()
    {
        Application.Quit();
    }
}