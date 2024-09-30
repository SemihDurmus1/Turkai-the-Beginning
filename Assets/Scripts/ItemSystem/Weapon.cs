using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Weapon")]
public class Weapon : Item
{
    public string weaponName;
    public float attackDamage;
    public float attackRate;

    public float durability;

    //public Animator weaponAnimator;

    public override void Use()
    {
        base.Use();

        PlayerManager.Instance.EquipWeapon(this);

        InventorySystem.Instance.RemoveItem(this, 1);
        // Silahi donat(burasi daha gelistirilmedi, acilen gelistirilmesi gerekiyor)
        //Controller.Instance.EquipWeapon(this);
    }


}
