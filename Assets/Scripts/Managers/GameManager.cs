using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public int Layer_GeoElement;
    public GameObject BG;
    public SpriteRenderer NaturalBG;
    public SpriteRenderer NatureCircle;
    public GameObject Nature;

    public float ScaleFactor = 100f;

    void Awake()
    {
        PrefabManager.Instance.LoadPrefabs_Editor();
        Layer_GeoElement = 1 << LayerMask.NameToLayer("GeoElement");
    }

    void Start()
    {
        UIManager.Instance.ShowUIForms<NaturalPanel>();
    }

    public float ColorCycleTime = 30f;
    private float ColorCycleTick = 0f;

    void Update()
    {
        if (ColorCycleTick < ColorCycleTime)
        {
            float ss = UIManager.Instance.GetBaseUIForm<NaturalPanel>().SimulateSpeed;
            if (!ss.Equals(0))
            {
                ColorCycleTick += Time.deltaTime / ss;
            }
            else
            {
                ColorCycleTick += Time.deltaTime;
            }
        }
        else
        {
            ColorCycleTick = 0;
        }

        float ratio = ColorCycleTick / ColorCycleTime;
        NatureCircle.color = ClientUtils.HSL_2_RGB(ratio, 1, 0.5f, 0.5f);
        NaturalBG.color = ClientUtils.HSL_2_RGB(ratio, 1, 0.5f);
    }
}