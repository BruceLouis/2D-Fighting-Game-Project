using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour {
	
	void Update (){		
		foreach(Transform child in transform){
			child.gameObject.tag = transform.gameObject.tag;
		}
	}

}
