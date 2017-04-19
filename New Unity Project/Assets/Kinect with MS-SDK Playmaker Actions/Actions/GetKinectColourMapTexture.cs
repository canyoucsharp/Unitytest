// (c) Copyright TemperTantrum, http://tempertantrum.com.au/. All rights reserved.
// (c) Copyright F.T.R, http://www.fantasytoreality.com.au/. All rights reserved.
// (c) Playmaker plugin by HutongGames, LLC 2010-2013. All rights reserved.
// (c) Kinect plugin by RF Solutions. All rights reserved.
using HutongGames.PlayMaker;
using UnityEngine;
using System.Collections;
using System;

/*
 * This playmaker script gets the color map as a texture 
 * from the Kinect and stores it in the variable provided
 * by the user.
 */
namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Kinect SDK")]//Adds the script to the current category in playmaker actions, or adds the category if it does not exist
	[Tooltip("Allows you to get the color map from the kinect and save it as a texture that is always updated. NOTE That you must make sure compute color map is enabled under KinectManager Script.")]//The tooltip to appear when hovering over the action
	
	public class GetKinectColorMapTexture : FsmStateAction//Extends FsmStateAction to be a playmaker script
	{
		[RequiredField]//The next variable is required
		[CheckForComponent(typeof(KinectManager))]//Check that the GameObject has the KinectManager component
		[Tooltip("The GameObject to get KinectManager from.")]//Tooltip to display when hovering over the variable
		public FsmOwnerDefault kinectManager;//Holds the GameObject that contains the Kinect manager
		
		[UIHint(UIHint.Variable)]//Display what type of variable the user should pass in
		[Tooltip("Texture to store the color map from the Kinect.")]//Tooltip to display when hovering over the variable
		public FsmTexture storeResult;//texture to store the color map
		
		private KinectManager manager;//Holds the KinectManager from kinectManager passed in by user
		
		//when the script is first run
		public override void OnEnter()
		{			
			manager = kinectManager.GameObject.Value.gameObject.GetComponent<KinectManager>();//Retrieve the KinectManager component
			
			ColorMap();
			
			Finish ();//Got the texture so can finish
		}
		
		/*
		 * This method gets the color map and stores it
		 * in the variable provided by the user
		 */
		private void ColorMap()
		{		
			if(manager != null && KinectManager.IsKinectInitialized())
			{
				storeResult.Value = manager.GetUsersClrTex();//Get the color map and store it
			}
		}
	}//End of class
}//End of namespace