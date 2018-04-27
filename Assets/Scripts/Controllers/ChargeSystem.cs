using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeSystem : MonoBehaviour {

	public int backChargeReached, backChargedWindow;
	public int downChargeReached, downChargedWindow;
	
	private Player player;
	private Opponent opponent;
	
	//will revisit this as public if shit hits the fan	
	private int turnPunchCharge;	
	
	private int backCharging, backChargedWindowInput;
	private bool backCharged, backLetGoWhenCharged;	
	
	private int downCharging, downChargedWindowInput;
	private bool downCharged, downLetGoWhenCharged;

	// Use this for initialization
	void Start () {	
	
		if (gameObject.tag == "Player1"){
			player = GetComponentInParent<Player>();
		}
		else if (gameObject.tag == "Player2"){
			opponent = GetComponentInParent<Opponent>();
		}
		
		turnPunchCharge = 0;
		backCharging = 0;				
		backChargedWindowInput = backChargedWindow; 
		downCharging = 0;				
		downChargedWindowInput = downChargedWindow; 
	}
	
	// Update is called once per frame
	void Update () {
		BackCharging ();		
		DownCharging ();
	}

	void BackCharging (){
		if (player != null){			
			if (player.GetBackPressed) {
				backCharging++;
			}
			else {
				IsBackCharged();
			}
		}
		else if (opponent != null){
			if (opponent.GetBackPressed) {
				backCharging++;
			}
			else {
				IsBackCharged();
			}
		}
		if (backCharging >= backChargeReached) {
			backCharged = true;
		}
		if (backChargedWindow <= 0) {
			backCharged = false;
			ResetBackChargedProperties ();
		}
		if (backLetGoWhenCharged && backCharged) {
			backChargedWindow--;
		}
	}
	
	void DownCharging (){
		if (player != null){
			if (player.GetDownPressed) {
				downCharging++;
			}
			else {
				if (downCharged) {
					downLetGoWhenCharged = true;
				}
				downCharging = 0;
			}
		}
		else if (opponent != null){
			if (opponent.GetDownPressed) {
				downCharging++;
			}
			else {
				if (downCharged) {
					downLetGoWhenCharged = true;
				}
				downCharging = 0;
			}
		}
		if (downCharging >= downChargeReached) {
			downCharged = true;
		}
		if (downChargedWindow <= 0) {
			downCharged = false;
			ResetDownChargedProperties ();
		}
		if (downLetGoWhenCharged && downCharged) {
			downChargedWindow--;
		}
	}

	void IsBackCharged (){
		if (backCharged) {
			backLetGoWhenCharged = true;
		}
		backCharging = 0;
	}
	
	public void ResetBackChargedProperties()
	{
		backChargedWindow = backChargedWindowInput;
		backLetGoWhenCharged = false;
	}
	
	public void ResetDownChargedProperties()
	{
		downChargedWindow = downChargedWindowInput;
		downLetGoWhenCharged = false;
	}
	
	public void ResetBackCharged(){
		backCharging = 0;
	}
	
	public void ResetDownCharged(){
		downCharging = 0;
	}
	
	public void SetBackCharged(bool isBackCharged){
		backCharged = isBackCharged;
	}
	
	public void SetDownCharged(bool isDownCharged){
		downCharged = isDownCharged;
	}	
	
	public void ChargeTurnPunch(){
		turnPunchCharge++;
	}
	
	public void ResetTurnPunch(){
		turnPunchCharge = 0;
	}
		
	public bool GetBackCharged(){
		return backCharged;
	}
	
	public bool GetDownCharged(){
		return downCharged;
	}
	
	public float GetDownChargedWindowInput(){
		return downChargedWindowInput;
	}
	
	public int GetTurnPunchCharge(){
		return turnPunchCharge;
	}
}
