using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using instinctai.usr.behaviours;
using UnityEngine.Events;
using UnityEngine.UI;

public class EditArea : MonoBehaviour
{
    [SerializeField] private Texture2D Cursor_None;
    [SerializeField] private Texture2D Cursor_Select;
    [SerializeField] private Texture2D Cursor_Draw;
    [SerializeField] private Texture2D Cursor_DuringDrawing;
    [SerializeField] private Texture2D Cursor_Eraser;

    [SerializeField] private List<GeoElement> GeoElements = new List<GeoElement>();
    [SerializeField] private DrawFunctionButton[] DrawFunctionButtons;

    void Start()
    {
        for (int i = 0; i < DrawFunctionButtons.Length; i++)
        {
            DrawFunctionButton dfb = DrawFunctionButtons[i];
            dfb.Initialize((DrawFunctionButton.FunctionTypes) i, delegate(DrawFunctionButton tar_dfb)
            {
                foreach (DrawFunctionButton d in DrawFunctionButtons)
                {
                    d.IsSelected = d == tar_dfb;
                }

                MyState = DrawFunctionButton.TarStateDict[tar_dfb.MyFunctionType];
                UIManager.Instance.GetBaseUIForm<CreatureEditorPanel>().CancelButtonSelection();
            });
        }
    }

    public void LoadGeoGroupInfo(GeoGroupInfo ggi)
    {
        foreach (GeoElement geo in GeoElements)
        {
            geo.PoolRecycle();
        }

        GeoElements.Clear();
        MyState = States.None;
        StartDragMove = false;
        IsMouseIn = false;
        MouseLeftDown = false;
        SortingOrder = 2;

        foreach (GeoInfo gi in ggi.GeoInfos)
        {
            GeoElement ge = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.GeoElement].AllocateGameObject<GeoElement>(transform);
            ge.transform.localPosition = gi.Position;
            ge.transform.rotation = gi.Rotation;
            ge.Initialize(gi.GeoType, gi.Size * GameManager.Instance.ScaleFactor * GameManager.Instance.ScaleFactor, gi.Color, gi.SortingOrder);
            SortingOrder = Mathf.Max(SortingOrder, gi.SortingOrder + 1);

            GeoElements.Add(ge);
        }
    }

    public bool IsMouseIn;

    private States myState;

    public States MyState
    {
        get { return myState; }
        set
        {
            if (myState != value)
            {
                myState = value;
                RefreshCursor();
            }
        }
    }

    private void RefreshCursor()
    {
        if (IsMouseIn)
        {
            switch (MyState)
            {
                case States.None:
                {
                    UnSelect();
                    Cursor.SetCursor(Cursor_None, Vector2.zero, CursorMode.Auto);
                    break;
                }
                case States.Select:
                {
                    foreach (DrawFunctionButton dfb in DrawFunctionButtons)
                    {
                        dfb.IsSelected = dfb.MyFunctionType == DrawFunctionButton.FunctionTypes.Select;
                    }

                    Cursor.SetCursor(Cursor_Select, Vector2.zero, CursorMode.Auto);
                    break;
                }
                case States.Draw:
                {
                    UnSelect();
                    foreach (DrawFunctionButton drawFunctionButton in DrawFunctionButtons)
                    {
                        drawFunctionButton.IsSelected = false;
                    }

                    Cursor.SetCursor(Cursor_Draw, Vector2.one * 16, CursorMode.Auto);
                    break;
                }
                case States.DuringDrawing:
                {
                    UnSelect();
                    Cursor.SetCursor(Cursor_DuringDrawing, Vector2.one * 16, CursorMode.Auto);
                    break;
                }
                case States.Delete:
                {
                    UnSelect();
                    Cursor.SetCursor(Cursor_Eraser, Vector2.one * 16, CursorMode.Auto);
                    break;
                }
            }
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    private void CancelCurrentAction()
    {
    }

    public void OnMouseHover()
    {
        IsMouseIn = true;
        RefreshCursor();
    }

    public void OnMouseExit()
    {
        IsMouseIn = false;
        RefreshCursor();
        CancelCurrentAction();
    }

    public enum States
    {
        None,
        Select,
        Draw,
        DuringDrawing,
        Delete,
    }

    private GeoGroupInfo Cur_GGI;

    void Update()
    {
        RefreshInfo();
        if (IsMouseIn)
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnMouseLeftDown();
            }

            if (MouseLeftDown && Input.GetMouseButton(0))
            {
                OnMouseLeftDrag();
            }

            if (Input.GetMouseButtonUp(0))
            {
                OnMouseLeftUp();
            }
        }
    }

    public void RefreshInfo()
    {
        Cur_GGI = new GeoGroupInfo();
        foreach (GeoElement geo in GeoElements)
        {
            GeoInfo gi = geo.ExportGeoInfo();
            Cur_GGI.GeoInfos.Add(gi);
        }

        Cur_GGI.RefreshInfo();
        UIManager.Instance.GetBaseUIForm<CreatureEditorPanel>().RefreshLeftPanelInfo(Cur_GGI);
        UIManager.Instance.GetBaseUIForm<CreatureEditorPanel>().GetLeftPanelManualInfo(Cur_GGI);
    }

    private bool MouseLeftDown = false;
    private Vector2 MouseLeftDownPos;

    public void OnMouseLeftDown()
    {
        MouseLeftDown = true;
        MouseLeftDownPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastMousePos = MouseLeftDownPos;
        switch (MyState)
        {
            case States.None:
            {
                break;
            }
            case States.Select:
            {
                GeoElement ge = ClickSelectGeoElement();
                if (ge != null)
                {
                    if (CurrentEditGeoElement == ge)
                    {
                        StartDragMove = true;
                    }
                }

                break;
            }
            case States.Draw:
            {
                MyState = States.DuringDrawing;
                break;
            }
            case States.DuringDrawing:
            {
                break;
            }
            case States.Delete:
            {
                break;
            }
        }
    }

    internal GeoElement CurrentEditGeoElement = null;
    private int SortingOrder = 2;

    private bool StartDragMove = false;
    private Vector2 lastMousePos;
    private Vector2 endPos;

    public void OnMouseLeftDrag()
    {
        Vector2 startPos = MouseLeftDownPos;
        endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        switch (MyState)
        {
            case States.None:
            {
                break;
            }
            case States.Select:
            {
                if (StartDragMove)
                {
                    if (CurrentEditGeoElement != null)
                    {
                        CurrentEditGeoElement.transform.Translate(endPos.x - lastMousePos.x, endPos.y - lastMousePos.y, 0, Space.World);
                    }
                }

                break;
            }
            case States.Draw:
            {
                break;
            }
            case States.DuringDrawing:
            {
                GeoTypes gt = UIManager.Instance.GetBaseUIForm<CreatureEditorPanel>().CurrentDrawGeoType;
                GeoManager.GeoDrawingSetting gds = GeoManager.Instance.GeoDrawingSettingsDict[gt];
                Color c = UIManager.Instance.GetBaseUIForm<CreatureEditorPanel>().GetCurrentColor();
                if (CurrentEditGeoElement == null)
                {
                    CurrentEditGeoElement = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.GeoElement].AllocateGameObject<GeoElement>(transform);

                    CurrentEditGeoElement.transform.position = startPos;
                    CurrentEditGeoElement.transform.localRotation = Quaternion.AngleAxis(gds.DefaultRotation, Vector3.back);
                    float size = (endPos - startPos).magnitude * gds.SizeRatio;
                    SortingOrder += 2;
                    CurrentEditGeoElement.Initialize(gt, Vector2.one * size, c, SortingOrder);
                    GeoElements.Add(CurrentEditGeoElement);
                    break;
                }
                else
                {
                    float rotateAngle = Vector2.SignedAngle(endPos - startPos, Vector2.right) + gds.DefaultRotation;
                    CurrentEditGeoElement.transform.localRotation = Quaternion.AngleAxis(rotateAngle, Vector3.back);

                    float size = (endPos - startPos).magnitude * gds.SizeRatio;
                    CurrentEditGeoElement.Initialize(gt, Vector2.one * size, c, SortingOrder);

                    Vector2 rotatedOffset = new Vector2(gds.PivotOffset.x * Mathf.Cos(Mathf.Deg2Rad * rotateAngle) + gds.PivotOffset.y * Mathf.Sin(Mathf.Deg2Rad * rotateAngle), -gds.PivotOffset.x * Mathf.Sin(Mathf.Deg2Rad * rotateAngle) + gds.PivotOffset.y * Mathf.Cos(Mathf.Deg2Rad * rotateAngle));
                    Vector2 pivotPos = startPos + rotatedOffset * size / GameManager.Instance.ScaleFactor / GameManager.Instance.ScaleFactor;
                    CurrentEditGeoElement.transform.position = pivotPos;
                    break;
                }
            }
            case States.Delete:
            {
                break;
            }
        }

        lastMousePos = endPos;
    }

    public void OnMouseLeftUp()
    {
        if (MyState == States.DuringDrawing)
        {
            MyState = States.Draw;
            CurrentEditGeoElement = null;
        }

        if (MyState == States.Select)
        {
            StartDragMove = false;
            GeoElement ge = ClickSelectGeoElement();
            if (ge != null)
            {
                CurrentEditGeoElement = ge;
                foreach (GeoElement gee in GeoElements)
                {
                    gee.OnSelected = false;
                }

                CurrentEditGeoElement.OnSelected = true;
            }
        }

        if (MyState == States.Delete)
        {
            GeoElement ge = ClickSelectGeoElement();
            if (ge != null)
            {
                if (CurrentEditGeoElement == ge)
                {
                    ge.PoolRecycle();
                    CurrentEditGeoElement = null;
                    GeoElements.Remove(ge);
                    RefreshInfo();
                }
                else
                {
                    CurrentEditGeoElement = ge;
                    foreach (GeoElement gee in GeoElements)
                    {
                        gee.OnSelected = false;
                    }

                    CurrentEditGeoElement.OnSelected = true;
                }
            }
        }
    }

    private GeoElement ClickSelectGeoElement()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        GeoElement frontGE = null;
        if (hits != null & hits.Length != 0)
        {
            foreach (RaycastHit2D hit in hits)
            {
                GeoElement ge = hit.collider.gameObject.GetComponent<GeoElement>();
                if (ge != null)
                {
                    if (frontGE == null)
                    {
                        frontGE = ge;
                    }
                    else
                    {
                        if (frontGE.MySortingOrder < ge.MySortingOrder)
                        {
                            frontGE = ge;
                        }
                    }
                }
            }

            return frontGE;
        }

        return null;
    }

    // 点选形状后，左键绘图，右键退出并回到选择及移动光标

    public void OnMouseRightDown()
    {
    }

    public void OnDump()
    {
        foreach (GeoElement ge in GeoElements)
        {
            ge.PoolRecycle();
        }

        GeoElements.Clear();
        CurrentEditGeoElement = null;
        RefreshInfo();
    }

    public void OnSave()
    {
        ConfirmPanel cp = UIManager.Instance.ShowUIForms<ConfirmPanel>();
        cp.Initialize("Please input species name:", "Confirm", "Cancel", delegate
        {
            if (!string.IsNullOrEmpty(cp.InputText1))
            {
                Cur_GGI.Name = cp.InputText1;
                Cur_GGI.ResetCenterAndSortingOrder();
                if (NatureController.Instance.AllGeoGroupInfo.ContainsKey(Cur_GGI.Name))
                {
                    NatureController.Instance.AllGeoGroupInfo[Cur_GGI.Name] = Cur_GGI;
                }
                else
                {
                    NatureController.Instance.AllGeoGroupInfo.Add(Cur_GGI.Name, Cur_GGI);
                }

                cp.CloseUIForm();
            }
        }, delegate { cp.CloseUIForm(); }, "Species name");
    }

    public void UnSelect()
    {
        foreach (GeoElement ge in GeoElements)
        {
            ge.OnSelected = false;
        }

        CurrentEditGeoElement = null;
    }
}