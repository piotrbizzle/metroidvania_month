using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryIcon : MonoBehaviour, IPointerClickHandler
{
    public Inventory parentInventory;
    public PickUpable parentPickUpable; // name this better
    
    public void OnPointerClick(PointerEventData pointerEventData) {
	// check if this object is in the player's inventory
	if (this.parentInventory.gameObject.GetComponent<Player>() != null) {
	    return;
	}
	
	// if not, try to add it
	GameObject playerGo = GameObject.Find("Player");
	Inventory playerInventory = playerGo.GetComponent<Inventory>();
	bool added = playerInventory.Add(this.parentPickUpable);

	// if added, remove from container
	// TODO handle multiple items in a container
	if (added) {
	    this.parentInventory.PopLast();
	}
    }
}
