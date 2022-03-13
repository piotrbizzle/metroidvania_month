using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpableThrower : MonoBehaviour
{
    public PickUpable thrownItem;

    public bool facingRight;
    public float tossForce = 3.0f;
    public float maxThrowPeriod = 1.0f; // in seconds
    private float currentThrowPeriod;
    private float tossTorque = 1.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        if (thrownItem != null && thrownItem.gameObject != null) {
	    thrownItem.RemoveFromScreen();
	}
    }

    // Update is called once per frame
    void Update()
    {
	// timer
	this.currentThrowPeriod -= Time.deltaTime;
	if (this.currentThrowPeriod > 0) {
	    return;
	}
	this.currentThrowPeriod = this.maxThrowPeriod;

	// throw the object
	GameObject addedGo = this.thrownItem.AddToScreen();
	addedGo.transform.position = this.gameObject.transform.position + new Vector3(0.5f * (this.facingRight ? 1 : -1), 1.0f, 0.0f);

	// assumes parent of this is the zone
	addedGo.transform.parent = this.gameObject.transform.parent;

	// add force
	Rigidbody2D inventoryItemRb = addedGo.GetComponent<Rigidbody2D>();
	float tossForceX = this.tossForce * (this.facingRight ? 2 : -2);
	inventoryItemRb.AddForce(new Vector2(tossForceX, tossForce), ForceMode2D.Impulse);

	// add torque
	inventoryItemRb.AddTorque(this.tossTorque);        
    }
}
