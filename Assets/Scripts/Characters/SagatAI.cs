using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SagatAI : MonoBehaviour
{

    [SerializeField] float decisionTimer, antiAirTimer;

    private Animator animator;
    private Player player, playerController;
    private Opponent opponent, opponentController;
    private Character playerCharacter, opponentCharacter;
    private Character character;
    private SharedProperties sharedProperties;
    private AIControls AIcontrols;

    private int decision;
    private float decisionTimerInput, antiAirTimerInput;

    // Use this for initialization
    void Start()
    {
        character = GetComponent<Character>();
        animator = GetComponent<Animator>();
        AIcontrols = GetComponentInParent<AIControls>();
        sharedProperties = GetComponentInParent<SharedProperties>();

        if (GetComponentInParent<Opponent>() != null)
        {
            player = FindObjectOfType<Player>();
            playerCharacter = player.GetComponentInChildren<Character>();
            opponentController = GetComponentInParent<Opponent>();
            animator.SetInteger("tigerShotOwner", 2);
        }
        else if (GetComponentInParent<Player>() != null)
        {
            opponent = FindObjectOfType<Opponent>();
            opponentCharacter = opponent.GetComponentInChildren<Character>();
            playerController = GetComponentInParent<Player>();
            animator.SetInteger("tigerShotOwner", 1);
        }

        decisionTimerInput = decisionTimer;
        antiAirTimerInput = antiAirTimer;
        antiAirTimer = 0f;
        decision = Random.Range(0, 100);
    }
    
    public void Behaviors()
    {
        decisionTimer--;
        antiAirTimer--;
        if (AIcontrols.FreeToMakeDecisions() && !TimeControl.inSuperStartup[0] && !TimeControl.inSuperStartup[1])
        {
            if (animator.GetBool("isAirborne") == true && animator.GetBool("isLiftingOff") == false)
            {
                decision = Random.Range(0, 100);
                if (decision <= 3)
                {
                    AIcontrols.AIJumpFierce();
                    sharedProperties.CharacterNeutralState();
                }
                else if (decision <= 6 && decision > 3)
                {
                    AIcontrols.AIJumpRoundhouse();
                    sharedProperties.CharacterNeutralState();
                }
            }
            else if (sharedProperties.GetAbDistanceFromOtherFighter() >= 2f)
            {
                RegularFarRangeDecisions();
            }
            else if (sharedProperties.GetAbDistanceFromOtherFighter() >= 1f && sharedProperties.GetAbDistanceFromOtherFighter() < 2f)
            {
                RegularMidRangeDecisions();
            }
            else
            {
                if (player != null)
                {
                    //anti air
                    if (playerCharacter.GetAirborne() == true && playerCharacter.GetKnockDown() == false && playerCharacter.GetThrown() == false)
                    {
                        if (antiAirTimer <= 0f)
                        {
                            sharedProperties.AIAntiAirDecision(67, RegularCloseRangeDecisions, PreparationForAntiAir);
                            antiAirTimer = antiAirTimerInput;
                        }
                        else
                        {
                            RegularCloseRangeDecisions();
                        }
                    }
                    else if (playerCharacter.GetHitStunned() == true)
                    {
                        OtherFighterGotHitDecisions();
                    }
                    else if (playerCharacter.GetBlockStunned() == true)
                    {
                        CloseRangeOtherFighterBlockedDecisions();
                    }
                    else
                    {
                        RegularCloseRangeDecisions();
                    }
                }
                else if (opponent != null)
                {
                    //anti air
                    if (opponentCharacter.GetAirborne() == true && opponentCharacter.GetKnockDown() == false && opponentCharacter.GetThrown() == false)
                    {
                        if (antiAirTimer <= 0f)
                        {
                            sharedProperties.AIAntiAirDecision(67, RegularCloseRangeDecisions, PreparationForAntiAir);
                            antiAirTimer = antiAirTimerInput;
                        }
                        else
                        {
                            RegularCloseRangeDecisions();
                        }
                    }
                    else if (opponentCharacter.GetHitStunned() == true)
                    {
                        OtherFighterGotHitDecisions();
                    }
                    else if (opponentCharacter.GetBlockStunned() == true)
                    {
                        CloseRangeOtherFighterBlockedDecisions();
                    }
                    else
                    {
                        RegularCloseRangeDecisions();
                    }
                }
            }
            AIcontrols.AIWalks();
        }
    }

    void RegularFarRangeDecisions()
    {
        DecisionMade(5, 1);
        if (decision <= 10)
        {
            AIcontrols.AIPressedForward();
            character.SetBackPressed(false);
        }
        else if (decision <= 15 && decision > 10)
        {
            AIcontrols.AIPressedBackward();
            character.SetBackPressed(true);
        }
        else if (decision <= 25 && decision > 15)
        {
            AITigerKnees();
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
            decisionTimer = 0f;
        }
        else if (decision <= 45 && decision > 25)
        {
            if (playerController != null)
            {
                if (playerController.GetProjectileP1Parent().transform.childCount <= 0)
                {
                    AITigerShots("Upper", 6);
                    AIcontrols.DoesAIBlock();
                    sharedProperties.CharacterNeutralState();
                    decisionTimer = 0f;
                }
                else
                {
                    AIcontrols.AIJab(2);
                    sharedProperties.CharacterNeutralState();
                    AIcontrols.DoesAIBlock();
                }
            }
            else if (opponentController != null)
            {
                if (opponentController.GetProjectileP2Parent().transform.childCount <= 0)
                {
                    AITigerShots("Upper", 6);
                    AIcontrols.DoesAIBlock();
                    sharedProperties.CharacterNeutralState();
                    decisionTimer = 0f;
                }
                else
                {
                    AIcontrols.AIJab(2);
                    sharedProperties.CharacterNeutralState();
                    AIcontrols.DoesAIBlock();
                }
            }
        }
        else
        {
            if (playerController != null)
            {
                if (playerController.GetProjectileP1Parent().transform.childCount <= 0)
                {
                    AITigerShots("Lower", 10);
                    AIcontrols.DoesAIBlock();
                    sharedProperties.CharacterNeutralState();
                    decisionTimer = 0f;
                }
                else
                {
                    AIcontrols.AICrouch();
                    AIcontrols.AIPressedBackward();
                    character.SetBackPressed(true);
                }
            }
            else if (opponentController != null)
            {
                if (opponentController.GetProjectileP2Parent().transform.childCount <= 0)
                {
                    AITigerShots("Lower", 10);
                    AIcontrols.DoesAIBlock();
                    sharedProperties.CharacterNeutralState();
                    decisionTimer = 0f;
                }
                else
                {
                    AIcontrols.AICrouch();
                    AIcontrols.AIPressedBackward();
                    character.SetBackPressed(true);
                }
            }
        }
    }

    void RegularMidRangeDecisions()
    {
        DecisionMade(5, 1);
        if (decision <= 10)
        {
            AIcontrols.AIPressedForward();
            character.SetBackPressed(false);
        }
        else if (decision <= 15)
        {
            AIcontrols.AIPressedBackward();
            character.SetBackPressed(true);
        }
        else if (decision <= 25)
        {
            AITigerKnees();
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
            decisionTimer = 0f;
        }
        else if (decision <= 27)
        {
            AIcontrols.AIJab(2);
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
            decisionTimer = 0f;
        }
        else if (decision <= 29)
        {
            if (character.GetSuper >= 100f)
            {
                AITigerCannons();
            }
            else
            {
                AIcontrols.AIShort(2);
            }
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
            decisionTimer = 0f;
        }
        else if (decision <= 33)
        {
            AIcontrols.AIRoundhouse(1, 10);
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
            decisionTimer = 0f;
        }
        else if (decision <= 35)
        {
            if (character.GetSuper >= 100f)
            {
                AITigerCannons();
            }
            else
            {
                AIcontrols.AIFierce(2, 1);
            }
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
            decisionTimer = 0f;
        }
        else if (decision <= 37)
        {
            if (character.GetSuper >= 100f)
            {
                AITigerCannons();
            }
            else
            {
                AIcontrols.AIForward(2);
            }
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
            decisionTimer = 0f;
        }
        else if (decision <= 39)
        {
            if (character.GetSuper >= 100f)
            {
                AITigerCannons();
            }
            else
            {
                AIcontrols.AIStrong(40);
            }
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
            decisionTimer = 0f;
        }
        else if (decision <= 41)
        {
            AIcontrols.AIPressedForward();
            AIcontrols.AIJump();
            character.SetBackPressed(false);
        }
        else if (decision <= 44)
        {
            sharedProperties.CharacterNeutralState();
            AIcontrols.AIJump();
            character.SetBackPressed(false);
        }
        else if (decision <= 45)
        {
            AIcontrols.AIPressedBackward();
            AIcontrols.AIJump();
            character.SetBackPressed(true);
        }

        else if (decision <= 60)
        {
            if (playerController != null)
            {
                if (playerController.GetProjectileP1Parent().transform.childCount <= 0)
                {
                    AITigerShots("Upper", 6);
                    AIcontrols.DoesAIBlock();
                    sharedProperties.CharacterNeutralState();
                    decisionTimer = 0f;
                }
                else
                {
                    AIcontrols.AIJab(2);
                    sharedProperties.CharacterNeutralState();
                    AIcontrols.DoesAIBlock();
                }
            }
            else if (opponentController != null)
            {
                if (opponentController.GetProjectileP2Parent().transform.childCount <= 0)
                {
                    AITigerShots("Upper", 6);
                    AIcontrols.DoesAIBlock();
                    sharedProperties.CharacterNeutralState();
                    decisionTimer = 0f;
                }
                else
                {
                    AIcontrols.AIJab(2);
                    sharedProperties.CharacterNeutralState();
                    AIcontrols.DoesAIBlock();
                }
            }
        }
        else
        {
            if (playerController != null)
            {
                if (playerController.GetProjectileP1Parent().transform.childCount <= 0)
                {
                    AITigerShots("Lower", 10);
                    AIcontrols.DoesAIBlock();
                    sharedProperties.CharacterNeutralState();
                    decisionTimer = 0f;
                }
                else
                {
                    AIcontrols.AICrouch();
                    AIcontrols.AIPressedBackward();
                    character.SetBackPressed(true);
                }
            }
            else if (opponentController != null)
            {
                if (opponentController.GetProjectileP2Parent().transform.childCount <= 0)
                {
                    AITigerShots("Lower", 10);
                    AIcontrols.DoesAIBlock();
                    sharedProperties.CharacterNeutralState();
                    decisionTimer = 0f;
                }
                else
                {
                    AIcontrols.AICrouch();
                    AIcontrols.AIPressedBackward();
                    character.SetBackPressed(true);
                }
            }
        }
    }

    void RegularCloseRangeDecisions()
    {
        DecisionMade(5, 1);
        if (decision <= 10)
        {
            AIcontrols.AIPressedForward();
            character.SetBackPressed(false);
        }
        else if (decision <= 15)
        {
            AIcontrols.AIPressedBackward();
            character.SetBackPressed(true);
        }
        else if (decision <= 35)
        {
            AIcontrols.AICrouch();
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
        }
        else if (decision <= 42)
        {
            AIcontrols.AIJab(4);
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
            decisionTimer = 0f;
        }
        else if (decision <= 49)
        {
            AIcontrols.AIShort(2);
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
            decisionTimer = 0f;
        }
        else if (decision <= 54)
        {
            AIcontrols.AIRoundhouse(1, 10);
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
            decisionTimer = 0f;
        }
        else if (decision <= 57)
        {
            AIcontrols.AIFierce(2, 1);
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
            decisionTimer = 0f;
        }

        else if (decision <= 60)
        {
            AIcontrols.AIThrow();
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
            decisionTimer = 0f;
        }
        else if (decision <= 67)
        {
            AIcontrols.AIForward(4);
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
            decisionTimer = 0f;
        }
        else if (decision <= 74)
        {
            AIcontrols.AIStrong(40);
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
            decisionTimer = 0f;
        }
        else if (decision <= 76)
        {
            AIcontrols.AIPressedForward();
            AIcontrols.AIJump();
            character.SetBackPressed(false);
            decisionTimer = 0f;
        }
        else if (decision <= 79)
        {
            sharedProperties.CharacterNeutralState();
            AIcontrols.AIJump();
            character.SetBackPressed(false);
            decisionTimer = 0f;
        }
        else if (decision <= 81)
        {
            AIcontrols.AIPressedBackward();
            AIcontrols.AIJump();
            character.SetBackPressed(true);
            decisionTimer = 0f;
        }
        else if (decision <= 85 && !animator.GetBool("isAttacking"))
        {
            AITigerKnees();
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
            decisionTimer = 0f;
        }
        else if (decision <= 90 && !animator.GetBool("isAttacking"))
        {
            AITigerUppercuts();
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
            decisionTimer = 0f;
        }
        else if (decision <= 93)
        {
            if (playerController != null)
            {
                if (playerController.GetProjectileP1Parent().transform.childCount <= 0 && !animator.GetBool("isAttacking"))
                {
                    AITigerShots("Upper", 6);
                    AIcontrols.DoesAIBlock();
                    sharedProperties.CharacterNeutralState();
                    decisionTimer = 0f;
                }
                else
                {
                    AIcontrols.AIJab(2);
                    sharedProperties.CharacterNeutralState();
                    AIcontrols.DoesAIBlock();
                }
            }
            else if (opponentController != null)
            {
                if (opponentController.GetProjectileP2Parent().transform.childCount <= 0 && !animator.GetBool("isAttacking"))
                {
                    AITigerShots("Upper", 6);
                    AIcontrols.DoesAIBlock();
                    sharedProperties.CharacterNeutralState();
                    decisionTimer = 0f;
                }
                else
                {
                    AIcontrols.AIJab(2);
                    sharedProperties.CharacterNeutralState();
                    AIcontrols.DoesAIBlock();
                }
            }
        }
        else
        {
            if (playerController != null)
            {
                if (playerController.GetProjectileP1Parent().transform.childCount <= 0 && !animator.GetBool("isAttacking"))
                {
                    AITigerShots("Lower", 10);
                    AIcontrols.DoesAIBlock();
                    sharedProperties.CharacterNeutralState();
                    decisionTimer = 0f;
                }
                else
                {
                    AIcontrols.AICrouch();
                    AIcontrols.AIPressedBackward();
                    character.SetBackPressed(true);
                }
            }
            else if (opponentController != null)
            {
                if (opponentController.GetProjectileP2Parent().transform.childCount <= 0 && !animator.GetBool("isAttacking"))
                {
                    AITigerShots("Lower", 10);
                    AIcontrols.DoesAIBlock();
                    sharedProperties.CharacterNeutralState();
                    decisionTimer = 0f;
                }
                else
                {
                    AIcontrols.AICrouch();
                    AIcontrols.AIPressedBackward();
                    character.SetBackPressed(true);
                }
            }
        }
    }

    void PreparationForAntiAir()
    {
        decision = Random.Range(0, 100);
        if (decision <= 50)
        {
            AITigerUppercuts();
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
        }
        else if (decision <= 75)
        {
            AITigerKnees();
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
        }
        else if (decision <= 90)
        {
            if (character.GetSuper >= 100f)
            {
                AITigerCannons();
            }
            else
            {
                AIcontrols.AIRoundhouse(0, 70);
            }
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
        }
        else
        {
            AIcontrols.AIStrong(0);
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
        }
    }

    void CloseRangeOtherFighterBlockedDecisions()
    {
        decision = Random.Range(0, 100);
        if (decision <= 10)
        {
            if (playerController != null)
            {
                if (playerController.GetProjectileP1Parent().transform.childCount <= 0)
                {
                    AITigerShots("Lower", 10);
                    AIcontrols.DoesAIBlock();
                    sharedProperties.CharacterNeutralState();
                    decisionTimer = 0f;
                }
                else
                {
                    AIcontrols.AIJab(2);
                    sharedProperties.CharacterNeutralState();
                    AIcontrols.DoesAIBlock();
                }
            }
            else if (opponentController != null)
            {
                if (opponentController.GetProjectileP2Parent().transform.childCount <= 0)
                {
                    AITigerShots("Lower", 10);
                    AIcontrols.DoesAIBlock();
                    sharedProperties.CharacterNeutralState();
                    decisionTimer = 0f;
                }
                else
                {
                    AIcontrols.AIJab(2);
                    sharedProperties.CharacterNeutralState();
                    AIcontrols.DoesAIBlock();
                }
            }
        }
        else if (decision <= 30)
        {
            AIcontrols.AIForward(2);
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
        }
        else if (decision <= 20)
        {
            if (character.GetSuper >= 100f)
            {
                AITigerCannons();
            }
            else
            {
                AIcontrols.AIStrong(10);
            }
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
        }
        else if (decision <= 40)
        {
            AIcontrols.AIJab(2);
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
        }
        else if (decision <= 60)
        {
            AIcontrols.AIShort(10);
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
        }
        else if (decision <= 65)
        {
            AITigerKnees();
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
            decisionTimer = 0f;
        }
        else
        {
            AIcontrols.AICrouch();
            sharedProperties.CharacterNeutralState();
            character.SetBackPressed(true);
        }
    }

    void OtherFighterGotHitDecisions()
    {
        decision = Random.Range(0, 100);
        if (decision <= 30)
        {
            AITigerUppercuts();
            sharedProperties.CharacterNeutralState();
        }
        else if (decision <= 50 && decision > 30)
        {
            AITigerKnees();
            sharedProperties.CharacterNeutralState();
        }
        else if (decision <= 75 && decision > 50)
        {
            AIcontrols.AIForward(50);
            sharedProperties.CharacterNeutralState();
        }
        else
        {
            if (character.GetSuper >= 100f)
            {
                AITigerCannons();
            }
            else
            {
                AIcontrols.AIRoundhouse(0, 50);
            }
            sharedProperties.CharacterNeutralState();
        }
    }

    void AITigerShots(string whichTigerShot, int maxNum)
    {
        int randNum = Random.Range(0, maxNum);
        if (AIcontrols.GetConditionsSpecialAttack())
        {
            animator.SetTrigger(whichTigerShot.ToLower() + "TigerShotInputed");
            if (animator.GetBool("isAttacking") == false)
            {
                AIcontrols.AIStand();
                character.AttackState();
                animator.Play("Sagat" + whichTigerShot + "TigerShot", 0);
                switch (randNum)
                {
                    case 0:
                        animator.SetInteger(whichTigerShot.ToLower() + "TigerShotType", 0);
                        break;
                    case 1:
                        animator.SetInteger(whichTigerShot.ToLower() + "TigerShotType", 1);
                        break;
                    default:
                        animator.SetInteger(whichTigerShot.ToLower() + "TigerShotType", 2);
                        break;
                }
                
            }
        }
    }

    void AITigerKnees()
    {
        int tigerKneeType = Random.Range(0, 3);
        if (AIcontrols.GetConditionsSpecialAttack())
        {
            animator.SetTrigger("tigerKneeInputed");
            if (animator.GetBool("isAttacking") == false)
            {
                AIcontrols.AIStand();
                character.AttackState();
                animator.SetInteger("tigerKneeKickType", tigerKneeType);
                animator.Play("SagatTigerKnee", 0);
            }
        }
    }

    void AITigerUppercuts()
    {
        int tigerUppercutType = Random.Range(0, 3);
        if (AIcontrols.GetConditionsSpecialAttack())
        {
            animator.SetTrigger("tigerUppercutInputed");
            if (animator.GetBool("isAttacking") == false)
            {
                AIcontrols.AIStand();
                character.AttackState();
                switch (tigerUppercutType)
                {
                    case 0:
                        animator.Play("SagatTigerUppercutJab", 0);
                        animator.SetInteger("tigerUppercutPunchType", 0);
                        break;

                    case 1:
                        animator.Play("SagatTigerUppercutStrong", 0);
                        animator.SetInteger("tigerUppercutPunchType", 1);
                        break;

                    default:
                        animator.Play("SagatTigerUppercutFierce", 0);
                        animator.SetInteger("tigerUppercutPunchType", 2);
                        break;
                }
            }
        }
    }

    void AITigerCannons()
    {
        if (AIcontrols.GetConditionsSpecialAttack())
        {
            animator.SetTrigger("motionSuperInputed");
            if (animator.GetBool("isAttacking") == false)
            {
                AIcontrols.AIStand();
                character.AttackState();
                animator.Play("SagatTigerCannon", 0);
            }
        }
    }

    void DoesSagatBlock(int maxNum)
    {
        int coinflip = Random.Range(0, maxNum);
        if (coinflip >= 2)
        {
            character.SetBackPressed(false);
        }
        else
        {
            character.SetBackPressed(true);
        }
    }

    void DecisionMade(int minDivisor, int maxDivisor)
    {
        if (decisionTimer <= 0)
        {
            decision = Random.Range(0, 100);
            decisionTimer = Random.Range(decisionTimerInput / minDivisor, decisionTimerInput / maxDivisor);
        }
    }
}
