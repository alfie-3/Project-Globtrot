using UnityEngine;

public class SpawnBoxesTesty : MonoBehaviour
{
    private void Awake() {
        GetComponent<StockBoxController>().AddItemServer_Rpc("Hot Dog");
    }
}
