using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public bool isStopped;
    private float currentTime;
	
    // Update is called once per frame
    void Update()
    {
	if (this.isStopped) {
	    return;
	}
			
	this.currentTime += Time.deltaTime;
	int minutes = (int)(this.currentTime / 60);
	int seconds = (int)(this.currentTime % 60);
	int milliseconds = (int)((this.currentTime % 1) * 1000);
	this.gameObject.GetComponent<Text>().text = minutes + ":" + seconds + ":" + milliseconds;
    }
}
