// (c) Copyright TemperTantrum, http://tempertantrum.com.au/. All rights reserved.
// (c) Copyright F.T.R, http://www.fantasytoreality.com.au/. All rights reserved.
// (c) Playmaker plugin by HutongGames, LLC 2010-2013. All rights reserved.
// (c) Kinect plugin by RF Solutions. All rights reserved.using HutongGames.PlayMaker;using HutongGames.PlayMaker;using HutongGames.PlayMaker;using HutongGames.PlayMaker;
using UnityEngine;
using System.Collections;

/*
 * This playmaker script allows the user to use either
 * their left or right hand to control a GameObject as
 * an on-screen cursor.
 */
namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Kinect SDK")]//Adds the script to the current category in playmaker actions, or adds the category if it does not exist
	[Tooltip("Allows you to use a Gameobject as a 3D cursor. Use this action to alternate control of the cursor between left and right hand.")]//The tooltip to appear when hovering over the action
	
	public class Kinect3DHandCursor : FsmStateAction//Extends FsmStateAction to be a playmaker script
	{
		[RequiredField]//The next variable is required
		[CheckForComponent(typeof(KinectManager))]//Check that the GameObject has the KinectManager component
		[Tooltip("The GameObject to get KinectManager from.")]//Tooltip to display when hovering over the variable
		public FsmOwnerDefault kinectManager;//Holds the GameObject that contains the Kinect manager
		
		[CheckForComponent(typeof(Camera))]//Check that the GameObject has a Camera component
		[Tooltip("The Camera that the world coordinates will be translated to. Main camera used if none provided")]//Tooltip to display when hovering over the variable
		public FsmGameObject cameraToUse;//Holds the camera that world coordinates will be translated to
		
		[Tooltip("The GameObject to get KinectManager from.")]//Tooltip to display when hovering over the variable
		public FsmGameObject handCursor;//Holds the GameObject that will be moved around as a cursor
		
		[RequiredField]//The next variable is required
		[Tooltip("How far away the cursor should appear.")]//Tooltip to display when hovering over the variable
		public FsmFloat distance;//Hold how far away the object should appear
		
		public enum KinectHand {EitherHand, RightHand, LeftHand};//Define an enum
		[Tooltip("Select how you want to control the cursor.")]//Tooltip to display when hovering over the variable
		public KinectHand hand;//Create an instance of the enum

		public enum PlayerType {PLAYER_ONE, PLAYER_TWO};//Define the enum
		[Tooltip("Which player to track.")]//Tooltip to display when hovering over the variable
		public PlayerType player;//Create an instance of the enum

		[Tooltip("Repeat this action every frame. Useful if Activate changes over time.")]
		public bool everyFrame;

		private KinectManager manager;//Holds the KinectManager from the kinectManager the user passed in
		private float speed = 3.0f;//The speed to lerp at
		private uint userId;//Holds the ID of the player
		private Camera camera;//Holds the Camera from the cameraToUse passed in by user
		
		//when the script is first run
		public override void OnEnter()
		{			
			manager = kinectManager.GameObject.Value.gameObject.GetComponent<KinectManager>();//Retrieve the KinectManager component
			if(cameraToUse.Value != null)
				camera = cameraToUse.Value.gameObject.GetComponent<Camera>();//Retrieve the Camera component

			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			UpdateHands();			
		}
		
		/*
		 * This method checks the type of hand tracking the user
		 * wanted to use and then calls the appropriate method
		 * to track that hand.
		 * If the user wants to track both hands, the right hand
		 * takes priority of tracking over the left hand.
		 */
		private void UpdateHands()
		{
			if(manager != null && KinectManager.IsKinectInitialized())
			{
				if(player == PlayerType.PLAYER_ONE && manager.GetPlayer1ID() > 0)//If user wanted to track player 1 and they exist on screen
					userId = manager.GetPlayer1ID();//Set the userId to player 1
				else if(player == PlayerType.PLAYER_TWO && manager.GetPlayer2ID() > 0)//If user wanted to track player 2 and they exist on screen
					userId = manager.GetPlayer2ID();//Set the userId to player 2
				
				if (hand == KinectHand.EitherHand)//If the user wants to use both hands for tracking
				{
					if(!rightHand())//Try track the right hand, and if its not found:
						leftHand();//Try track left hand
				}
				else if(hand == KinectHand.RightHand)//If the user wants to track only the right hand
				{
					rightHand();//Try track the right hand
				}
				else if(hand == KinectHand.LeftHand)//If the user wants to track only the left hand
				{
					leftHand();//Try track the left hand
				}
			}
		}
		
		/*
		 * This method checks to see if the players right hand is moving at all
		 * and if it is it gets the position and lerps the movement between
		 * the current position and the new position.
		 * 
		 * True is returned if the right hand was being tracked. False otherwise.
		 * Used for the priority order for bothHands() to determine whether this
		 * hand was successfully tracked or not.
		 */
		private bool rightHand()
		{
			if(manager.GetGestureProgress(userId, KinectGestures.Gestures.RightHandCursor) >= 0.1f)//If the gesture has started (hand is moving)
			{
				Vector3 screenNormalPos;//Holds the Viewport position
				Vector3 targetPos;//screenNormalPos converted to world points, where the object should lerp towards
				
				if(handCursor.Value)//If user put something in the handCursor variable
				{
					screenNormalPos = manager.GetGestureScreenPos(userId, KinectGestures.Gestures.RightHandCursor);//Get the viewport position of gesture				
					screenNormalPos.z = distance.Value;//Change the z axis to the distance the user wants		
					
					if(camera == null)//If the user didn't define a camera
						targetPos = Camera.main.ViewportToWorldPoint(screenNormalPos);//Use the main Camera and convert to world points
					else//If the user did define a camera
						targetPos = camera.ViewportToWorldPoint(screenNormalPos);//Use the camera they provided and convert to world points
					
					handCursor.Value.transform.position = Vector3.Lerp(handCursor.Value.transform.position, targetPos, speed * Time.deltaTime);//Set the position to the lerp of current and new position
					
					return true;//Right hand was successfully tracked so return true
				}
			}
			
			return false;//Right hand was not successfully tracked so return false
		}
		
		/*
		 * This method checks to see if the players left hand is moving at all
		 * and if it is it gets the position and lerps the movement between
		 * the current position and the new position.
		 */
		private void leftHand()
		{
			if(manager.GetGestureProgress(userId, KinectGestures.Gestures.LeftHandCursor) >= 0.1f)//If the gesture has started (hand is moving)
			{
				Vector3 screenNormalPos;//Holds the Viewport position
				Vector3 targetPos;//screenNormalPos converted to world points, where the object should lerp towards
				
				if(handCursor.Value)//If user put something in the handCursor variable
				{
					screenNormalPos = manager.GetGestureScreenPos(userId, KinectGestures.Gestures.LeftHandCursor);//Get the viewport position of gesture					
					screenNormalPos.z = distance.Value;//Change the z axis to the distance the user wants	
					
					if(camera == null)//If the user didn't define a camera
						targetPos = Camera.main.ViewportToWorldPoint(screenNormalPos);//Use the main Camera and convert to world points
					else//If the user did define a camera
						targetPos = camera.ViewportToWorldPoint(screenNormalPos);//Use the camera they provided and convert to world points
					
					handCursor.Value.transform.position = Vector3.Lerp(handCursor.Value.transform.position, targetPos, speed * Time.deltaTime);//Set the position to the lerp of current and new position
				}	
			}
		}
	}//End of class
}//End of namespace