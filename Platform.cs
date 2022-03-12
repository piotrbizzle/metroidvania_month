using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public Materials.Material material;
    
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
	GameObject triggerGo = new GameObject(this.name + " Floor Trigger"); 
	triggerGo.AddComponent<BoxCollider2D>();
	triggerGo.GetComponent<BoxCollider2D>().isTrigger = true;
	triggerGo.transform.parent = this.gameObject.transform;
	triggerGo.transform.localScale = new Vector3(0.98f, 1.0f, 1.0f);
	triggerGo.transform.localPosition = new Vector3(0.0f, 0.2f, 0.0f);      
    }

    public void ChangeMaterial(Materials.Material newMaterial) {
	// changing to air deletes the platform
	if (newMaterial == Materials.Material.Air) {
	    GameObject.Destroy(this.gameObject);
	}

	// otherwise, update material and color
	this.material = newMaterial;
	
	SpriteRenderer sr = this.GetComponent<SpriteRenderer>();
	sr.color = Materials.MaterialToColor[newMaterial];
    }
}
