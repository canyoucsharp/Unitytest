// (c) Copyright TemperTantrum, http://tempertantrum.com.au/. All rights reserved.
// (c) Copyright F.T.R, http://www.fantasytoreality.com.au/. All rights reserved.
// (c) Playmaker plugin by HutongGames, LLC 2010-2013. All rights reserved.
// (c) Kinect plugin by RF Solutions. All rights reserved.using HutongGames.PlayMaker;using HutongGames.PlayMaker;using HutongGames.PlayMaker;using HutongGames.PlayMaker;
using HutongGames.PlayMaker;
using UnityEngine;
using System.Collections;
using System;

/*
 * This playmaker script will wait until the chosen gesture is detected
 * and will then send the event chosen by the player, as well as store
 * the gesture as a string once it has been detected.
 */
namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Kinect SDK")]//Adds the script to the current category in playmaker actions, or adds the category if it does not exist
	[Tooltip("Allows you to detect a gesture from the kinect. NOTE That you must make sure the gesture is listed under KinectManager Script.")]//The tooltip to appear when hovering over the action
	
	public class OnKinectGesture : FsmStateAction//Extends FsmStateAction to be a playmaker script
	{
		[RequiredField]//Make the next variable required
		[CheckForComponent(typeof(KinectManager))]//Check that the GameObject has the KinectManager component
		[Tooltip("The GameObject to get KinectManager from.")]//Tooltip to display when hovering over the variable
		public FsmOwnerDefault kinectManager;//Holds the GameObject that contains the Kinect manager
		
		[Tooltip("Which gesture you want to detect.")]//Tooltip to display when hovering over the variable
		public KinectGestures.Gestures kinectGesture;//Get the list from Gestures enum found in KinectWrapper
		
		[UIHint(UIHint.Variable)]//Display what type of variable the user should pass in
		[Tooltip("Store the name of the gesture called.")]//Tooltip to display when hovering over the variable
		public FsmString storeResult;//String to store the name of the gesture
				
		[RequiredField]//The next variable is required
		[UIHint(UIHint.Variable)]//Display what type of variable the user should pass in
		[Tooltip("Event to send after gesture has been detected.")]//Tooltip to display when hovering over the variable
		public FsmEvent sendEvent;//Stores the event the user wishes to be sent upon gesture being detected

		public enum PlayerType {PLAYER_ONE, PLAYER_TWO};//Define the enum
		[Tooltip("Which player to track.")]//Tooltip to display when hovering over the variable
		public PlayerType player;//Create an instance of the enum

		[Tooltip("Repeat this action every frame. Useful if Activate changes over time.")]
		public bool everyFrame;

		private KinectManager manager;//Holds the KinectManager from kinectManager passed in by user
		private uint userId;//Holds the ID of the player
		
		//when the script is first run
		public override void OnEnter()
		{			
			manager = kinectManager.GameObject.Value.gameObject.GetComponent<KinectManager>();//Retrieve the KinectManager component

			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			sendKinectEvent();			
		}
		
		/*
		 * This method checks to see if the gesture selected by the user
		 * has been completed or not. If it has it sets the storeResult
		 * variable to the gesture as a string, and then sends the event
		 * chosen by the user.
		 */
		private void sendKinectEvent()
		{		
			if(manager != null && KinectManager.IsKinectInitialized())
			{
				if(player == PlayerType.PLAYER_ONE && manager.GetPlayer1ID() > 0)//If user wanted to track player 1 and they exist on screen
					userId = manager.GetPlayer1ID();//Set the userId to player 1
				else if(player == PlayerType.PLAYER_TWO && manager.GetPlayer2ID() > 0)//If user wanted to track player 2 and they exist on screen
					userId = manager.GetPlayer2ID();//Set the userId to player 2			
						
				if(manager.IsGestureComplete(userId, kinectGesture, true))//If the gesture has successfully been done
				{
					storeResult.Value = kinectGesture.ToString();//Convert the gesture name to a string and store it
					Fsm.Event(sendEvent);//Send the event
				}
			}
		}
	}//End of class
}//End of namespace