using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // configurable
    public Zone currentZone;
    public Image inventorySelector;
    
    // constants
    private float speedForce = 300.0f;
    private float maxSpeed = 3.0f;
    private float jumpSpeed = 6.0f;
    private float tossForce = 1.0f;
    private float baseGroundBonus = 2.0f;

    // controls
    private bool _facingRight = false;

    private bool _jumpKeyDown = false;
    private int _jumpsRemaining = 0;
    private int _maxJumps = 2;  // change this back to 1 if you hate fun
    private int _onGround = 0;
    
    private bool _pickUpKeyDown = false;
    private bool _pickingUp = false;

    private bool _dropKeyDown = false;

    // pointers and indices
    public PickUpableContainer openContainer = null;
    public int selectedInventorySlot = 0;

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

	bool one = Input.GetKey("1");
	bool two = Input.GetKey("2");
	bool three = Input.GetKey("3");
	bool four = Input.GetKey("4");
	bool five = Input.GetKey("5");
	
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

	// change inventory selected slot
	if (one) {
	    this.UpdateInventorySelector(0);	
	} else if (two) {
	    this.UpdateInventorySelector(1);
	} else if (three) {
	    this.UpdateInventorySelector(2);
	} else if (four) {
	    this.UpdateInventorySelector(3);
	} else if (five) {
	    this.UpdateInventorySelector(4);
	}

	// pick up item
	if (!pickup) {
	    this._pickingUp = false;
	} else if (pickup && !this._pickUpKeyDown) {
	    this._pickingUp = true;
	}
	this._pickUpKeyDown = pickup;
	
	// drop item
	if (drop && !this._dropKeyDown && !this.gameObject.GetComponent<Inventory>().IsSlotEmpty(this.selectedInventorySlot)) {
	    PickUpable inventoryItem = this.gameObject.GetComponent<Inventory>().Pop(this.selectedInventorySlot);
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

	    // update selector to the next non-empty slot if there is one
	    int nextFullSlot = this.GetComponent<Inventory>().GetNextFullSlot(this.selectedInventorySlot);
	    if (nextFullSlot != -1) {
		this.UpdateInventorySelector(nextFullSlot);
	    }
	}
	this._dropKeyDown = drop;
	
	// decrement onGround
	if (this._onGround > 0) {
	    this._onGround--;
	}
    }

    /*
        MINGUS

         \_\
        /oo|
          ||____
          \     >
           || ||
     */

    // update selected slot and move UI component
    private void UpdateInventorySelector(int idx) {
	// do nothing if the slot is already selected
	if (idx == this.selectedInventorySlot) {
	    return;
	}

	// remove label of previously selected object
	Inventory inventory = this.GetComponent<Inventory>();
	GameObject[] iconGameObjects = inventory.iconGameObjects;
	if (!inventory.IsSlotEmpty(this.selectedInventorySlot)) {	    
	    iconGameObjects[this.selectedInventorySlot].GetComponent<InventoryIcon>().selected = false;
	    iconGameObjects[this.selectedInventorySlot].GetComponent<InventoryIcon>().RemoveLabel();
	}

	// display label of selected object if not already shown
	if (!inventory.IsSlotEmpty(idx)) {
	    if (!iconGameObjects[idx].GetComponent<InventoryIcon>().showingLabel) {
		iconGameObjects[idx].GetComponent<InventoryIcon>().AddLabel();
	    }
	    iconGameObjects[idx].GetComponent<InventoryIcon>().selected = true;
	}	

	// update selected slot
	this.selectedInventorySlot = idx;

	// update position of selector ui element
	RectTransform selectorRectangle = this.inventorySelector.GetComponent<RectTransform>();
	selectorRectangle.anchoredPosition = new Vector2(57f * (idx + 1 - (this.GetComponent<Inventory>().capacity + 1) / 2), selectorRectangle.anchoredPosition.y);      	
    }
    
    // Check if we're in a new zone and update if needed
    private void CheckCurrentZone() {
	// TODO: add up and down zones also
	if (this.transform.position.x < this.currentZone.gameObject.transform.position.x - this.currentZone.width / 2.0f) {
	    if (this.currentZone.leftZone != null) {
		if (this.currentZone.rightZone != null) {
		    this.currentZone.rightZone.SetActive(false);
		}
		this.currentZone = this.currentZone.leftZone;
		if (this.currentZone.leftZone) {
		    this.currentZone.leftZone.SetActive(true);
		}
	    }
	}
	else if (this.transform.position.x > this.currentZone.gameObject.transform.position.x + this.currentZone.width / 2.0f) {
	    if (this.currentZone.rightZone != null) {
		if (this.currentZone.leftZone != null) {	  
		    this.currentZone.leftZone.SetActive(false);
		}
		this.currentZone = this.currentZone.rightZone;
		if (this.currentZone.rightZone) {
		    this.currentZone.rightZone.SetActive(true);
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

	PickUpableContainer collidedContainer = collider.gameObject.GetComponent<PickUpableContainer>();
	
	// reset jumps if it's a floor
	// TODO: maybe move this to onTriggerEnter
	if (collidedPlatform != null && this.gameObject.GetComponent<Rigidbody2D>().velocity.y < 0.1) {
	    this._jumpsRemaining = this._maxJumps;
	    this._onGround = 10;
	}

	// maybe pick up if it's an item
	if (this._pickingUp && collidedPickUpable != null) {
	    int addedIdx = this.gameObject.GetComponent<Inventory>().Add(collidedPickUpable);
	    if (addedIdx != -1) {
		collidedPickUpable.RemoveFromScreen();
		this._pickingUp = false;

		// add a label if the item is added to selected slot
		if (this.selectedInventorySlot == addedIdx) {
		    InventoryIcon inventoryIcon = this.GetComponent<Inventory>().iconGameObjects[addedIdx].GetComponent<InventoryIcon>();
		    inventoryIcon.AddLabel();
    		    inventoryIcon.selected = true;;
		}
	    }
	}
    }

    void OnTriggerEnter2D(Collider2D collider) {
	PickUpableContainer collidedContainer = collider.gameObject.GetComponent<PickUpableContainer>();

	// maybe open a container
	if (collidedContainer != null) {
	    collidedContainer.Open();
	    this.openContainer = collidedContainer;
	}
    }

    // Ontriggerexit2d called when this stops colliding with another BoxCollider2D w/ isTrigger=true
    void OnTriggerExit2D(Collider2D collider) {
	PickUpableContainer collidedContainer = collider.gameObject.GetComponent<PickUpableContainer>();

	// maybe open a container
	if (collidedContainer != null) {
	    collidedContainer.Close();
    	    this.openContainer = null;
	}
    }
}
