using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class TimeControl : MonoBehaviour {

	public static bool slowDown, gettingDemoned, demonKO, superKO;
	public static bool[] inSuperStartup;
	public static float slowDownTimer;	
	public static string winner;
		
	public enum GameState {introPose, countDown, fight, KOHappened, victoryPose};
	public GameState gameState;
	
	public GameObject superPanel, shunGokuSatsuPanel, shunGokuSatsuKOPanel, superKOPanel;
	
	public Text gameTimerText, countDownText, winnerText;
	public AudioClip threeSound, twoSound, oneSound, fightSound, youLoseSound, youWinSound;
	public AudioClip[] winQuotes;
	
	private int gameTimer, internalTimer, countDownTimer, KOslowTimer, KOedTimer, playedOnce;
	private float introTimer, restartTimer;
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
		restartTimer = 2f;
		KOedTimer = 150;
		introTimer = 1.75f;
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
		gameState = GameState.introPose;	
//		gameState = GameState.fight;	
		slowDown = false;	
		inSuperStartup = new bool[2];	
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
		
		if (gameState == GameState.introPose){
			StartCoroutine(IntroPoseState(introTimer));
		}
		
		if (gameState == GameState.countDown){
			CountDownCommence(countDownTimer);
		}
		
		if (opponentChar.GetKOed() == true || playerChar.GetKOed() == true){
			gameState = GameState.KOHappened;
			if (demonKO){
				StartCoroutine(ShunGokuSatsuKO());
			}
			else if (superKO){
				StartCoroutine(SuperKO());
			}			
			else{
				KOedTimer--;
			}
		}
				
		if (KOedTimer <= 0){
			gameState = GameState.victoryPose;
		}
		if (gameState == GameState.victoryPose){
			StartCoroutine(VictoryPoseTime(restartTimer));
		}
		
//		if (restartTimer <= 0){
//			levelManager.LoadLevel(SceneManager.GetActiveScene ().name);
//		}
		
		// may go back to hit stops using coroutines
//		if (slowDown){
//			StartCoroutine(SlowDown(slowDownTimer));
//		}
		
		if (inSuperStartup[0]){ 
			StartCoroutine(SuperPauseP1());	
		}
		if (inSuperStartup[1]){
			StartCoroutine(SuperPauseP2());	
		}
		
		if (gettingDemoned){			
			shunGokuSatsuPanel.SetActive(true);
		}
		else{			
			shunGokuSatsuPanel.SetActive(false);
		}
		
		if (slowDown && !inSuperStartup[0] && !inSuperStartup[1]){
			Time.timeScale = 0.5f;
			slowDownTimer--;
		}
		if (slowDownTimer <= 0 && !TimeControl.inSuperStartup[0] && !TimeControl.inSuperStartup[1] && !demonKO){
			slowDown = false;
			Time.timeScale = 1f;
		}		
	}

	IEnumerator SuperPauseP1(){
		Time.timeScale = 0f;
		superPanel.SetActive(true);
		yield return new WaitUntil( () => inSuperStartup[0] == false);
		Time.timeScale = 1f;
		superPanel.SetActive(false);
	}
	
	IEnumerator SuperPauseP2(){
		Time.timeScale = 0f;
		superPanel.SetActive(true);
		yield return new WaitUntil( () => inSuperStartup[1] == false);
		Time.timeScale = 1f;
		superPanel.SetActive(false);
	}
	
	IEnumerator ShunGokuSatsuKO(){
		Time.timeScale = 0.1f;
		shunGokuSatsuKOPanel.SetActive(true);
		yield return new WaitForSecondsRealtime(2f);
		demonKO = false;
		Time.timeScale = 1f;
		KOedTimer = 25;
		shunGokuSatsuKOPanel.SetActive(false);
		
	}
	
	IEnumerator SuperKO(){
		Time.timeScale = 0.2f;
		superKOPanel.SetActive(true);
		yield return new WaitForSeconds(2f);
		superKO = false;
		Time.timeScale = 1f;
		KOedTimer = 25;
		superKOPanel.SetActive(false);
		
	}
//	IEnumerator SlowDown(float slowDownTimer){
//		Time.timeScale = 0.5f;
//		while (slowDownTimer > 0f){
//			yield return null;
//			slowDownTimer -= Time.deltaTime;
//			Debug.Log (slowDownTimer);
//		}		
//		Time.timeScale = 1f;
//		slowDown = false;
//	}

	IEnumerator IntroPoseState (float timer){
		yield return new WaitForSeconds(timer);
		gameState = GameState.countDown;
	}

	IEnumerator VictoryPoseTime(float timer){
		winnerText.text = winner;
		int randWinQuote = Random.Range (0, winQuotes.Length);
		if (SceneManager.GetActiveScene().name == "Game"){
			if (winner == "You Win") {
				PlayWinnerClip (youWinSound);
			}
			else if (winner == "You Lose") {
				PlayWinnerClip (youLoseSound);
			}
		}
		else{
			PlayWinnerClip (winQuotes[randWinQuote]);
		}
		yield return new WaitForSeconds(timer);
		levelManager.LoadLevel(SceneManager.GetActiveScene ().name);
	}

	void PlayWinnerClip (AudioClip clip){
		if (!audioSource.isPlaying && playedOnce == 1) {
			audioSource.PlayOneShot (clip, 1f);
			playedOnce = 0;
		}
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
