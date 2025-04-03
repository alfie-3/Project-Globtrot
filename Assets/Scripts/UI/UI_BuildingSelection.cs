using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class UI_BuildingSelection: MonoBehaviour {

    [SerializeField]
    RectTransform ItemsPanel;

    [SerializeField]
    List<FunitureSlot> slots;

    [SerializeField]
    GameObject itemSlotCarrige;


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


    int[] _menuX;
    int MenuX { get { return _menuX[MenuY]; } set { _menuX[MenuY] = value; } }

    int[] maxMenuX;


    private void Awake()
    {
        foreach (FunitureSlot slot in slots)
        {
            GameObject carrige = Instantiate(itemSlotCarrige, ItemsPanel);
            GameObject itemIcon = carrige.transform.GetChild(0).GetChild(0).gameObject;
            itemIcon.GetComponent<RawImage>().texture = slot.children[0].ItemIcon.texture;
            for (int i = 1; i < slot.children.Count; i++)
            {
                itemIcon = Instantiate(itemIcon, carrige.transform.GetChild(0));
                itemIcon.GetComponent<RawImage>().texture = slot.children[i].ItemIcon.texture;
            }
        }


        maxMenuY = ItemsPanel.childCount - 1;
        _menuX = new int[ItemsPanel.childCount];
        maxMenuX = new int[ItemsPanel.childCount];
        for (int i = 0; i < ItemsPanel.childCount; i++)
        {
            maxMenuX[i] = ItemsPanel.GetChild(i).GetChild(0).childCount-1;
        }
    }

    public void MoveX(int dir) {
        int x = MenuX + dir;
        MenuX = Mathf.Max(Mathf.Min(x, maxMenuX[MenuY]), 0);
        RectTransform HorizontalSlice = ItemsPanel.GetChild(MenuY).GetChild(0).GetComponent<RectTransform>();

        DOTween.To(() => HorizontalSlice.localPosition, x => HorizontalSlice.localPosition = x, new Vector3(MenuX * -75, 0, 0), 0.2f).SetEase(Ease.InOutFlash);

    }

    public void ScrolPanel(int dir) {
        MenuY += dir;
        Debug.Log(MenuY);
        DOTween.To(() => ItemsPanel.localPosition, x => ItemsPanel.localPosition = x, new Vector3(0, MenuY * 75, 0), 0.2f).SetEase(Ease.InOutFlash);
        //SetScales();
    }

    void SetScales() {
        for(int i = 0; i < ItemsPanel.childCount; i++) {
            float scale = 1 - (0.1f * Mathf.Abs(MenuY - i));
            DOTween.To(() => ItemsPanel.GetChild(i).localScale, x => ItemsPanel.GetChild(i).localScale = x, new Vector3(scale, scale, scale),0.2f).SetEase(Ease.InOutFlash);
        }
    }

    public void SetVisabiltiy(bool visabiltiy) {
        GetComponent<Canvas>().enabled = visabiltiy;
    }

    public string GetSelectedId()
    {
        return slots[MenuY].children[MenuX].ItemID;
    }

    [System.Serializable]
    public struct FunitureSlot
    {
        public string name;
        public List<PlacableFurniture_Item> children;
    }
}