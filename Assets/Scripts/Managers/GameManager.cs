using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public int Layer_GeoElement;
    public GameObject BG;
    public GameObject Nature;

    public float ScaleFactor = 100f;

    void Awake()
    {
        PrefabManager.Instance.LoadPrefabs_Editor();
        Layer_GeoElement = 1 << LayerMask.NameToLayer("GeoElement");
    }

    void Start()
    {
        UIManager.Instance.ShowUIForms<CreatureEditorPanel>();
    }

    void Update()
    {
    }
}