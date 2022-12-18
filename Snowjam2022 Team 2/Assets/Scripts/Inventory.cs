using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    private Settings settings;
    private PlayerController player;

    [SerializeField] Sprite nullItemSprite;

    private GameObject[] itemSlots;
    private Dictionary<string, int> itemDict;
    [SerializeField] Sprite[] itemSprites;
    [SerializeField] string[] itemList;

    int craftSlot = 0;
    private GameObject[] craftSlots; // 3 Crafting slots, scrollable
    [SerializeField] Sprite[] craftSprites;
    private List<CraftableItem> craftList;
    private TMP_Text craftDescription;
    private string craftStatus = "";

    // Start is called before the first frame update
    void Start()
    {
        settings = GameObject.FindGameObjectWithTag("Settings").GetComponent<Settings>();
        player = FindObjectOfType<PlayerController>();

        itemSlots = GameObject.FindGameObjectsWithTag("InventoryItems");
        craftSlots = GameObject.FindGameObjectsWithTag("CraftingItems");

        craftList = player.getCraftableItems();
        craftDescription = GameObject.FindGameObjectWithTag("CraftingDescription").GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update() // Might want to change this to only update when the inventory changes instead of every frame
    {
        if (craftList.Count == 0 || itemList.Length == 0) return; // Don't run this script if there's no craftables or items
        if (craftList.Count != craftSprites.Length)
        {
            Debug.Log("Check the crafting lists in the Player and Inventory to make sure they line up");
            return;
        }

            itemDict = player.GetDict();

        int itemSlot = 0;
        foreach(KeyValuePair<string, int> item in itemDict)
        {
            if (itemList.Contains(item.Key)) // If it's an inventory item, add to inventory
            {
                if (item.Value > 0)
                {
                    // Find the index of the sprite in itemList
                    int spriteIndex = 0;
                    while (!itemList[spriteIndex].Equals(item.Key))
                    {
                        spriteIndex++;
                    }

                    // Update Icon at itemSlot
                    itemSlots[itemSlot].GetComponentsInChildren<Image>()[1].sprite = itemSprites[spriteIndex];

                    // Update Count at itemSlot
                    itemSlots[itemSlot].GetComponentInChildren<TMP_Text>().text = "" + item.Value;

                    itemSlot++;
                }
            }
            else
            {
                //Debug.Log("Item not found, define the object in the Inventory");
            }
        }
        // Blank out the rest of the inventory slots
        while (itemSlot < itemSlots.Length)
        {
            itemSlots[itemSlot].GetComponentsInChildren<Image>()[1].sprite = nullItemSprite;
            itemSlots[itemSlot].GetComponentInChildren<TMP_Text>().text = "";
            itemSlot++;
        }

        // Crafting display
        craftSlots[1].GetComponentsInChildren<Image>()[1].sprite = craftSprites[craftSlot];
        if (craftSlot < craftList.Count - 1)
        {
            craftSlots[2].GetComponentsInChildren<Image>()[1].sprite = craftSprites[craftSlot + 1];
        }
        else
        {
            craftSlots[2].GetComponentsInChildren<Image>()[1].sprite = craftSprites[0];
        }
        if (craftSlot > 0)
        {
            craftSlots[0].GetComponentsInChildren<Image>()[1].sprite = craftSprites[craftSlot - 1];
        }
        else
        {
            craftSlots[0].GetComponentsInChildren<Image>()[1].sprite = craftSprites[craftList.Count - 1];
        }
        // Required materials window
        List<string> materialsList = craftList[craftSlot].requiredMaterials;
        
        craftDescription.text = materialsList[0];
        for (int i = 1; i < materialsList.Count; i++)
        {
            craftDescription.text += ", " + materialsList[i];
        }
        //Crafting status
        craftDescription.text += "\n\n" + craftStatus;

    }

    public void ScrollCraftingWindow(string dir)
    {
        if (dir.ToLower() == "left")
        {
            if (craftSlot > 0)
                craftSlot--;
            else
                craftSlot = craftList.Count - 1;
        }
        else if (dir.ToLower() == "right")
        {
            if (craftSlot < craftList.Count - 1)
                craftSlot++;
            else
                craftSlot = 0;
        }
        else
        {
            Debug.Log("Crafting grid button not linked correctly!");
        }
    }

    public void CraftThisItem()
    {
        bool success = player.Craft(craftList[craftSlot]);

        if (success)
        {
            craftStatus = "";
        }
        else
        {
            craftStatus = "<color=red>Crafting failed!</color>";
        }
    }
}
