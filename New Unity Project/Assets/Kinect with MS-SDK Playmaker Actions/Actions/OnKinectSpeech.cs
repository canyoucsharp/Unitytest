// (c) Copyright TemperTantrum, http://tempertantrum.com.au/. All rights reserved.
// (c) Copyright F.T.R, http://www.fantasytoreality.com.au/. All rights reserved.
// (c) Playmaker plugin by HutongGames, LLC 2010-2013. All rights reserved.
// (c) Kinect plugin by RF Solutions. All rights reserved.using HutongGames.PlayMaker;using HutongGames.PlayMaker;using HutongGames.PlayMaker;using HutongGames.PlayMaker;
using HutongGames.PlayMaker;
using UnityEngine;
using System.Collections;
using System;

/*
 * This script listens for a phrase and then triggers an event
 * when that phrase is detected.
 */
namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Kinect SDK")]//Adds the script to the current category in playmaker actions, or adds the category if it does not exist
	[Tooltip("Listens for a single word/phrase then triggers a word when it is detected.")]//The tooltip to appear when hovering over the action
	
	public class OnKinectSpeech : FsmStateAction//Extends FsmStateAction to be a playmaker script
	{
		[RequiredField]//The next variable is required
		[CheckForComponent(typeof(SpeechManager))]//Check that the GameObject has the SpeechManager component
		[Tooltip("The GameObject to get SpeechManager from.")]//Tooltip to display when hovering over the variable
		public FsmOwnerDefault speechManager;//Holds the GameObject that contains the Kinect manager

		[Tooltip("The phrase to listen for.")]//Tooltip to display when hovering over the variable
		public FsmString phrase;//Holds the phrase the user wants to listen for

		[UIHint(UIHint.Variable)]//Display what type of variable the user should pass in
		[Tooltip("Event to send when phrase is detected.")]//Tooltip to display when hovering over the variable
		public FsmEvent sendEvent;//Holds the event the user wants to trigger when the phrase is detected
		
		private SpeechManager manager;//Holds the SpeechManager from speechManager passed in by user
		
		//when the script is first run
		public override void OnEnter()
		{			
			manager = speechManager.GameObject.Value.gameObject.GetComponent<SpeechManager>();//Get the speech manager
		}

		public override void OnUpdate()
		{
			if(manager != null && manager.IsSapiInitialized())
			{
				ListenForWord();
			}
		}

		/*
		 * This method listens for the phrase the user wants to detect
		 * and then sends the event if it was found.
		 */
		private void ListenForWord()
		{
			if(manager.IsPhraseRecognized())//If a phrase is recognised
			{
				string sPhraseTag = manager.GetPhraseTagRecognized();//Get what the phrase is

				if(sPhraseTag.Equals(phrase.Value))//If it is what the user was after
				{
					Fsm.Event(sendEvent);//Send the event
					manager.ClearPhraseRecognized();//Phrase was detected so clear it
				}
			}
		}
	}//End of class
}//End of namespace