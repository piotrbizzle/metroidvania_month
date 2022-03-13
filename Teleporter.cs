using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public float destinationX;
    public float destinationY;
    
    // Start is called before the first frame update
    void Start()
    {
	GameObject triggerGo = new GameObject(this.name + " Teleport Trigger");
	triggerGo.AddComponent<BoxCollider2D>();
	triggerGo.GetComponent<BoxCollider2D>().isTrigger = true;
	triggerGo.transform.parent = this.gameObject.transform;
	triggerGo.transform.localScale = new Vector3(1.02f, 1.02f, 1.02f);
	triggerGo.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);      
    }
}
