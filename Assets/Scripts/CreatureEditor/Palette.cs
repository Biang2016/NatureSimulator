using System.Collections.Generic;
using UnityEngine;

public class Palette : MonoBehaviour
{
    [SerializeField] private Transform Container;

    public int RowNum = 8;
    public int ColumnNum = 20;
    private float Size = 70f;

    private List<PaletteColorButton> PaletteColorButtons = new List<PaletteColorButton>();

    void Start()
    {
        float realSize = Size * 4 / RowNum;

        int tempLow = 0;
        int tempHi = RowNum;
        for (int j = 0; j < ColumnNum; j++)
        {
            if (j == ColumnNum - 1) tempHi--;
            for (int i = tempLow; i < tempHi; i++)
            {
                PaletteColorButton pcb = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.PaletteColorButton].AllocateGameObject<PaletteColorButton>(Container);
                PaletteColorButtons.Add(pcb);
                Color c;
                if (j <= ColumnNum - 2)
                {
                    c = ClientUtils.HSL_2_RGB((float) (j) / (ColumnNum - 1), (float) (i - tempLow) / RowNum + 0.2f, (float) (i - tempLow + j * 0.5f) / (RowNum + ColumnNum));
                }
                else
                {
                    c = ClientUtils.HSL_2_RGB(0f, 0f, (float) (i - tempLow) / RowNum);
                }

                pcb.Initialize(i, j, realSize, c, OnPaletteColorButtonClick);
            }

            if (j % 2 == 1)
            {
                tempLow--;
                tempHi--;
            }
        }

        CurrentPaletteColorButton = PaletteColorButtons[0];
    }

    public PaletteColorButton CurrentPaletteColorButton;

    public void OnPaletteColorButtonClick(PaletteColorButton btn)
    {
        CurrentPaletteColorButton = btn;
        foreach (PaletteColorButton pcb in PaletteColorButtons)
        {
            pcb.IsSelected = btn == pcb;
            if (btn == pcb) pcb.transform.SetAsLastSibling();
        }

        UIManager.Instance.GetBaseUIForm<CreatureEditorPanel>().EditArea.CurrentEditGeoElement?.ChangeColor(btn.Color);
        gameObject.SetActive(false);
    }
}