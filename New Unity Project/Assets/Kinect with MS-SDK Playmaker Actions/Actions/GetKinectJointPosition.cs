// (c) Copyright TemperTantrum, http://tempertantrum.com.au/. All rights reserved.
// (c) Copyright F.T.R, http://www.fantasytoreality.com.au/. All rights reserved.
// (c) Playmaker plugin by HutongGames, LLC 2010-2013. All rights reserved.
// (c) Kinect plugin by RF Solutions. All rights reserved.
using HutongGames.PlayMaker;
using UnityEngine;
using System.Collections;
using System;

/*
 * This playmaker script gets the position of the Kinect
 * joint specified. Depending on the PlayMakerUpdateCallType value
 * selected by the user, one of three update calls may be made, or
 * if GetOnce was selected the script will finish after attempting 
 * to get the joint position.
 * 
 * It should be noted that if the player has selected GetOnce and 
 * the player is not on the screen, no variable will be set as
 * no data has been gathered about the selected joint.
 */
namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Kinect SDK")]//Adds the script to the current category in playmaker actions, or adds the category if it does not exist
	[Tooltip("Get the position of the selected joint.")]//The tooltip to appear when hovering over the action
	
	public class GetKinectJointPosition : FsmStateAction//Extends FsmStateAction to be a playmaker script
	{
		[RequiredField]//The next variable is required
		[CheckForComponent(typeof(KinectManager))]//Check that the GameObject has the KinectManager component
		[Tooltip("The GameObject to get KinectManager from.")]//Tooltip to display when hovering over the variable
		public FsmOwnerDefault kinectManager;//Holds the GameObject that contains the Kinect manager
		
		[Tooltip("Which joint you want to get the position of.")]//Tooltip to display when hovering over the variable
		public KinectWrapper.SkeletonJoint kinJoint;//Create an enum using the SkeletonJoint enum found in KinectWrapper class, selectable by user
		
		[UIHint(UIHint.Variable)]//Display what type of variable the user should pass in
		[Tooltip("Store the position of the joint selected.")]//Tooltip to display when hovering over the variable
		public FsmVector3 storeResult;//The Vector3 to restore the result of the position
				
		//Allow the user to determine which coordinate type to use
		public enum CoordinateType {VIEWPORT, SCREEN, WORLD};//Define the enum
		[Tooltip("Type of coordinates to retrieve.")]//Tooltip to display when hovering over the variable
		public CoordinateType coordinateType;//Create an instance of the enum
		
		public enum PlayerType {PLAYER_ONE, PLAYER_TWO};//Define the enum
		[Tooltip("Which player to track.")]//Tooltip to display when hovering over the variable
		public PlayerType player;//Create an instance of the enum
		
		[CheckForComponent(typeof(Camera))]//Check that the GameObject has the Camera component
		[UIHint(UIHint.Variable)]//Display what type of variable the user should pass in
		[Tooltip("Optional: Camera to use for world or viewport coordinates. Main used if empty.")]//Tooltip to display when hovering over the variable
		public FsmGameObject cameraToUse;//Camera to use to convert to world or viewport coordinates

		[Tooltip("Repeat this action every frame. Useful if Activate changes over time.")]
		public bool everyFrame;

		private KinectManager manager;//Holds the KinectManager from kinectManager passed in by user
		private Camera camera;//Holds the value from cameraToUse
		
		//when the script is first run
		public override void OnEnter()
		{			
			manager = kinectManager.GameObject.Value.gameObject.GetComponent<KinectManager>();//Retrieve the KinectManager component
			
			if(cameraToUse.Value != null)//If the user actually passed in a variable for the camera
				camera = cameraToUse.Value.gameObject.GetComponent<Camera>();//Retrieve the Camera component
			
			getKinJointPos();

			if (!everyFrame)
			{
				Finish();
			}

		}
		
		public override void OnUpdate()
		{
			getKinJointPos();			
		}
		
		/*
		 * This method gets the joint position and converts it to the type of coordinates
		 * selected by the user. The converted coordinates are then stored in the variable
		 * given by the user.
		 */
		private void getKinJointPos()
		{		
			if(manager != null && KinectManager.IsKinectInitialized())
			{
				uint userId;//Holds the ID of the player
				Vector3 coords;//Holds coordinates of joint
				
				if(player == PlayerType.PLAYER_ONE)//If user wants player ones info
					userId = manager.GetPlayer1ID();//Get player ones ID
				else//Otherwise they must want player twos info
					userId = manager.GetPlayer2ID();//Get player twos info
				
				coords = manager.GetJointPosition(userId, (int)kinJoint);//Get the position of the selected joint
				
				if(coordinateType == CoordinateType.WORLD)//If the user wants world coordinates
				{
					if(camera != null)//If the user defined a camera
						storeResult.Value = camera.ViewportToWorldPoint(coords);//Get the coordinates using their camera
					else
						storeResult.Value = Camera.main.ViewportToWorldPoint(coords);//Get the coordinates using main camera
				}
				else if(coordinateType == CoordinateType.SCREEN)//If the user wanted screen coordinates
				{
					if(camera != null)//If user defined a camera
						storeResult.Value = camera.ViewportToScreenPoint(coords);//Get the coordinates using their camera
					else
						storeResult.Value = Camera.main.ViewportToScreenPoint(coords);//Get the coordinates using main camera
				}
				else//If the user wanted viewport coordinates
					storeResult.Value = coords;//Store the coordinates, viewport by default
			}
		}
	}//End of class
}//End of namespace