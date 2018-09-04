using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

/*  This is pretty much the game manager, where it controls the state of the rounds itself whether it's
 *  the intro pose, the countdown before the round start, the actual fight and when someone is KOed and 
 *  the fight ends. The time control also handles the timer within the actual fight, although right now
 *  the timer is inconsequential as there is no time out victory. The time controlling game manager also
 *  controls when the super panels become activated, which is when a character executes a super at the 
 *  start of a super and when a character gets KOed by a super. All the timers here use co routines for
 *  implementation except for the hitstun slowdown timer and KO slowdown timer.
 */ 

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
	
	private int gameTimer, internalTimer, KOslowTimer, KOedTimer;
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
//		gameState = GameState.introPose;	
		gameState = GameState.fight;	
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
    
    IEnumerator CountDownCommence()
    {
        countDownStarted = true;

        CountedDown(threeSound, "3");

        yield return new WaitForSecondsRealtime(1f);

        CountedDown(twoSound, "2");

        yield return new WaitForSecondsRealtime(1f);

        CountedDown(oneSound, "1");

        yield return new WaitForSecondsRealtime(1f);

        CountedDown(fightSound, "FIGHT");

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

    void CountedDown(AudioClip audioClip, string text)
    {
        announcementPlayed = false;
        if (!audioSource.isPlaying && !announcementPlayed)
        {
            audioSource.PlayOneShot(audioClip, 0.7f);
            announcementPlayed = true;
        }
        countDownText.text = text;
    }

    void PlayWinnerClip(AudioClip clip){
        if (!audioSource.isPlaying && !announcementPlayed)
        {
            audioSource.PlayOneShot(clip, 1f);
            announcementPlayed = true;
        }
    }
}
