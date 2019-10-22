using System;
using System.Collections.Generic;
using instinctai.usr.behaviours;
using UnityEngine;
using UnityEngine.UI;

public class CreatureEditorPanel : BaseUIForm
{
    [SerializeField] private Transform TopMenu;
    public EditArea EditArea;
    [SerializeField] private Dictionary<GeoTypes, GeoButton> GeoButtons = new Dictionary<GeoTypes, GeoButton>();

    public void Initialize(GeoGroupInfo ggi)
    {
        LoadInfoForLeftPanel(ggi);
        EditArea.LoadGeoGroupInfo(ggi);
    }

    void Start()
    {
        foreach (string s in Enum.GetNames(typeof(GeoTypes)))
        {
            GeoTypes gt = (GeoTypes) Enum.Parse(typeof(GeoTypes), s);
            GeoButton geoButton = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.GeoButton].AllocateGameObject<GeoButton>(TopMenu);
            geoButton.Initialize(gt, ChangeCurrentDrawGeoType);
            GeoButtons.Add(gt, geoButton);
        }
    }

    public GeoTypes CurrentDrawGeoType;

    public void ChangeCurrentDrawGeoType(GeoTypes geoType)
    {
        CurrentDrawGeoType = geoType;
        foreach (KeyValuePair<GeoTypes, GeoButton> kv in GeoButtons)
        {
            kv.Value.IsSelected = geoType == kv.Key;
        }

        EditArea.MyState = EditArea.States.Draw;
    }

    [SerializeField] private Slider LifeBar;
    [SerializeField] private Text LifeText;
    [SerializeField] private Slider SpeedBar;
    [SerializeField] private Text SpeedText;
    [SerializeField] private Slider DamageBar;
    [SerializeField] private Text DamageText;
    [SerializeField] private Slider VisionBar;
    [SerializeField] private Text VisionText;
    [SerializeField] private Dropdown DietDrpDropdown;
    [SerializeField] private InputField FertilityRateInputField;
    [SerializeField] private InputField OffspringSize;
    [SerializeField] private InputField MatureSize;
    [SerializeField] private InputField MinSize;
    [SerializeField] private InputField MaxSize;
    [SerializeField] private Text GeneralSize;
    [SerializeField] private Text Mass;

    public void RefreshLeftPanelInfo(GeoGroupInfo ci)
    {
        LifeBar.value = ci.Life;
        LifeText.text = Mathf.RoundToInt(ci.Life).ToString();

        SpeedBar.value = ci.Speed;
        SpeedText.text = Mathf.RoundToInt(ci.Speed).ToString();

        DamageBar.value = ci.Damage;
        DamageText.text = Mathf.RoundToInt(ci.Damage).ToString();

        VisionBar.value = ci.Vision;
        VisionText.text = Mathf.RoundToInt(ci.Vision).ToString();

        GeneralSize.text = Mathf.Round(ci.GeneralSize / 10) + "cm";
        Mass.text = Mathf.Round(ci.Mass) + "kg";
    }

    public void LoadInfoForLeftPanel(GeoGroupInfo ci)
    {
        RefreshLeftPanelInfo(ci);
        FertilityRateInputField.text = ci.FertilityRate.ToString();
        OffspringSize.text = ci.OffspringSizeRatio.ToString();
        MatureSize.text = ci.MatureSizeRatio.ToString();
        MinSize.text = ci.MinSizeRatio.ToString();
        MaxSize.text = ci.MaxSizeRatio.ToString();
    }

    public void GetLeftPanelManualInfo(GeoGroupInfo ci)
    {
        if (int.TryParse(FertilityRateInputField.text, out int fr))
        {
            ci.FertilityRate = fr;
        }
        else
        {
            ci.FertilityRate = 100;
            FertilityRateInputField.text = "100";
        }

        if (int.TryParse(OffspringSize.text, out int os))
        {
            ci.OffspringSizeRatio = os;
        }
        else
        {
            ci.OffspringSizeRatio = 50;
            OffspringSize.text = "50";
        }

        if (int.TryParse(MatureSize.text, out int ms))
        {
            ci.MatureSizeRatio = ms;
        }
        else
        {
            ci.MatureSizeRatio = 50;
            MatureSize.text = "50";
        }

        if (int.TryParse(MinSize.text, out int mins))
        {
            ci.MinSizeRatio = mins;
        }
        else
        {
            ci.MinSizeRatio = 30;
            MinSize.text = "30";
        }

        if (int.TryParse(MaxSize.text, out int maxs))
        {
            ci.MaxSizeRatio = maxs;
        }
        else
        {
            ci.MaxSizeRatio = 130;
            MaxSize.text = "130";
        }
    }

    void Update()
    {
    }

    [SerializeField] private Palette PalettePanel;

    public Color GetCurrentColor()
    {
        if (PalettePanel.CurrentPaletteColorButton)
        {
            return PalettePanel.CurrentPaletteColorButton.Color;
        }
        else
        {
            return Color.black;
        }
    }

    public void OnPaletteButtonClick()
    {
        PalettePanel.gameObject.SetActive(!PalettePanel.gameObject.activeInHierarchy);
    }

    public void OnEditAreaRightClick()
    {
        CancelButtonSelection();
        EditArea.MyState = EditArea.States.Select;
    }

    public void CancelButtonSelection()
    {
        foreach (KeyValuePair<GeoTypes, GeoButton> kv in GeoButtons)
        {
            kv.Value.IsSelected = false;
        }
    }

    public void OnReturnButtonClick()
    {
        GameManager.Instance.BG.SetActive(false);
        CloseUIForm();
        NatureController.Instance.RecreateAllSpecies();
        NaturalPanel np = UIManager.Instance.ShowUIForms<NaturalPanel>();
        np.Initialize();
    }
}