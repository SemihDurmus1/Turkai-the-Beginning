using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public List<InventorySlot> inventorySlots = new();
    [SerializeField] private int maxSlots = 19;

    //public Weapon activeWeapon;

    public static InventorySystem Instance { get; private set; }// Singleton Instance
    private void Awake()//Singleton
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)//Instance null degilse ve instance bu degilse
        {
            Destroy(gameObject);//intihar et
        }
        else//Eger instance bos ise veya instance bu ise
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);// Sahneler arasi geciste yok edilmez
        }

        for (int i = 0; i < maxSlots; i++)//Envanter slotlarini olustur
        {
            inventorySlots.Add(new InventorySlot());
        }

    }

    public bool AddItem(Item item, int amount)
    {
        if (item.isStackable)
        {
            InventorySlot existingSlot = FindStackableSlot(item);//Stackable olan bi slot bul
            if (existingSlot != null)//Eger stackable slot bos degilse amount kadar ekleme yap
            {
                existingSlot.AddItem(item, amount);
                return true;
            }
        }

        //Eger stackable degilse veya existingSlot bos ise
        InventorySlot emptySlot = FindEmptySlot();//Bos slot
        if (emptySlot != null)//bos slot var ise
        {
            emptySlot.AddItem(item, amount);//bos slotu item ile doldur
            return true;
        }

        Debug.Log("Envanter dolu!");
        return false;  // Envanter doluysa false döndür

    }
    public void RemoveItem(Item item, int amount)
    {
        InventorySlot slot = FindSlotWithItem(item);//O itemi iceren slotu bul
        if (slot != null)//Slot null degilse
        { 
            slot.RemoveItem(amount);//remove slot's item as amount
        }
    }


    private InventorySlot FindEmptySlot()
    {
        return inventorySlots.Find(slot => slot.IsEmpty());//IsEmpty degeri true olan slotu return et
    }
    private InventorySlot FindStackableSlot(Item item)// Ayný eþyadan olan bir slot bulma
    {
                                    //Slot.item, verdigimiz item olan ve o slottaki miktari(quantity), itemin max sinirini gecmiyorsa returnle 
        return inventorySlots.Find(slot => slot.item == item && slot.quantity < item.maxStackAmount);
    }
    private InventorySlot FindSlotWithItem(Item item)// Belirli bir eþyayý içeren slot bulma
    {
        //Her slot icin slot.item ile verilen item’i karsilastirir. Esitse, o slot dondurulur.
        return inventorySlots.Find(slot => slot.item == item);
    }


    public void PrintInventory()// Envanteri ekrana yazdýrma (debug amaçlý)
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            if (!slot.IsEmpty())
            {
                Debug.Log("Item: " + slot.item.itemName + " - Quantity: " + slot.quantity);
            }
        }
    }

}
