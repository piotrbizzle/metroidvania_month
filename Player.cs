using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // configurable
    public Zone currentZone;
    public Image inventorySelector;
    public int maxJumps = 1; 
    public Sprite[] sprites;
    
    // constants
    private float speedForce = 500.0f;
    private float maxSpeed = 3.0f;
    private float jumpSpeed = 8.0f;
    private float tossForce = 3.0f;
    private float baseGroundBonus = 2.0f;

    // controls
    private bool facingRight = false;
    private bool jumpKeyDown = false;
    private int jumpsRemaining = 0;
    private int onGround = 0;
    
    private bool pickUpKeyDown = false;
    private bool pickingUp = false;

    private bool dropKeyDown = false;

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
	float groundBonus = this.onGround > 0 ? this.baseGroundBonus : 1.0f;
	if (left && !right && rb.velocity.x > -1 * maxSpeed * groundBonus) {
	    rb.AddForce(new Vector2(-1 * speedForce * groundBonus * Time.deltaTime, 0));
	    this.facingRight = false;
	    this.GetComponent<SpriteRenderer>().sprite = this.sprites[0];
	}
	if (right && !left && rb.velocity.x < maxSpeed * groundBonus) {
	    rb.AddForce(new Vector2(speedForce * Time.deltaTime * groundBonus, 0));
	    this.facingRight = true;
	    this.GetComponent<SpriteRenderer>().sprite = this.sprites[2];
	}
	
	if (up && !this.jumpKeyDown && this.jumpsRemaining > 0) {
	    this.jumpsRemaining--;
	    gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.GetComponent<Rigidbody2D>().velocity.x, jumpSpeed);
	}
	this.jumpKeyDown = up;

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
	    this.pickingUp = false;
	} else if (pickup && !this.pickUpKeyDown) {
	    this.pickingUp = true;
	}
	this.pickUpKeyDown = pickup;
	
	// drop item
	if (drop && !this.dropKeyDown && !this.gameObject.GetComponent<Inventory>().IsSlotEmpty(this.selectedInventorySlot)) {
	    PickUpable inventoryItem = this.gameObject.GetComponent<Inventory>().Pop(this.selectedInventorySlot);
	    inventoryItem.OnDrop(this);

	    GameObject addedGo = inventoryItem.AddToScreen();
	    addedGo.transform.position = this.gameObject.transform.position + new Vector3(0.5f * (this.facingRight ? 1 : -1), 1.0f, 0.0f);
	    addedGo.transform.parent = this.currentZone.gameObject.transform;

	    // give it a bit of a toss
	    Rigidbody2D inventoryItemRb = addedGo.GetComponent<Rigidbody2D>();
	    inventoryItemRb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
	    float tossForceX = this.facingRight ? 2 * tossForce : -2 * tossForce;;
	    inventoryItemRb.AddForce(new Vector2(tossForceX, tossForce), ForceMode2D.Impulse);

	    // add moar spin the faster you're moving
	    float tossTorqueModifier = this.facingRight ? 5.0f : -5.0f;
	    inventoryItemRb.AddTorque(tossTorqueModifier * rb.velocity.x);

	    // update selector to the next non-empty slot if there is one
	    int nextFullSlot = this.GetComponent<Inventory>().GetNextFullSlot(this.selectedInventorySlot);
	    if (nextFullSlot != -1) {
		this.UpdateInventorySelector(nextFullSlot);
	    }
	}
	this.dropKeyDown = drop;
	
	// decrement onGround
	if (this.onGround > 0) {
	    this.onGround--;
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
    public void UpdateInventorySelector(int idx) {
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
	// reset jumps if colldier is a floor
	JumpTrigger collidedJumpTrigger = collider.gameObject.GetComponent<JumpTrigger>();

	if (collidedJumpTrigger != null && this.gameObject.GetComponent<Rigidbody2D>().velocity.y < 0.1) {
	    this.jumpsRemaining = this.maxJumps;
	    this.onGround = 10;
	}

	// maybe pick up if collider is an item
	PickUpable collidedPickUpable = collider.gameObject.transform.parent.gameObject.GetComponent<PickUpable>();

	if (this.pickingUp && collidedPickUpable != null) {
	    int addedIdx = this.gameObject.GetComponent<Inventory>().Add(collidedPickUpable);

	    // successful pickup
	    if (addedIdx != -1) {
		collidedPickUpable.OnPickup(this);
		collidedPickUpable.RemoveFromScreen();
		this.pickingUp = false;

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
	// open if collider is a container
	PickUpableContainer collidedContainer = collider.gameObject.GetComponent<PickUpableContainer>();
	if (collidedContainer != null) {
	    collidedContainer.Open();
	    this.openContainer = collidedContainer;
	}

	if (collider.gameObject.transform.parent != null) {
	    // show text if collider has dialogue
	    Dialogue collidedDialogue = collider.gameObject.transform.parent.GetComponent<Dialogue>();
	    if (collidedDialogue != null) {
		collidedDialogue.AddText();
	    }

	    // teleport if collider is a teleporter
    	    Teleporter collidedTeleporter = collider.gameObject.transform.parent.GetComponent<Teleporter>();
    	    if (collidedTeleporter != null) {
		this.transform.position = new Vector3(collidedTeleporter.destinationX, collidedTeleporter.destinationY, 0.0f); 
	    }
	}
    }

    // Ontriggerexit2d called when this stops colliding with another BoxCollider2D w/ isTrigger=true
    void OnTriggerExit2D(Collider2D collider) {
	// close if collider is a container
	PickUpableContainer collidedContainer = collider.gameObject.GetComponent<PickUpableContainer>();

	if (collidedContainer != null) {
	    collidedContainer.Close();
    	    this.openContainer = null;
	}

	// remove text if collider has dialogue
	if (collider.gameObject.transform.parent != null) {
	    Dialogue collidedDialogue = collider.gameObject.transform.parent.GetComponent<Dialogue>();
	    if (collidedDialogue != null) {
		collidedDialogue.RemoveText();
	    }
	}	    
    }
}
