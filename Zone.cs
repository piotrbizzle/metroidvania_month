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
	    this.SetActive(false);	    
	}
    }

    public void SetActive(bool isActive) {
	foreach (Transform child in this.transform) {
	    child.gameObject.SetActive(isActive);
	}
	this.isActive = isActive;
    }

    public void AddGameObject(GameObject go) {
	go.transform.parent = this.gameObject.transform;
	if (!this.isActive) {
	    go.SetActive(false);
	}
    }
}
