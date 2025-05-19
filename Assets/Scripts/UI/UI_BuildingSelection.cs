using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using static PlayerUI_Manager;


public class UI_BuildingSelection: MonoBehaviour, IInitPlayerUI {

    [SerializeField]
    RectTransform ItemsPanel;

    [SerializeField]
    TextMeshProUGUI Moneytxt;

    [SerializeField]
    TextMeshProUGUI ObjectNameText;

    [SerializeField]
    TextMeshProUGUI ObjectPriceText;

    [SerializeField]
    TextMeshProUGUI ObjectDescriptionText;

    [SerializeField]
    List<FunitureSlot> slots;

    [SerializeField]
    GameObject itemSlotCarrige;

    [SerializeField] float scaleFalloff = 0.2f;
    [SerializeField] float alphaFalloff = 0.3f;

    [SerializeField] float horizontalValue = 12f;


    int _menuY;
    int MenuY
    {
        get { return _menuY; }
        set
        {
            _menuY = Mathf.Max(Mathf.Min(value, slots.Count-1), 0);
        }
    }


    List<int> _menuX;
    
    int MenuX 
    { 
        get { return _menuX[MenuY]; }
        set 
        { 
            _menuX[MenuY] = Mathf.Max(Mathf.Min(value, slots[MenuY].Items.Count-1), 0); 
        } 
    }

    //int[] maxMenuX;





    private void Awake()
    {
        GameStateManager.OnDayChanged += (day) => ProccessNewItems();

        foreach (FunitureSlot slot in slots)
        {
            GameObject carrige = Instantiate(itemSlotCarrige, ItemsPanel);
            carrige.name = slot.name;
            GameObject itemIcon = carrige.transform.GetChild(0).GetChild(0).gameObject;
            try
            {
                itemIcon.GetComponent<RawImage>().texture = slot.Items[0].ItemIcon.texture;
            }
            catch { Debug.LogWarning($"Failed to load item icon. Item: {slot.Items[0].ItemID}"); };


            //if(slot.children[0].ItemIcon.texture != null)
                
            for (int i = 1; i < slot.Items.Count; i++)
            {
                itemIcon = Instantiate(itemIcon, carrige.transform.GetChild(0));
                //if (slot.children[i].ItemIcon.texture)
                //itemIcon.GetComponent<RawImage>().texture = slot.children[i].ItemIcon.texture;
                try
                {
                    itemIcon.GetComponent<RawImage>().texture = slot.Items[i].ItemIcon.texture;
                }
                catch { Debug.LogWarning($"Failed to load item icon. Item: {slot.Items[i].ItemID}"); };
            }

            MoneyManager.Instance.OnBuildCoinsChanged += (money) => Moneytxt.text = "<sprite=0> " + money.ToString();
            Moneytxt.text = "<sprite=0> " + MoneyManager.Instance.BuildCoins.Value.ToString();
        }


        //maxMenuY = ItemsPanel.childCount - 1;
        // _menuX = new int[ItemsPanel.childCount];
        //maxMenuX = new int[ItemsPanel.childCount];
        _menuX = new();
        slots.ForEach(x => _menuX.Add(0));
        //GameStateManager.OnDayChanged += (y) => Debug.LogError("HG");
       // GameStateManager.OnDayChanged += (y) => GameStateManager.Instance.GetCurrentDayData().AddedPlaceables.ForEach(x => AddItem(x.prefab, x.category));
        //GameStateManager.Instance.GetCurrentDayData().AddedPlaceables.ForEach(x => AddItem(x.prefab, x.category));
    }

    void ProccessNewItems()
    {
        DayData dayData = GameStateManager.Instance.GetCurrentDayData();

        if (dayData == null) return;

        dayData.AddedPlaceables.ForEach(x => { AddItem(x.prefab, x.category);});
    }


    public void MoveX(int dir) {
        MenuX += dir;
        RectTransform HorizontalSlice = ItemsPanel.GetChild(MenuY).GetChild(0).GetComponent<RectTransform>();

        DOTween.To(() => HorizontalSlice.localPosition, x => HorizontalSlice.localPosition = x, new Vector3(MenuX * -75, 0, 0), 0.2f).SetEase(Ease.InOutFlash);

        UpdateBuildText();
    }

    public void ScrolPanel(int dir) {
        MenuY += dir;
        DOTween.To(() => ItemsPanel.localPosition, x => ItemsPanel.localPosition = x, new Vector3(horizontalValue, MenuY * 75, 0), 0.2f).SetEase(Ease.InOutFlash);

        UpdateBuildText();
        SetScales();
        SetVisabilaties();
    }

    public void AddItem(PlacableFurniture_Item item, string category)
    {
        if (item.Required)
        {
            RequiredBuildablesManager.AddRequiredBuildable(item);
        }

        Transform carrigeT = ItemsPanel.Find(category);
        GameObject carrige;
        GameObject itemIcon;
        if (carrigeT == null)
        {
            carrige = Instantiate(itemSlotCarrige, ItemsPanel);
            carrige.name = category;
            slots.Add(new FunitureSlot(category));
            _menuX.Add(0);


            itemIcon = carrige.transform.GetChild(0).GetChild(0).gameObject;
            /*try
            {
                carrige.transform.GetChild(0).GetChild(0).GetComponent<RawImage>().texture = item.ItemIcon.texture;
            }
            catch { Debug.LogWarning($"Failed to load item icon. Item: {item.ItemID}"); };
            slots[carrige.transform.GetSiblingIndex()].Items.Add(item);*/
        }
        else
        {
            carrige = carrigeT.gameObject;
            itemIcon = Instantiate(carrige.transform.GetChild(0).GetChild(0).gameObject, carrige.transform.GetChild(0));
        }

        
        try
        {
            itemIcon.GetComponent<RawImage>().texture = item.ItemIcon.texture;
        }
        catch { Debug.LogWarning($"Failed to load item icon. Item: {item.ItemID}"); };
        


        slots[carrige.transform.GetSiblingIndex()].Items.Add(item);
    }


    void SetScales() {
        for(int i = 0; i < ItemsPanel.childCount; i++) {
            float scale = 1 - (scaleFalloff * Mathf.Abs(MenuY - i));
            //transform.DOScale(scale, scale,2f);
            ItemsPanel.GetChild(i).DOScale(new Vector3(scale, scale, scale), 0.2f).SetEase(Ease.InOutFlash);//
            //DOTween.To(() => ItemsPanel.GetChild(i).localScale, x => ItemsPanel.GetChild(i).localScale = x, new Vector3(scale, scale, scale),0.2f).SetEase(Ease.InOutFlash);
        }
    }

    void SetVisabilaties()
    {
        for (int i = 0; i < ItemsPanel.childCount; i++)
        {
            float scale = 1 - (alphaFalloff * Mathf.Abs(MenuY - i));
            //transform.DOScale(scale, scale,2f);
            CanvasGroup canvas = ItemsPanel.GetChild(i).GetComponent<CanvasGroup>();
            DOTween.To(() => canvas.alpha, x => canvas.alpha = x, scale, 0.2f).SetEase(Ease.InOutFlash);
            //ItemsPanel.GetChild(i).DO//.DOScale(new Vector3(scale, scale, scale), 0.2f);//
            //DOTween.To(() => ItemsPanel.GetChild(i).localScale, x => ItemsPanel.GetChild(i).localScale = x, new Vector3(scale, scale, scale),0.2f).SetEase(Ease.InOutFlash);
        }
    }

    public void SetVisabiltiy(bool visabiltiy) {
        GetComponent<Canvas>().enabled = visabiltiy;
    }

    public void UpdateBuildText()
    {
        PlacableFurniture_Item item = slots[MenuY].Items[MenuX];

        ObjectNameText.text = item.ItemName;
        ObjectPriceText.text = $"<sprite=0> {item.FurniturePrice}";
        ObjectDescriptionText.text = item.Description;
    }

    public string GetSelectedId()
    {
        return slots[MenuY].Items[MenuX].ItemID;
    }

    [System.Serializable]
    public struct FunitureSlot
    {
        public string name;
        public List<PlacableFurniture_Item> Items;

        public FunitureSlot(string name) {  this.name = name; Items = new(); }
    }

    private void OnDestroy()
    {
        GameStateManager.OnDayChanged -= (day) => ProccessNewItems();

    }

    public void Init(PlayerUI_Manager uiManager)
    {
        ProccessNewItems();
    }
}