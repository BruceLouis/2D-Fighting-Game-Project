using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboSystem : MonoBehaviour {
	
	public int hadoukenComboTimer;
	public int shoryukenComboTimer;
	public int hurricaneKickComboTimer;
	public int reverseShoryukenComboTimer;
	public int shunGokuSatsuComboTimer;
	public int motionSuperComboTimer;
	private bool[] hadoukenSequence;
	private bool[] shoryukenSequence;
	private bool[] hurricaneKickSequence;
	private bool[] reverseShoryukenSequence;
	private bool[] shunGokuSatsuSequence;
	private bool[] motionSuperSequence;
	private int hadoukenComboIterator;
	private int shoryukenComboIterator;
	private int hurricaneKickComboIterator;
	private int reverseShoryukenComboIterator;
	private int shunGokuSatsuComboIterator;
	private int motionSuperComboIterator;
	private int hadoukenComboTimerInput;
	private int shoryukenComboTimerInput;
	private int hurricaneKickComboTimerInput;
	private int reverseShoryukenComboTimerInput;
	private int shunGokuSatsuComboTimerInput;
	private int motionSuperComboTimerInput;
	private Character character;

	// Use this for initialization
	void Start () {
		hadoukenComboIterator = 0;		
		shoryukenComboIterator = 0;		
		hurricaneKickComboIterator = 0;	
		reverseShoryukenComboIterator = 0;		
		shunGokuSatsuComboIterator = 0;	
		motionSuperComboIterator = 0;	
		hadoukenSequence = new bool[3];	
		shoryukenSequence = new bool[3];
		hurricaneKickSequence = new bool[3];
		reverseShoryukenSequence = new bool[3];
		shunGokuSatsuSequence = new bool[5];
		motionSuperSequence = new bool[6];
		hadoukenComboTimerInput = hadoukenComboTimer;
		shoryukenComboTimerInput = shoryukenComboTimer;
		hurricaneKickComboTimerInput = hurricaneKickComboTimer;
		reverseShoryukenComboTimerInput = reverseShoryukenComboTimer;
		shunGokuSatsuComboTimerInput = shunGokuSatsuComboTimer;
		motionSuperComboTimerInput = motionSuperComboTimer;
		character = GetComponentInChildren<Character>();
	}
	
	// Update is called once per frame
	void Update () {	
		QuarterCircleStart();
		HadoukenSequence();		
		ShoryukenSequence();	
		HurricaneKickSequence();	
		ReverseShoryukenSequence();	
		ShunGokuSatsuSequence();
		MotionSuperSequence();
	}
	
	void QuarterCircleStart(){
		if (Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow)){
			if (hadoukenComboIterator == 0){			
				ComboSequencer(hadoukenComboIterator, hadoukenSequence);
				hadoukenComboIterator++;			
			}
			if (hurricaneKickComboIterator == 0){			
				ComboSequencer(hurricaneKickComboIterator, hurricaneKickSequence);
				hurricaneKickComboIterator++;
			}			
			if (motionSuperComboIterator == 0){			
				ComboSequencer(motionSuperComboIterator, motionSuperSequence);
				motionSuperComboIterator++;			
			}
		}
	}
	
	void ComboSequencer(int i, bool[] comboSequence){
		try{
			comboSequence[i] = true;	
		}
		catch{
			Debug.LogWarning("array index i: " + i + " out of range");
		}
	}	
		
	void HadoukenSequence(){		
		if (character.side == Character.Side.P1){	
			if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightArrow) && hadoukenComboIterator == 1){
				ComboSequencer(hadoukenComboIterator, hadoukenSequence);
				hadoukenComboIterator++;
			}		
			if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.DownArrow) && hadoukenComboIterator == 2){
				ComboSequencer(hadoukenComboIterator, hadoukenSequence);
			}
		}
		else{	
			if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftArrow) && hadoukenComboIterator == 1){
				ComboSequencer(hadoukenComboIterator, hadoukenSequence);
				hadoukenComboIterator++;
			}		
			if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.DownArrow) && hadoukenComboIterator == 2){
				ComboSequencer(hadoukenComboIterator, hadoukenSequence);
			}
		}					
	
		if (hadoukenSequence[1] == true){
			hadoukenComboTimer--;
		}
		if (hadoukenComboTimer <= 0){
			for (int i=0; i<hadoukenSequence.Length; i++){
				hadoukenSequence[i] = false;
			}
			hadoukenComboIterator = 0;
			hadoukenComboTimer = hadoukenComboTimerInput;
		}			
	}
	
	void ShoryukenSequence(){	
		if (character.side == Character.Side.P1){
			if (Input.GetKey(KeyCode.RightArrow) && shoryukenComboIterator == 0){
				ComboSequencer(shoryukenComboIterator, shoryukenSequence);
				shoryukenComboIterator++;
			}
			if (Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.RightArrow) && shoryukenComboIterator == 1){
				ComboSequencer(shoryukenComboIterator, shoryukenSequence);		
				shoryukenComboIterator++;
			}		
			if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightArrow) && shoryukenComboIterator == 2){
				ComboSequencer(shoryukenComboIterator, shoryukenSequence);		
			}	
		}
		else{
			if (Input.GetKey(KeyCode.LeftArrow) && shoryukenComboIterator == 0){
				ComboSequencer(shoryukenComboIterator, shoryukenSequence);		
				shoryukenComboIterator++;
			}
			if (Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.LeftArrow) && shoryukenComboIterator == 1){
				ComboSequencer(shoryukenComboIterator, shoryukenSequence);		
				shoryukenComboIterator++;
			}		
			if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftArrow) && shoryukenComboIterator == 2){
				ComboSequencer(shoryukenComboIterator, shoryukenSequence);		
			}	
		}			
		if (shoryukenSequence[1] == true){
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
				ComboSequencer(hurricaneKickComboIterator, hurricaneKickSequence);	
				hurricaneKickComboIterator++;
			}		
			if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.DownArrow) && hurricaneKickComboIterator == 2){
				ComboSequencer(hurricaneKickComboIterator, hurricaneKickSequence);	
			}
		}
		else{
			if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftArrow) && hurricaneKickComboIterator == 1){
				ComboSequencer(hurricaneKickComboIterator, hurricaneKickSequence);		
				hurricaneKickComboIterator++;
			}		
			if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.DownArrow) && hurricaneKickComboIterator == 2){
				ComboSequencer(hurricaneKickComboIterator, hurricaneKickSequence);	
			}
		}			
		if (hurricaneKickSequence[1] == true){
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
				ComboSequencer(reverseShoryukenComboIterator, reverseShoryukenSequence);	
				reverseShoryukenComboIterator++;
			}
			if (Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.RightArrow) && reverseShoryukenComboIterator == 1){
				ComboSequencer(reverseShoryukenComboIterator, reverseShoryukenSequence);
				reverseShoryukenComboIterator++;
			}		
			if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightArrow) && reverseShoryukenComboIterator == 2){
				ComboSequencer(reverseShoryukenComboIterator, reverseShoryukenSequence);
			}	
		}
		else{
			if (Input.GetKey(KeyCode.LeftArrow) && reverseShoryukenComboIterator == 0){
				ComboSequencer(reverseShoryukenComboIterator, reverseShoryukenSequence);
				reverseShoryukenComboIterator++;
			}
			if (Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.LeftArrow) && reverseShoryukenComboIterator == 1){
				ComboSequencer(reverseShoryukenComboIterator, reverseShoryukenSequence);
				reverseShoryukenComboIterator++;
			}		
			if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftArrow) && reverseShoryukenComboIterator == 2){
				ComboSequencer(reverseShoryukenComboIterator, reverseShoryukenSequence);
			}	
		}			
		if (reverseShoryukenSequence[1] == true){
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
	
	void ShunGokuSatsuSequence(){	
		if (Input.GetKeyDown(KeyCode.A) && shunGokuSatsuComboIterator == 0){
			ComboSequencer(shunGokuSatsuComboIterator, shunGokuSatsuSequence);
			shunGokuSatsuComboIterator++;
		}
		else if (Input.GetKeyDown(KeyCode.A) && shunGokuSatsuComboIterator == 1){
			ComboSequencer(shunGokuSatsuComboIterator, shunGokuSatsuSequence);
			shunGokuSatsuComboIterator++;
		}		
		if (character.side == Character.Side.P1){
			if (Input.GetKeyDown(KeyCode.RightArrow) && shunGokuSatsuComboIterator == 2){
				ComboSequencer(shunGokuSatsuComboIterator, shunGokuSatsuSequence);
				shunGokuSatsuComboIterator++;
			}
		}	
		else{
			if (Input.GetKeyDown(KeyCode.LeftArrow) && shunGokuSatsuComboIterator == 2){
				ComboSequencer(shunGokuSatsuComboIterator, shunGokuSatsuSequence);
				shunGokuSatsuComboIterator++;
			}	
		}
		if (Input.GetKeyDown(KeyCode.Z) && shunGokuSatsuComboIterator == 3){
			ComboSequencer(shunGokuSatsuComboIterator, shunGokuSatsuSequence);
			shunGokuSatsuComboIterator++;
		}		
		if (Input.GetKeyDown(KeyCode.D) && shunGokuSatsuComboIterator == 4){
			ComboSequencer(shunGokuSatsuComboIterator, shunGokuSatsuSequence);
		}			
		if (shunGokuSatsuSequence[1] == true){
			shunGokuSatsuComboTimer--;
		}
		if (shunGokuSatsuComboTimer <= 0){
			for (int i=0; i<hadoukenSequence.Length; i++){
				shunGokuSatsuSequence[i] = false;
			}
			shunGokuSatsuComboIterator = 0;
			shunGokuSatsuComboTimer = shunGokuSatsuComboTimerInput;
		}	
	}
	
	void MotionSuperSequence(){
		
		if (character.side == Character.Side.P1){	
			if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightArrow) && motionSuperComboIterator == 1){
				ComboSequencer(motionSuperComboIterator, motionSuperSequence);
				motionSuperComboIterator++;
			}		
			if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.DownArrow) && motionSuperComboIterator == 2){
				ComboSequencer(motionSuperComboIterator, motionSuperSequence);
				motionSuperComboIterator++;
			}			
			if (Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.RightArrow) && motionSuperComboIterator == 3){		
				ComboSequencer(motionSuperComboIterator, motionSuperSequence);
				motionSuperComboIterator++;			
			}			
			if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightArrow) && motionSuperComboIterator == 4){
				ComboSequencer(motionSuperComboIterator, motionSuperSequence);
				motionSuperComboIterator++;
			}		
			if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.DownArrow) && motionSuperComboIterator == 5){
				ComboSequencer(motionSuperComboIterator, motionSuperSequence);
			}			
		}
		else{	
			if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftArrow) && motionSuperComboIterator == 1){
				ComboSequencer(motionSuperComboIterator, motionSuperSequence);
				motionSuperComboIterator++;
			}		
			if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.DownArrow) && motionSuperComboIterator == 2){
				ComboSequencer(motionSuperComboIterator, motionSuperSequence);
				motionSuperComboIterator++;
			}
			if (Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.LeftArrow) && motionSuperComboIterator == 3){		
				ComboSequencer(motionSuperComboIterator, motionSuperSequence);
				motionSuperComboIterator++;			
			}	
			if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftArrow) && motionSuperComboIterator == 4){
				ComboSequencer(motionSuperComboIterator, motionSuperSequence);
				motionSuperComboIterator++;
			}		
			if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.DownArrow) && motionSuperComboIterator == 5){
				ComboSequencer(motionSuperComboIterator, motionSuperSequence);
			}				
		}	
		
		if (motionSuperSequence[1] == true){
			motionSuperComboTimer--;
		}
		if (motionSuperComboTimer <= 0){
			for (int i=0; i<motionSuperSequence.Length; i++){
				motionSuperSequence[i] = false;
			}
			motionSuperComboIterator = 0;
			motionSuperComboTimer = motionSuperComboTimerInput;
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
	
	public void ResetShunGokuSatsuSequence(){
		for (int i=0; i<shunGokuSatsuSequence.Length; i++){
			shunGokuSatsuSequence[i] = false;
		}
		shunGokuSatsuComboIterator = 0;
		shunGokuSatsuComboTimer = shunGokuSatsuComboTimerInput;
	}
	
	public void ResetMotionSuperSequence(){
		for (int i=0; i<motionSuperSequence.Length; i++){
			motionSuperSequence[i] = false;
		}
		motionSuperComboIterator = 0;
		motionSuperComboTimer = motionSuperComboTimerInput;
	}
	
	public void ResetAllSequences(){
		ResetHadoukenSequence();
		ResetHurricaneKickSequence();
		ResetReverseShoryukenSequence();
		ResetShoryukenSequence();
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
	
	public bool[] GetShunGokuSatsuSequence(){
		return shunGokuSatsuSequence;
	}
	
	public bool[] GetMotionSuperSequence(){
		return motionSuperSequence;
	}
}
