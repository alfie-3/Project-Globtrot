using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using static UnityEngine.Rendering.DebugUI;


public class UI_BuildingSelection: MonoBehaviour {

    [SerializeField]
    RectTransform ItemsPanel;






    int _menuY;
    int MenuY
    {
        get { return _menuY; }
        set
        {
            _menuY = Mathf.Max(Mathf.Min(value, maxMenuY), 0);
        }
    }
    int maxMenuY;

    int[] MenuX;
    int[] maxMenuX;


    private void Awake()
    {
        maxMenuY = ItemsPanel.childCount - 1;
        MenuX = new int[ItemsPanel.childCount];
        maxMenuX = new int[ItemsPanel.childCount];
        for (int i = 0; i < ItemsPanel.childCount; i++)
        {
            maxMenuX[i] = ItemsPanel.GetChild(i).GetChild(0).childCount-1;
        }
    }

    public void MoveX(int dir) {
        int x = MenuX[MenuY] + dir;
        MenuX[MenuY] = Mathf.Max(Mathf.Min(x, maxMenuX[MenuY]), 0);
        RectTransform HorizontalSlice = ItemsPanel.GetChild(MenuY).GetChild(0).GetComponent<RectTransform>();

        DOTween.To(() => HorizontalSlice.localPosition, x => HorizontalSlice.localPosition = x, new Vector3(MenuX[MenuY] * -75, 0, 0), 0.2f).SetEase(Ease.InOutFlash);

    }

    public void ScrolPanel(int dir) {
        MenuY += dir;
        Debug.Log(MenuY);
        DOTween.To(() => ItemsPanel.localPosition, x => ItemsPanel.localPosition = x, new Vector3(0, MenuY * 75, 0), 0.2f).SetEase(Ease.InOutFlash);
    }

    public void SetVisabiltiy(bool visabiltiy) {
        GetComponent<Canvas>().enabled = visabiltiy;
    }

    /*public string GetSelectedId()
    {
        ItemsPanel.GetChild(MenuY).GetChild
    }*/


}