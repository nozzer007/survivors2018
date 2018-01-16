using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundLandingCollider : MonoBehaviour {
	GameController gameController;
	void Awake()
	{
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null)
		{
			gameController = gameControllerObject.GetComponent <GameController>();
		}
		if (gameController == null)
		{
			Debug.Log ("Cannot find 'GameController' in ShipGunGeneric script");
		}
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnTriggerExit(Collider col)
	{

		//hidden
		if (col.transform.gameObject.name == "Hovercraft") {

			//landed,inLandingzone
			gameController.UpdateFlightStatus(false,false,"TERRAINTOFF",false);
			//gameController.LandingPressed = false;

		}
	}
}
