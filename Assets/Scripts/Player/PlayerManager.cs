using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }//Singleton

    [Header("Player Stats")]
    private int maxHealth = 100;
    public int currentHealth;
    private float stamina;

    public Transform onHandSlot;// Oyuncunun elinde silahi tutacagi pozisyon
    public GameObject equippedWeapon;

    private void Awake()//Singleton
    {
        if (Instance != null && Instance != this)//Singleton
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        // Oyuncunun canýný maksimuma çýkar
        currentHealth = maxHealth;
    }

    public void EquipWeapon(Item weapon)
    {
        if (equippedWeapon != null)//Elimiz bos degilse
        {
            DequipWeapon();//Eldekini envantere birak
        }
        
        if (weapon.itemPrefab != null)
        {

            ////Weaponu el pozisyonunda ve kendi rotasyonunda instantiate et
            //equippedWeapon = Instantiate(weapon.itemPrefab,
            //                             onHandSlot.position,
            //                             weapon.itemPrefab.transform.rotation,
            //                             onHandSlot);

            equippedWeapon = Instantiate(weapon.itemPrefab, onHandSlot);

            Destroy(equippedWeapon.GetComponent<ItemCollect>());
        }
    }

    /// <summary>
    /// Elimizde bir sey varsa onun ItemCollect scriptindeki 
    /// ScriptableItem referansini al ve envantere ekle, sonra da gameobjecti sil
    /// </summary>
    public void DequipWeapon()
    {
        // ItemCollect script'ini kontrol et
        AttackSystem attackSystem = equippedWeapon.GetComponent<AttackSystem>();

        if (attackSystem == null)
        {
            Debug.Log("AttackSystem null amina koyim");
        }

        InventorySystem.Instance.AddItem(attackSystem.weaponReference, 1);
        InventoryUI.Instance.RefreshUI();

        Destroy(equippedWeapon);
    }

    // Health management
    public void TakeDamage(int damage)
    {
        //if (isDead) return;

        currentHealth -= damage;
        Debug.Log("Player Current Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        Debug.Log("Player healed: " + healAmount + ". Current Health: " + currentHealth);
    }

    private void Die()
    {
        Debug.Log("Player has died.");
        //gameObject.SetActive(false);
    }

}
