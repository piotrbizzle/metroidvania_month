using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public int blocksX;
    public int blocksY;
    public Materials.Material material;
    public float floatingY;
    public float sinkingY;
    public bool isMoveable;
    private float isFlashing = 0.0f;
    private float maxFlash = 0.1f; // time in seconds to flash
    private bool isMoving;

    
    // Start is called before the first frame update
    void Start()
    {
	// set color from material
	SpriteRenderer sr = this.GetComponent<SpriteRenderer>();
	sr.color = Materials.MaterialToColor[this.material];
	
	// add physics
	this.gameObject.AddComponent<Rigidbody2D>();
	this.gameObject.AddComponent<BoxCollider2D>();

	// freeze in place
	this.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;	    

	// add trigger to reset jumps
	GameObject jumpTriggerGo = new GameObject(this.name + " Floor Trigger");
	jumpTriggerGo.AddComponent<JumpTrigger>();
	jumpTriggerGo.AddComponent<BoxCollider2D>();
	jumpTriggerGo.GetComponent<BoxCollider2D>().isTrigger = true;
	jumpTriggerGo.transform.parent = this.gameObject.transform;
	jumpTriggerGo.transform.localScale = new Vector3(1.25f * this.blocksX, 1.25f * this.blocksY, 1.0f);
	jumpTriggerGo.transform.localPosition = new Vector3(0.0f, 0.25f, 0.0f);
	jumpTriggerGo.layer = 7;

	// add trigger to detect potions. Keep on default layer.
	GameObject potionTriggerGo = new GameObject(this.name + " Potion Trigger");
	potionTriggerGo.AddComponent<PotionTrigger>();
	potionTriggerGo.AddComponent<BoxCollider2D>();
	potionTriggerGo.GetComponent<BoxCollider2D>().isTrigger = true;
	potionTriggerGo.transform.parent = this.gameObject.transform;
	potionTriggerGo.transform.localScale = new Vector3(1.25f * this.blocksX + 0.1f, 1.25f * this.blocksY + 0.1f, 1.0f);
	potionTriggerGo.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

	// set initial location as floating or sinking height based on material
	if (Materials.DoesFloat(this.material)) {
	    this.floatingY = this.transform.position.y;
	} else {
	    this.sinkingY = this.transform.position.y;
	}
    }

    void Update() {
	// maybe stop flashing white after impact
	if (this.isFlashing > 0) {
	    this.isFlashing -= Time.deltaTime;
	}
	if (this.isFlashing <= 0) {
	    // changing to air deletes the platform, otherwise change color
	    if (this.material == Materials.Material.Air) {
		GameObject.Destroy(this.gameObject);
	    } else {
		SpriteRenderer sr = this.GetComponent<SpriteRenderer>();
		sr.color = Materials.MaterialToColor[this.material];
	    }	    
	}
	    
	// move floating or sinking platform
	if (!this.isMoveable || !this.isMoving) {
	    return;
	}

	if (Materials.DoesFloat(this.material)) {
	    this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.02f, 0.0f);
	    if (this.transform.position.y > this.floatingY) {
		this.isMoving = false;
	    }
	} else {
	    this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 0.02f, 0.0f);
	    if (this.transform.position.y < this.sinkingY) {
		this.isMoving = false;
	    }
	}		
    }

    public void ChangeMaterial(Materials.Material newMaterial) {
	// otherwise, update material and color
	if (Materials.DoesFloat(this.material) != Materials.DoesFloat(newMaterial)) {
	    this.isMoving = true;
	}
	
	this.material = newMaterial;		
    }

    public void StartFlashing() {
	SpriteRenderer sr = this.gameObject.GetComponent<SpriteRenderer>();
	sr.color = new Color(1.0f, 1.0f, 1.0f);
	this.isFlashing = this.maxFlash;
    }
}
