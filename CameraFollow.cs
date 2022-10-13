using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
        // related objects
    public Player player;

    public float CloseEnoughOffset = 0.1f;
    public float MaximumOffset = .75f;
    public float PanSpeed = 2.0f;
    
    void Update()
    {
	Vector2 thisPosition2D = new Vector2(this.transform.position.x, this.transform.position.y);
	Vector2 playerPosition2D = new Vector2(this.player.transform.position.x, this.player.transform.position.y);
	
	float distanceToPlayer = Vector2.Distance(thisPosition2D, playerPosition2D);
	// do nothing if we're close to the player
	if (distanceToPlayer < this.CloseEnoughOffset) {
	    return;
	}
	
	Vector2 normalizedToPlayer = (playerPosition2D - thisPosition2D).normalized;

	// snap to maximum offset if beyond it
	if (distanceToPlayer > this.MaximumOffset) {
	    Vector2 newPosition2D = -1 * normalizedToPlayer * this.MaximumOffset;
	    this.transform.position = this.player.transform.position + new Vector3(newPosition2D.x, newPosition2D.y, this.transform.position.z);
	    return;
	}
	
	// otherwise, move toward player
	Vector2 moveTowardPlayer = normalizedToPlayer * this.PanSpeed * Time.deltaTime;
	this.transform.Translate(moveTowardPlayer.x, moveTowardPlayer.y, 0);
    }
}
