using UnityEngine;

public enum ItemType//Burasi sonradan genisletilebilir ve degistirilebilir
{
    Weapon,
    Consumable,
    Armor,
    Miscellaneous
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public ItemType itemType;
    public Sprite itemIcon;
    public bool isStackable;
    public int maxStackAmount;

    //Ele alinabilir mi diye bir bool gerekebilir buraya

    public GameObject itemPrefab;

    public virtual void Use()
    {
        // Her bir item türüne göre bu fonksiyon override edilebilir.
    }
}
