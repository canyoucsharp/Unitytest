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
 * This playmaker script gets the current frame of the Kinect feed and saves it as 
 * a texture. The texture is then saved as a file if the user wants.
 * 
 * TODO: 
 * 1) Decide whether to save here or not. Currently does have the option to save 
 * 	  although it needs to be modified to allow for entering save location.
 * 
 * 2) Could take out the saving and create a new  FSM that saves a texture. 
 * 
 * 3) Perform overwrite check if saving is done here. Add a (1) or something if it
 * 	  does exist, or have the option to overwrite regardless
 */
namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Kinect SDK")]//Adds the script to the current category in playmaker actions, or adds the category if it does not exist
	[Tooltip("Allows you to get the colour map from the kinect and save it as a texture. Can be used for a snapshot effect. NOTE That you must make sure compute colour map is enabled under KinectManager Script.")]//The tooltip to appear when hovering over the action
	
	public class KinectSnapShot : FsmStateAction//Extends FsmStateAction to be a playmaker script
	{
		[CheckForComponent(typeof(KinectManager))]//Check that the GameObject has the KinectManager component
		[Tooltip("The GameObject to get KinectManager from.")]//Tooltip to display when hovering over the variable
		public FsmOwnerDefault kinectManager;//Holds the GameObject that contains the Kinect manager
		
		[UIHint(UIHint.Variable)]//Display what type of variable the user should pass in
		[Tooltip("Stores the colour map from the Kinect.")]//Tooltip to display when hovering over the variable
		public FsmTexture storeResult;//Texture that will store the colour map from Kinect
		
		[UIHint(UIHint.Variable)]//Display what type of variable the user should pass in
		[Tooltip("Stores the colour map from the Kinect.")]//Tooltip to display when hovering over the variable
		public FsmString storeNameOfImage;//Name the image should be saved as
		
		[Tooltip("Whether to save the texture to a file or not.")]//Tooltip to display when hovering over the variable
		public bool saveImage = false;//Holds the value entered by the user
		
		private KinectManager manager;//Holds the KinectManager from kinectManager passed in by user
		
		//when the script is first run
		public override void OnEnter()
		{			
			manager = kinectManager.GameObject.Value.gameObject.GetComponent<KinectManager>();//Retrieve the KinectManager component
			
			colourMap();
		}
		
		/*
		 * This method gets the colour map from the Kinect, converts it to a material,
		 * then if the player wishes to save the texture to a file that is done so.
		 * The method finally ends by calling the Finish event, as there is no option
		 * to continually update.
		 */
		private void colourMap()
		{	
			if(manager != null && KinectManager.IsKinectInitialized())
			{
				Texture2D tex2d = manager.GetUsersClrTex();//Get the colour map
				Texture2D newTexture = new Texture2D(tex2d.width, tex2d.height, TextureFormat.ARGB32, false);//Create a new texture that will hold the pixels
				
				newTexture.SetPixels(0,0, tex2d.width, tex2d.height, tex2d.GetPixels());//'Get the current frame' and set it to newTexture
				newTexture.Apply();//Apply the SetPixels change
				
				storeResult.Value = newTexture;//Store the result as a new Texture
				
				if(saveImage)//If the user wants to save the image
					SaveTextureToFile(newTexture, storeNameOfImage.Value);//Save it
			}
			
			Finish ();//Send the Finish event
		}
		
		/*
		 * This method saves the texture passed in at a set directory,
		 * under the file name that was passed in.
		 * 
		 * TODO: This method needs to either be modified or deleted. See
		 * todo in the header above namespace.
		 * 
		 * texture is the texture that should be saved.
		 * fileName is the name of the file that should be saved.
		 */
		private void SaveTextureToFile( Texture2D texture, String fileName)
		{
			byte[] bytes = texture.EncodeToPNG();//Convert the texture to bytes.
			File.WriteAllBytes(Application.dataPath + "/../testscreen-" + fileName + ".png", bytes);//Write the file
			//Tell unity to delete the texture, by default it seems to keep hold of it and memory crashes will occur after too many screenshots.
		}
	}//End of class
}//End of namespace