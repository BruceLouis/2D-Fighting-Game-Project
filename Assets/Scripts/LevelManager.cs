using UnityEngine; 
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour {

	public void LoadLevel(string name){
		Debug.Log ("New Level load: " + name);
        TimeControl.paused = false;
        TimeControl.slowDown = false;
        TimeControl.slowDownTimer = 0f;
        SceneManager.LoadScene(name);
	}
}
