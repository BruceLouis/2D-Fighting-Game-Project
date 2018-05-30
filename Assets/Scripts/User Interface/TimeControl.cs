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
	public static GameState gameState;
	
	public GameObject superPanel, shunGokuSatsuPanel, shunGokuSatsuKOPanel, superKOPanel;
	
	public Text gameTimerText, countDownText, winnerText;
	public AudioClip threeSound, twoSound, oneSound, fightSound, youLoseSound, youWinSound;
	public AudioClip[] winQuotes;
	
	private int gameTimer, internalTimer, countDownTimer, KOslowTimer, KOedTimer;
    private bool countDownStarted, gameTimerCountingDown, announcementPlayed;
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
		restartTimer = 2f;
		KOedTimer = 150;
		introTimer = 1.75f;
		countDownTimer = 3;

		announcementPlayed = false;
        countDownStarted = false;
        gameTimerCountingDown = false;

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
			
		if (gameState == GameState.introPose){
			StartCoroutine(IntroPoseState(introTimer));
		}
		
		if (gameState == GameState.countDown && !countDownStarted){
			StartCoroutine(CountDownCommence());
		}
		
        if (gameState == GameState.fight && !gameTimerCountingDown){
            StartCoroutine(InGameTimer());
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
		if (slowDownTimer <= 0 && !inSuperStartup[0] && !inSuperStartup[1] && !demonKO && !superKO){
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
		yield return new WaitForSecondsRealtime(2f);
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
    
    IEnumerator CountDownCommence(){
        countDownStarted = true;

		if (!audioSource.isPlaying && !announcementPlayed) {
			audioSource.PlayOneShot (threeSound, 0.7f);
			announcementPlayed = true;
		}
		countDownText.text = countDownTimer.ToString();

        yield return new WaitForSecondsRealtime(1f);

        announcementPlayed = false;
        if (!audioSource.isPlaying && !announcementPlayed) {
			audioSource.PlayOneShot (twoSound, 0.7f);
			announcementPlayed = true;
		}
        countDownTimer--;
        countDownText.text = countDownTimer.ToString();

        yield return new WaitForSecondsRealtime(1f);

        announcementPlayed = false;
        if (!audioSource.isPlaying && !announcementPlayed) {
			audioSource.PlayOneShot (oneSound, 0.7f);
			announcementPlayed = true;
		}
        countDownTimer--;
        countDownText.text = countDownTimer.ToString();

        yield return new WaitForSecondsRealtime(1f);

        announcementPlayed = false;
        if (!audioSource.isPlaying && !announcementPlayed) {
			audioSource.PlayOneShot (fightSound, 0.7f);
			announcementPlayed = true;
		}
        countDownTimer--;
        countDownText.text = "FIGHT";

        yield return new WaitForSecondsRealtime(1f);
        
		gameState = GameState.fight;
		countDownText.text = "";

        announcementPlayed = false;
        countDownStarted = false;
    }

    IEnumerator InGameTimer(){
        gameTimerCountingDown = true;
        yield return new WaitForSeconds(1f);
        gameTimer--;
        gameTimerText.text = gameTimer.ToString();
        gameTimerCountingDown = false;
    }

    void PlayWinnerClip(AudioClip clip){
        if (!audioSource.isPlaying && !announcementPlayed)
        {
            audioSource.PlayOneShot(clip, 1f);
            announcementPlayed = true;
        }
    }
}
