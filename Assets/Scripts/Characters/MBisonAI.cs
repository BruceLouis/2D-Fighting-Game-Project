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
    private ChargeSystem chargeSystem;

    private int decision;
    private float decisionTimerInput, antiAirTimerInput;
    private const int DECISION_MAX = 100;

    List<string> regularFarRangeDecisions, regularMidRangeDecisions, regularCloseRangeDecisions, airAttackDecisions;
    List<string> otherFighterBlockedDecisions, otherFighterGotHitDecisions, knockDownDecisions, antiAirDecisions;

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
        chargeSystem = GetComponentInParent<ChargeSystem>();
        sharedProperties = GetComponentInParent<SharedProperties>();
        
        regularFarRangeDecisions = new List<string>();
        regularMidRangeDecisions = new List<string>();
        regularCloseRangeDecisions = new List<string>();
        airAttackDecisions = new List<string>();
        otherFighterBlockedDecisions = new List<string>();
        otherFighterGotHitDecisions = new List<string>();
        knockDownDecisions = new List<string>();
        antiAirDecisions = new List<string>();
        
        RegularFarRangeDecisionTrees();
        RegularMidRangeDecisionTrees();
        RegularCloseRangeDecisionTrees();
        AirAttackDecisionTrees();
        OtherFighterBlockedDecisionTrees();
        OtherFighterGotHitDecisionTrees();
        KnockDownDecisionTrees();
        AntiAirDecisionTrees();

        decision = Random.Range(0, DECISION_MAX);
        decisionTimerInput = decisionTimer;
        antiAirTimerInput = antiAirTimer;
        antiAirTimer = 0f;
    }

    public void Behaviors()
    {
        decisionTimer--;
        antiAirTimer--;

        if (AIcontrols.FreeToMakeDecisions() && !TimeControl.inSuperStartup[0] && !TimeControl.inSuperStartup[1])
        {
            if (animator.GetBool("isAirborne") == true && animator.GetBool("isLiftingOff") == false)
            {
                if (animator.GetBool("reverseActive") == true || animator.GetBool("devilReverseActive") == true)
                {
                    SomerSaultDecisions();
                }
                else
                {
                    RegularDecisionTree(airAttackDecisions, 1, 1);
                }
                AIcontrols.AICharges();
                character.SetBackPressed(true);
            }
            else if (character.GetKnockDown() == true)
            {
                AIcontrols.AICrouch();
                AIcontrols.AICharges();
                character.SetBackPressed(true);
            }
            else if (sharedProperties.GetAbDistanceFromOtherFighter() >= 2f)
            {
                RegularDecisionTree(regularFarRangeDecisions, 5, 2);
            }
            else if (sharedProperties.GetAbDistanceFromOtherFighter() >= 1f && sharedProperties.GetAbDistanceFromOtherFighter() < 2f)
            {        
                if (player != null)
                {
                    DecisionBranches(playerCharacter, regularMidRangeDecisions, regularMidRangeDecisions.Count);
                }
                else if (opponent != null)
                {
                    DecisionBranches(opponentCharacter, regularMidRangeDecisions, regularMidRangeDecisions.Count);
                }
            }
            else
            {
                if (player != null)
                {
                    DecisionBranches(playerCharacter, regularCloseRangeDecisions, regularCloseRangeDecisions.Count);
                }
                else if (opponent != null)
                {
                    DecisionBranches(opponentCharacter, regularCloseRangeDecisions, regularCloseRangeDecisions.Count);
                }
            }
            AIcontrols.AIWalks();
        }
    }

    void DecisionBranches(Character whichCharacter, List<string> distanceDecision, int distanceDecisionCount)
    {
        if (whichCharacter.GetBlockStunned() == true)
        {
            RegularDecisionTree(otherFighterBlockedDecisions, 1, 1);
        }
        else if (whichCharacter.GetHitStunned() == true)
        {
            RegularDecisionTree(otherFighterGotHitDecisions, 1, 1);
        }
        else if (whichCharacter.GetKnockDown() == true && whichCharacter.GetAirborne() == false)
        {
            RegularDecisionTree(knockDownDecisions, 4, 2);
        }
        else if (whichCharacter.GetAirborne() == true && whichCharacter.GetKnockDown() == false && whichCharacter.GetThrown() == false)
        {
            if (antiAirTimer <= 0f)
            {
                sharedProperties.RefinedAIAntiAirDecision(
                    50, RegularDecisionTree,
                    antiAirDecisions, 1, 1,
                    distanceDecision, 5, 3);
                antiAirTimer = antiAirTimerInput;
            }
            else
            {
                RegularDecisionTree(distanceDecision, 5, 3);
            }
        }
        else
        {
            RegularDecisionTree(distanceDecision, 5, 3);
        }
    }

    void RegularFarRangeDecisionTrees()
    {
        int walkForwardsInBox = 35;
        int walkBackwardsInBox = 5;
        int crouchInBox = 100;
        int neutralJumpInBox = 10;
        int forwardJumpInBox = 10;
        int headStompInBox = 40;
        int devilsReverseInBox = 30;

        int sumOfBox = walkForwardsInBox + walkBackwardsInBox + crouchInBox + neutralJumpInBox + forwardJumpInBox + headStompInBox + devilsReverseInBox;

        walkForwardsInBox = normalizedDecisions(walkForwardsInBox, sumOfBox);
        walkBackwardsInBox = normalizedDecisions(walkBackwardsInBox, sumOfBox);
        crouchInBox = normalizedDecisions(crouchInBox, sumOfBox);
        neutralJumpInBox = normalizedDecisions(neutralJumpInBox, sumOfBox);
        forwardJumpInBox = normalizedDecisions(forwardJumpInBox, sumOfBox);
        headStompInBox = normalizedDecisions(headStompInBox, sumOfBox);
        devilsReverseInBox = normalizedDecisions(devilsReverseInBox, sumOfBox);

        int normalizedSum = walkForwardsInBox + walkBackwardsInBox + crouchInBox + neutralJumpInBox + forwardJumpInBox + headStompInBox + devilsReverseInBox;
        crouchInBox = NormalizedDifference(crouchInBox, normalizedSum);

        InsertNewAction(regularFarRangeDecisions, "WalkForward", walkForwardsInBox);
        InsertNewAction(regularFarRangeDecisions, "WalkBackward", walkBackwardsInBox);
        InsertNewAction(regularFarRangeDecisions, "Crouch", crouchInBox);
        InsertNewAction(regularFarRangeDecisions, "NeutralJump", neutralJumpInBox);
        InsertNewAction(regularFarRangeDecisions, "ForwardJump", forwardJumpInBox);
        InsertNewAction(regularFarRangeDecisions, "HeadStomp", headStompInBox);
        InsertNewAction(regularFarRangeDecisions, "DevilsReverse", devilsReverseInBox);
    }
    
    void RegularMidRangeDecisionTrees()
    {
        int walkForwardInBox = 20;
        int walkBackwardInBox = 5;
        int crouchInBox = 60;
        int neutralJumpInBox = 10;
        int forwardJumpInBox = 30;
        int roundhouseInBox = 40;
        int fierceInBox = 10;
        int forwardInBox = 25;
        int strongInBox = 20;
        int shortInBox = 10;
        int jabInBox = 10;
        int headStompInBox = 25;
        int devilsReverseInBox = 20;
        int scissorKickInBox = 120;
        int psychoCrusherInBox = 120;
        int superInBox = 40;

        int sumInBox =  walkForwardInBox + walkBackwardInBox + crouchInBox + neutralJumpInBox + forwardJumpInBox + roundhouseInBox +
                        fierceInBox + forwardInBox + strongInBox + shortInBox + jabInBox + headStompInBox + devilsReverseInBox +
                        scissorKickInBox + psychoCrusherInBox + superInBox;

        walkForwardInBox = normalizedDecisions(walkForwardInBox, sumInBox);
        walkBackwardInBox =normalizedDecisions(walkBackwardInBox, sumInBox);
        crouchInBox = normalizedDecisions(crouchInBox, sumInBox);
        neutralJumpInBox = normalizedDecisions(neutralJumpInBox, sumInBox);
        forwardJumpInBox = normalizedDecisions(forwardJumpInBox, sumInBox);
        roundhouseInBox = normalizedDecisions(roundhouseInBox, sumInBox);
        fierceInBox = normalizedDecisions(fierceInBox, sumInBox);
        forwardInBox = normalizedDecisions(forwardInBox, sumInBox);
        strongInBox = normalizedDecisions(strongInBox, sumInBox);
        shortInBox = normalizedDecisions(shortInBox, sumInBox);
        jabInBox = normalizedDecisions(jabInBox, sumInBox);
        headStompInBox = normalizedDecisions(headStompInBox, sumInBox);
        devilsReverseInBox = normalizedDecisions(devilsReverseInBox, sumInBox);
        scissorKickInBox = normalizedDecisions(scissorKickInBox, sumInBox);
        psychoCrusherInBox = normalizedDecisions(psychoCrusherInBox, sumInBox);
        superInBox = normalizedDecisions(superInBox, sumInBox);

        int normalizedSum = walkForwardInBox + walkBackwardInBox + crouchInBox + neutralJumpInBox + forwardJumpInBox + roundhouseInBox +
                            fierceInBox + forwardInBox + strongInBox + shortInBox + jabInBox + headStompInBox + devilsReverseInBox +
                            scissorKickInBox + psychoCrusherInBox + superInBox;

        devilsReverseInBox = NormalizedDifference(devilsReverseInBox, normalizedSum);

        InsertNewAction(regularMidRangeDecisions, "WalkForward", walkForwardInBox);
        InsertNewAction(regularMidRangeDecisions, "WalkBackward", walkBackwardInBox);
        InsertNewAction(regularMidRangeDecisions, "Crouch", crouchInBox);
        InsertNewAction(regularMidRangeDecisions, "NeutralJump", neutralJumpInBox);
        InsertNewAction(regularMidRangeDecisions, "ForwardJump", forwardJumpInBox);
        InsertNewAction(regularMidRangeDecisions, "Roundhouse", roundhouseInBox);
        InsertNewAction(regularMidRangeDecisions, "Fierce", fierceInBox);
        InsertNewAction(regularMidRangeDecisions, "Forward", forwardInBox);
        InsertNewAction(regularMidRangeDecisions, "Strong", strongInBox);
        InsertNewAction(regularMidRangeDecisions, "Short", shortInBox);
        InsertNewAction(regularMidRangeDecisions, "Jab", jabInBox);
        InsertNewAction(regularMidRangeDecisions, "HeadStomp", headStompInBox);
        InsertNewAction(regularMidRangeDecisions, "DevilsReverse", devilsReverseInBox);
        InsertNewAction(regularMidRangeDecisions, "ScissorKick", scissorKickInBox);
        InsertNewAction(regularMidRangeDecisions, "PsychoCrusher", psychoCrusherInBox);
        InsertNewAction(regularMidRangeDecisions, "Super", superInBox);
    }
    
    void RegularCloseRangeDecisionTrees()
    {
        int walkForwardInBox = 15;
        int walkBackwardInBox = 2;
        int crouchInBox = 20;
        int neutralJumpInBox = 10;
        int forwardJumpInBox = 30;
        int roundhouseInBox = 15;
        int fierceInBox = 5;
        int forwardInBox = 25;
        int strongInBox = 20;
        int shortInBox = 50;
        int jabInBox = 45;
        int headStompInBox = 10;
        int devilsReverseInBox = 5;
        int scissorKickInBox = 60;
        int psychoCrusherInBox = 50;
        int throwInBox = 50;
        int superInBox = 40;

        int sumInBox =  walkForwardInBox + walkBackwardInBox + crouchInBox + neutralJumpInBox + forwardJumpInBox +
                        roundhouseInBox + fierceInBox + forwardInBox + strongInBox + shortInBox + jabInBox +
                        headStompInBox + devilsReverseInBox + scissorKickInBox + psychoCrusherInBox +
                        throwInBox + superInBox;

        walkForwardInBox = normalizedDecisions(walkForwardInBox, sumInBox);
        walkBackwardInBox = normalizedDecisions(walkBackwardInBox, sumInBox);
        crouchInBox = normalizedDecisions(crouchInBox, sumInBox);
        neutralJumpInBox = normalizedDecisions(neutralJumpInBox, sumInBox);
        forwardJumpInBox = normalizedDecisions(forwardJumpInBox, sumInBox);
        roundhouseInBox = normalizedDecisions(roundhouseInBox, sumInBox);
        fierceInBox = normalizedDecisions(fierceInBox, sumInBox);
        forwardInBox = normalizedDecisions(forwardInBox, sumInBox);
        strongInBox = normalizedDecisions(strongInBox, sumInBox);
        shortInBox = normalizedDecisions(shortInBox, sumInBox);
        jabInBox = normalizedDecisions(jabInBox, sumInBox);
        headStompInBox = normalizedDecisions(headStompInBox, sumInBox);
        devilsReverseInBox = normalizedDecisions(devilsReverseInBox, sumInBox);
        scissorKickInBox = normalizedDecisions(scissorKickInBox, sumInBox);
        psychoCrusherInBox = normalizedDecisions(psychoCrusherInBox, sumInBox);
        throwInBox = normalizedDecisions(throwInBox, sumInBox);
        superInBox = normalizedDecisions(superInBox, sumInBox);


        int normalizedSum = walkForwardInBox + walkBackwardInBox + crouchInBox + neutralJumpInBox + forwardJumpInBox +
                            roundhouseInBox + fierceInBox + forwardInBox + strongInBox + shortInBox + jabInBox +
                            headStompInBox + devilsReverseInBox + scissorKickInBox + psychoCrusherInBox +
                            throwInBox + superInBox;

        devilsReverseInBox = NormalizedDifference(devilsReverseInBox, normalizedSum);

        InsertNewAction(regularCloseRangeDecisions, "WalkForward", walkForwardInBox);
        InsertNewAction(regularCloseRangeDecisions, "WalkBackward", walkBackwardInBox);
        InsertNewAction(regularCloseRangeDecisions, "Crouch", crouchInBox);
        InsertNewAction(regularCloseRangeDecisions, "NeutralJump", neutralJumpInBox);
        InsertNewAction(regularCloseRangeDecisions, "ForwardJump", forwardJumpInBox);
        InsertNewAction(regularCloseRangeDecisions, "Roundhouse",roundhouseInBox);
        InsertNewAction(regularCloseRangeDecisions, "Fierce", fierceInBox);
        InsertNewAction(regularCloseRangeDecisions, "Forward", forwardInBox);
        InsertNewAction(regularCloseRangeDecisions, "Strong", strongInBox);
        InsertNewAction(regularCloseRangeDecisions, "Short", shortInBox);
        InsertNewAction(regularCloseRangeDecisions, "Jab", jabInBox);
        InsertNewAction(regularCloseRangeDecisions, "HeadStomp", headStompInBox);
        InsertNewAction(regularCloseRangeDecisions, "DevilsReverse", devilsReverseInBox);
        InsertNewAction(regularCloseRangeDecisions, "ScissorKick", scissorKickInBox);
        InsertNewAction(regularCloseRangeDecisions, "PsychoCrusher", psychoCrusherInBox);
        InsertNewAction(regularCloseRangeDecisions, "Throw", throwInBox);
        InsertNewAction(regularCloseRangeDecisions, "Super", superInBox);
    }

    void AirAttackDecisionTrees()
    {
        int airRoundhouseInBox = 30;
        int airFierceInBox = 40;
        int airForwardInBox = 10;
        int airStrongInBox = 10;
        int bigFatNothingInBox = 150;

        int sumInBox = airRoundhouseInBox + airFierceInBox + airForwardInBox + airStrongInBox + bigFatNothingInBox;

        airRoundhouseInBox = normalizedDecisions(airRoundhouseInBox, sumInBox);
        airFierceInBox = normalizedDecisions(airFierceInBox, sumInBox);
        airForwardInBox = normalizedDecisions(airForwardInBox, sumInBox);
        airStrongInBox = normalizedDecisions(airStrongInBox, sumInBox);
        bigFatNothingInBox = normalizedDecisions(bigFatNothingInBox, sumInBox);

        int normalizedSum = airRoundhouseInBox + airFierceInBox + airForwardInBox + airStrongInBox + bigFatNothingInBox;
        bigFatNothingInBox = NormalizedDifference(bigFatNothingInBox, normalizedSum);

        InsertNewAction(airAttackDecisions, "AirRoundhouse", airRoundhouseInBox);
        InsertNewAction(airAttackDecisions, "AirFierce", airFierceInBox);
        InsertNewAction(airAttackDecisions, "AirForward", airForwardInBox);
        InsertNewAction(airAttackDecisions, "AirStrong", airStrongInBox);
        InsertNewAction(airAttackDecisions, "", bigFatNothingInBox);
    }

    void OtherFighterBlockedDecisionTrees()
    {
        int jabInBox = 30;
        int shortInBox = 30;
        int lowForwardInBox = 60;
        int strongInBox = 10;
        int specialCancelScissorKickInBox = 120;
        int specialCancelPsychoCrusherInBox = 70;
        int specialCancelHeadStompInBox = 10;
        int specialCancelDevilsReverseInBox = 10;
        int superInBox = 40;

        int sumInBox =  jabInBox + shortInBox + lowForwardInBox + strongInBox + specialCancelScissorKickInBox + superInBox +
                        specialCancelPsychoCrusherInBox + specialCancelHeadStompInBox + specialCancelDevilsReverseInBox;

        jabInBox  = normalizedDecisions(jabInBox, sumInBox);
        shortInBox = normalizedDecisions(shortInBox, sumInBox);
        lowForwardInBox = normalizedDecisions(lowForwardInBox, sumInBox);
        strongInBox = normalizedDecisions(strongInBox, sumInBox);
        specialCancelScissorKickInBox = normalizedDecisions(specialCancelScissorKickInBox, sumInBox);
        specialCancelPsychoCrusherInBox = normalizedDecisions(specialCancelPsychoCrusherInBox, sumInBox);
        specialCancelHeadStompInBox = normalizedDecisions(specialCancelHeadStompInBox, sumInBox);
        specialCancelDevilsReverseInBox = normalizedDecisions(specialCancelDevilsReverseInBox, sumInBox);
        superInBox = normalizedDecisions(superInBox, sumInBox);

        int normalizedSum = jabInBox + shortInBox + lowForwardInBox + strongInBox + specialCancelScissorKickInBox + superInBox +
                            specialCancelPsychoCrusherInBox + specialCancelHeadStompInBox + specialCancelDevilsReverseInBox;

        specialCancelDevilsReverseInBox = NormalizedDifference(specialCancelDevilsReverseInBox, normalizedSum);

        InsertNewAction(otherFighterBlockedDecisions, "Jab", jabInBox);
        InsertNewAction(otherFighterBlockedDecisions, "Short", shortInBox);
        InsertNewAction(otherFighterBlockedDecisions, "LowForward", lowForwardInBox);
        InsertNewAction(otherFighterBlockedDecisions, "Strong", strongInBox);
        InsertNewAction(otherFighterBlockedDecisions, "SpecialCancelScissorKick", specialCancelScissorKickInBox);
        InsertNewAction(otherFighterBlockedDecisions, "SpecialCancelPsychoCrusher", specialCancelPsychoCrusherInBox);
        InsertNewAction(otherFighterBlockedDecisions, "SpecialCancelHeadStomp", specialCancelHeadStompInBox);
        InsertNewAction(otherFighterBlockedDecisions, "SpecialCancelDevilsReverse", specialCancelDevilsReverseInBox);
        InsertNewAction(otherFighterBlockedDecisions, "SuperOnBlock", superInBox);
    }

    void OtherFighterGotHitDecisionTrees()
    {
        int jabInBox = 30;
        int shortInBox = 40;
        int lowForwardInBox = 70;
        int specialCancelScissorKickInBox = 120;
        int specialCancelPsychoCrusherInBox = 70;
        int superInBox = 40;

        int sumInBox = jabInBox + shortInBox + lowForwardInBox + specialCancelPsychoCrusherInBox + specialCancelScissorKickInBox + superInBox;

        jabInBox = normalizedDecisions(jabInBox, sumInBox);
        shortInBox = normalizedDecisions(shortInBox, sumInBox);
        lowForwardInBox = normalizedDecisions(lowForwardInBox, sumInBox);
        specialCancelScissorKickInBox = normalizedDecisions(specialCancelScissorKickInBox, sumInBox);
        specialCancelPsychoCrusherInBox = normalizedDecisions(specialCancelPsychoCrusherInBox, sumInBox);
        superInBox = normalizedDecisions(superInBox, sumInBox);

        int normalizedSum = jabInBox + shortInBox + lowForwardInBox + specialCancelPsychoCrusherInBox + specialCancelScissorKickInBox + superInBox;
        superInBox = NormalizedDifference(superInBox, normalizedSum);

        InsertNewAction(otherFighterGotHitDecisions, "Jab", jabInBox);
        InsertNewAction(otherFighterGotHitDecisions, "Short", shortInBox);
        InsertNewAction(otherFighterGotHitDecisions, "LowForward", lowForwardInBox);
        InsertNewAction(otherFighterGotHitDecisions, "SpecialCancelScissorKick", specialCancelScissorKickInBox);
        InsertNewAction(otherFighterGotHitDecisions, "SpecialCancelPsychoCrusher", specialCancelPsychoCrusherInBox);
        InsertNewAction(otherFighterGotHitDecisions, "SuperOnHit", superInBox);
    }

    void KnockDownDecisionTrees()
    {
        int walkForwardInBox = 40;
        int walkBackwardInBox = 20;
        int crouchInBox = 30;
        int neutralJumpInBox = 40;
        int forwardJumpInBox = 30;
        int sweepInBox = 40;
        int fierceInBox = 2;
        int forwardInBox = 2;
        int strongInBox = 2;
        int shortInBox = 30;
        int jabInBox = 30;
        int headStompInBox = 20;
        int devilsReverseInBox = 20;
        int scissorKickInBox = 30;
        int psychoCrusherInBox = 30;

        int sumInBox =  walkForwardInBox + walkBackwardInBox + crouchInBox + neutralJumpInBox + forwardJumpInBox +
                        sweepInBox + fierceInBox + forwardInBox + strongInBox + shortInBox + jabInBox +
                        headStompInBox + devilsReverseInBox + scissorKickInBox + psychoCrusherInBox;

        walkForwardInBox = normalizedDecisions(walkForwardInBox, sumInBox);
        walkBackwardInBox = normalizedDecisions(walkBackwardInBox, sumInBox);
        crouchInBox = normalizedDecisions(crouchInBox, sumInBox);
        neutralJumpInBox = normalizedDecisions(neutralJumpInBox, sumInBox);
        forwardJumpInBox = normalizedDecisions(forwardJumpInBox, sumInBox);
        sweepInBox = normalizedDecisions(sweepInBox, sumInBox);
        fierceInBox = normalizedDecisions(fierceInBox, sumInBox);
        forwardInBox = normalizedDecisions(forwardInBox, sumInBox);
        strongInBox = normalizedDecisions(strongInBox, sumInBox);
        shortInBox = normalizedDecisions(shortInBox, sumInBox);
        jabInBox = normalizedDecisions(jabInBox, sumInBox);
        headStompInBox = normalizedDecisions(headStompInBox, sumInBox);
        devilsReverseInBox = normalizedDecisions(devilsReverseInBox, sumInBox);
        scissorKickInBox = normalizedDecisions(scissorKickInBox, sumInBox);
        psychoCrusherInBox = normalizedDecisions(psychoCrusherInBox, sumInBox);

        int normalizedSum = walkForwardInBox + walkBackwardInBox + crouchInBox + neutralJumpInBox + forwardJumpInBox +
                            sweepInBox + fierceInBox + forwardInBox + strongInBox + shortInBox + jabInBox +
                            headStompInBox + devilsReverseInBox + scissorKickInBox + psychoCrusherInBox;

        walkBackwardInBox = NormalizedDifference(walkBackwardInBox, normalizedSum);

        InsertNewAction(knockDownDecisions, "WalkForward", walkForwardInBox);
        InsertNewAction(knockDownDecisions, "WalkBackward", walkBackwardInBox);
        InsertNewAction(knockDownDecisions, "Crouch", crouchInBox);
        InsertNewAction(knockDownDecisions, "NeutralJump", neutralJumpInBox);
        InsertNewAction(knockDownDecisions, "ForwardJump", forwardJumpInBox);
        InsertNewAction(knockDownDecisions, "Sweep", sweepInBox);
        InsertNewAction(knockDownDecisions, "Fierce", fierceInBox);
        InsertNewAction(knockDownDecisions, "Forward", forwardInBox);
        InsertNewAction(knockDownDecisions, "Strong", strongInBox);
        InsertNewAction(knockDownDecisions, "Short", shortInBox);
        InsertNewAction(knockDownDecisions, "Jab", jabInBox);
        InsertNewAction(knockDownDecisions, "HeadStomp", headStompInBox);
        InsertNewAction(knockDownDecisions, "DevilsReverse", devilsReverseInBox);
        InsertNewAction(knockDownDecisions, "ScissorKick", scissorKickInBox);
        InsertNewAction(knockDownDecisions, "PsychoCrusher", psychoCrusherInBox);
    }

    void AntiAirDecisionTrees()
    {
        int roundhouseInBox = 20;
        int fierceInBox = 20;
        int scissorKicksInBox = 20;
        int psychoCrusherInBox = 20;

        int sumInBox = roundhouseInBox + fierceInBox + scissorKicksInBox + psychoCrusherInBox;

        roundhouseInBox = normalizedDecisions(roundhouseInBox, sumInBox);
        fierceInBox = normalizedDecisions(fierceInBox, sumInBox);
        scissorKicksInBox = normalizedDecisions(scissorKicksInBox, sumInBox);
        psychoCrusherInBox = normalizedDecisions(psychoCrusherInBox, sumInBox);

        int normalizedSum = roundhouseInBox + fierceInBox + scissorKicksInBox + psychoCrusherInBox;
        fierceInBox = NormalizedDifference(fierceInBox, normalizedSum);

        InsertNewAction(antiAirDecisions, "Roundhouse", roundhouseInBox);
        InsertNewAction(antiAirDecisions, "Fierce", fierceInBox);
        InsertNewAction(antiAirDecisions, "ScissorKicks", scissorKicksInBox);
        InsertNewAction(antiAirDecisions, "PsychoCrusher", psychoCrusherInBox);
    }

    void RegularDecisionTree(List<string> decisionBox, int minDiv, int maxDiv)
    {
        DecisionMade(minDiv, maxDiv);
        if (decisionBox[decision] == "WalkForward" && !animator.GetBool("isAttacking"))
        {
            AIcontrols.AIStand();
            AIcontrols.AIPressedForward();
            character.SetBackPressed(false);
        }
        else if (decisionBox[decision] == "WalkBackward")
        {
            AIcontrols.AIStand();
            AIcontrols.AIPressedBackward();
            character.SetBackPressed(true);
        }
        else if (decisionBox[decision] == "Crouch")
        {
            AIcontrols.AICrouch();
            AIcontrols.AICharges();
            character.SetBackPressed(true);
        }
        else if (decisionBox[decision] == "NeutralJump" && !animator.GetBool("isAttacking"))
        {
            sharedProperties.CharacterNeutralState();
            AIcontrols.AIJump();
            character.SetBackPressed(false);
            decisionTimer = 0f;
        }
        else if (decisionBox[decision] == "ForwardJump" && !animator.GetBool("isAttacking"))
        {
            AIcontrols.AIPressedForward();
            AIcontrols.AIJump();
            character.SetBackPressed(false);
            decisionTimer = 0f;
        }
        else if (decisionBox[decision] == "Roundhouse")
        {
            AIcontrols.AIRoundhouse(2, 0);
            AIcontrols.AICharges();
            character.SetBackPressed(true);
            decisionTimer = 0f;
        }
        else if (decisionBox[decision] == "Fierce")
        {
            AIcontrols.AIFierce(2, 0);
            AIcontrols.AICharges();
            character.SetBackPressed(true);
            decisionTimer = 0f;
        }
        else if (decisionBox[decision] == "Forward")
        {
            AIcontrols.AIForward(2);
            AIcontrols.AICharges();
            character.SetBackPressed(true);
            decisionTimer = 0f;
        }
        else if (decisionBox[decision] == "Strong")
        {
            AIcontrols.AIStrong(10);
            AIcontrols.AICharges();
            character.SetBackPressed(true);
            decisionTimer = 0f;
        }
        else if (decisionBox[decision] == "Short")
        {
            AIcontrols.AIShort(40);
            AIcontrols.AICharges();
            character.SetBackPressed(true);
            decisionTimer = 0f;
        }
        else if (decisionBox[decision] == "Jab")
        {
            AIcontrols.AIJab(50);
            AIcontrols.AICharges();
            character.SetBackPressed(true);
            decisionTimer = 0f;
        }
        else if (decisionBox[decision] == "LowForward")
        {
            AIcontrols.AILowForward();
            AIcontrols.AICharges();
            character.SetBackPressed(true);
            decisionTimer = 0f;
        }
        else if (decisionBox[decision] == "Sweep")
        {
            AIcontrols.AISweep();
            AIcontrols.AICharges();
            character.SetBackPressed(true);
            decisionTimer = 0f;
        }
        else if (decisionBox[decision] == "ScissorKick")
        {
            if (chargeSystem.GetBackCharged() && !animator.GetBool("isAttacking"))
            {
                AIScissorKicks();
                AIcontrols.AICharges();
                character.SetBackPressed(true);
            }
            else
            {
                AIcontrols.AIForward(2);
                AIcontrols.AICharges();
                character.SetBackPressed(true);
            }
            decisionTimer = 0f;
        }
        else if (decisionBox[decision] == "PsychoCrusher")
        {
            if (chargeSystem.GetDownCharged() && !animator.GetBool("isAttacking"))
            {
                AIPsychoCrushers();
                AIcontrols.AICharges();
                character.SetBackPressed(true);
            }
            else
            {
                AILightAttacks(50, 1);
                AIcontrols.AICharges();
                character.SetBackPressed(true);
            }
            decisionTimer = 0f;
        }
        else if (decisionBox[decision] == "HeadStomp")
        {
            if (chargeSystem.GetDownCharged() && !animator.GetBool("isAttacking"))
            {
                AIHeadStomps();
                AIcontrols.AICharges();
                character.SetBackPressed(true);
            }
            else
            {
                AILightAttacks(50, 1);
                AIcontrols.AICharges();
                character.SetBackPressed(true);
            }
            decisionTimer = 0f;
        }
        else if (decisionBox[decision] == "DevilsReverse")
        {
            if (chargeSystem.GetDownCharged() && !animator.GetBool("isAttacking"))
            {
                AIDevilsReverses();
                AIcontrols.AICharges();
                character.SetBackPressed(true);
            }
            else
            {
                AILightAttacks(50, 0);
                AIcontrols.AICharges();
                character.SetBackPressed(true);
            }
            decisionTimer = 0f;
        }
        else if (decisionBox[decision] == "SpecialCancelScissorKick")
        {
            if (chargeSystem.GetBackCharged())
            {
                AIScissorKicks();
                AIcontrols.AICharges();
                character.SetBackPressed(true);
            }
            else
            {
                AIcontrols.AIForward(2);
                AIcontrols.AICharges();
                character.SetBackPressed(true);
            }
            decisionTimer = 0f;
        }
        else if (decisionBox[decision] == "SpecialCancelPsychoCrusher")
        {
            if (chargeSystem.GetBackCharged())
            {
                AIPsychoCrushers();
                AIcontrols.AICharges();
                character.SetBackPressed(true);
            }
            else
            {
                AILightAttacks(50, 1);
                AIcontrols.AICharges();
                character.SetBackPressed(true);
            }
            decisionTimer = 0f;
        }
        else if (decisionBox[decision] == "SpecialCancelHeadStomp")
        {
            if (chargeSystem.GetDownCharged())
            {
                AIHeadStomps();
                AIcontrols.AICharges();
                character.SetBackPressed(true);
            }
            else
            {
                AILightAttacks(50, 1);
                AIcontrols.AICharges();
                character.SetBackPressed(true);
            }
            decisionTimer = 0f;
        }
        else if (decisionBox[decision] == "SpecialCancelDevilsReverse")
        {
            if (chargeSystem.GetDownCharged())
            {
                AIDevilsReverses();
                AIcontrols.AICharges();
                character.SetBackPressed(true);
            }
            else
            {
                AILightAttacks(50, 0);
                AIcontrols.AICharges();
                character.SetBackPressed(true);
            }
            decisionTimer = 0f;
        }
        else if (decisionBox[decision] == "Super")
        {
            if (character.GetSuper >= 100f)
            {
                AIKneePressNightmares();
                AIcontrols.AICharges();
                character.SetBackPressed(true);
            }
            else
            {
                AIcontrols.AICrouch();
                AIcontrols.AICharges();
                character.SetBackPressed(true);
            }
        }
        else if (decisionBox[decision] == "SuperOnBlock")
        {
            if (character.GetSuper >= 100f)
            {
                AIKneePressNightmares();
                AIcontrols.AICharges();
                character.SetBackPressed(true);
            }
            else
            {
                AIcontrols.AIForward(3);
                AIcontrols.AICharges();
                character.SetBackPressed(true);
            }
        }
        else if (decisionBox[decision] == "SuperOnHit")
        {
            if (character.GetSuper >= 100f)
            {
                AIKneePressNightmares();
                AIcontrols.AICharges();
                character.SetBackPressed(true);
            }
            else
            {
                AIcontrols.AIForward(10);
                AIcontrols.AICharges();
                character.SetBackPressed(true);
            }
        }
        else if (decisionBox[decision] == "Throw")
        {
            AIcontrols.AIThrow();
            AIcontrols.AICharges();
            character.SetBackPressed(true);
        }
        else if (decisionBox[decision] == "AirStrong")
        {
            AIcontrols.AIJumpStrong();
            AIcontrols.AICharges();
            character.SetBackPressed(true);
        }
        else if (decisionBox[decision] == "AirForward")
        {
            AIcontrols.AIJumpForwardAttack();
            AIcontrols.AICharges();
            character.SetBackPressed(true);
        }
        else if (decisionBox[decision] == "AirFierce")
        {
            AIcontrols.AIJumpFierce();
            AIcontrols.AICharges();
            character.SetBackPressed(true);
        }
        else if (decisionBox[decision] == "AirRoundhouse")
        {
            AIcontrols.AIJumpRoundhouse();
            AIcontrols.AICharges();
            character.SetBackPressed(true);
        }
    }

    void AIHeadStomps()
    {
        int randNum = Random.Range(0, 3);
        if (AIcontrols.GetConditionsSpecialAttack())
        {
            animator.SetTrigger("headStompInputed");
            if (animator.GetBool("isAttacking") == false)
            {
                character.AttackState();
                animator.Play("MBisonHeadStomp", 0);
                switch (randNum)
                {
                    case 0: 
                        animator.SetInteger("headStompKickType", 0);
                        break;
                    case 1: 
                        animator.SetInteger("headStompKickType", 1);
                        break;
                    default: 
                        animator.SetInteger("headStompKickType", 2);
                        break;
                }                    
                chargeSystem.SetDownCharged(false);
                chargeSystem.ResetDownChargedProperties();
                chargeSystem.ResetDownCharged();
            }
        }
    }    

    void AIDevilsReverses()
    {
        int randNum = Random.Range(0, 3);
        if (AIcontrols.GetConditionsSpecialAttack())
        {
            animator.SetTrigger("headStompInputed");
            if (animator.GetBool("isAttacking") == false)
            {
                character.AttackState();
                animator.Play("MBisonDevilReverse", 0);
                chargeSystem.SetDownCharged(false);
                chargeSystem.ResetDownChargedProperties();
                chargeSystem.ResetDownCharged();
            }
        }
    }

    void SomerSaultDecisions()
    {
        int somerSaultDecision = Random.Range(0, 100);
        if (somerSaultDecision < 3)
        {
            animator.SetTrigger("somerSaultInputed");
        }
    }

    void AIScissorKicks()
    {
        int randNum = Random.Range(0, 3);
        if (AIcontrols.GetConditionsSpecialAttack())
        {
            animator.SetTrigger("scissorKicksInputed");
            if (animator.GetBool("isAttacking") == false)
            {
                character.AttackState();
                switch (randNum)
                {
                    case 0:
                        animator.Play("MBisonScissorKicksShort", 0);
                        animator.SetInteger("scissorKicksKickType", 0);
                        break;
                    case 1:
                        animator.Play("MBisonScissorKicksForward", 0);
                        animator.SetInteger("scissorKicksKickType", 1);
                        break;
                    default:
                        animator.Play("MBisonScissorKicksRoundhouse", 0);
                        animator.SetInteger("scissorKicksKickType", 2);
                        break;
                }
                chargeSystem.SetBackCharged(false);
                chargeSystem.ResetBackChargedProperties();
                chargeSystem.ResetBackCharged();
            }
        }
    }

    void AIPsychoCrushers()
    {
        int randNum = Random.Range(0, 3);
        if (AIcontrols.GetConditionsSpecialAttack())
        {
            animator.SetTrigger("psychoCrusherInputed");
            if (animator.GetBool("isAttacking") == false)
            {
                character.AttackState();
                animator.Play("MBisonPsychoCrusherStartUp", 0);
                switch (randNum)
                {
                    case 0:
                        animator.SetInteger("psychoCrusherPunchType", 0);
                        break;
                    case 1:
                        animator.SetInteger("psychoCrusherPunchType", 1);
                        break;
                    default:
                        animator.SetInteger("psychoCrusherPunchType", 2);
                        break;
                }
                chargeSystem.SetBackCharged(false);
                chargeSystem.ResetBackChargedProperties();
                chargeSystem.ResetBackCharged();
            }
        }
    }

    void AIKneePressNightmares()
    {
        if (AIcontrols.GetConditionsSpecialAttack())
        {
            animator.SetTrigger("motionSuperInputed");
            if (animator.GetBool("isAttacking") == false)
            {
                character.AttackState();
                animator.Play("MBisonKneePressNightmare", 0);
            }
        }
    }

    void AILightAttacks(int maxNum, int whichLightAttack)
    {
        int crouchOrStand = Random.Range(0, maxNum);
        if (AIcontrols.GetConditions())
        {
            character.AttackState();
            if (crouchOrStand == 0)
            {
                AIcontrols.AIStand();
            }
            else
            {
                AIcontrols.AICrouch();
            }
            if (whichLightAttack == 0)
            {
                character.CharacterJab();
            }
            else
            {
                character.CharacterShort();
            }
        }
    }

    void InsertNewAction(List<string> decisionTree, string action, int numActions)
    {
        for (int i = 0; i < numActions; i++)
        {
            decisionTree.Add(action);
        }
    }

    void DecisionMade(int minDivisor, int maxDivisor)
    {
        if (decisionTimer <= 0)
        {
            decision = Random.Range(0, DECISION_MAX);
            decisionTimer = Random.Range(decisionTimerInput / minDivisor, decisionTimerInput / maxDivisor);
        }
    }

    private int normalizedDecisions(int numInBox, int sum)
    {
        return Mathf.RoundToInt(((float)numInBox / sum) * 100f);
    }

    private int NormalizedDifference(int beneficaryDecision, int sum)
    {
        int normalizedDifference = DECISION_MAX - sum;

        if (normalizedDifference != 0)
        {
            beneficaryDecision += normalizedDifference;
        }

        return beneficaryDecision;
    }
}
