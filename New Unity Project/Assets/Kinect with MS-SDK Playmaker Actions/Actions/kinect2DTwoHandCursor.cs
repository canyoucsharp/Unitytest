// (c) Copyright TemperTantrum, http://tempertantrum.com.au/. All rights reserved.
// (c) Copyright F.T.R, http://www.fantasytoreality.com.au/. All rights reserved.
// (c) Playmaker plugin by HutongGames, LLC 2010-2013. All rights reserved.
// (c) Kinect plugin by RF Solutions. All rights reserved.using HutongGames.PlayMaker;using HutongGames.PlayMaker;using HutongGames.PlayMaker;
using UnityEngine;
using System.Collections;

/*
 * This playmaker script allows the user to use both
 * their hands to simultaneously control two GUITextures
 * as on-screen cursors.
 */
namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Kinect SDK")]//Adds the script to the current category in playmaker actions, or adds the category if it does not exist
	[Tooltip("Allows you to track either the left or right hand from kinect to control the cursor on the screen. Use this action to control 1-2 cursors.")]//The tooltip to appear when hovering over the action
	
	public class Kinect2DTwoHandCursor : FsmStateAction//Extends FsmStateAction to be a playmaker script
	{
		[RequiredField]//The next variable is required
		[CheckForComponent(typeof(KinectManager))]//Check that the GameObject has the KinectManager component
		[Tooltip("The GameObject to get KinectManager from.")]//Tooltip to display when hovering over the variable
		public FsmOwnerDefault kinectManager;//Holds the GameObject that contains the Kinect manager
		
		[CheckForComponent(typeof(GUITexture))]//Check the that the GameObject has a GUITexture component
		[Tooltip("Cursor to be controlled by the users right hand.")]//Tooltip to display when hovering over the variable
		public FsmGameObject handCursorRight;//GameObject with the GUITexture, tracked by users right hand
		
		[CheckForComponent(typeof(GUITexture))]//Check the that the GameObject has a GUITexture component
		[Tooltip("Cursor to be controlled by the users left hand.")]//Tooltip to display when hovering over the variable
		public FsmGameObject handCursorLeft;//GameObject with the GUITexture, tracked by users left hand

		public enum PlayerType {PLAYER_ONE, PLAYER_TWO};//Define the enum
		[Tooltip("Which player to track.")]//Tooltip to display when hovering over the variable
		public PlayerType player;//Create an instance of the enum

		[Tooltip("Repeat this action every frame. Useful if Activate changes over time.")]
		public bool everyFrame;

		private KinectManager manager;//Holds the KinectManager from the kinectManager the user passed in
		
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
			UpdateHands();			
		}
		
		// Update is called once per frame
		private void UpdateHands()
		{
			if(manager != null && KinectManager.IsKinectInitialized())
			{	
				uint userId;//Get which player is being tracked
				Vector3 screenNormalPos;//Holds the position	
				
				if(player == PlayerType.PLAYER_ONE)//If user wants player ones info
					userId = manager.GetPlayer1ID();//Get player ones ID
				else//Otherwise they must want player twos info
					userId = manager.GetPlayer2ID();//Get player twos info
				
				if(manager.GetGestureProgress(userId, KinectGestures.Gestures.RightHandCursor) >= 0.1f)//If the gesture has started (hand is moving)
				{
					if(handCursorRight.Value)
					{
						screenNormalPos = manager.GetGestureScreenPos(userId, KinectGestures.Gestures.RightHandCursor);//Get the screeen position of gesture
						handCursorRight.Value.transform.position = Vector3.Lerp(handCursorRight.Value.transform.position, screenNormalPos, 3 * Time.deltaTime);//Lerp between old and new position
					}
				}
				
				if(manager.GetGestureProgress(userId, KinectGestures.Gestures.LeftHandCursor) >= 0.1f)//If the gesture has started (hand is moving)
				{
					if(handCursorLeft.Value)
					{
						screenNormalPos = manager.GetGestureScreenPos(userId, KinectGestures.Gestures.LeftHandCursor);//Get the screeen position of gesture
						handCursorLeft.Value.transform.position = Vector3.Lerp(handCursorLeft.Value.transform.position, screenNormalPos, 3 * Time.deltaTime);//Lerp between old and new position
					}
				}
			}
		}
	}//End of class
}//End of namespace
