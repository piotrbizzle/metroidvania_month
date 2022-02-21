using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
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
    
    // Update is called once per frame
    void Update()
    {
	MovePlayer();
    }


    void MovePlayer() 
    {
	// read inputs
        bool up = Input.GetKey("w");
	bool down = Input.GetKey("s");
	bool left = Input.GetKey("a");
	bool right = Input.GetKey("d");

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

	// drop item
	if (drop && !this.gameObject.GetComponent<Inventory>().IsEmpty()) {
	    ZoneItem inventoryItem = this.gameObject.GetComponent<Inventory>().Pop(0);
	    GameObject addedGo = inventoryItem.AddToScreen();
	    addedGo.transform.position = this.gameObject.transform.position + new Vector3(0.0f, 1.0f, 0.0f);

	    // give it a bit of a toss
	    Rigidbody2D inventoryItemRb = addedGo.GetComponent<Rigidbody2D>();
	    inventoryItemRb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
	    float tossForceX = this._facingRight ? 2 * tossForce : -2 * tossForce;;
	    inventoryItemRb.AddForce(new Vector2(tossForceX, tossForce), ForceMode2D.Impulse);

	    // add moar spin the faster you're moving
	    float tossTorqueModifier = this._facingRight ? 5.0f : -5.0f;
	    inventoryItemRb.AddTorque(tossTorqueModifier * rb.velocity.x);
	}

	// decrement onGround
	if (this._onGround > 0) {
	    this._onGround--;
	}
    }

    // OnTriggerStay2D is called when this collides with another BoxCollider2D w/ isTrigger=true
    void OnTriggerStay2D(Collider2D collider)
    {
	// check if the thing you collider with is attached to a zone item
	ZoneItem collidedZoneItem = collider.gameObject.transform.parent.gameObject.GetComponent<ZoneItem>();
	if (collidedZoneItem == null) {
	    return;
	}
	
	// reset jumps if it's a floor
	// TODO: maybe move this to onTriggerEnter
	// TODO: don't make this work for pickupable also
	if ((collidedZoneItem.isPickUpable || collidedZoneItem.isFloor) && this.gameObject.GetComponent<Rigidbody2D>().velocity.y < 0.1) {
	    this._jumpsRemaining = this._maxJumps;
	    this._onGround = 10;
	}

	// maybe pick up if it's an item
	bool pickup = Input.GetKey("e");
	if (pickup && collidedZoneItem != null && collidedZoneItem.isPickUpable) {
	    bool added = this.gameObject.GetComponent<Inventory>().Add(collidedZoneItem);
	    if (added) {
		collidedZoneItem.RemoveFromScreen();
	    }
	}
    }
}
