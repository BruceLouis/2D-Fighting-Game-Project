using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour {

	public Sprite[] backgrounds, foregrounds;
	public ParticleSystem mist1, mist2;
	
	private int randNum;
	private SpriteRenderer[] spriteRenderers;
	// Use this for initialization
	void Start () {
		randNum = Random.Range (0, backgrounds.Length);
		spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
		spriteRenderers[0].sprite = foregrounds[randNum];
		spriteRenderers[1].sprite = backgrounds[randNum];
		if (randNum == 0){
			Instantiate(mist1, new Vector3(-1.5f, 0.5f, 0f), Quaternion.Euler(new Vector3(0f, -100.7f, 0f)));
			Instantiate(mist2, new Vector3(3f, 0.75f, 0f), Quaternion.Euler(new Vector3(0f, -100.7f, 0f)));
		}
	}	
}
