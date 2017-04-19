// (c) Copyright TemperTantrum, http://tempertantrum.com.au/. All rights reserved.
// (c) Copyright F.T.R, http://www.fantasytoreality.com.au/. All rights reserved.
// (c) Playmaker plugin by HutongGames, LLC 2010-2013. All rights reserved.
// (c) Kinect plugin by RF Solutions. All rights reserved.using HutongGames.PlayMaker;
using UnityEngine;
using System.Collections;
using System;

/*
 * This playmaker script gets the rotation of the Kinect
 * joint specified. Depending on the PlayMakerUpdateCallType value
 * selected by the user, one of three update calls may be made, or
 * if GetOnce was selected the script will finish after attempting 
 * to get the joint rotation.
 * 
 * It should be noted that if the player has selected GetOnce and 
 * the player is not on the screen, no variable will be set as
 * no data has been gathered about the selected joint.
 */
namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Kinect SDK")]//Adds the script to the current category in playmaker actions, or adds the category if it does not exist
	[Tooltip("Get the rotation of the selected joint.")]//The tooltip to appear when hovering over the action
	
	public class GetKinectJointRotation : FsmStateAction//Extends FsmStateAction to be a playmaker script
	{
		[RequiredField]//The next variable is required
		[CheckForComponent(typeof(KinectManager))]//Check that the GameObject has the KinectManager component
		[Tooltip("The GameObject to get KinectManager from.")]//Tooltip to display when hovering over the variable
		public FsmOwnerDefault kinectManager;//Holds the GameObject that contains the Kinect manager
		
		[Tooltip("Which joint you want to get the rotation of.")]//Tooltip to display when hovering over the variable
		public KinectWrapper.SkeletonJoint kinJoint;//Create an enum using the SkeletonJoint enum found in KinectWrapper class, selectable by user
		
		[UIHint(UIHint.Variable)]//Display what type of variable the user should pass in
		[Tooltip("Quaternion to store the position of the joint selected.")]//Tooltip to display when hovering over the variable
		public FsmQuaternion storeResult;//Quaternion to store the result
				
		public enum PlayerType {PLAYER_ONE, PLAYER_TWO};//Define the enum
		[Tooltip("Which player to track.")]//Tooltip to display when hovering over the variable
		public PlayerType player;//Create an instance of the enum
		
		[Tooltip("Flips the rotation.")]//Tooltip to display when hovering over the variable
		public FsmBool flipRotation = false;//Whether to flip rotation or not, false by default 

		[Tooltip("Repeat this action every frame. Useful if Activate changes over time.")]
		public bool everyFrame;

		private KinectManager manager;//Holds the KinectManager from kinectManager passed in by user
		private uint userId;//Holds the ID of the player
		private bool flipped;//Holds the value passed in by flipRotation
		
		//when the script is first run
		public override void OnEnter()
		{			
			manager = kinectManager.GameObject.Value.gameObject.GetComponent<KinectManager>();//Retrieve the KinectManager component
			flipped = flipRotation.Value;//Retrieve the value from flipRotation
			
			getKinJointRotation();

			if (!everyFrame)
			{
				Finish();
			}
		}

		
		public override void OnUpdate()
		{
			getKinJointRotation();			
		}
		
		/*
		 * This method stores the rotation of the selected joint and will finishes if
		 * the user selected to only run once, regardless of whether the joint was found or not.
		 */
		private void getKinJointRotation()
		{	
			if(manager != null && KinectManager.IsKinectInitialized())
			{
				if(player == PlayerType.PLAYER_ONE && manager.GetPlayer1ID() > 0)//If user wanted to track player 1 and they exist on screen
					userId = manager.GetPlayer1ID();//Set the userId to player 1
				else if(player == PlayerType.PLAYER_TWO && manager.GetPlayer2ID() > 0)//If user wanted to track player 2 and they exist on screen
					userId = manager.GetPlayer2ID();//Set the userId to player 2
				
				storeResult.Value = manager.GetJointOrientation(userId, (int)kinJoint, flipped);//Get the joint rotation and set the returned value to storeResult
			}
		}
	}//End of class
}//End of namespace