//----------------------------------------------
//      Realistic Hovercraft Controller
//
// Copyright © 2015 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using System.Collections;

public class RHC_HovercraftAnimator : MonoBehaviour {

	private RHC_HovercraftController hoverController;
	private Animator animator;


	void Awake () {
	
		hoverController = GetComponent<RHC_HovercraftController>();
		animator = GetComponent<Animator>();

	}

	void Update () {

		animator.SetFloat("steer", hoverController.steerInput);
	
	}

}
