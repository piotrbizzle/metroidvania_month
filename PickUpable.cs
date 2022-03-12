using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpable : MonoBehaviour
{
    // configuration
    // TODO: maybe subclass instead
    public bool isBreakable;
    public Materials.Material inMaterial;
    public Materials.Material outMaterial;
    public bool isUnlimited;
    
    // inferred fields
    public Sprite sprite;
    public string itemName;
    private Vector3 position;
    private bool placedProgramatically = false;

    // Start is called before the first frame update
    void Start()
    {
	// init some inferred fields
	this.itemName = this.gameObject.name;
       	this.sprite = this.GetComponent<SpriteRenderer>().sprite;

	// move to separate layer from player
	this.gameObject.layer = 3;
	
	// if this was placed from editor, add some stuff every PickUpable needs
	if (!this.placedProgramatically) {
	    this.gameObject.AddComponent<Rigidbody2D>();
	    this.gameObject.AddComponent<BoxCollider2D>();
	}

	// add trigger for pickup
	GameObject triggerGo = new GameObject(this.name + " PickUp Trigger"); 
	triggerGo.AddComponent<BoxCollider2D>().isTrigger = true;
	triggerGo.transform.parent = this.gameObject.transform;
	triggerGo.transform.localScale = new Vector3(1.1f, 1.1f, 1.0f);
	triggerGo.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
    }

    // Update is called once per frame
    void Update() {
	this.CheckZone();
    }

    private void CheckZone () {
	if (this.transform.position.x < this.transform.parent.position.x - this.transform.parent.GetComponent<Zone>().width / 2.0f) {
	    if (this.transform.parent.GetComponent<Zone>().leftZone != null) {  
		this.transform.parent.GetComponent<Zone>().leftZone.AddGameObject(this.gameObject);
	    }
	}
	else if (this.transform.position.x > this.transform.parent.position.x + this.transform.parent.GetComponent<Zone>().width / 2.0f) {
	    if (this.transform.parent.GetComponent<Zone>().rightZone != null) {  
		this.transform.parent.GetComponent<Zone>().rightZone.AddGameObject(this.gameObject);
	    }
	}

    }
    
    public void RemoveFromScreen() {
	// save properties of gameObject before removing
	this.itemName = this.gameObject.name;
       	this.sprite = this.GetComponent<SpriteRenderer>().sprite;
	this.position = this.gameObject.transform.position;

	// then remove
	GameObject.Destroy(this.gameObject);
    }

    public GameObject AddToScreen() {
	// add a new gameObject with a new PickUpable
	GameObject go = new GameObject(this.itemName);
	go.AddComponent<SpriteRenderer>().sprite = this.sprite;
	go.AddComponent<Rigidbody2D>();
	go.AddComponent<BoxCollider2D>();
	go.AddComponent<PickUpable>();

	// copy all the properties over to the new PickUpable. This is kinda dumb
	PickUpable newPickUpable = go.GetComponent<PickUpable>();
	newPickUpable.placedProgramatically = true;
	newPickUpable.sprite = this.sprite;
	newPickUpable.itemName = this.itemName;
	newPickUpable.isBreakable = this.isBreakable;
	newPickUpable.isUnlimited = this.isUnlimited;
	newPickUpable.inMaterial = this.inMaterial;
	newPickUpable.outMaterial = this.outMaterial;
	
	// position the gameObject
	go.transform.Translate(this.position);

	return go;
    }

    void OnTriggerEnter2D(Collider2D collider) {
	// non-breakable items don't break
	if (!this.isBreakable) {
	    return;
	}
	
	// try to get platform if there is one
	PotionTrigger potionTrigger = collider.gameObject.GetComponent<PotionTrigger>();
	
	// non-potionTriggers don't break items
	if (potionTrigger == null) {
	    return;
	}	
	Platform platform = collider.gameObject.transform.parent.GetComponent<Platform>();

		
	// TODO: visually indicate breaking
	
	// remove from screen and end early if there is no material transition
	if (this.inMaterial == Materials.Material.None && this.outMaterial == Materials.Material.None) {
	    this.RemoveFromScreen();
	    return;
	}

	// remove from screen and end early if platform material is not a match
	if (platform.material != this.inMaterial) {
    	    this.RemoveFromScreen();
	    return;
	}

	// otherwise, it's a match! change platform material
	platform.ChangeMaterial(this.outMaterial);
	this.RemoveFromScreen();
    }
}
