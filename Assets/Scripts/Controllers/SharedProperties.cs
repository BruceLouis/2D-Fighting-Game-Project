using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedProperties : MonoBehaviour {
	
	private Player player;
	private Opponent opponent;
	private Animator animator;
	private Character character;
	private TimeControl timeControl;
	private bool isKOSoundPlayed;
	
	void Start(){
		timeControl = FindObjectOfType<TimeControl>();
		
		if (GetComponent<Player>() != null){
			player = GetComponent<Player>();
		}
		else if (GetComponent<Opponent>() != null){
			opponent = GetComponent<Opponent>();
		}
		animator = GetComponentInChildren<Animator>();
		character = GetComponentInChildren<Character>();
		isKOSoundPlayed = false;
	}
	
	public void KOSequence (string winnerString){
		if (timeControl.gameState == TimeControl.GameState.KOHappened){
			if (animator.GetBool ("isKOed") == true && !isKOSoundPlayed) {				
				character.KOSound ();
				isKOSoundPlayed = true;
			}
		}
		if (timeControl.gameState == TimeControl.GameState.victoryPose && animator.GetBool ("isKOed") == false) {
			TimeControl.winner = winnerString;
			animator.Play ("VictoryPose");
		}
	}
	

	public void CharacterNeutralState(){
		if (player != null){
			player.GetForwardPressed = false;
			player.GetBackPressed = false;
		}
		else if (opponent != null){
			opponent.GetForwardPressed = false;
			opponent.GetBackPressed = false;
		}
	}
	
	public void IsThrown(Animator thrownAnim, Character throwingCharacter, Character thrownCharacter){
		if (thrownAnim.GetBool("isThrown") == true){
			if (throwingCharacter.GetComponent<Ken>() != null){
				if (thrownCharacter.side == Character.Side.P2){
					thrownCharacter.transform.position = new Vector3(throwingCharacter.transform.position.x + 0.25f, throwingCharacter.transform.position.y, 0f);
				}
				else{
					thrownCharacter.transform.position = new Vector3(throwingCharacter.transform.position.x - 0.25f, throwingCharacter.transform.position.y, 0f);
				}		
			}
			else if (throwingCharacter.GetComponent<FeiLong>() != null){
				if (thrownCharacter.side == Character.Side.P2){
					thrownCharacter.transform.position = new Vector3(throwingCharacter.transform.position.x + 0.5f, throwingCharacter.transform.position.y, 0f);
				}
				else{
					thrownCharacter.transform.position = new Vector3(throwingCharacter.transform.position.x - 0.5f, throwingCharacter.transform.position.y, 0f);
				}		
			}		
			else if (throwingCharacter.GetComponent<Balrog>() != null){
				if (thrownCharacter.side == Character.Side.P2){
					thrownCharacter.transform.position = new Vector3(throwingCharacter.transform.position.x + 0.3f, throwingCharacter.transform.position.y, 0f);
				}
				else{
					thrownCharacter.transform.position = new Vector3(throwingCharacter.transform.position.x - 0.3f, throwingCharacter.transform.position.y, 0f);
				}		
			}		
		}
	}	
	
	public float GetDistanceFromOtherFighter(){
		if (opponent != null){
			return Mathf.Abs(opponent.GetDistance());
		}
		else {
			return Mathf.Abs(player.GetDistance());
		}
	}		
}
