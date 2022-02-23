using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private int capacity = 0;

    [SerializeField]
    private List<PickUpable> inventoryItems = new List<PickUpable>();

    // TODO: probably attach inventories to Images instead of Canvases
    [SerializeField]
    private Canvas canvas;

    private List<GameObject> iconGameObjects = new List<GameObject>();
    
    public bool Add(PickUpable elt) {
	if (this.inventoryItems.Count >= capacity) {
	    return false;
	}
	this.inventoryItems.Add(elt);
	
	// add this item to a ui is applicable
	if (this.canvas != null) {
	    GameObject itemIconGo = new GameObject(elt.itemName);

	    // TODO: update positions and sizes for real ui, make them variable
	    RectTransform itemRectangle = itemIconGo.AddComponent<RectTransform>();
	    itemRectangle.anchoredPosition = new Vector2(-47.5f + 47.5f * (this.inventoryItems.Count - 1), -275f);
	    itemRectangle.sizeDelta = new Vector2(40f, 40f); 

	    Image itemImage = itemIconGo.AddComponent<Image>();
	    itemImage.sprite = elt.sprite;

	    itemIconGo.transform.SetParent(this.canvas.transform, false);

	    this.iconGameObjects.Add(itemIconGo);
	}

	return true;
    }

    public PickUpable Pop(int idx) {
	PickUpable elt = this.inventoryItems[idx];
	this.inventoryItems.RemoveAt(idx);

	// remove this item from a ui if applicable
	if (this.canvas != null) {	    
	    GameObject.Destroy(this.iconGameObjects[idx]);
	    this.iconGameObjects.RemoveAt(idx);
	}

	return elt;
    }

    // TODO: remove this method when you make inventory shift items on drop
    public PickUpable PopLast() {
	return this.Pop(this.inventoryItems.Count - 1);
    }

    public bool IsEmpty() {
	return this.inventoryItems.Count == 0;
    }
}
