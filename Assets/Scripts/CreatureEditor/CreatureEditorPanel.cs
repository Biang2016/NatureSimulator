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

    void Awake()
    {
        GeneralSize.text = "0cm";
        Mass.text = "0kg";
    }

    public void Initialize(GeoGroupInfo ggi)
    {
        LoadInfoForLeftPanel(ggi);
        EditArea.LoadGeoGroupInfo(ggi);
        UIManager.Instance.ShowUIForms<ConfirmPanel>().InputField1.text = ggi.Name;
        UIManager.Instance.CloseUIForm<ConfirmPanel>();
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
    [SerializeField] private Slider GrowthRateBar;
    [SerializeField] private Text GrowthRateText;
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
    [SerializeField] private InputField MaxNumber;
    [SerializeField] private Text GeneralSize;
    [SerializeField] private Text Mass;

    public void RefreshLeftPanelInfo(GeoGroupInfo ci)
    {
        LifeBar.value = ci.Life;
        LifeText.text = Mathf.RoundToInt(ci.Life).ToString();

        SpeedBar.value = ci.Speed;
        SpeedText.text = Mathf.RoundToInt(ci.Speed).ToString();

        GrowthRateBar.value = ci.GrowUpRate * 1000f;
        GrowthRateText.text = Mathf.RoundToInt(ci.GrowUpRate * 1000f).ToString() + "‰";

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
        OffspringSize.text = ci.OffspringSizePercent.ToString();
        MatureSize.text = ci.MatureSizePercent.ToString();
        MinSize.text = ci.MinSizePercent.ToString();
        MaxSize.text = ci.MaxSizePercent.ToString();
        MaxNumber.text = ci.MaxNumber.ToString();
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
            ci.OffspringSizePercent = os;
        }
        else
        {
            ci.OffspringSizePercent = 30;
            OffspringSize.text = "30";
        }

        if (int.TryParse(MatureSize.text, out int ms))
        {
            ci.MatureSizePercent = ms;
        }
        else
        {
            ci.MatureSizePercent = 70;
            MatureSize.text = "70";
        }

        if (int.TryParse(MinSize.text, out int mins))
        {
            ci.MinSizePercent = mins;
        }
        else
        {
            ci.MinSizePercent = 20;
            MinSize.text = "20";
        }

        if (int.TryParse(MaxSize.text, out int maxs))
        {
            ci.MaxSizePercent = maxs;
        }
        else
        {
            ci.MaxSizePercent = 130;
            MaxSize.text = "130";
        }

        if (int.TryParse(MaxNumber.text, out int mn))
        {
            ci.MaxNumber = mn;
        }
        else
        {
            ci.MaxNumber = 100;
            MaxNumber.text = "100";
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