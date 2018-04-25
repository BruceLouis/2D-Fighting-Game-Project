using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushBox : MonoBehaviour {
	
	private Animator animator;

	// Use this for initialization
	void Start () {
		animator = GetComponentInParent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		if (gameObject.tag == "Player1"){
			
			if (animator.GetBool("isAirborne") == true || animator.GetBool("isLiftingOff") == true || animator.GetBool("isRolling") == true){
				gameObject.layer = LayerMask.NameToLayer("IgnorePlayer2");
			}
			else{
				gameObject.layer = LayerMask.NameToLayer("PushBoxP1");
			}
		}
		else if (gameObject.tag == "Player2"){
			
			if (animator.GetBool("isAirborne") == true || animator.GetBool("isLiftingOff") == true || animator.GetBool("isRolling") == true){
				gameObject.layer = LayerMask.NameToLayer("IgnorePlayer1");
			}
			else{
				gameObject.layer = LayerMask.NameToLayer("PushBoxP2");	
			}
		}				
		
	}
}
