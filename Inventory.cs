using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private int capacity = 0;

    [SerializeField]
    private List<ZoneItem> inventoryItems = new List<ZoneItem>();
 
    public bool Add(ZoneItem elt) {
	if (this.inventoryItems.Count >= capacity) {
	    return false;
	}
	this.inventoryItems.Add(elt);
	return true;
    }

    public ZoneItem Pop(int idx) {
	ZoneItem elt = this.inventoryItems[idx];
	this.inventoryItems.RemoveAt(idx);
	return elt;
    }

    public bool IsEmpty() {
	return this.inventoryItems.Count == 0;
    }
}
