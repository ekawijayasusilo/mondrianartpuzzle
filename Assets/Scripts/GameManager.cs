using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;

public class GameManager : MonoBehaviour {
	public Vector3[] scoringarray;
	[HideInInspector] public int size;
	public InputField InputX;
	public InputField InputY;
	[HideInInspector] public int x;
	[HideInInspector] public int y;
	public GameObject Object;
	public GameObject Frame;
	[HideInInspector] public int[][] occupiedmap;
	[HideInInspector] public int[][] comparisonmap;
	[HideInInspector] public GameObject[][] framearray;
	[HideInInspector] public float[] areaarray;
	[HideInInspector] public int indexarea;
	[HideInInspector] public GameObject ObjectNow;
	[HideInInspector] float starting;
	[HideInInspector] public int totaloccupied=0;
	[HideInInspector] public float min;
	[HideInInspector] public float max;
	[HideInInspector] public float selisih;
	[HideInInspector] public bool submitted;
	public Text textscore;
	public Text textgoal;
	public Text textrecord;
	public Text textachievement;
	public GameObject gameover;
	public GameObject pauseui;
	public GameObject tableui;
	public Text textmin;
	public Text textmax;

	[HideInInspector] public int goalsc;
	[HideInInspector] public int worldrec;

	[HideInInspector] public Vector2[] used;
	[HideInInspector] public int usedindex;
	public GameObject textused;
	[HideInInspector] public float timerused=0;

	[HideInInspector]public bool delmode=false;
	public GameObject deletemodetext;

	public GameObject textsubmitable;
	public GameObject textunsubmitable;

	[HideInInspector] public float timer;
	public Text texttime;
	[HideInInspector] public DifficultyChosen dc;

	public Texture2D fadeoutTexture;
	public float fadeSpeed=0.8f;

	private int drawDepth = -1000;
	private float alpha=1.0f;
	private int fadeDir = -1;

	public string mainmenuname;

	public Texture2D normalCursor;
	public Texture2D deleteCursor;
	public bool cursorstate;

	private bool criticaltimer=false;

	private string filepath;
	//private string folderpath;
	private string lastfilenumber;
	public Text textischecked;
	[HideInInspector] public bool ischecked;
	[HideInInspector] public bool solutionaccepted;
	[HideInInspector] public bool checkstarted;
	public GameObject submissionui;
	private int occupiedcode;
	private int teamnumber;
	public InputField InputTeam;
	private bool isproceed;

	// Use this for initialization
	void Start () {
		//folderpath = "MondrianArtPuzzle_Data/";
		InputTeam.onValueChanged.AddListener (delegate {
			DisableIsCheck();
		});
		ischecked = false;
		solutionaccepted = false;
		isproceed = false;
		checkstarted = false;
		indexarea = 0;
		selisih = 0;
		max = 0;
		min = 0;
		textscore.text = "SCORE : -";
		timer = 420.0f;
		used=new Vector2[2000];//perbaiki
		usedindex = 0;
		submitted = false;
		dc = GameObject.Find ("DiffSave").GetComponent<DifficultyChosen> ();
		size = dc.size;
		for (int i = 0; i < scoringarray.Length; i++) {
			if (size == scoringarray [i].x) {
				textgoal.text = "GOAL : SCORE <= " + scoringarray [i].y;
				textrecord.text = "WORLD RECORD : " + scoringarray [i].z;
				goalsc = (int)scoringarray [i].y;
				worldrec = (int)scoringarray [i].z;
			}
		}
		SetGameFrames (size);
	}
	
	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;
		texttime.text = "TIME : " + Mathf.Round (timer).ToString();

		if (textused.activeSelf && timerused < 2f) {
			timerused += Time.deltaTime;
		} else if (timerused > 2f && textused.activeSelf) {
			timerused = 0;
			textused.SetActive (false);
		}

		if (timer <= 30f && !criticaltimer) {
			criticaltimer = true;
			texttime.GetComponent<Animator> ().SetBool ("Critical", true);
		}

		if (timer <= 0f && submitted==false) {
			Time.timeScale=0;
			texttime.GetComponent<Animator> ().SetBool ("Critical", false);
			Submit ();
		}
		if (Input.GetKeyDown (KeyCode.P)) {
			PauseButtonFunction ();
		}

	}

	public void SetGameFrames(int psize){
		size = psize;
		occupiedmap = new int[size][];
		comparisonmap = new int[size][];
		framearray = new GameObject[size][];
		for (int i = 0; i < size; i++) {
			occupiedmap [i] = new int[size];
			comparisonmap [i] = new int[size];
		}
		for (int i = 0; i < size; i++) {
			framearray [i] = new GameObject[size];
		}
		areaarray = new float[indexarea];

		CreateFrame ();
	}

	void CreateFrame(){
		starting = (-1.0f*(20 - size) * 0.5f/2.0f)+0.25f;
		//float startingy = 5f+(-1.0f*(20 - size) * 0.5f/2.0f)+0.25f;
		for (int i = 0; i < size; i++) {
			for (int j = 0; j < size; j++) {
				occupiedmap [i] [j] = 0;
				float posx =(size - j) * -0.5f;
				float posy = i * -0.5f;
				Vector2 pos = new Vector2 (0+starting+posx,4.5f+starting+posy);
				framearray[i][j]= (GameObject) Instantiate(Frame,pos,transform.rotation);
			}
		}
	}

	public void CreateObject(){
		GameObject[] temp = GameObject.FindGameObjectsWithTag ("Square");
		if (temp.Length > 0) {
			foreach (GameObject a in temp) {
				ObjectController oc = a.GetComponent<ObjectController> ();
				//if (a.transform.position.x >=1f && a.transform.position.x <=6f && a.transform.position.y >=0f && a.transform.position.y <= 5f) {
				if (oc.inArea==false) {
					MyDestroyFunc (a,oc.scale);
					/*
					Vector2 tempusedscale=a.GetComponent<ObjectController> ().scale;
					for (int i = 0; i < usedindex; i++) {
						if (tempusedscale.x == used[i].x && tempusedscale.y == used[i].y) {
							used [i].x = -1;//perbaiki
							used [i].y = -1;
						}
					}
					Destroy (a);
					*/
				}
			}
		}
		if (InputX.text != "") {
			x = int.Parse (InputX.text);
			if (x > size) {
				x = size;
			}
		} else {
			x = 1;
		}
		if (InputY.text != "") {
			y = int.Parse (InputY.text);
			if (y > size) {
				y = size;
			}
		} else {
			y = 1;
		}
		if (CheckUsed(x,y)) {
			Vector2 pos = new Vector2 (3.5f, 2.5f);
			ObjectNow = (GameObject)Instantiate (Object, pos, transform.rotation);
			ObjectNow.GetComponent<SpriteRenderer> ().color = new Color (Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f),0.95f);
			ObjectNow.transform.localScale = new Vector3 (x * 0.25f, y * 0.25f, 1);
			ObjectNow.GetComponent<ObjectController> ().scale = new Vector2 (x, y);
			ObjectNow.GetComponent<ObjectController> ().shrink = true;
		} else {
			textused.SetActive (true);
			timerused = 0;

		}
	}

	public bool OccupyFrame(Vector3 objpos, Vector2 objscale){
		float row=-2f*(objpos.y-4.5f-starting);
		float column=2f*(objpos.x-starting)+size;
		if (objscale.x % 2 == 0) {
			column -= (0.5f + 1f * (objscale.x - 2) / 2);
		} else {
			column -= ((objscale.x - 1) / 2);
		}
		if (objscale.y % 2 == 0) {
			row -= (0.5f+1f*(objscale.y-2)/2);
		}else {
			row -= ((objscale.y - 1) / 2);
		}
		for (int i = 0; i < objscale.y; i++) {
			for (int j = 0; j < objscale.x; j++) {
				if (occupiedmap [(int)row + i] [(int)column + j] != 0) {
					return false;
				}
			}
		}
		for (int i = 0; i < objscale.y; i++) {
			for (int j = 0; j < objscale.x; j++) {
				occupiedcode = ((int)objscale.x * 100) + ((int)objscale.y);
				occupiedmap [(int)row+i] [(int)column+j] = occupiedcode;
				totaloccupied += 1;
			}
		}
		float[] temparea = new float[indexarea];
		for (int i = 0; i < indexarea; i++) {
			temparea [i] = areaarray [i];
		}
		areaarray = new float[indexarea + 1];
		for (int i = 0; i < indexarea; i++) {
			areaarray [i] = temparea [i];
		}
		areaarray [indexarea] = objscale.x * objscale.y;
		indexarea += 1;
		CalculateScore ();
		return true;
	}

	public void CalculateScore(){
		if (indexarea >= 2) {
			min = Mathf.Min (areaarray);
		} else {
			min = 0;
		}
		if (indexarea >= 1) {
			max = Mathf.Max (areaarray);
		} else {
			max = 0;
		}
		textmin.text = "MIN : " + min.ToString();
		textmax.text = "MAX : " + max.ToString();
		selisih = max - min;
		textscore.text = "SCORE : " + selisih.ToString ();
		if (totaloccupied == (size * size)) {
			if (selisih <= goalsc) {
				textsubmitable.SetActive (true);
			}else if (selisih > goalsc) {
				textunsubmitable.SetActive (true);
			}
		} else if (totaloccupied < (size * size)) {
			textsubmitable.SetActive (false);
			textunsubmitable.SetActive (false);
		}
		//Debug.Log ("Indexarea=" + indexarea + " & Usedindex=" + usedindex);

	}

	public void UnoccupyFrame(Vector3 objpos, Vector2 objscale){
		float row=-2f*(objpos.y-4.5f-starting);
		float column=2f*(objpos.x-starting)+size;
		if (objscale.x % 2 == 0) {
			column -= (0.5f + 1f * (objscale.x - 2) / 2);
		} else {
			column -= ((objscale.x - 1) / 2);
		}
		if (objscale.y % 2 == 0) {
			row -= (0.5f+1f*(objscale.y-2)/2);
		}else {
			row -= ((objscale.y - 1) / 2);
		}
		for (int i = 0; i < objscale.y; i++) {
			for (int j = 0; j < objscale.x; j++) {
				occupiedmap [(int)row+i] [(int)column+j] = 0;
				totaloccupied -= 1;
			}
		}
		float[] temparea = new float[indexarea];
		int indexdeleted=0;
		for (int i = 0; i < indexarea; i++) {
			temparea [i] = areaarray [i];
			if (areaarray [i] == objscale.x * objscale.y) {
				indexdeleted = i;
			}
		}
		areaarray = new float[indexarea - 1];
		for (int i = 0; i < indexdeleted; i++) {
			areaarray [i] = temparea [i];
		}
		for (int i = indexdeleted + 1; i < indexarea; i++) {
			areaarray [i - 1] = temparea [i];
		}
		indexarea -= 1;
		CalculateScore ();
	}

	public void ClearAll(){
		GameObject[] temp = GameObject.FindGameObjectsWithTag ("Square");
		if (temp.Length > 0) {
			foreach (GameObject a in temp) {
				ObjectController oc = a.GetComponent<ObjectController> ();
				if (oc.inArea == true) {
					oc.inArea = false;
					UnoccupyFrame (a.transform.position, oc.scale);
					MyDestroyFunc (a,oc.scale);
				}
				/*
				Vector2 tempusedscale=a.GetComponent<ObjectController> ().scale;
				for (int i = 0; i < usedindex; i++) {
					if (tempusedscale.x == used[i].x && tempusedscale.y == used[i].y) {
						used [i].x = -1;//perbaiki
						used [i].y = -1;
					}
				}

				if (a.transform.position.x <-5f+(size*0.25) && a.transform.position.x>-5f-(size*0.25) && a.transform.position.y <0+(size*0.25) && a.transform.position.y >0-(size*0.25)) {
					Destroy (a);
				}
				*/
			}
		}
	}

	public void MyDestroyFunc(GameObject delobj, Vector2 delobjscale){
		for (int i = 0; i < usedindex; i++) {
			if (delobjscale.x == used[i].x && delobjscale.y == used[i].y) {
				used [i].x = -1;//perbaiki
				used [i].y = -1;
			}
		}
		Destroy (delobj);
	}

	public void Submit(){
		if (timer <= 0f) {
			if (totaloccupied != size * size) {
				textachievement.text = "You Lose!";
				timer = 0f;
				Time.timeScale = 0;
				gameover.SetActive (true);
				submitted = true;
			} else {
				if (selisih > goalsc) {
					textachievement.text = "You Lose!";
					timer = 0f;
					Time.timeScale = 0;
					gameover.SetActive (true);
					submitted = true;
				} else {
					timer = 0f;
					Time.timeScale = 0;
					textischecked.text = "Please enter your team number!";
					submissionui.SetActive (true);
					submitted = true;
				}
			}
		} else {
			if (totaloccupied == size * size && selisih <= goalsc) {
				Time.timeScale = 0;
				textischecked.text = "Please enter your team number!";
				submissionui.SetActive (true);
			}
		}
	}

	public void SubmissionCheck(){
		bool breakloop;
		bool foundpattern = false;
		int foundfile = -1;
		if (!checkstarted) {
			checkstarted = true;
			if (InputTeam.text != "") {
				teamnumber = int.Parse (InputTeam.text);
				string containwhat=InputTeam.text+"X"+dc.size.ToString()+"Y";
				string notcontainwhat = "meta";
				DirectoryInfo dir = new DirectoryInfo (Application.dataPath);
				FileInfo[] infofile = dir.GetFiles ("*.*");
				foreach (FileInfo f in infofile) {
					if (f.ToString ().Contains (containwhat) && !f.ToString ().Contains (notcontainwhat)) {
						//----------------------------------------------------
						bool allowedcheck=true;
						if (teamnumber < 10) {
							allowedcheck = CheckContainForOneDigit (f.ToString (), teamnumber);
						}
						if (allowedcheck) {
							int mulai = f.ToString ().IndexOf ('Y');
							mulai += 1;
							int brpbanyak = f.ToString ().IndexOf ('.');
							if (int.Parse (f.ToString ().Substring (mulai, brpbanyak - mulai)) > foundfile) {
								lastfilenumber = f.ToString ().Substring (mulai, brpbanyak - mulai);
								foundfile = int.Parse (f.ToString ().Substring (mulai, brpbanyak - mulai));
							}

							int startsearchnya = f.ToString ().IndexOf ('X');
							string namafilenya;
							if (teamnumber >= 10) {
								namafilenya = f.ToString ().Substring (startsearchnya - 2);
							} else {
								namafilenya = f.ToString ().Substring (startsearchnya - 1);
							}

							//string namafilenya = f.ToString ().Substring (startsearchnya);
							comparisonmap = GetData (namafilenya);//f.ToString ());
							breakloop = false;
							for (int i = 0; i < size; i++) {
								for (int j = 0; j < size; j++) {
									if (comparisonmap [i] [j] != occupiedmap [i] [j]) {
										breakloop = true;
										break;
									}
								}
								if (breakloop) {
									break;
								}
							}
							if (!breakloop) {
								foundpattern = true;
								break;
							}
						}
						//------------------------------------------
					}
				}
				if (foundfile == -1) {
					lastfilenumber = "-1";
				}
				if (foundpattern) {
					solutionaccepted = false;
					if (timer <= 0f) {
						textischecked.text = "You have used this pattern. Press Proceed to continue.";
					} else {
						textischecked.text = "You have used this pattern. Press Back to try other pattern.";
					}
				} else {
					solutionaccepted = true;
					textischecked.text = "You haven't used this pattern before. Press Proceed to finish the game.";
				}
				ischecked = true;
			}
			checkstarted = false;
		}
	}

	int[][] GetData(string path) {
		StreamReader reader = new StreamReader (Application.dataPath + "/" + path);
		int[][] temparr = reader.ReadToEnd().Split('\n').Select(r=>r.Split(new [] {','}).Select(c=>System.Convert.ToInt32(c)).ToArray()).ToArray();
		reader.Close ();
		return temparr;
	}

	bool CheckContainForOneDigit(string ftostring, int teamnumb){
		//InputTeam.text+"X"+dc.size.ToString()+"Y";
		string namepart=teamnumb.ToString()+"X"+dc.size.ToString()+"Y";
		if(ftostring.Contains("1"+namepart)){
			return false;
		}
		if(ftostring.Contains("2"+namepart)){
			return false;
		}
		if(ftostring.Contains("3"+namepart)){
			return false;
		}
		if(ftostring.Contains("4"+namepart)){
			return false;
		}
		if(ftostring.Contains("5"+namepart)){
			return false;
		}
		if(ftostring.Contains("6"+namepart)){
			return false;
		}
		if(ftostring.Contains("7"+namepart)){
			return false;
		}
		if(ftostring.Contains("8"+namepart)){
			return false;
		}
		if(ftostring.Contains("9"+namepart)){
			return false;
		}
		return true;
	}

	public void DisableIsCheck(){
		textischecked.text = "Please enter your team number!";
		ischecked = false;
		solutionaccepted = false;
	}

	public void BackToGame(){
		if (timer > 0) {
			Time.timeScale = 1;
			submissionui.SetActive (false);
			ischecked = false;
			solutionaccepted = false;
		}
	}

	public void Proceed(){
		if (!isproceed) {
			isproceed = true;
			if (ischecked) {
				if (solutionaccepted) {
					if (selisih < worldrec) {
						textachievement.text = "Team "+InputTeam.text+" Set a NEW WORLD RECORD!!!";
					} else if (selisih <= goalsc) {
						textachievement.text = "Team "+InputTeam.text+" Win!";
					}
					int newlastnumber = int.Parse (lastfilenumber) + 1;
					filepath = Application.dataPath + "/" + teamnumber.ToString () + "X" + dc.size.ToString () + "Y" + newlastnumber.ToString () + ".txt";
					StreamWriter writer = new StreamWriter (filepath, true);
					for (int i = 0; i < size; i++) {
						for (int j = 0; j < size; j++) {
							if (j != size - 1) {
								writer.Write (occupiedmap [i] [j].ToString () + ",");
							} else {
								writer.Write (occupiedmap [i] [j].ToString ());
							}
						}
						if (i != size - 1) {
							writer.Write ("\n");
						}
					}
					writer.Close ();
					gameover.SetActive (true);
					submissionui.SetActive (false);
					submitted = true;
				} else {
					if (timer <= 0) {
						textachievement.text = "You Lose!";
						gameover.SetActive (true);
						submissionui.SetActive (false);
						submitted = true;
					}
				}
			}
			isproceed = false;
		}
	}


	public bool CheckUsed(int x, int y){
		for (int i = 0; i < usedindex; i++) {
			if (used [i].x == x && used [i].y == y) {
				return false;
			} else if (used [i].x == y && used[i].y==x) {
				return false;
			}
		}
		used [usedindex] = new Vector2 (x, y);
		usedindex+=1;
		return true;
	}

	public void PauseButtonFunction(){
		if (timer > 0) {
			Time.timeScale = 0;
			pauseui.SetActive (true);
		}
	}
	public void TableButtonFunction(){
		if (timer > 0) {
			tableui.SetActive (true);
		}
	}
	public void CloseTableButtonFunction(){
		if (timer > 0) {
			tableui.SetActive (false);
		}
	}
	public void ResumeButtonFunction(){
		if (timer > 0) {
			Time.timeScale = 1;
			pauseui.SetActive (false);
		}
	}
	public void MainMenuButtonFunction(){
		Time.timeScale = 1;
		WaitFunction ();
		SceneManager.LoadScene (mainmenuname);
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

	public void ActivateDeleteMode(){
		if (cursorstate == false) {
			Cursor.SetCursor (deleteCursor, Vector2.zero, CursorMode.Auto);
			cursorstate = true;
		} else {
			Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
			cursorstate = false;
		}
	}
}
