// (c) Copyright TemperTantrum, http://tempertantrum.com.au/. All rights reserved.
// (c) Copyright F.T.R, http://www.fantasytoreality.com.au/. All rights reserved.
// (c) Playmaker plugin by HutongGames, LLC 2010-2013. All rights reserved.
// (c) Kinect plugin by RF Solutions. All rights reserved.using HutongGames.PlayMaker;using HutongGames.PlayMaker;using HutongGames.PlayMaker;using HutongGames.PlayMaker;
using HutongGames.PlayMaker;
using UnityEngine;
using System.Collections;
using System;

/*
 * This script waits for the player to be lost and then 
 * sends the event when they are found
 */
namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Kinect SDK")]//Adds the script to the current category in playmaker actions, or adds the category if it does not exist
	[Tooltip("Waits for the selected player to be detected, then triggers the event.")]//The tooltip to appear when hovering over the action
	
	public class OnKinectPlayerLost : FsmStateAction//Extends FsmStateAction to be a playmaker script
	{
		[RequiredField]//The next variable is required
		[CheckForComponent(typeof(KinectManager))]//Check that the GameObject has the KinectManager component
		[Tooltip("The GameObject to get KinectManager from.")]//Tooltip to display when hovering over the variable
		public FsmOwnerDefault kinectManager;//Holds the GameObject that contains the Kinect manager
		
		public enum PlayerType { PLAYER_ONE, PLAYER_TWO };
		[Tooltip("The player to detect.")]//Tooltip to display when hovering over the variable
		public PlayerType player;//Holds the player the user wants to detect
		
		[UIHint(UIHint.Variable)]//Display what type of variable the user should pass in
		[Tooltip("Event to send when player is lost.")]//Tooltip to display when hovering over the variable
		public FsmEvent sendEvent;//Holds the event the user wants to trigger when the player is lost
		
		private KinectManager manager;//Holds the KinectManager from kinectManager passed in by user
		
		//when the script is first run
		public override void OnEnter()
		{			
			manager = kinectManager.GameObject.Value.gameObject.GetComponent<KinectManager>();//Get the speech manager
		}
		
		public override void OnUpdate()
		{
			if(manager != null && KinectManager.IsKinectInitialized())
			{
				DetectPlayer();
			}
		}
		
		/*
		 * This method checks if the user is not on the screen. If they
		 * aren't the event is sent.
		 */
		private void DetectPlayer()
		{
			if(player == PlayerType.PLAYER_ONE && manager.GetPlayer1ID() == 0)//If user wanted to track player 1 and they don't exist on screen
				Fsm.Event(sendEvent);//Send the event
			else if(player == PlayerType.PLAYER_TWO && manager.GetPlayer2ID() == 0)//If user wanted to track player 2 and don't they exist on screen
				Fsm.Event(sendEvent);//Send the event
		}
	}//End of class
}//End of namespace