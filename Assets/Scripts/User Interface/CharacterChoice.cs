using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterChoice : MonoBehaviour {

	private int charChoice;
    private bool mainMenuClicked = false;
	
	void Awake()
    {
		GameObject.DontDestroyOnLoad(gameObject);
	}

    void Update()
    {
        if (mainMenuClicked)
        {
            Destroy(gameObject);
        }            
    }

    public void ChooseCharacter(int chooseChar){
		charChoice = chooseChar;
	}
	
	public int GetChosenChar(){
		return charChoice;
	}	

    public bool GetMainMenuClicked {
        get { return mainMenuClicked; }
        set { mainMenuClicked = value; }
    }
}
