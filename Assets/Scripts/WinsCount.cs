using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinsCount : MonoBehaviour {

    [SerializeField] Text akumaWinsText, balrogWinsText, feiLongWinsText, kenWinsText, sagatWinsText;
    public int akumaWins, balrogWins, feiLongWins, kenWins, sagatWins;
    
    private static WinsCount instance = null;

    private Player player;
    private Opponent opponent;
    public bool updated;

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
    void Update ()
    {
        if (!updated && TimeControl.gameState == TimeControl.GameState.victoryPose)
        {
            if (!player.GetComponentInChildren<Animator>().GetBool("isKOed"))
            {
                CountWin(player.gameObject);
            }
            else if (!opponent.GetComponentInChildren<Animator>().GetBool("isKOed"))
            {
                CountWin(opponent.gameObject);
            }
        }
    }

    void CountWin(GameObject fighter)
    {
        if (fighter.GetComponentInChildren<Akuma>() != null)
        {
            akumaWins++;
            akumaWinsText.text = akumaWins.ToString();
        }
        else if (fighter.GetComponentInChildren<Balrog>() != null)
        {
            balrogWins++;
            balrogWinsText.text = balrogWins.ToString();
        }
        else if (fighter.GetComponentInChildren<FeiLong>() != null)
        {
            feiLongWins++;
            feiLongWinsText.text = feiLongWins.ToString();
        }
        else if (fighter.GetComponentInChildren<Ken>() != null)
        {
            kenWins++;
            kenWinsText.text = kenWins.ToString();
        }
        else if (fighter.GetComponentInChildren<Sagat>() != null)
        {
            sagatWins++;
            sagatWinsText.text = sagatWins.ToString();
        }
        updated = true;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = FindObjectOfType<Player>();
        opponent = FindObjectOfType<Opponent>();
        akumaWinsText.text = akumaWins.ToString();
        updated = false;
    }
}
