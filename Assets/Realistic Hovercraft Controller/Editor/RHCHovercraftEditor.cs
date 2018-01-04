//----------------------------------------------
//      Realistic Hovercraft Controller
//
// Copyright © 2015 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(RHC_HovercraftController))]
public class RHCHovercraftEditor : Editor {

	RHC_HovercraftController hoverController;
	Color defBackgroundColor;
	static bool firstInit = false;

	[MenuItem("Tools/BoneCracker Games/Realistic Hovercraft Controller/Add Main Controller To Vehicle")]
	static void CreateBehavior(){
		
		if(!Selection.activeGameObject.GetComponent<RHC_HovercraftController>()){

			firstInit = true;

			Selection.activeGameObject.AddComponent<RHC_HovercraftController>();
			
			Selection.activeGameObject.GetComponent<Rigidbody>().mass = 500f;
			Selection.activeGameObject.GetComponent<Rigidbody>().drag = .5f;
			Selection.activeGameObject.GetComponent<Rigidbody>().angularDrag = 1f;
			Selection.activeGameObject.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
			Selection.activeGameObject.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

		}else{
			
			EditorUtility.DisplayDialog("Your Gameobject Already Has Realistic Hovercraft Controller", "Your Gameobject Already Has Realistic Hovercraft Controller", "Ok");
			
		}
		
	}

	void Awake(){

		defBackgroundColor = GUI.backgroundColor;

	}

	public override void OnInspectorGUI () {

		serializedObject.Update();

		hoverController = (RHC_HovercraftController)target;

		if(firstInit){
			CreateCOM();
			CreateThruster();
			EngineCurveInit();
		}

		EditorGUILayout.Space();
		GUI.color = Color.cyan;
		EditorGUILayout.HelpBox("Behavior Type", MessageType.None);
		GUI.color = defBackgroundColor;
		EditorGUILayout.Space();

		EditorGUILayout.HelpBox("Choose your behavior type. Stable, or speedy?", MessageType.Info);
		EditorGUILayout.Space();

		hoverController.GetComponent<Rigidbody>().drag = EditorGUILayout.Slider(new GUIContent("Stable - Speedy", "More stable or more speedy?"), hoverController.GetComponent<Rigidbody>().drag, 1f, .1f);
		//EditorGUILayout.PropertyField(serializedObject.FindProperty("engineTorqueCurve"), new GUIContent("Engine Torque Curve", "Engine Torque Curve"));

		EditorGUILayout.Space();
		GUI.color = Color.cyan;
		EditorGUILayout.HelpBox("Misc", MessageType.None);
		GUI.color = defBackgroundColor;
		EditorGUILayout.Space();

		EditorGUILayout.PropertyField(serializedObject.FindProperty("_controllerType"), new GUIContent("Controller Type", "Controller Type"));
		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("canControl"), new GUIContent("Can Control", "Craft can be controllable now?"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("engineRunning"), new GUIContent("Engine Running", "Is craft engine running?"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("COM"), new GUIContent("Center Of Mass Position", "Center of mass position"));

		EditorGUILayout.Space();
		GUI.color = Color.cyan;
		EditorGUILayout.HelpBox("Stabilizer Thrusters", MessageType.None);
		GUI.color = defBackgroundColor;
		EditorGUILayout.Space();

		if(GUILayout.Button("Create New Thruster"))
			CreateThruster();

		EditorGUILayout.PropertyField(serializedObject.FindProperty("stabilizerThrusters"), new GUIContent("Stabilizer Thrusters", "Stabilizer Thrusters"), true);
		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("stabilizerConstant"), new GUIContent("Stabilizer Thrusters Force", "Stabilizer Thrusters Force"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("stabilizerDamperConstant"), new GUIContent("Stabilizer Thrusters Damper", "Stabilizer Thrusters Damper"), true);
		hoverController.stableHeight = EditorGUILayout.Slider(new GUIContent("Stable Height", "Stable height of the craft."), hoverController.stableHeight, 1f, 20f);
		EditorGUILayout.Space();
		GUI.color = Color.cyan;
		EditorGUILayout.HelpBox("Torques", MessageType.None);
		GUI.color = defBackgroundColor;
		EditorGUILayout.Space();

		EditorGUILayout.PropertyField(serializedObject.FindProperty("engineTorqueCurve"), new GUIContent("Engine Torque Curve", "Engine Torque Curve"));
		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("motorTorque"), new GUIContent("Engine Torque", "Engine Torque"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("steerTorque"), new GUIContent("Steering Torque", "Steering Torque"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("maximumSpeed"), new GUIContent("Maximum Speed", "Maximum speed of the craft"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("maximumAngularVelocity"), new GUIContent("Maximum Angular Velocity", "Maximum Angular Velocity"));

		EditorGUILayout.Space();
		GUI.color = Color.cyan;
		EditorGUILayout.HelpBox("Sounds", MessageType.None);
		GUI.color = defBackgroundColor;
		EditorGUILayout.Space();

		EditorGUILayout.PropertyField(serializedObject.FindProperty("engineSound"), new GUIContent("Engine Sound", "Engine Sound"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("crashSounds"), new GUIContent("Crash Sounds", "Crash Sounds"), true);

		EditorGUILayout.Space();
		GUI.color = Color.cyan;
		EditorGUILayout.HelpBox("Particles", MessageType.None);
		GUI.color = defBackgroundColor;
		EditorGUILayout.Space();

		EditorGUILayout.PropertyField(serializedObject.FindProperty("thrusterParticles"), new GUIContent("Thruster Particles", "Thruster Particles"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("thrusterGroundSmoke"), new GUIContent("Thruster Ground Particles", "Thruster Ground Particles"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("contactSparkle"), new GUIContent("Contact Sparkle", "Contact Sparkle"));

		EditorGUILayout.Space();
		GUI.color = Color.cyan;
		EditorGUILayout.HelpBox("Lights", MessageType.None);
		GUI.color = defBackgroundColor;
		EditorGUILayout.Space();

		EditorGUILayout.PropertyField(serializedObject.FindProperty("headLights"), new GUIContent("Head Lights", "Head Lights"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("particleLights"), new GUIContent("Particle Lights", "Particle Lights"), true);

		EditorGUILayout.Space();
		GUI.color = Color.cyan;
		EditorGUILayout.HelpBox("Damage", MessageType.None);
		GUI.color = defBackgroundColor;
		EditorGUILayout.Space();

		if(hoverController.useDamage){
			
			EditorGUILayout.PropertyField(serializedObject.FindProperty("deformableMeshFilters"), new GUIContent("Deformable Mesh Filters", "If no mesh filters selected, will collect all mesh filters in children."), true);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("randomizeVertices"), new GUIContent("Randomize Vertices", "Randomizes vertices movement angle."), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("damageRadius"), new GUIContent("Damage Radius on Contact", "Damage radius on contact."), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("maximumDamage"), new GUIContent("Maximum Damage", "Maximum deformable damage."), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("damageMultiplier"), new GUIContent("Damage Multiplier", "Damage mutliplier."), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("contactSparkle"), new GUIContent("Contact Sparkle", "Contact sparkle particles"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("maximumContactSparkle"), new GUIContent("Maximum Contact Sparkle Count", "Script is re-using existing collision particles. 3 is good for mobile, 5 and above is good for PC."), false);
			
			if(hoverController.sleep){
				GUI.color = Color.gray;
				GUILayout.Button("Repaired");
			}else{
				GUI.color = Color.green;
				if(GUILayout.Button("Repair Now"))
					hoverController.repairNow = true;
			}
			
		}

		serializedObject.ApplyModifiedProperties();
		serializedObject.UpdateIfDirtyOrScript();

		if(GUI.changed && !EditorApplication.isPlaying){
			EngineCurveInit();
		}

		if (GUI.changed)
			EditorUtility.SetDirty(hoverController);

	}

	void CreateCOM(){
		
		GameObject COM = new GameObject("COM");
		COM.transform.parent = hoverController.transform;
		COM.transform.localPosition = Vector3.zero;
		COM.transform.localScale = Vector3.one;
		COM.transform.rotation = hoverController.transform.rotation;
		hoverController.COM = COM.transform;
		
		firstInit = false;
		
	}

	void CreateThruster(){

		GameObject newThruster = new GameObject();
		newThruster.transform.name = "Stabilizer Thruster";
		newThruster.transform.SetParent(hoverController.transform);
		newThruster.transform.localPosition = new Vector3(0f, -.5f, 0f);
		newThruster.transform.localRotation = Quaternion.identity;
		newThruster.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
		newThruster.AddComponent<RHC_Thruster> ();
		hoverController.stabilizerThrusters.Add(newThruster.transform);

	}

	void EngineCurveInit (){
		
		hoverController.engineTorqueCurve = new AnimationCurve(new Keyframe(0, 1f));
		hoverController.engineTorqueCurve.AddKey(new Keyframe(hoverController.maximumSpeed, .1f));
		hoverController.engineTorqueCurve.postWrapMode = WrapMode.Clamp;
		
	}

}
