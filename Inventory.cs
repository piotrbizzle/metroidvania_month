using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private int capacity = 0;

    public List<PickUpable> inventoryItems = new List<PickUpable>();

    [SerializeField]
    private Image inventoryBackground;

    private List<GameObject> iconGameObjects = new List<GameObject>();
    
    public bool Add(PickUpable item) {
	if (this.inventoryItems.Count >= capacity) {
	    return false;
	}
	this.inventoryItems.Add(item);
	this.AddItemIcon(item);
       
	return true;
    }

    public void AddItemIcon(PickUpable item) {
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
	itemRectangle.anchoredPosition = new Vector2(backgroundPosition.x + 47.5f * (this.inventoryItems.Count - (this.capacity + 1) / 2), backgroundPosition.y);
	itemRectangle.sizeDelta = new Vector2(40f, 40f); 

	Image itemImage = itemIconGo.AddComponent<Image>();
	itemImage.sprite = item.sprite;
	itemIconGo.transform.SetParent(canvas.transform, false);

	InventoryIcon icon = itemIconGo.AddComponent<InventoryIcon>();
	icon.parentInventory = this;
	icon.parentPickUpable = item;

	this.iconGameObjects.Add(itemIconGo);
    }

    public PickUpable Pop(int idx) {
	PickUpable item = this.inventoryItems[idx];
	this.inventoryItems.RemoveAt(idx);

	// remove this item from a ui if applicable
	if (this.inventoryBackground != null) {
	    // assumes that parent of inventoryBackground will be a Canvas
	    GameObject.Destroy(this.iconGameObjects[idx]);
	    this.iconGameObjects.RemoveAt(idx);
	}

	return item;
    }

    // TODO: remove this method when you make inventory shift items on drop
    public PickUpable PopLast() {
	return this.Pop(this.inventoryItems.Count - 1);
    }

    public bool IsEmpty() {
	return this.inventoryItems.Count == 0;
    }

    public void SetActive(bool isActive) {
	this.inventoryBackground.gameObject.SetActive(isActive);
	foreach (GameObject go in this.iconGameObjects) {
	    go.SetActive(isActive);
	}
    }
}
