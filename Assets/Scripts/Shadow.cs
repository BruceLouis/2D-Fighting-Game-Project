using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour {

	private Character character;
	private float originalY;
	// Use this for initialization
	void Start () {
		character = GetComponentInParent<Character>();
		originalY = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3(character.transform.position.x, originalY, 0f);
	}
}
