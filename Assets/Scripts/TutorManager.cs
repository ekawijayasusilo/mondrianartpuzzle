using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorManager : MonoBehaviour {

	public Texture2D fadeoutTexture;
	public float fadeSpeed=1f;

	private int drawDepth = -1000;
	private float alpha=1.0f;
	private int fadeDir = -1;

	public string tryitscenename;

	public GameObject[] tutorpanels;

	// Use this for initialization
	void Start () {
		tutorpanels [0].SetActive (true);
		tutorpanels [0].GetComponent<Animator> ().SetTrigger ("StartTutor");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void LoadTutor(int totalcode){
		bool changescene = totalcode % 10 == 0 ? false : true;
		totalcode /= 10;
		bool next = totalcode % 10 == 0 ? false : true;
		totalcode /= 10;
		int tutornumber=totalcode;

		if (!changescene) {
			if (!next) {
				tutorpanels [tutornumber + 1].GetComponent<Animator> ().SetTrigger ("EndTutor");
				tutorpanels [tutornumber].SetActive (true);
				tutorpanels [tutornumber].GetComponent<Animator> ().SetTrigger ("StartTutor");
			} else {
				tutorpanels [tutornumber - 1].GetComponent<Animator> ().SetTrigger ("EndTutor");
				tutorpanels [tutornumber].SetActive (true);
				tutorpanels [tutornumber].GetComponent<Animator> ().SetTrigger ("StartTutor");
			}
		} else {
			WaitFunction ();
			SceneManager.LoadScene (tryitscenename);
		}
	}

	IEnumerator WaitFunction(){
		float fadeTime = BeginFade (1);
		yield return new WaitForSeconds (fadeTime);
	}

	void OnGUI(){
		alpha += fadeDir * fadeSpeed * Time.deltaTime;
		alpha = Mathf.Clamp01 (alpha);

		GUI.color = new Color (GUI.color.r, GUI.color.g, GUI.color.b, alpha);
		GUI.depth = drawDepth;
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), fadeoutTexture);
	}
	public float BeginFade(int direction){
		fadeDir = direction;
		return (fadeSpeed);
	}
	void OnLevelWasLoaded(){
		BeginFade (-1);
	}
		
}
