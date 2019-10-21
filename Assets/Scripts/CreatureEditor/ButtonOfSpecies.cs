using UnityEngine;
using UnityEngine.UI;

public class ButtonOfSpecies : PoolObject
{
    [SerializeField] private Button Button;
    [SerializeField] private Text Text;

    private GeoGroupInfo GGI;

    public void Initialize(GeoGroupInfo ggi)
    {
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
        });
    }
}