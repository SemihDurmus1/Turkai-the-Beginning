[System.Serializable]
public class InventorySlot
{
    /// <summary>
    /// Envanter slotudur, icinde tek tur item 
    /// ve onun quantity degerini barindirir
    /// </summary>
    
    public Item item; // Bu slotta hangi esya var
    public int quantity; // Kac tane var

    
    /// <summary>
    /// </summary>
    /// item == null: this line checks items null or not
    /// If item is null, returns true; if not null, returns false.
    /// <returns></returns>
    public bool IsEmpty() => item == null;//Slotun empty olup olmadigini kontrol et

    //Yeni esya
    public void AddItem(Item newItem, int amount)
    {
        item = newItem;
        quantity += amount;
    }

    public void RemoveItem(int amount)
    {
        quantity -= amount;
        if (quantity <= 0)
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        item = null;
        quantity = 0;
    }
}
