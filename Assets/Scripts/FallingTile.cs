using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingTile : MonoBehaviour {
	Vector3 postemp;
	Vector3 temp;
	bool state=false;
	SpriteRenderer sr;
	float timer;
	float changetime;
	int type;
	float lastpos;
	private GMStart gm;
	// Use this for initialization
	void Start () {
		sr = gameObject.GetComponent<SpriteRenderer> ();
		gm = GameObject.Find ("Canvas").GetComponent<GMStart> ();
		changetime = Random.Range (5f, 10f);
		lastpos = Random.Range (-7f, 0f);
		//lastpos=0f;
		type = Random.Range (1, 100);
		if ((type <= 30 && type>=21) || (type <= 60 && type>=51) || (type <= 90 && type>=81)) {
			type = 1;
			sr.color = new Color (1f,1f,1f,1f);
		} else {
			type = 2;
		}
	}
	
	// Update is called once per frame
	void Update () {
		///*
		if (gm.allowedfalling == true) {
			lastpos = 0f;
		}
		//*/
		if (gameObject.transform.position.z < lastpos) {
			postemp = gameObject.transform.position;
			postemp = new Vector3 (postemp.x, postemp.y, postemp.z + 0.1f);
			gameObject.transform.position = postemp;
		} else {
			if (state == false) {
				state = true;
			}
		}
		if (state && type==2) {
			timer += Time.deltaTime;
			if (timer >= changetime) {
				sr.color = new Color (Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f),1f);
				timer = 0f;
			}
		}
	}
}
