using Unity.AppUI.UI;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "New Placable Item", menuName = "Items/Placable")]
public class Placable_Item : ItemBase
{
    protected GameObject _PlaceablePrefab;
    [field: SerializeField] public GameObject PlaceablePrefab { get; protected set; }
    //    { 
    //    get { return _PlaceablePrefab; }
    //    private set { _PlaceablePrefab = value; holoMesh = value.GetComponent<MeshFilter>().sharedMesh;}
    //}
    private Mesh holoMesh;
    private RenderParams renderParams;

    public const float PLACABLE_DISTANCE = 5;

    public override void OnPrimary(PlayerHoldingManager holdingManager)
    {
        base.OnPrimary(holdingManager);

        if (PlaceablePrefab != null)
            PlaceItem_Rpc(holdingManager);
    }

    [Rpc(SendTo.Server)]
    public void PlaceItem_Rpc(PlayerHoldingManager holdingManager)
    {
        Ray ray = new(holdingManager.CameraManager.CamTransform.position, holdingManager.CameraManager.CamTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, PLACABLE_DISTANCE, LayerMask.GetMask("Placeable")))
        {
            holdingManager.PlaceItem_Rpc(ItemID, hit.point, Quaternion.Euler(0, holdingManager.Rotation, 0));
            holdingManager.ClearItem();
        }
    }

    public override void OnUpdate (PlayerHoldingManager holdingManager) {
        Ray ray = new(holdingManager.CameraManager.CamTransform.position, holdingManager.CameraManager.CamTransform.forward);
        RenderParams rp = new RenderParams(holdingManager.Material);
        if (Physics.Raycast(ray, out RaycastHit hit, PLACABLE_DISTANCE, LayerMask.GetMask("Placeable"))) {
            Graphics.RenderMesh(rp,PlaceablePrefab.GetComponent<MeshFilter>().sharedMesh, 0, Matrix4x4.TRS((hit.point + PlaceablePrefab.transform.position), Quaternion.Euler(0,holdingManager.Rotation,0),Vector3.one));
        }
    }
}
