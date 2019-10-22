using UnityEngine;
using UnityEngine.UI;

public class ButtonOfSpecies : PoolObject
{
    [SerializeField] private Button Button;
    [SerializeField] private Text Text;

    private GeoGroupInfo GGI;

    public void Initialize(GeoGroupInfo ggi)
    {
        Button.image.color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        GGI = ggi;
        Text.text = ggi.Name;
        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(delegate
        {
            NatureController.Instance.ClearAll();
            GameManager.Instance.BG.SetActive(true);
            CreatureEditorPanel cep = UIManager.Instance.ShowUIForms<CreatureEditorPanel>();
            cep.Initialize(GGI);
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
        NatureController.Instance.AllSpecies.Remove(Text.text);
        UIManager.Instance.GetBaseUIForm<NaturalPanel>().RestartSimulateButtonClick();
        PoolRecycle();
    }
}