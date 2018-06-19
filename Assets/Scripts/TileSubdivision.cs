using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSubdivision : MonoBehaviour {

	private int[][] map;
	private float squareunit=0.5f;
	public GameObject container;
	private int amountver;
	private int amounthor;
	public GameObject thesquare;
	private int horlimit = 8;
	private int verlimit = 4;
	private float horstart = -9f;
	private float verstart = 5f;


	// Use this for initialization
	void Start () {
		amountver = Mathf.RoundToInt(container.transform.localScale.y / squareunit);
		amounthor = Mathf.RoundToInt(container.transform.localScale.x / squareunit);
		map = new int[amountver][];
		for (int i = 0; i < amountver; i++) {
			map [i] = new int[amounthor];
			for (int j = 0; j < amounthor; j++) {
				map [i] [j] = 0;
			}
		}
		StartCoroutine ("Subdivision");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator Subdivision(){
		float scalever = 0f, scalehor = 0f;
		int maxver = 0, maxhor = 0;
		int verscope = 0, horscope = 0;
		float tilever = 0f, tilehor = 0f;
		float posver = 0f, poshor = 0f;
		Vector3 pos;
		for (int i = 0; i < amountver; i++) {
			for (int j = 0; j < amounthor; j++) {
				if (map [i] [j] == 0) {
					maxver = amountver - i;
					maxhor = amounthor - j;
					for (int m = j; m < amounthor; m++) {
						if (map [i] [m] == 1) {
							maxhor = m - j+1;
							break;
						}
					}
					//Debug.Log ("max :"+maxhor+" + "+maxver);
					verscope = Mathf.Min (maxver, verlimit);
					horscope = Mathf.Min (maxhor, horlimit);
					//Debug.Log ("scope :"+horscope+" + "+verscope);
					scalever = Random.Range (1, verscope);
					scalehor = Random.Range (1, horscope);
					//Debug.Log ("scale :"+scalehor+" + "+scalever);
					for (int k = 0; k < scalever; k++) {
						for (int l = 0; l < scalehor; l++) {
							map [i+k] [j+l] = 1;
							//Debug.Log ("masuk");
						}
					}
					tilever = i + (scalever) / 2;
					tilehor = j + (scalehor) / 2;
					//Debug.Log ("tile :"+tilehor+" + "+tilever);
					posver = verstart - (tilever * squareunit);
					poshor = horstart + (tilehor * squareunit);
					//Debug.Log ("pos :"+poshor+" + "+posver);
					pos = new Vector3 (poshor, posver, -10f);
					if ((i /2)%2==3) {
						yield return new WaitForSeconds (0.25f);
					}
					GameObject temp= (GameObject) Instantiate(thesquare,pos,transform.rotation);
					temp.transform.localScale = new Vector3 (scalehor*squareunit, scalever*squareunit);
					temp.GetComponent<SpriteRenderer> ().color = new Color (Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f),1f);
					//Debug.Log ("-------------------------------------------------");
				}
			}
		}
	}

}
