using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private int capacity = 0;

    [SerializeField]
    private List<PickUpable> inventoryItems = new List<PickUpable>();
 
    public bool Add(PickUpable elt) {
	if (this.inventoryItems.Count >= capacity) {
	    return false;
	}
	this.inventoryItems.Add(elt);
	return true;
    }

    public PickUpable Pop(int idx) {
	PickUpable elt = this.inventoryItems[idx];
	this.inventoryItems.RemoveAt(idx);
	return elt;
    }

    public bool IsEmpty() {
	return this.inventoryItems.Count == 0;
    }
}
