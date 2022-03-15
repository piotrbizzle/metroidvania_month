using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    public string[] dialogueLines;

    private GameObject dialogueLineGo;
    private int currentDialogueLine;
    private bool showingText;
    
    // Start is called before the first frame update    
    void Start()
    {
	GameObject triggerGo = new GameObject(this.name + " Dialogue Trigger");
	triggerGo.layer = 7;
	triggerGo.AddComponent<BoxCollider2D>();
	triggerGo.GetComponent<BoxCollider2D>().isTrigger = true;
	triggerGo.transform.parent = this.gameObject.transform;
	triggerGo.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
	triggerGo.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
    }

    public void AddText() {
	// do nothing if text is already displayed
	if (this.showingText) {
	    return;
	}

	// do nothing if dialogue is empty somehow
	if (this.dialogueLines.Length == 0) {
	    return;
	}

	// create new gameobject with text
	this.dialogueLineGo = new GameObject();
	this.dialogueLineGo.name = "Dialogue Line";
	this.dialogueLineGo.transform.SetParent(GameObject.Find("Canvas").transform, false);

	RectTransform labelRectangle = dialogueLineGo.AddComponent<RectTransform>();
	// TODO: maybe make position and dimensions configurable
	// TODO: dialogue should not be pinned to player
	labelRectangle.anchoredPosition = new Vector2(0f, 56f);
	labelRectangle.sizeDelta = new Vector2(300f, 100f);
	
	Text dialogueText = this.dialogueLineGo.AddComponent<Text>();
	dialogueText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
	dialogueText.text = this.dialogueLines[currentDialogueLine];
	dialogueText.alignment = TextAnchor.MiddleCenter;

	this.dialogueLineGo.AddComponent<Outline>();

	this.showingText = true;	
    }

    public void RemoveText() {
	// do nothing if text is not displayed
	if (!this.showingText) {
	    return;
	}
	GameObject.Destroy(this.dialogueLineGo);
	this.showingText = false;
    }

    public void Advance() {
	if (this.currentDialogueLine >= this.dialogueLines.Length - 1) {
	    return;
	}

	this.currentDialogueLine += 1;
	if (this.showingText) {	    
	    this.RemoveText();
	    this.AddText();
	}
    }
}
