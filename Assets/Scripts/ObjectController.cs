using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour {

	//private SpriteRenderer sr;
	private bool isPressed = false;
	private bool needfixed = false;
	public GameObject textmeshobject;
	private Rigidbody2D rb;
	private float movex=50f;
	private float movey=50f;
	public Vector2 scale;
	public bool shrink;
	private GameManager gm;
	public bool inArea = false;
	private TextMesh tm;
	// Use this for initialization
	void Start () {
		//sr = gameObject.GetComponent<SpriteRenderer>();
		rb = gameObject.GetComponent<Rigidbody2D> ();
		gm = GameObject.Find ("Canvas").GetComponent<GameManager> ();
		tm = gameObject.GetComponentInChildren<TextMesh> ();
	}
	
	// Update is called once per frame
	void Update () {
		Pressed();
		if (gameObject.transform.position.x < 0 && shrink) {
			gameObject.transform.localScale = new Vector3 (0.5f * scale.x, 0.5f * scale.y, 1);
			shrink = false;
		}
		if (gameObject.transform.position.x >= 0 && !shrink) {
			gameObject.transform.localScale = new Vector3 (0.25f * scale.x, 0.25f * scale.y, 1);
			shrink = true;
		}
		if (needfixed) {
			FixPosition (1);
		}
	}

	void OnMouseDown(){
		if (gm.cursorstate == false) {
			//must unoccupy frame DAN UPDATE MIN MAX;
			needfixed = false;///cobabetulinbug
			rb.velocity = new Vector2 (0, 0);//cobaebetulinbug
			if (inArea == true) {
				inArea = false;
				gm.UnoccupyFrame (gameObject.transform.position, scale);
				tm.text = "";
			}
			isPressed = true;
		} else {
			if (inArea) {
				gm.UnoccupyFrame (gameObject.transform.position, scale);
			}
			gm.MyDestroyFunc (gameObject, scale);
		}
	}
	void OnMouseUp(){
		isPressed = false;
		if (CheckPosition ()) {
			FixPosition (2);
			if (!gm.OccupyFrame (gameObject.transform.position, scale)) {
				needfixed = true;
			} else {
				needfixed = false;//cobabetulinbug
				inArea = true;
				tm.text = (scale.x * scale.y).ToString();

				float textsizescale;
				if (scale.x >= scale.y) {
					textsizescale = scale.y / scale.x;
					textmeshobject.transform.localScale = new Vector3 (textsizescale, textmeshobject.transform.localScale.y, textmeshobject.transform.localScale.z);
				} else if (scale.y > scale.x) {
					textsizescale = scale.x / scale.y;
					textmeshobject.transform.localScale = new Vector3 (textmeshobject.transform.localScale.x, textsizescale, textmeshobject.transform.localScale.z);
				}
			}
		} else {
			needfixed = true;
		}
	}

	void Pressed(){
		if (isPressed) {
			Vector2 MousePosition = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
			Vector2 objPosition = Camera.main.ScreenToWorldPoint (MousePosition);
			transform.position = objPosition;
			/*if (gameObject.transform.position.x < 0) {
				Vector3 temp = gameObject.transform.localScale;
				gameObject.transform.localScale = new Vector3 (2f * temp.x, 2f * temp.y, temp.z);
			} else {

			}*/
		}
	}

	bool CheckPosition(){
		if (!shrink) {
			float limitxright=(-5f+(gm.size*0.25f))-(0.5f*gameObject.transform.localScale.x);
			float limitxleft=(-5f-(gm.size*0.25f))+(0.5f*gameObject.transform.localScale.x);
			float limitytop=(0+(gm.size*0.25f))-(0.5f*gameObject.transform.localScale.y);
			float limitybottom=(0-(gm.size*0.25f))+(0.5f*gameObject.transform.localScale.y);
			if (scale.x != gm.size && scale.y != gm.size) {
				if (gameObject.transform.position.x <= limitxright+0.2f && gameObject.transform.position.x >= limitxleft-0.2f && gameObject.transform.position.y <= limitytop+0.2f && gameObject.transform.position.y >= limitybottom-0.2f) {
					return true;
				} else {
				
					return false;
				}
			} else if (scale.x == gm.size && scale.y!=gm.size) {
				if (gameObject.transform.position.y <= limitytop+0.2f && gameObject.transform.position.y >= limitybottom-0.2f) {
					return true;
				} else {

					return false;
				}
			} else if (scale.y == gm.size && scale.x!=gm.size) {
				if (gameObject.transform.position.x <= limitxright+0.2f && gameObject.transform.position.x >= limitxleft-0.2f) {
					return true;
				} else {

					return false;
				}
			} else if(scale.y==gm.size&&scale.x==gm.size){
				return true;
			}
			return false;//cobacobabetulinbug
		} else {
			return false;
		}
	}

	void FixPosition(int type){
		if (type == 1) {
			if (gameObject.transform.position.y < 2.5f) {
				movey = 50f*(2.5f-gameObject.transform.position.y)/10;
			} else if (gameObject.transform.position.y > 2.5f) {
				movey = -50f*(2.5f-gameObject.transform.position.y)/-10;
			} else if (gameObject.transform.position.y == 2.5f){movey = 0;}
			if (gameObject.transform.position.x < 3.5f) {
				movex = 50f*(3.5f-gameObject.transform.position.x)/10;
			} else if (gameObject.transform.position.x > 3.5f) {
				movex = -50f*(3.5f-gameObject.transform.position.x)/-10;
			}else if (gameObject.transform.position.x == 3.5f){movex = 0;}
			rb.velocity = new Vector2 (movex, movey);
			if ((gameObject.transform.position.y < 2.55f && gameObject.transform.position.y > 2.45f) && (gameObject.transform.position.x < 3.55f && gameObject.transform.position.x > 3.45f)) {
				movex = 0;
				movey = 0;
				gameObject.transform.position = new Vector2 (3.5f, 2.5f);
				rb.velocity = new Vector2 (0, 0);
				needfixed = false;
			}
		} else if (type == 2) {
			//perlu disesuaikan 
			Vector3 temppos = gameObject.transform.position;
			temppos.x *= 2.0f;
			temppos.x = Mathf.Round (temppos.x);
			temppos.x /= 2.0f;
			if (scale.x % 2 == 1) {
				if (gameObject.transform.position.x < temppos.x) {
					temppos.x -= 0.25f;
				} else {
					temppos.x += 0.25f;
				}
			}
			if (scale.x == gm.size) {
				temppos.x = -5;
			}
			temppos.y *= 2f;
			temppos.y = Mathf.Round (temppos.y);
			temppos.y /= 2f;
			if (scale.y % 2 == 1) {
				if (gameObject.transform.position.y < temppos.y) {
					temppos.y -= 0.25f;
				} else {
					temppos.y += 0.25f;
				}
			}
			if (scale.y == gm.size) {
				temppos.y = 0;
			}
			gameObject.transform.position = temppos;
		}
	}
}
