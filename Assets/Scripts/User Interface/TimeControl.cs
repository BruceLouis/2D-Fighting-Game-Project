using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class TimeControl : MonoBehaviour {

	public static bool slowDown;
	public static int slowDownTimer;	
	public static string winner;
	
	public enum GameState {introPose, countDown, fight, KOHappened, victoryPose};
	public GameState gameState;
	
	public Text gameTimerText, countDownText, winnerText;
	public AudioClip threeSound, twoSound, oneSound, fightSound, youLoseSound, youWinSound;
	
	private int gameTimer, internalTimer, countDownTimer, restartTimer, KOslowTimer, KOedTimer, playedOnce, introTimer;
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
		levelManager = FindObjectOfType<LevelManager>();
		audioSource = GetComponent<AudioSource>();		
		
		gameTimer = 99;
		internalTimer = 50;
		restartTimer = 100;
		KOedTimer = 150;
		introTimer = 100;
		countDownTimer = 3;
		playedOnce = 1;
		
		gameTimerText.text = gameTimer.ToString();
		countDownText.text = "";
		winner = "";
		winnerText.text = "";
		
	}
	
	void Start () {			
		playerChar = player.GetComponentInChildren<Character>();
		opponentChar = opponent.GetComponentInChildren<Character>();		
//		gameState = GameState.introPose;	
		gameState = GameState.fight;	
		slowDown = false;		
	}
	
	// Update is called once per frame
	void Update () {
	
		if (gameState != GameState.introPose){
			internalTimer--;		
		}		
		if (internalTimer <= 0){
			switch(gameState){
				case GameState.countDown:				
					countDownTimer--;
					internalTimer = 50;
					playedOnce = 1;
					break;
				case GameState.fight:
					gameTimer--;
					internalTimer = 75;
					break;
			}
		}
		
		gameTimerText.text = gameTimer.ToString();
		
		IntroPoseState();
		
		if (gameState == GameState.countDown){
			CountDownCommence(countDownTimer);
		}
		
		if (opponentChar.GetKOed() == true || playerChar.GetKOed() == true){
			gameState = GameState.KOHappened;
			KOedTimer--;
		}
				
		if (KOedTimer <= 0){
			gameState = GameState.victoryPose;
		}
		if (gameState == GameState.victoryPose){
			VictoryPoseTime();
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

	void IntroPoseState (){
		if (gameState == GameState.introPose) {
			introTimer--;
		}
		if (introTimer <= 0) {
			gameState = GameState.countDown;
		}
	}

	void VictoryPoseTime(){
		winnerText.text = winner;
		if (winner == "You Win") {
			if (!audioSource.isPlaying && playedOnce == 1) {
				audioSource.PlayOneShot (youWinSound, 1f);
				playedOnce = 0;
			}
		}
		else if (winner == "You Lose") {
			if (!audioSource.isPlaying && playedOnce == 1) {
				audioSource.PlayOneShot (youLoseSound, 1f);
				playedOnce = 0;
			}
		}
		Debug.Log ("playedOnce " + playedOnce);
		restartTimer--;
	}

	void CountDownCommence(int countDown){
		switch(countDown){
			case 3:
				if (!audioSource.isPlaying && playedOnce == 1) {
					audioSource.PlayOneShot (threeSound, 0.7f);
					playedOnce = 0;
				}
				countDownText.text = countDownTimer.ToString();
				break;
				
			case 2: 
				if (!audioSource.isPlaying && playedOnce == 1) {
					audioSource.PlayOneShot (twoSound, 0.7f);
					playedOnce = 0;
				}
				countDownText.text = countDownTimer.ToString();
				break;
				
			case 1:
				if (!audioSource.isPlaying && playedOnce == 1) {
					audioSource.PlayOneShot (oneSound, 0.7f);
					playedOnce = 0;
				}
				countDownText.text = countDownTimer.ToString();
				break;
			
			case 0:
				if (!audioSource.isPlaying && playedOnce == 1) {
					audioSource.PlayOneShot (fightSound, 0.7f);
					playedOnce = 0;
				}
				countDownText.text = "FIGHT";
				break;
			
			default:
				gameState = GameState.fight;
				countDownText.text = "";	
				break;	
		}	
	}
}
