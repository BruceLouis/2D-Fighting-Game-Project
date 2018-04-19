using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class TimeControl : MonoBehaviour {

	public static bool slowDown, roundOver, victoryPose;
	public static int slowDownTimer;	
	public static string winner;
	public bool gameOn;
	public Text gameTimerText, countDownText, winnerText;
	public AudioClip threeSound, twoSound, oneSound, fightSound, youLoseSound, youWinSound;
	
	private int gameTimer, internalTimer, countDownTimer, restartTimer, KOslowTimer, playedOnce;
	private AudioSource audioSource;
	private LevelManager levelManager;
	private Opponent opponent;
	private Player player;
	private Character playerChar;
	private Character opponentChar;
	// Use this for initialization
	void Awake () {
		opponent = FindObjectOfType<Opponent>();
		player = FindObjectOfType<Player>();
		playerChar = player.GetComponentInChildren<Character>();
		opponentChar = opponent.GetComponentInChildren<Character>();
		levelManager = FindObjectOfType<LevelManager>();
		audioSource = GetComponent<AudioSource>();
		gameTimer = 99;
		internalTimer = 50;
		restartTimer = 100;
//		countDownTimer = 0;
		countDownTimer = 3;
		playedOnce = 1;
		gameTimerText.text = gameTimer.ToString();
		countDownText.text = countDownTimer.ToString();
		winnerText.text = "";
		slowDown = false;		
		gameOn = false;
		roundOver = false;
		victoryPose = false;
	}
	
	// Update is called once per frame
	void Update () {
		internalTimer--;
		if (internalTimer <= 0){
			if (!gameOn && !victoryPose){					
				countDownTimer--;
				internalTimer = 50;
				playedOnce = 1;
			}
			else{
				gameTimer--;
				internalTimer = 75;
			}
		}
		
		gameTimerText.text = gameTimer.ToString();
		countDownText.text = countDownTimer.ToString();
		
		if (countDownTimer == 3){ 
			if (!audioSource.isPlaying && playedOnce == 1){
				audioSource.PlayOneShot(threeSound, 0.7f);
				playedOnce = 0;
			}
		}
		else if (countDownTimer == 2){
			if (!audioSource.isPlaying && playedOnce == 1){
				audioSource.PlayOneShot(twoSound, 0.7f);
				playedOnce = 0;
			}
		}
		else if (countDownTimer == 1){
			if (!audioSource.isPlaying && playedOnce == 1){
				audioSource.PlayOneShot(oneSound, 0.7f);
				playedOnce = 0;
			}
		}
		else if (countDownTimer == 0){
			if (!audioSource.isPlaying && playedOnce == 1){
				audioSource.PlayOneShot(fightSound, 0.7f);
				playedOnce = 0;
			}
			countDownText.text = "FIGHT";
		}
		else{	
			gameOn = true;
			countDownText.text = "";
		}
		
		if (opponentChar.GetKOed() == true || playerChar.GetKOed() == true){
			gameOn = false;
		}
				
		if (victoryPose){
			winnerText.text = winner;
			if (winner == "You Win"){		
				if (!audioSource.isPlaying && playedOnce == 1){			
					audioSource.PlayOneShot(youWinSound, 1f);
					playedOnce = 0;
				}
			}
			else{		
				if (!audioSource.isPlaying && playedOnce == 1){
					audioSource.PlayOneShot(youLoseSound, 1f);
					playedOnce = 0;
				}
			}
			Debug.Log ("playedOnce " + playedOnce);
			restartTimer--;
		}
		
		if (restartTimer <= 0){
			levelManager.LoadLevel(SceneManager.GetActiveScene ().name);
		}
						
		if (slowDown){
			Time.timeScale = 0.5f;
			slowDownTimer--;
		}
		if (slowDownTimer <= 0){
			slowDown = false;
			Time.timeScale = 1f;
		}
	}
}
