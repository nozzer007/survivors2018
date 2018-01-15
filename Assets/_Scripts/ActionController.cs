using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionController : MonoBehaviour {

	public bool inLandingZone = true;
	public bool fireLaser = false;
	public bool landed = true;


	void Awake()
	{
	}

	// Use this for initialization
	void Start () {


	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public string PerformActionButton2()
	{
		//I've pressed the Action button triggered in RHC_HoverController
		//Debug.Log("Called Action");
		//Im in the landing zone and landed
		if (inLandingZone && landed) {
			//TriggerTakeOff;
			fireLaser = false;
			return "LANDED";
		}
		//Im in the landing zone but not landed
		if (inLandingZone && !landed) {
			//TriggerTakeOff
			//landed = true;
			fireLaser = false;
			return "READYTOLAND";
		}

		if (!inLandingZone) {
			//FireLaser
			fireLaser = true;
			return "LASER";
		}

		return "LASER";
			
	}
		
}
