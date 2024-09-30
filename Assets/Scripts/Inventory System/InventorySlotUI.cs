using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public Image icon;
    private InventorySlot slot;

    public void SetSlot(InventorySlot newSlot)
    {
        slot = newSlot;
        icon.sprite = slot.item.itemIcon;
    }

    public void OnClick()//UI button method
    {
        //Itemin ustune tiklayinca itemin use fonksiyonunu kullanir ve 1 eksiltir
        if (slot != null && slot.item != null)//Slot bos degilse ve slotun icindeki item bos degilse
        {
            slot.item.Use();//slottaki itemin use methodu

            InventoryUI.Instance.RefreshUI();
        }
    }

}
