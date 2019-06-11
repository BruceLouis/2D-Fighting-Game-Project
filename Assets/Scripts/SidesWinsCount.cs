using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SidesWinsCount : MonoBehaviour
{

    [SerializeField] Text p1WinsText, p2WinsText;
    private int p1Wins, p2Wins;

    private static SidesWinsCount instance = null;

    private Player player;
    private Opponent opponent;
    private bool updated;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Update is called once per frame
    void Update()
    {
        if (!updated && TimeControl.gameState == TimeControl.GameState.victoryPose)
        {
            if (!player.GetComponentInChildren<Animator>().GetBool("isKOed"))
            {
                p1Wins++;
                p1WinsText.text = p1Wins.ToString();
            }
            else if (!opponent.GetComponentInChildren<Animator>().GetBool("isKOed"))
            {
                p2Wins++;
                p2WinsText.text = p2Wins.ToString();
            }
            updated = true;
        }
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = FindObjectOfType<Player>();
        opponent = FindObjectOfType<Opponent>();
        updated = false;
    }
}
