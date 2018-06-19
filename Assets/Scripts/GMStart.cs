using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GMStart : MonoBehaviour {
	private DifficultyChosen dc;
	public Texture2D fadeoutTexture;
	public float fadeSpeed=2f;

	private int drawDepth = -1000;
	private float alpha=1.0f;
	private int fadeDir = -1;

	public string maingame;
	public string tutorscenename;
	public int[] arrsize;

	private bool enabl=false;

	public bool allowedfalling = false;
	//private bool returnoriginal = false;

	public GameObject cam;
	public GameObject tutorbutton;
	public GameObject easybutton;
	public GameObject mediumbutton;
	public GameObject hardbutton;
	public GameObject quitbutton;
	public GameObject blackpanel;
	public GameObject title;

	//private Vector2 moveforce;
	//private Vector2 rotateforce;

	//private Rigidbody2D camrb;

	void Start(){
		dc = GameObject.Find ("DiffSave").GetComponent<DifficultyChosen> ();
		//camrb = cam.GetComponent<Rigidbody2D> ();
	}

	public void StartGame (int whatdiff) {
		if (enabl == false) {
			enabl = true;
			//int selectedindex = Random.Range (0, arrsize.Length);
			//dc.size = arrsize [selectedindex];
			dc.size=whatdiff;
			StartCoroutine ("WaitAnim");
			//WaitFunction ();
			//SceneManager.LoadScene (maingame);
		}
	}

	/*
	void Update(){
		if (returnoriginal) {
			moveforce.x = 0f;moveforce.y = 0f;rotateforce.x = 0f;rotateforce.y = 0f;
			if (cam.transform.position.x != 0f) {
				moveforce.x = (0f - cam.transform.position.x)/10;
			}
			if (cam.transform.position.y != 0f) {
				moveforce.y = (0f - cam.transform.position.y)/10;
			}
			if (cam.transform.eulerAngles.x != 0f) {
				rotateforce.x = (0f - cam.transform.eulerAngles.x)/10;
			}
			if (cam.transform.eulerAngles.y != 0f) {
				rotateforce.y = (0f - cam.transform.eulerAngles.y)/10;
			}
			camrb.velocity = moveforce;
			//cam.transform.position = new Vector2 (cam.transform.position.x + moveforce.x, cam.transform.position.y + moveforce.y);
			if (cam.transform.position.x == 0f && cam.transform.position.y == 0f && cam.transform.eulerAngles.x == 0f && cam.transform.eulerAngles.y == 0f) {
				returnoriginal = false;
			}
		}
	}
	*/
	IEnumerator WaitAnim(){
		allowedfalling = true;
		//returnoriginal = true;
		yield return new WaitForSeconds (2f);
		cam.GetComponent<Animator> ().SetTrigger ("StartCam");
		tutorbutton.GetComponent<Animator> ().SetTrigger ("ButtonStart");
		easybutton.GetComponent<Animator> ().SetTrigger ("ButtonStart");
		mediumbutton.GetComponent<Animator> ().SetTrigger ("ButtonStart");
		hardbutton.GetComponent<Animator> ().SetTrigger ("ButtonStart");
		quitbutton.GetComponent<Animator> ().SetTrigger ("ButtonStart");
		title.GetComponent<Animator> ().SetTrigger ("TitleStart");
		blackpanel.GetComponent<Animator> ().SetTrigger ("GoesBlack");
		//yield return new WaitForSeconds (1f);
		//cam.GetComponent<Animator> ().SetTrigger ("EndCam");
		//startbutton.GetComponent<Animator> ().SetTrigger ("ButtonEnd");
		yield return new WaitForSeconds (3f);
		WaitFunction ();
		SceneManager.LoadScene (maingame);
	}

	IEnumerator WaitFunction(){
		float fadeTime = BeginFade (1);
		yield return new WaitForSeconds (fadeTime);
	}
	public void QuitGame(){
		Application.Quit ();
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

	public void LoadTutorScene(){
		dc.size=12;
		WaitFunction ();
		SceneManager.LoadScene (tutorscenename);
	}
}
