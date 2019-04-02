using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperEffect : MonoBehaviour {

	ParticleSystem theParticleSystem;
    ParticleSystem[] childrenParticleSystems;
	
	void Start () {
		theParticleSystem = GetComponent<ParticleSystem>();
        childrenParticleSystems = GetComponentsInChildren<ParticleSystem>();
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
        if (theParticleSystem.isEmitting)
        {
            if (TimeControl.paused)
            {
                foreach (ParticleSystem item in childrenParticleSystems)
                {
                    ParticleSystem.MainModule itemMainModule = item.main;
                    itemMainModule.useUnscaledTime = false;
                }
            }
            else
            {
                foreach (ParticleSystem item in childrenParticleSystems)
                {
                    ParticleSystem.MainModule itemMainModule = item.main;
                    itemMainModule.useUnscaledTime = true;
                }
            }
        }
	}
}
