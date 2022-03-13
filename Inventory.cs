using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public int capacity = 0;
    private int count = 0;

    public PickUpable[] inventoryItems;
    public GameObject[] iconGameObjects;

    public string desiredItemName;
    public PickUpable tradedItem;
	
    public Image inventoryBackground;
    
    public int Add(PickUpable item) {
       	if (this.count >= this.capacity) {
	    return -1;
	}

	// check if adding item completes a trade
	if (item.itemName == this.desiredItemName) {
	    // move to next line
	    Dialogue dialogue = this.gameObject.GetComponent<Dialogue>();
	    if (dialogue != null) {
		dialogue.Advance();
	    }

	    // place traded item instead of received item
	    item = this.tradedItem;
	}

	this.count += 1;

	for (int idx = 0; idx < this.capacity; idx++) {
	    if (this.iconGameObjects[idx] == null) {
		this.inventoryItems[idx] = item;
		this.AddItemIcon(item, idx);
		return idx;
	    }
	}
	
	// should be unreachable unless we mess up the count
	return -1;
    }

    public void AddItemIcon(PickUpable item, int idx, bool preserveIsUnlimited=false) {
	// add this item to a ui is applicable
	if (this.inventoryBackground == null) {
	    return;
	}
	
	// assumes that parent of inventoryBackground will be a Canvas
	Canvas canvas = this.inventoryBackground.transform.parent.gameObject.GetComponent<Canvas>();
    
	GameObject itemIconGo = new GameObject(item.itemName);

	// TODO: update positions and sizes for real ui, make them variable
	RectTransform itemRectangle = itemIconGo.AddComponent<RectTransform>();
	Vector2 backgroundPosition = this.inventoryBackground.gameObject.GetComponent<RectTransform>().anchoredPosition;

	// add the item's icon to UI
	// assumes the inventory has an odd number of slots
	// TODO: clean up this line
	itemRectangle.anchoredPosition = new Vector2(57f * (idx + 1 - (this.capacity + 1) / 2), 0f);
	itemRectangle.sizeDelta = new Vector2(40f, 40f); 

	Image itemImage = itemIconGo.AddComponent<Image>();
	itemImage.sprite = item.sprite;
	itemIconGo.transform.SetParent(this.inventoryBackground.transform, false);

	InventoryIcon icon = itemIconGo.AddComponent<InventoryIcon>();
	icon.parentInventory = this;
	icon.parentPickUpable = item;
	icon.slotIdx = idx;
	if (preserveIsUnlimited) {
	    icon.isUnlimited = item.isUnlimited;
	}
		
	this.iconGameObjects[idx] = itemIconGo;
    }

    public PickUpable Pop(int idx) {
	PickUpable item = this.inventoryItems[idx];
	this.inventoryItems[idx] = null;
	this.count -= 1;

	// remove this item from a ui if applicable
	if (this.inventoryBackground != null) {
	    // assumes that parent of inventoryBackground will be a Canvas
	    GameObject.Destroy(this.iconGameObjects[idx]);
	    this.iconGameObjects[idx] = null;
	}

	return item;
    }

    public int GetNextFullSlot(int startIdx) {
	for (int i = 0; i < this.capacity; i++) {
	    int idx = (startIdx + i) % this.capacity;
	    if (this.iconGameObjects[idx] != null) {
		return idx;
	    }
	}
	return -1;
    }

    public bool IsEmpty() {
	return this.count == 0;
    }

    public bool IsSlotEmpty(int idx) {
	return this.iconGameObjects[idx] == null;
    }

    public void SetActive(bool isActive) {
	foreach (GameObject go in this.iconGameObjects) {
	    if (go != null) {
		InventoryIcon icon = go.GetComponent<InventoryIcon>();
		if (!isActive && icon.labelGo != null) {
		    GameObject.Destroy(icon.labelGo);
		}
		go.SetActive(isActive);
	    }
	}
	this.inventoryBackground.gameObject.SetActive(isActive);
    }
}
