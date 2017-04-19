// (c) Copyright TemperTantrum, http://tempertantrum.com.au/. All rights reserved.
// (c) Copyright F.T.R, http://www.fantasytoreality.com.au/. All rights reserved.
// (c) Playmaker plugin by HutongGames, LLC 2010-2013. All rights reserved.
// (c) Kinect plugin by RF Solutions. All rights reserved.using HutongGames.PlayMaker;using HutongGames.PlayMaker;using HutongGames.PlayMaker;using HutongGames.PlayMaker;
using HutongGames.PlayMaker;
using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Kinect SDK")]
	[Tooltip("Allows you to use a Gameobject as a 3D cursor. Use this action to alternate control of the cursor between left and right hand.")]
	
	public class kinect3DTwoHandCursor : FsmStateAction
	{
		[CheckForComponent(typeof(KinectManager))]
		[Tooltip("The GameObject to get KinectManager from.")]
		public FsmOwnerDefault kinectManager;
		
		[RequiredField]
		[CheckForComponent(typeof(Camera))]
		public FsmGameObject cameraToUse;
		
		[Tooltip("Cursor to be controlled by the users right hand.")]//Tooltip to display when hovering over the variable
		public FsmGameObject handCursorRight;//GameObject with the GUITexture, tracked by users right hand
		
		[Tooltip("Cursor to be controlled by the users left hand.")]//Tooltip to display when hovering over the variable
		public FsmGameObject handCursorLeft;//GameObject with the GUITexture, tracked by users left hand
		
		[RequiredField]
		public FsmFloat distance;

		public enum PlayerType {PLAYER_ONE, PLAYER_TWO};//Define the enum
		[Tooltip("Which player to track.")]//Tooltip to display when hovering over the variable
		public PlayerType player;//Create an instance of the enum

		[Tooltip("Repeat this action every frame. Useful if Activate changes over time.")]
		public bool everyFrame;

		private KinectManager manager;
		private float speed = 3.0f;
		private Camera camera;
		private uint userId;//Holds the ID of the player
		
		//when the script is first run
		public override void OnEnter()
		{			
			manager = kinectManager.GameObject.Value.gameObject.GetComponent<KinectManager>();
			camera = cameraToUse.Value.GetComponent<Camera>();

			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			UpdateHands();			
		}
		
		private void UpdateHands()
		{
			if(manager != null && KinectManager.IsKinectInitialized())
			{	
				Vector3 screenNormalPos = Vector3.zero;
				Vector3 targetPos;
				Ray ray;
				
				if(player == PlayerType.PLAYER_ONE && manager.GetPlayer1ID() > 0)//If user wanted to track player 1 and they exist on screen
					userId = manager.GetPlayer1ID();//Set the userId to player 1
				else if(player == PlayerType.PLAYER_TWO && manager.GetPlayer2ID() > 0)//If user wanted to track player 2 and they exist on screen
					userId = manager.GetPlayer2ID();//Set the userId to player 2

				// cursor control
				if(manager.GetGestureProgress(userId, KinectGestures.Gestures.RightHandCursor) >= 0.1f)
				{
					if(handCursorRight.Value)
					{
						screenNormalPos = manager.GetGestureScreenPos(userId, KinectGestures.Gestures.RightHandCursor);
						screenNormalPos.z = distance.Value;
						targetPos = camera.ScreenToWorldPoint(new Vector3(screenNormalPos.x * Screen.width, screenNormalPos.y * Screen.height, camera.transform.forward.z + distance.Value));
						handCursorRight.Value.transform.position = Vector3.Lerp(handCursorRight.Value.transform.position, targetPos, 1 * Time.deltaTime);
					}
				}

				if(manager.GetGestureProgress(userId, KinectGestures.Gestures.LeftHandCursor) >= 0.1f)
				{
					if(handCursorLeft.Value)
					{
						screenNormalPos = manager.GetGestureScreenPos(userId, KinectGestures.Gestures.LeftHandCursor);
						screenNormalPos.z = distance.Value;			
						targetPos = camera.ScreenToWorldPoint(new Vector3(screenNormalPos.x * Screen.width, screenNormalPos.y * Screen.height, camera.transform.forward.z + distance.Value));
						handCursorLeft.Value.transform.position = Vector3.Lerp(handCursorLeft.Value.transform.position, targetPos, 1 * Time.deltaTime);
					}
				}
			}
		}
	}
}