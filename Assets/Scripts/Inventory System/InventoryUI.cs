using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }// Singleton Instance

    public GameObject inventoryPanel;
    public GameObject inventorySlotPrefab;
    public Transform inventorySlotParent;

    private InventorySystem inventory;

    //private bool isInventoryOpen = false; //Checks inventory open-close state

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)//If Instance not null and instance not this
        {
            Destroy(gameObject);//Suicide :D
        }
        else//If instance null and instance is this
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);// Sahneler arasi geciste yok edilmez
        }
    }

    private void Start()
    {
        inventory = InventorySystem.Instance;
        RefreshUI();
    }
        
    public void RefreshUI()
    {
        KillAllChilds();

        foreach (InventorySlot slot in inventory.inventorySlots)
        {
            if (!slot.IsEmpty())
            {
                GameObject slotObj = Instantiate(inventorySlotPrefab, inventorySlotParent);
                InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();
                slotUI.SetSlot(slot);
            }
        }
    }
    private void KillAllChilds()
    {
        foreach (Transform child in inventorySlotParent)//InventorySlot objectinin tum childlarini oldurur
        {
            Destroy(child.gameObject);
        }
    }
}
