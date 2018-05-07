using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboCounter : MonoBehaviour {

	public Text comboTextP1;
	public Text comboTextP2;
	public int comboFinishedTimer;
	public int displayTimer;

	private int comboCounterP1;	
	private int comboCounterP2;
	private int comboFinishedP1;
	private int comboFinishedP2;
	private int comboFinishedTimerInput;
	private int displayTimerInput;
	private bool startTimer;
	private bool displayCounter;
		
	void Start(){
		comboCounterP1 = 1;	
		comboCounterP2 = 1;
		comboFinishedTimerInput = comboFinishedTimer;
		displayTimerInput = displayTimer;
		startTimer = false;
		displayCounter = false;
		comboTextP1.text = ""; 
		comboTextP2.text = ""; 
	}
		
	void Update(){
		if (comboCounterP1 > 1){
			comboTextP1.text = comboCounterP1.ToString() + "Hits";
//			comboFinishedP1 = comboCounterP1;
		}
		if (comboCounterP2 > 1){
			comboTextP2.text = comboCounterP2.ToString() + "Hits";
//			comboFinishedP2 = comboCounterP2;
		}
		if (startTimer){
			comboFinishedTimer--;
		}
		if (comboFinishedTimer <= 0){
//			if (comboFinishedP1 > 1){
//				comboTextP1.text = comboFinishedP1.ToString() + "Hits" + "  GOOD!";
//			}
//			if (comboFinishedP2 > 1){
//				comboTextP2.text = comboFinishedP2.ToString() + "Hits" + "  GOOD!";
//			}
			comboTextP1.text = "";
			comboTextP2.text = "";
			startTimer = false;
			comboFinishedTimer = comboFinishedTimerInput;
//			displayCounter = true;
		}
		
//		if (displayCounter){
//			displayTimer--;
//		}		
//		if (displayTimer <= 0){
//			comboTextP1.text = "";
//			comboTextP2.text = "";
//			comboFinishedP1 = 0;
//			comboFinishedP2 = 0;
//			displayCounter = false;
//			displayTimer = displayTimerInput;
//		}
	}
	
	public int GetComboCountP1{ 
		get { return comboCounterP1; }
		set { comboCounterP1 = value; }	
	}		
	
	public int GetComboCountP2{ 
		get { return comboCounterP2; }
		set { comboCounterP2 = value; }	
	}		
	
	public bool GetStartTimer{ 
		get { return startTimer; }
		set { startTimer = value; }	
	}		
	
	public void ResetComboFinishedTimer(){
		comboFinishedTimer = comboFinishedTimerInput;
	}
}
