using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperEffect : MonoBehaviour {

	ParticleSystem theParticleSystem;
	
	void Start () {
		theParticleSystem = GetComponent<ParticleSystem>();
	}

	// Update is called once per frame
	void Update () {
		if (!theParticleSystem.isEmitting){
			if (gameObject.tag == "Player1"){
				TimeControl.inSuperStartup[0] = false;
			}
			else if (gameObject.tag == "Player2"){
				TimeControl.inSuperStartup[1] = false;
			}			
		}
	}
}
