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
    
    public void OnPointerClick(PointerEventData pointerEventData) {
	// respond to left click only
	if (pointerEventData.button != PointerEventData.InputButton.Left) {
	    return;
	}

	// determine which containers to add to
	GameObject playerGo = GameObject.Find("Player");
	Inventory addingInventory = null;
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
	}
	
	// try to add it
	bool added = addingInventory.Add(this.parentPickUpable);

	// if added, remove from container
	if (added) {
	    this.parentInventory.Pop(this.slotIdx);
	}
    }

    public void OnPointerEnter(PointerEventData pointerEventData) {
	// assumes that parent of inventoryBackground will be a Canvas
	this.labelGo = new GameObject();
	this.labelGo.name = "Icon Label";  // TODO: move to constant
	this.labelGo.transform.SetParent(this.transform, false);

	RectTransform labelRectangle = labelGo.AddComponent<RectTransform>();
	labelRectangle.anchoredPosition = new Vector2(0f, 35f);
	
	Text labelText = this.labelGo.AddComponent<Text>();
	labelText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
	labelText.text = this.parentPickUpable.itemName;
	labelText.alignment = TextAnchor.MiddleCenter;
	
    }
    public void OnPointerExit(PointerEventData pointerEventData) {
	GameObject.Destroy(this.labelGo);
    }
}
