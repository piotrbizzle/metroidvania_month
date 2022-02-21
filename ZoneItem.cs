using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneItem : MonoBehaviour
{
    // TODO: make these options an enum or write as subclasses if they are mutually exclusive
    public bool isPickUpable;
    public bool isFloor;

    // inferred fields
    private string zoneItemName;
    private Sprite sprite;
    private Vector3 position;
    private bool placedProgramatically = false;

    // Start is called before the first frame update
    void Start()
    {	
	// if this was placed from editor, add some stuff every ZoneItem needs
	if (!this.placedProgramatically) {
	    this.gameObject.AddComponent<Rigidbody2D>();
	    this.gameObject.AddComponent<BoxCollider2D>();
	}

	// handle special ZoneItem types. See TODO at top of the class
	if (this.isPickUpable) {
	    // make bouncy :)
   	    // this.gameObject.GetComponent<BoxCollider2D>().sharedMaterial = Resources.Load("Bouncy") as PhysicsMaterial2D;

	    // add trigger for pickup
	    GameObject triggerGo = new GameObject(this.zoneItemName + " PickUp Trigger"); 
	    triggerGo.AddComponent<BoxCollider2D>();
	    triggerGo.GetComponent<BoxCollider2D>().isTrigger = true;
	    triggerGo.transform.parent = this.gameObject.transform;
	    triggerGo.transform.localScale = new Vector3(1.1f, 1.1f, 1.0f);
	    triggerGo.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
	}
	else if (this.isFloor) {
	    // freeze in place
	    this.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;	    

	    // add trigger to reset jumps
	    GameObject triggerGo = new GameObject(this.zoneItemName + " Floor Trigger"); 
	    triggerGo.AddComponent<BoxCollider2D>();
	    triggerGo.GetComponent<BoxCollider2D>().isTrigger = true;
	    triggerGo.transform.parent = this.gameObject.transform;
	    triggerGo.transform.localScale = new Vector3(0.98f, 1.0f, 1.0f);
	    triggerGo.transform.localPosition = new Vector3(0.0f, 0.2f, 0.0f);
	}
    }

    public void RemoveFromScreen() {
	// save properties of gameObject before removing
	this.zoneItemName = this.gameObject.name;
       	this.sprite = this.GetComponent<SpriteRenderer>().sprite;
	this.position = this.gameObject.transform.position;

	// then remove
	GameObject.Destroy(this.gameObject);
    }

    public GameObject AddToScreen() {
	// add a new gameObject with a new zoneItem
	GameObject go = new GameObject(this.zoneItemName);
	go.AddComponent<SpriteRenderer>();
	go.GetComponent<SpriteRenderer>().sprite = this.sprite;
	go.AddComponent<Rigidbody2D>();
	go.AddComponent<BoxCollider2D>();

	go.AddComponent<ZoneItem>();

	// copy all the properties over to the new ZoneItem
	ZoneItem newZoneItem = go.GetComponent<ZoneItem>();
	newZoneItem.placedProgramatically = true;
	newZoneItem.isPickUpable = this.isPickUpable;
	newZoneItem.sprite = this.sprite;
	newZoneItem.zoneItemName = this.zoneItemName;

	// position the gameObject
	go.transform.Translate(this.position);

	return go;
    }
}
