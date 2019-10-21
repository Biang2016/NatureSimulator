using System;
using System.Collections.Generic;
using UnityEngine;

public class CreatureEditorPanel : BaseUIForm
{
    [SerializeField] private Transform TopMenu;
    public EditArea EditArea;
    [SerializeField] private Dictionary<GeoTypes, GeoButton> GeoButtons = new Dictionary<GeoTypes, GeoButton>();

    public void Initialize(GeoGroupInfo ggi)
    {
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