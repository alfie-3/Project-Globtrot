using System.Linq;
using System.Reflection;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;

public class OrderScreenUpgrade : MonoBehaviour
{
    [SerializeField]
    Upgrade orderportUpgrade;
    [Space]
    [SerializeField]
    OrderPort orderPort1;
    [SerializeField]
    UI_OrderScreen orderScreen1;
    [SerializeField]
    OrderPort orderPort2;
    [SerializeField]
    UI_OrderScreen orderScreen2;


    private void Awake()
    {
        UpgradesManager.OnUnlockedUpgrade += onUpgrade;
        GameStateManager.OnDayChanged += onNewDay;
    }

    private void onUpgrade(Upgrade upgrade)
    {
        if(upgrade != orderportUpgrade) return;

        UpgradesManager.OnUnlockedUpgrade -= onUpgrade;

        EnableScreen(orderScreen1, orderPort1);
        if(GameStateManager.Instance.CurrentDay.Value >= 5)
            EnableScreen(orderScreen2, orderPort2);
    }

    private void onNewDay(int day)
    {
        if(day != 5) return;
        
        if (UpgradesManager.Instance.CurrentUpgrades.Contains(orderportUpgrade))
            EnableScreen(orderScreen2, orderPort2);
    }


    private void EnableScreen(UI_OrderScreen screen, OrderPort port)
    {
        port.OnOrderAdded.AddListener(() => screen.AddOrder(port.Order));
        port.OnOrderCorrect.AddListener(() =>  screen.PlayResponse(true));
        port.OnOrderIncorrect.AddListener(() => screen.PlayResponse(false));
        port.OnOrderTimout.AddListener(() => screen.PlayResponse(false));

        screen.transform.parent.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        UpgradesManager.OnUnlockedUpgrade -= onUpgrade;
        GameStateManager.OnDayChanged -= onNewDay;
    }
}
