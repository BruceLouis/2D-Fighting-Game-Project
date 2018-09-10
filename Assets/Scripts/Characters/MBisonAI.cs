using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MBisonAI : MonoBehaviour {

    [SerializeField] float decisionTimer, antiAirTimer;

    private Animator animator;
    private Player player;
    private Opponent opponent;
    private Character playerCharacter, opponentCharacter;
    private Character character;
    private SharedProperties sharedProperties;
    private AIControls AIcontrols;

    private int decision;
    private float decisionTimerInput, antiAirTimerInput;

    // Use this for initialization
    void Start()
    {

        if (GetComponentInParent<Opponent>() != null)
        {
            player = FindObjectOfType<Player>();
            playerCharacter = player.GetComponentInChildren<Character>();
        }
        else if (GetComponentInParent<Player>() != null)
        {
            opponent = FindObjectOfType<Opponent>();
            opponentCharacter = opponent.GetComponentInChildren<Character>();
        }

        character = GetComponent<Character>();
        animator = GetComponent<Animator>();
        AIcontrols = GetComponentInParent<AIControls>();
        sharedProperties = GetComponentInParent<SharedProperties>();

        decisionTimerInput = decisionTimer;
        antiAirTimerInput = antiAirTimer;
        antiAirTimer = 0f;
        decision = Random.Range(0, 100);
    }
    
    public void Behaviors()
    {

    }
}
