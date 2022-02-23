using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // configurable
    public Zone currentZone;
    
    // constants
    private float speedForce = 300.0f;
    private float maxSpeed = 3.0f;
    private float jumpSpeed = 6.0f;
    private float tossForce = 3.0f;
    private float baseGroundBonus = 2.0f;

    private bool _facingRight = false;

    private bool _jumpKeyDown = false;
    private int _jumpsRemaining = 0;
    private int _maxJumps = 2;  // change this back to 1 if you hate fun
    private int _onGround = 0;

    private bool _pickUpKeyDown = false;
    private bool _pickingUp = false;

    private bool _dropKeyDown = false;

    void Start()
    {
	this.gameObject.layer = 6;
    }
    
    // Update is called once per frame
    void Update()
    {
	this.MovePlayer();
	this.CheckCurrentZone();
    }

    private void MovePlayer() 
    {
	// read inputs
        bool up = Input.GetKey("w");
	bool down = Input.GetKey("s");
	bool left = Input.GetKey("a");
	bool right = Input.GetKey("d");
	bool pickup = Input.GetKey("e");	
	bool drop = Input.GetKey("q");
	
	// movement
	Rigidbody2D rb = this.gameObject.GetComponent<Rigidbody2D>();
	float groundBonus = this._onGround > 0 ? this.baseGroundBonus : 1.0f;
	if (left && !right && rb.velocity.x > -1 * maxSpeed * groundBonus) {
	    rb.AddForce(new Vector2(-1 * speedForce * groundBonus * Time.deltaTime, 0));
	    this._facingRight = false;
	}
	if (right && !left && rb.velocity.x < maxSpeed * groundBonus) {
	    rb.AddForce(new Vector2(speedForce * Time.deltaTime * groundBonus, 0));
	    this._facingRight = true;
	}
	
	if (up && !this._jumpKeyDown && this._jumpsRemaining > 0) {
	    this._jumpsRemaining--;
	    gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.GetComponent<Rigidbody2D>().velocity.x, jumpSpeed);
	}
	this._jumpKeyDown = up;

	// pick up item
	if (!pickup) {
	    this._pickingUp = false;
	} else if (pickup && !this._pickUpKeyDown) {
	    this._pickingUp = true;
	}
	this._pickUpKeyDown = pickup;
	
	// drop item
	if (drop && !this._dropKeyDown && !this.gameObject.GetComponent<Inventory>().IsEmpty()) {
	    PickUpable inventoryItem = this.gameObject.GetComponent<Inventory>().PopLast();
	    GameObject addedGo = inventoryItem.AddToScreen();
	    addedGo.transform.position = this.gameObject.transform.position + new Vector3(0.0f, 1.0f, 0.0f);
	    addedGo.transform.parent = this.currentZone.gameObject.transform;

	    // give it a bit of a toss
	    Rigidbody2D inventoryItemRb = addedGo.GetComponent<Rigidbody2D>();
	    inventoryItemRb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
	    float tossForceX = this._facingRight ? 2 * tossForce : -2 * tossForce;;
	    inventoryItemRb.AddForce(new Vector2(tossForceX, tossForce), ForceMode2D.Impulse);

	    // add moar spin the faster you're moving
	    float tossTorqueModifier = this._facingRight ? 5.0f : -5.0f;
	    inventoryItemRb.AddTorque(tossTorqueModifier * rb.velocity.x);
	}
	this._dropKeyDown = drop;

	// decrement onGround
	if (this._onGround > 0) {
	    this._onGround--;
	}
    }

    // Check if we're in a new zone and update if needed
    private void CheckCurrentZone() {
	// TODO: add up and down zones also
	if (this.transform.position.x < this.currentZone.gameObject.transform.position.x - this.currentZone.width / 2.0f) {
	    if (this.currentZone.leftZone != null) {
		if (this.currentZone.rightZone != null) {
		    this.currentZone.rightZone.DeactivateAll();
		}
		this.currentZone = this.currentZone.leftZone;
		if (this.currentZone.leftZone) {
		    this.currentZone.leftZone.ActivateAll();
		}
	    }
	}
	else if (this.transform.position.x > this.currentZone.gameObject.transform.position.x + this.currentZone.width / 2.0f) {
	    if (this.currentZone.rightZone != null) {
		if (this.currentZone.leftZone != null) {	  
		    this.currentZone.leftZone.DeactivateAll();
		}
		this.currentZone = this.currentZone.rightZone;
		if (this.currentZone.rightZone) {
		    this.currentZone.rightZone.ActivateAll();
		}
	    }
	}
    }

    // Ontriggerstay2d called when this collides with another BoxCollider2D w/ isTrigger=true
    void OnTriggerStay2D(Collider2D collider)
    {
	// check if the thing you collider with is attached to a zone item
	PickUpable collidedPickUpable = collider.gameObject.transform.parent.gameObject.GetComponent<PickUpable>();

	Platform collidedPlatform = collider.gameObject.transform.parent.gameObject.GetComponent<Platform>();
	
	// reset jumps if it's a floor
	// TODO: maybe move this to onTriggerEnter
	if (collidedPlatform != null && this.gameObject.GetComponent<Rigidbody2D>().velocity.y < 0.1) {
	    this._jumpsRemaining = this._maxJumps;
	    this._onGround = 10;
	}

	// maybe pick up if it's an item
	if (this._pickingUp && collidedPickUpable != null) {
	    bool added = this.gameObject.GetComponent<Inventory>().Add(collidedPickUpable);
	    if (added) {
		collidedPickUpable.RemoveFromScreen();
		this._pickingUp = false;
	    }
	}
    }
}
