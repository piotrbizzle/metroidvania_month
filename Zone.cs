using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    // TODO: add up and down zones also
    public Zone leftZone;
    public Zone rightZone;
    public bool isStartingZone;
    public bool isActive = true;
    
    // constants
    public float width = 20.5f; // fix the size of the zones to be less dumb
    
    public void Start() {
	if (!this.isStartingZone && (this.leftZone == null || !this.leftZone.isStartingZone) && (this.rightZone == null || !this.rightZone.isStartingZone)) {
	    this.DeactivateAll();	    
	}
    }

    public void DeactivateAll() {
	foreach (Transform child in this.transform) {
	    child.gameObject.SetActive(false);
	}
	this.isActive = false;
    }

    public void ActivateAll() {
	foreach (Transform child in this.transform) {
	    child.gameObject.SetActive(true);
	}
	this.isActive = true;
    }

    public void AddGameObject(GameObject go) {
	go.transform.parent = this.gameObject.transform;
	if (!this.isActive) {
	    go.SetActive(false);
	}
    }
}
