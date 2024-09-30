using UnityEngine;

[CreateAssetMenu(fileName = "New Health Potion", menuName = "Inventory/Health Potion")]
public class HealthPotion : Item
{
    public int healthRestoreAmount;  // Kaç can dolduracak

    public override void Use()
    {
        if (PlayerManager.Instance.currentHealth >= 100)
        {
            Debug.Log("Your health is already full");
            return;
        }

        base.Use();

        Debug.Log("Restoring health by " + healthRestoreAmount);

        InventorySystem.Instance.RemoveItem(this, 1);

        PlayerManager.Instance.Heal(healthRestoreAmount);
    }
}
