using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpableContainer : MonoBehaviour
{
    public Inventory inventory;
    
    private bool isOpen = false;
    
    // Start is called before the first frame update
    void Start()
    {
	BoxCollider2D collider = this.gameObject.AddComponent<BoxCollider2D>();
	collider.isTrigger = true;
	this.gameObject.layer = 7;

	// gobble up trade item from screen
	if (inventory.tradedItem != null && inventory.tradedItem.gameObject != null) {
	    inventory.tradedItem.RemoveFromScreen();
	}

	// gobble up inventory items from screen
	for (int idx = 0; idx < this.inventory.capacity; idx++) {
	    PickUpable item = this.inventory.inventoryItems[idx];
	    if (item == null || item.gameObject == null) {
		continue;
	    }
	    item.RemoveFromScreen();
	    this.inventory.AddItemIcon(item, idx, true); // preserve isUnlimited
	}

	// clean up and hide the items added in editor
	this.inventory.SetActive(false);
    }

    public void Open() {
	if (isOpen) {
	    return;
	}

	this.inventory.SetActive(true);
	this.isOpen = true;
    }

    public void Close() {
	if (!isOpen) {
	    return;
	}

	this.inventory.SetActive(false);
	this.isOpen = false;
    }
}
