using UnityEngine;

[CreateAssetMenu(fileName = "New Shop Product", menuName = "Items/Shop Product")]
public class ShopProduct_Item : ItemBase
{
    [field: SerializeField] public float Price { get; private set; }
    [field: SerializeField] public Sprite ProductImage { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }

    public Vector3 ObjectBounds;
    public Vector3 StackBounds;
    public Vector3 BigStackBounds;
}
