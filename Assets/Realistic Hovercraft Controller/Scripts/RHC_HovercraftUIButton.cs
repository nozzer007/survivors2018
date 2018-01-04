//----------------------------------------------
//      Realistic Hovercraft Controller
//
// Copyright © 2015 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class RHC_HovercraftUIButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
	
	public float input;
	public float sensitivity = 3f;
	private bool pressing;
	
	public void OnPointerDown(PointerEventData eventData){
		
		pressing = true;
		
	}
	
	public void OnPointerUp(PointerEventData eventData){
		
		pressing = false;
		
	}

	void OnPress (bool isPressed)
	{

			
		if(isPressed)
			pressing = true;
		else
			pressing = false;
	}
	
	void Update(){

		//Analogue return if not Action Button
		if (this.gameObject.name != "Action")
		{
			if (pressing)
				input += Time.deltaTime * sensitivity;
			else
				input -= Time.deltaTime * sensitivity;
		
			if (input < 0f)
				input = 0f;
			if (input > 1f)
				input = 1f;
		}
		else
		//Digital return if Action Button
		{
			if (pressing)
				input = 1;
			else
				input = 0;
		}
		
	}
	
}
