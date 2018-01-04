using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

	private Transform[] spawnPoints;
	public GameObject player;

	public bool fireLaser = false;

	public LevelObjectives levelObjs;

	public bool landed = true;
	public bool inLandingZone = true;
	public string inZoneType;

	public bool LandingPressed = false;
	int lastLandedIndex;
	int temp_lastLandedIndex;

	//Timing variables
	private float timeForLevel;
	private float timeLeft;
	private bool isGamePaused = false;
	private float remainingTime = 0f;

	//timer text for messaging
	int seconds;
	int minutes;
	string niceTime;

	int livesRemaining = 3;

	public Button restart_button;
	public Text display_time;
	public Text display_fuel;

	//boolean to stop looping through when restart button is shown.  The game routines do not continue unless this is set to true.
	bool restarted = true;

	void Awake ()
	{
		spawnPoints = levelObjs.spawnPoints;
		lastLandedIndex = 0;
		ResetLevelTime();
		restart_button.gameObject.SetActive (false);
	}
		

	// Use this for initialization
	void Start () {
		readyToFly ();	

		UpdateTimer();
		Time.timeScale = 1;

	}

	// Update is called once per frame
	void Update () {
		if (!isGamePaused) {
			UpdateTimer ();
			timeLeft -= Time.deltaTime;
		}
	}

	public void ResetLevelTime()
	{
		timeForLevel = levelObjs.timeForLevel;
		timeLeft = timeForLevel;

	}

	public void SetLandingIndexFromLastLandedPad()
	{
		lastLandedIndex = temp_lastLandedIndex;
	}
	public void readyToFly()
	{
		
		player.transform.position = spawnPoints [lastLandedIndex].position;
		if (inZoneType == "FINAL") {
			LevelCompleted ();
		} else {
			player.GetComponent<RHC_HovercraftController> ().RestoreFuel ();
		}
	}

	void UpdateTimer()
	{
		
		remainingTime = 0f;
		if (timeLeft > 0) {
			remainingTime = timeLeft;
		} else {
			LoseALife ();
		}

		minutes = Mathf.FloorToInt(remainingTime / 60F);
		seconds = Mathf.FloorToInt(remainingTime - minutes * 60);
		niceTime = "TIME : " + minutes + ":"+ seconds;

		display_time.text = niceTime;
		UpdateDisplays ();
		//float remainingTimeBar = timeLeft / timeForLevel;
		//SetTimerBar (remainingTimeBar);
	}

	public void UpdateDisplays()
	{
		display_time.text = niceTime;
		display_fuel.text = player.GetComponent<RHC_HovercraftController> ().ReturnFuel ();
	}

	public void LoseALife()
	{
		if (restarted) {
			Debug.Log ("LOSE A LIFE");
			PauseGame ();
			livesRemaining = livesRemaining - 1;
			if (livesRemaining == 0 || livesRemaining < 0) {
				GameOver ();
				restarted = false;
			} else {
				restart_button.gameObject.SetActive (true);
				restart_button.onClick.AddListener (Restart);
				restarted = false;
			}
		}
	}


	void Respawn()
	{
		Debug.Log ("You have " + livesRemaining + " Lives Remaining");
		readyToFly ();
		UnPauseGame ();
	}

	void Restart()
	{
		restart_button.gameObject.SetActive (false);
		ResetLevelTime ();
		Respawn ();
	}



	void GameOver()
	{
		restart_button.gameObject.SetActive (false);
		Debug.Log ("GAME OVER");
	}

	void LevelCompleted ()
	{
		Debug.Log ("YOU HAVE COMPLETED THE LEVEL!");
		restarted = false;	
		PauseGame();
	}

	public void UnPauseGame()
	{
		Time.timeScale = 1;
		isGamePaused = false;
		restarted = true;
	}

	public void PauseGame()
	{
		Time.timeScale=0;
		isGamePaused = true;

	}

	public string getShipStatus()
	{
		//I've pressed the Action button triggered in RHC_HoverController
		//Im in the landing zone and landed
		if (landed && inLandingZone) {
			//TriggerTakeOff;
			fireLaser = false;
			//Debug.Log ("Returning - LANDED 1");
			//Used to be set to LANDED changed after when you died, not sure if landed is relevant anymore
			return "LANDED";
		}
		//Im in the landing zone but not landed
		if (!landed && inLandingZone) {
			//TriggerTakeOff
			//landed = true;
			fireLaser = false;
			//Debug.Log ("Returning - LANDED 2");
			return "READYTOLAND";
		}
		if (inZoneType == "FINAL") {
			return "LEVELCOMPLETE";
		}
		if (!inLandingZone) {
			//FireLaser
			fireLaser = true;
			//Debug.Log ("Returning - FLYING 1");
			return "FLYING";
		}

		//Debug.Log ("Returning - FLYING 2");
		return "FLYING";

	}

	public void UpdateFlightStatus(bool _landed,bool _inLandingzone,string _inZoneType)
	{
		landed = _landed;
		inLandingZone = _inLandingzone;
		inZoneType = _inZoneType;
	}

	public void SetLandingZoneIndex(int _index)
	{
		temp_lastLandedIndex = _index;

	}

	public void SaveASurvivor()
	{
		Debug.Log ("You just saved a survivor");
	}
}
