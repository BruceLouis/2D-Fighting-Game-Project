using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour {
	
	private bool hurtBoxCollided;
	
	void Update (){		
		foreach(Transform child in transform){
			child.gameObject.tag = transform.gameObject.tag;
		}
		hurtBoxCollided = false;
	}
	
	public bool GetHurtBoxCollided(){
		return hurtBoxCollided;
	}
	
	public void SetHurtBoxCollided(bool didItCollide){
		hurtBoxCollided = didItCollide;
	}
}
