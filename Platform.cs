using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
	this.gameObject.AddComponent<Rigidbody2D>();
	this.gameObject.AddComponent<BoxCollider2D>();

	// freeze in place
	this.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;	    

	// add trigger to reset jumps
	GameObject triggerGo = new GameObject(this.name + " Floor Trigger"); 
	triggerGo.AddComponent<BoxCollider2D>();
	triggerGo.GetComponent<BoxCollider2D>().isTrigger = true;
	triggerGo.transform.parent = this.gameObject.transform;
	triggerGo.transform.localScale = new Vector3(0.98f, 1.0f, 1.0f);
	triggerGo.transform.localPosition = new Vector3(0.0f, 0.2f, 0.0f);      
    }
}
