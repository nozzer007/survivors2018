using UnityEngine;
using System.Collections;

public class LevelObjectives : MonoBehaviour {

	protected string LevelTitle;
	protected string LevelName;
	public Transform[] levelTargets;

	public Transform[] spawnPoints;

	public int lastLandedIndex;
	public float timeForLevel = 121;//time in seconds


	public void Awake(){

		if (levelTargets.Length == 0) {
			Debug.Log ("There are no level targets declared in LevelObjectives cannot be removed from objectives list.");
		} else {
			for (int i = 0; i < levelTargets.Length; i++) {

				if (levelTargets [i] == null) {
					Debug.Log ("There is a missing objective in " + this.name + " at index number " + i + " you cant have missing objectives, either shorte the array or add in the missing objective");
				}
			}
		}

	}
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}


	public virtual void ObjectivesForLevel()
	{
		//list of gameobjectives that need to be completed e.g. things that need to be destroyed, people saved.
	}


	public void MessagesToPlayer()
	{
		
	}


	/*
	 * What is the current objective
	 * Are there any objectives before that, which arent completed?
	 * N - Continue
	 * Y - Highlight on radar what they are and put a messageto the player to say, you need to go back and complete those objectives on the radar.
	 * */

	//checks to see if you complete the next mandatory objective without completing the last.

	public virtual bool CheckObjectivesCompleted()
	{
		return false;
	}

	public virtual bool CheckObjectivesCompletedInOrder()
	{
		return false;
	}

	public void ShowRemainingObjectivesToBeCompleted(int lastindex)
	{
		return;
	}

	//public virtual Transform GetNextObjective()
	//{
	//
	//}

	//public virtual void StartRescue(int in_rescueIndex)
	//{
	//
	//}

}