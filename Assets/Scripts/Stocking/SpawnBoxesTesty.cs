using UnityEngine;

public class SpawnBoxesTesty : MonoBehaviour
{
    private void Awake() {
        GetComponent<StockBoxController>().AddItem("Hot Dog");
    }
}
