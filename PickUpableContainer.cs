using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpableContainer : MonoBehaviour
{

    [SerializeField]
    private Inventory inventory;

    private bool isOpen = false;
    
    // Start is called before the first frame update
    void Start()
    {
	BoxCollider2D collider = this.gameObject.AddComponent<BoxCollider2D>();
	collider.isTrigger = true;

	foreach (PickUpable item in this.inventory.inventoryItems) {
	    if (item.gameObject != null) {
		item.RemoveFromScreen();
		this.inventory.AddItemIcon(item);
	    }
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
