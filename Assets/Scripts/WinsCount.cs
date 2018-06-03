using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinsCount : MonoBehaviour {

    [SerializeField] Text[] akumaRecordText, balrogRecordText, feiLongRecordText, kenRecordText, sagatRecordText;
    private int[] akumaRecord = new int[2], balrogRecord = new int[2], feiLongRecord = new int[2], kenRecord = new int[2], sagatRecord = new int[2];
    
    private static WinsCount instance = null;

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
    void Update ()
    {
        if (!updated && TimeControl.gameState == TimeControl.GameState.victoryPose)
        {
            if (!player.GetComponentInChildren<Animator>().GetBool("isKOed"))
            {
                UpdateRecord(player.gameObject, 0);
                UpdateRecord(opponent.gameObject, 1);
            }
            else if (!opponent.GetComponentInChildren<Animator>().GetBool("isKOed"))
            {
                UpdateRecord(opponent.gameObject, 0);
                UpdateRecord(player.gameObject, 1);
            }
        }
    }

    void UpdateRecord(GameObject fighter, int winOrLoss)
    {
        if (fighter.GetComponentInChildren<Akuma>() != null)
        {
            akumaRecord[winOrLoss]++;
            akumaRecordText[winOrLoss].text = akumaRecord[winOrLoss].ToString();
        }
        else if (fighter.GetComponentInChildren<Balrog>() != null)
        {
            balrogRecord[winOrLoss]++;
            balrogRecordText[winOrLoss].text = balrogRecord[winOrLoss].ToString();
        }
        else if (fighter.GetComponentInChildren<FeiLong>() != null)
        {
            feiLongRecord[winOrLoss]++;
            feiLongRecordText[winOrLoss].text = feiLongRecord[winOrLoss].ToString();
        }
        else if (fighter.GetComponentInChildren<Ken>() != null)
        {
            kenRecord[winOrLoss]++;
            kenRecordText[winOrLoss].text = kenRecord[winOrLoss].ToString();
        }
        else if (fighter.GetComponentInChildren<Sagat>() != null)
        {
            sagatRecord[winOrLoss]++;
            sagatRecordText[winOrLoss].text = sagatRecord[winOrLoss].ToString();
        }
        updated = true;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = FindObjectOfType<Player>();
        opponent = FindObjectOfType<Opponent>();
        updated = false;
    }
}
