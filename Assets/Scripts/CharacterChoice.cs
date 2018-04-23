using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterChoice : MonoBehaviour {

	private int charChoice; 
	
	void Awake(){
		GameObject.DontDestroyOnLoad(gameObject);
	}
	
	public void ChooseCharacter(int chooseChar){
		charChoice = chooseChar;
	}
	
	public int GetChosenChar(){
		return charChoice;
	}	
}
