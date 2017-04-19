// (c) Copyright TemperTantrum, http://tempertantrum.com.au/. All rights reserved.
// (c) Copyright F.T.R, http://www.fantasytoreality.com.au/. All rights reserved.
// (c) Playmaker plugin by HutongGames, LLC 2010-2013. All rights reserved.
// (c) Kinect plugin by RF Solutions. All rights reserved.using HutongGames.PlayMaker;using HutongGames.PlayMaker;using HutongGames.PlayMaker;using HutongGames.PlayMaker;
using HutongGames.PlayMaker;
using UnityEngine;
using System.Collections;
using System;
using System.IO;

/*
 * This playmaker script takes the Kinect feed and scales it to fit the screen
 * then sets the feed to a GUITexture found on the object the user
 * chooses. If the user chooses to stretch the image it is stretched
 * across the whole screen, otherwise it is scaled as large as possible
 * to fit on the screen at the correct aspect ratio.
 */
namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Kinect SDK")]//Adds the script to the current category in playmaker actions, or adds the category if it does not exist
	[Tooltip("Feed from Kinect is set as background. NOTE That you must make sure compute colour map is enabled under KinectManager Script.")]//The tooltip to appear when hovering over the action
	
	public class KinectFeedBackground : FsmStateAction//Extends FsmStateAction to be a playmaker script
	{
		[RequiredField]//The next variable is required
		[CheckForComponent(typeof(KinectManager))]//Check that the GameObject has the KinectManager component
		[Tooltip("The GameObject to get KinectManager from.")]//Tooltip to display when hovering over the variable
		public FsmOwnerDefault kinectManager;//Holds the GameObject that contains the Kinect manager
		
		[RequiredField]//The next variable is required
		[CheckForComponent(typeof(GUITexture))]//Check the that the GameObject has the GUITexture component
		[Tooltip("GameObject with GUITexture to modify.")]//Tooltip to display when hovering over the variable
		public FsmGameObject objectWithGUITexture;//Holds the object that contains the GUITexture
		
		[Tooltip("Whether to get the feed every frame, or just once.")]//Tooltip to display when hovering over the variable
		public FsmBool everyFrame = true;//Holds the variable from user
		
		[Tooltip("Stretch to fit the screen, or use the normal aspect ratio (1.33).")]//Tooltip to display when hovering over the variable
		public FsmBool stretchToFit = true;//Holds the variable from user
		
		[Tooltip("Flip the feed horizontally or not.")]//Tooltip to display when hovering over the variable
		public FsmBool flipHorizontally = true;//Holds the variable from user
		
		private KinectManager manager;//Holds the KinectManager from kinectManager passed in by user
		private GUITexture guiTexture;//Used to manipulate the feed as a texture
		private const float aspectRatio = 1.333f;//The aspect ratio to use, 1.333 is the Kinects aspect ratio
		
		private bool eachFrame;//Holds the value passed in by user from everyFrame
		private bool fitScreen;//Holds the value passed in by user from stretchToFit
		private bool flip;//Holds the value passed in by user from flipHorizontally
		
		
		/*
		 * This method is called when the script is first run. It gets the GUITexture and then sets 
		 * up the GUITexture's pixelInset depending on whether they want the image flipped or not,
		 * and whether the user wants the image stretched. It then enters the RetrieveKinectFeed method
		 * to get the feed. If the player only wanted it once the script sends the Finish event.
		 * 
		 * Origin for texture is in center of GUITexture, hence the rect(x, y, width, height) math.
		 */
		public override void OnEnter()
		{			
			manager = kinectManager.GameObject.Value.gameObject.GetComponent<KinectManager>();//Retrieve the KinectManager component
			guiTexture = objectWithGUITexture.Value.GetComponent<GUITexture>();//Retrieve the GUITexture component
			
			fitScreen = stretchToFit.Value;//Convert from FSM to Unity variables
			eachFrame = everyFrame.Value;//Convert from FSM to Unity variables
			flip = flipHorizontally.Value;//Convert from FSM to Unity variables
			
			if(flip)//If the user wants the image flipped
			{
				if(fitScreen)//If the user wants the image to be stretched
					guiTexture.pixelInset = new Rect(Screen.width / 2, -Screen.height / 2 - 1, -Screen.width, Screen.height);//Entire screen, flipped
				else//If the user wants to use normal aspect ratio
					guiTexture.pixelInset = new Rect((Screen.height * aspectRatio) / 2, -Screen.height / 2 - 1, -Screen.height * aspectRatio, Screen.height);//Aspect ratio, centered, flipped
			}
			else//If the user wants the image not flipped
			{
				if(fitScreen)//If the user wants the image to be stretched
					guiTexture.pixelInset = new Rect(-Screen.width / 2, -Screen.height / 2 - 1, Screen.width, Screen.height);//Entire screen, not flipped
				else//If the user wants to use normal aspect ratio
					guiTexture.pixelInset = new Rect(-(Screen.height * aspectRatio) / 2, -Screen.height / 2 - 1, Screen.height * aspectRatio, Screen.height);//Aspect ratio, centered, not flipped
			}
			
			RetrieveKinectFeed();//Get the Kinect feed
			
			if(!eachFrame)
				Finish();//Send the finish event
		}
		
		public override void OnUpdate()
		{
			RetrieveKinectFeed();
		}
		
		/*
		 * If the Kinect is set up and the manager is not null the
		 * image feed is retrieved and set as the texture of the GUITexture
		 * component attached to the GameObject the user passed in.
		 */
		private void RetrieveKinectFeed()
		{
			if(manager != null && KinectManager.IsKinectInitialized())
			{
				Texture2D tex2d = manager.GetUsersClrTex();//Get the colour map from the Kinect
				
				if(!eachFrame)//If the user only wants one frame create a texture with the current frame
				{
					Texture2D newTexture = new Texture2D(tex2d.width, tex2d.height, TextureFormat.ARGB32, false);//Create a new texture that will hold the pixels
					
					newTexture.SetPixels(0,0, tex2d.width, tex2d.height, tex2d.GetPixels());//'Get the current frame' and set it to newTexture
					newTexture.Apply();//Apply the SetPixels change
					
					guiTexture.texture = newTexture;//Store the result as a new Texture
				}
				else
					guiTexture.texture = tex2d;//Store the result as the colour map
			}
		}
	}//End of class
}//End of namespace