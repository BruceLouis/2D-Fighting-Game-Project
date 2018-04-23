using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboSystem : MonoBehaviour {
	
	public int hadoukenComboTimer;
	public int shoryukenComboTimer;
	public int hurricaneKickComboTimer;
	public int reverseShoryukenComboTimer;
	private bool[] hadoukenSequence;
	private bool[] shoryukenSequence;
	private bool[] hurricaneKickSequence;
	private bool[] reverseShoryukenSequence;
	private Character character;
	private int hadoukenComboIterator;
	private int shoryukenComboIterator;
	private int hurricaneKickComboIterator;
	private int reverseShoryukenComboIterator;
	private int hadoukenComboTimerInput;
	private int shoryukenComboTimerInput;
	private int hurricaneKickComboTimerInput;
	private int reverseShoryukenComboTimerInput;

	// Use this for initialization
	void Start () {
		hadoukenComboIterator = 0;		
		shoryukenComboIterator = 0;		
		hurricaneKickComboIterator = 0;	
		reverseShoryukenComboIterator = 0;			
		hadoukenSequence = new bool[3];	
		shoryukenSequence = new bool[3];
		hurricaneKickSequence = new bool[3];
		reverseShoryukenSequence = new bool[3];
		hadoukenComboTimerInput = hadoukenComboTimer;
		shoryukenComboTimerInput = shoryukenComboTimer;
		hurricaneKickComboTimerInput = hurricaneKickComboTimer;
		reverseShoryukenComboTimerInput = reverseShoryukenComboTimer;
		character = GetComponentInChildren<Character>();
	}
	
	// Update is called once per frame
	void Update () {	
		QuarterCircleStart();
		HadoukenSequence();		
		ShoryukenSequence();	
		HurricaneKickSequence();	
		ReverseShoryukenSequence();	
	}
	
	void QuarterCircleStart(){
		if (Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow)  && hadoukenComboIterator == 0){			
			ComboSequencer(hadoukenComboIterator, hadoukenComboTimer, hadoukenComboTimerInput, hadoukenSequence);
			hadoukenComboIterator++;			
			ComboSequencer(hurricaneKickComboIterator, hurricaneKickComboTimer, hurricaneKickComboTimerInput, hurricaneKickSequence);
			hurricaneKickComboIterator++;
		}
	}
	
	void ComboSequencer(int i, int comboTimer, int comboTimerInput, bool[] comboSequence){
		try{
			comboSequence[i] = true;	
		}
		catch{
			Debug.LogWarning("array index i: " + i + " out of range");
		}
		comboTimer = comboTimerInput;	
	}	
		
	void HadoukenSequence(){		
		if (character.side == Character.Side.P1){	
			if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightArrow) && hadoukenComboIterator == 1){
				ComboSequencer(hadoukenComboIterator, hadoukenComboTimer, hadoukenComboTimerInput, hadoukenSequence);
				hadoukenComboIterator++;
			}		
			if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.DownArrow) && hadoukenComboIterator == 2){
				ComboSequencer(hadoukenComboIterator, hadoukenComboTimer, hadoukenComboTimerInput, hadoukenSequence);
			}
		}
		else{	
			if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftArrow) && hadoukenComboIterator == 1){
				ComboSequencer(hadoukenComboIterator, hadoukenComboTimer, hadoukenComboTimerInput, hadoukenSequence);
				hadoukenComboIterator++;
			}		
			if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.DownArrow) && hadoukenComboIterator == 2){
				ComboSequencer(hadoukenComboIterator, hadoukenComboTimer, hadoukenComboTimerInput, hadoukenSequence);
			}
		}					
	
		if (hadoukenSequence[0] == true){
			hadoukenComboTimer--;
		}
		if (hadoukenComboTimer <= 0){
			for (int i=0; i<hadoukenSequence.Length; i++){
				hadoukenSequence[i] = false;
			}
			hadoukenComboIterator = 0;
			hadoukenComboTimer = hadoukenComboTimerInput;
		}	
	
//		for (int i=0; i<hadoukenSequence.Length; i++){
//			Debug.Log ("hadoukenSequence " + i + " " + hadoukenSequence[i]);
//		}
//		Debug.Log ("hadoukenSequence [0]" + hadoukenSequence[0]);
//		Debug.Log ("hadoukenComboIterator " + hadoukenComboIterator);	
	}
	
	void ShoryukenSequence(){	
		if (character.side == Character.Side.P1){
			if (Input.GetKey(KeyCode.RightArrow) && shoryukenComboIterator == 0){
				ComboSequencer(shoryukenComboIterator, shoryukenComboTimer, shoryukenComboTimerInput, shoryukenSequence);
				shoryukenComboIterator++;
			}
			if (Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.RightArrow) && shoryukenComboIterator == 1){
				ComboSequencer(shoryukenComboIterator, shoryukenComboTimer, shoryukenComboTimerInput, shoryukenSequence);
				shoryukenComboIterator++;
			}		
			if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightArrow) && shoryukenComboIterator == 2){
				ComboSequencer(shoryukenComboIterator, shoryukenComboTimer, shoryukenComboTimerInput, shoryukenSequence);
			}	
		}
		else{
			if (Input.GetKey(KeyCode.LeftArrow) && shoryukenComboIterator == 0){
				ComboSequencer(shoryukenComboIterator, shoryukenComboTimer, shoryukenComboTimerInput, shoryukenSequence);
				shoryukenComboIterator++;
			}
			if (Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.LeftArrow) && shoryukenComboIterator == 1){
				ComboSequencer(shoryukenComboIterator, shoryukenComboTimer, shoryukenComboTimerInput, shoryukenSequence);
				shoryukenComboIterator++;
			}		
			if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftArrow) && shoryukenComboIterator == 2){
				ComboSequencer(shoryukenComboIterator, shoryukenComboTimer, shoryukenComboTimerInput, shoryukenSequence);
			}	
		}			
		if (shoryukenSequence[0] == true){
			shoryukenComboTimer--;
		}
		if (shoryukenComboTimer <= 0){
			for (int i=0; i<hadoukenSequence.Length; i++){
				shoryukenSequence[i] = false;
			}
			shoryukenComboIterator = 0;
			shoryukenComboTimer = shoryukenComboTimerInput;
		}	
	}
		
	void HurricaneKickSequence(){	
		if (character.side == Character.Side.P2){
			if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightArrow) && hurricaneKickComboIterator == 1){
				ComboSequencer(hurricaneKickComboIterator, hurricaneKickComboTimer, hurricaneKickComboTimerInput, hurricaneKickSequence);
				hurricaneKickComboIterator++;
			}		
			if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.DownArrow) && hurricaneKickComboIterator == 2){
				ComboSequencer(hurricaneKickComboIterator, hurricaneKickComboTimer, hurricaneKickComboTimerInput, hurricaneKickSequence);
			}
		}
		else{
			if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftArrow) && hurricaneKickComboIterator == 1){
				ComboSequencer(hurricaneKickComboIterator, hurricaneKickComboTimer, hurricaneKickComboTimerInput, hurricaneKickSequence);
				hurricaneKickComboIterator++;
			}		
			if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.DownArrow) && hurricaneKickComboIterator == 2){
				ComboSequencer(hurricaneKickComboIterator, hurricaneKickComboTimer, hurricaneKickComboTimerInput, hurricaneKickSequence);
			}
		}			
		if (hurricaneKickSequence[0] == true){
			hurricaneKickComboTimer--;
		}
		if (hurricaneKickComboTimer <= 0){
			for (int i=0; i<hurricaneKickSequence.Length; i++){
				hurricaneKickSequence[i] = false;
			}
			hurricaneKickComboIterator = 0;
			hurricaneKickComboTimer = hurricaneKickComboTimerInput;
		}	
	}
	
	void ReverseShoryukenSequence(){	
		if (character.side == Character.Side.P2){
			if (Input.GetKey(KeyCode.RightArrow) && reverseShoryukenComboIterator == 0){
				ComboSequencer(reverseShoryukenComboIterator, reverseShoryukenComboTimer, reverseShoryukenComboTimerInput, reverseShoryukenSequence);
				reverseShoryukenComboIterator++;
			}
			if (Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.RightArrow) && reverseShoryukenComboIterator == 1){
				ComboSequencer(reverseShoryukenComboIterator, reverseShoryukenComboTimer, reverseShoryukenComboTimerInput, reverseShoryukenSequence);
				reverseShoryukenComboIterator++;
			}		
			if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightArrow) && reverseShoryukenComboIterator == 2){
				ComboSequencer(reverseShoryukenComboIterator, reverseShoryukenComboTimer, reverseShoryukenComboTimerInput, reverseShoryukenSequence);
			}	
		}
		else{
			if (Input.GetKey(KeyCode.LeftArrow) && reverseShoryukenComboIterator == 0){
				ComboSequencer(reverseShoryukenComboIterator, reverseShoryukenComboTimer, reverseShoryukenComboTimerInput, reverseShoryukenSequence);
				reverseShoryukenComboIterator++;
			}
			if (Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.LeftArrow) && reverseShoryukenComboIterator == 1){
				ComboSequencer(reverseShoryukenComboIterator, reverseShoryukenComboTimer, reverseShoryukenComboTimerInput, reverseShoryukenSequence);
				reverseShoryukenComboIterator++;
			}		
			if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftArrow) && reverseShoryukenComboIterator == 2){
				ComboSequencer(reverseShoryukenComboIterator, reverseShoryukenComboTimer, reverseShoryukenComboTimerInput, reverseShoryukenSequence);
			}	
		}			
		if (reverseShoryukenSequence[0] == true){
			reverseShoryukenComboTimer--;
		}
		if (reverseShoryukenComboTimer <= 0){
			for (int i=0; i<hadoukenSequence.Length; i++){
				reverseShoryukenSequence[i] = false;
			}
			reverseShoryukenComboIterator = 0;
			reverseShoryukenComboTimer = reverseShoryukenComboTimerInput;
		}	
	}
	
	public void ResetHadoukenSequence(){
		for (int i=0; i<hadoukenSequence.Length; i++){
			hadoukenSequence[i] = false;
		}
		hadoukenComboIterator = 0;
		hadoukenComboTimer = hadoukenComboTimerInput;
	}
	
	public void ResetShoryukenSequence(){
		for (int i=0; i<shoryukenSequence.Length; i++){
			shoryukenSequence[i] = false;
		}
		shoryukenComboIterator = 0;
		shoryukenComboTimer = shoryukenComboTimerInput;
	}
	
	public void ResetHurricaneKickSequence(){
		for (int i=0; i<hurricaneKickSequence.Length; i++){
			hurricaneKickSequence[i] = false;
		}
		hurricaneKickComboIterator = 0;
		hurricaneKickComboTimer = hurricaneKickComboTimerInput;
	}
	
	public void ResetReverseShoryukenSequence(){
		for (int i=0; i<reverseShoryukenSequence.Length; i++){
			reverseShoryukenSequence[i] = false;
		}
		reverseShoryukenComboIterator = 0;
		reverseShoryukenComboTimer = reverseShoryukenComboTimerInput;
	}
	
	public bool[] GetHadoukenSequence(){
		return hadoukenSequence;
	}
	
	public bool[] GetShoryukenSequence(){
		return shoryukenSequence;
	}
	
	public bool[] GetHurricaneKickSequence(){
		return hurricaneKickSequence;
	}
	
	public bool[] GetReverseShoryukenSequence(){
		return reverseShoryukenSequence;
	}
}
