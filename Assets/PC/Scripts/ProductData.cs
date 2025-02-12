using UnityEngine;

// used to generate panels per product

[CreateAssetMenu(fileName = "NewProduct", menuName = "Products/Product Data")]
public class ProductData : ScriptableObject
{
    public string ProductName;
    public float Price;
    public Sprite ProductImage;
    public GameObject Prefab;
}
