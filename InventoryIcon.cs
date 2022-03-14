using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryIcon : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Inventory parentInventory;
    public PickUpable parentPickUpable; // name this better
    public int slotIdx;

    public GameObject labelGo;

    public bool selected;
    public bool showingLabel;

    public bool isUnlimited;
    
    public void OnPointerClick(PointerEventData pointerEventData) {
	// respond to left click only
	if (pointerEventData.button != PointerEventData.InputButton.Left) {
	    return;
	}

	// determine which containers to add to
	GameObject playerGo = GameObject.Find("Player");
	Player player = playerGo.GetComponent<Player>();
	Inventory addingInventory = null;
	bool addingToPlayer = false;
	if (this.parentInventory.gameObject.GetComponent<Player>() != null) {
	    // if in player's inventory, add to opened container
	    PickUpableContainer openContainer = playerGo.GetComponent<Player>().openContainer;
	    if (openContainer == null) {
		return;
	    }       		    
	    addingInventory = openContainer.inventory;
	} else {
	    // otherwise, add to player's inventory
	    addingInventory = playerGo.GetComponent<Inventory>();
	    addingToPlayer = true;
	}
	
	// item pickup / drop effects
	if (addingToPlayer) {
	    this.parentPickUpable.OnPickup(player);
	} else {
	    this.parentPickUpable.OnDrop(player);
	}

	// try to add it
	int addedIdx = addingInventory.Add(this.parentPickUpable);

	// if added, remove from container if not unlimited
	if (addedIdx != -1) {
	    // switch selected item
	    player.UpdateInventorySelector(addedIdx);

	    if (!this.isUnlimited) {
		this.parentInventory.Pop(this.slotIdx);
	    }
	}
    }

    public void OnPointerEnter(PointerEventData pointerEventData) {
	// selected items always show label
	if (this.selected) {
	    return;
	}
	this.AddLabel();	
    }

    public void AddLabel() {
	// assumes that parent of inventoryBackground will be a Canvas
	this.labelGo = new GameObject();
	this.labelGo.name = "Icon Label";
	this.labelGo.transform.SetParent(this.transform, false);

	// TODO: better fix for overlapping labels than this, please @_@
	RectTransform labelRectangle = labelGo.AddComponent<RectTransform>();
	labelRectangle.anchoredPosition = new Vector2(0f, 41f + (this.slotIdx % 2 == 1 ? 15f : 0f));
	
	Text labelText = this.labelGo.AddComponent<Text>();
	labelText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
	labelText.text = this.parentPickUpable.itemName;
	labelText.alignment = TextAnchor.MiddleCenter;

	this.labelGo.AddComponent<Outline>();
	
	this.showingLabel = true;
    }
    
    public void OnPointerExit(PointerEventData pointerEventData) {
	// selected items always show label
	if (this.selected) {
	    return;
	}
	this.RemoveLabel();
    }

    public void RemoveLabel() {
	GameObject.Destroy(this.labelGo);
	this.showingLabel = false;
    }
}
