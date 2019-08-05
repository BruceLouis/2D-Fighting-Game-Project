using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

	public static float offset = 0.5f; // space between screen border and nearest fighter
	public static float leftBGEdge = -4.25f;
	public static float rightBGEdge = 4.25f;

    private const float CHARACTER_DISTANCE = 3.8f;
    private float leftEdge = -2.25f;
	private float rightEdge = 2.25f;	
	private float wScene; // scene width
	private Player player; // fighter1 transform
	private Opponent opponent; // fighter2 transform
	private Character playerCharacter;
	private Character opponentCharacter;
	private float xL; // left screen X coordinate
	private float xR; // right screen X coordinate
    private float difference;

	void Start(){
		// find references to the fighters
		player = GameObject.FindObjectOfType<Player>();    
		opponent = GameObject.FindObjectOfType<Opponent>();
		
		playerCharacter = player.GetComponentInChildren<Character>();	
		opponentCharacter = opponent.GetComponentInChildren<Character>();	
		// initializes scene size and camera distance
		calcScreen(playerCharacter, opponentCharacter);
	}
	
	void Update(){
		calcScreen(playerCharacter, opponentCharacter);
        difference = Mathf.Abs(playerCharacter.transform.position.x - opponentCharacter.transform.position.x);
		// centers the camera
        if (difference <= CHARACTER_DISTANCE)
        {
            transform.position = new Vector3 ((xR+xL)/2, transform.position.y, transform.position.z);
		    float newX = Mathf.Clamp(transform.position.x, leftEdge, rightEdge);
		    transform.position = new Vector3 (newX, transform.position.y, transform.position.z);
        }		
    }

    void calcScreen(Character p1, Character p2)
    {
        // Calculates the xL and xR screen coordinates 
        if (p1.transform.position.x < p2.transform.position.x)
        {
            xL = p1.transform.position.x - offset;
            xR = p2.transform.position.x + offset;
        }
        else
        {
            xL = p2.transform.position.x - offset;
            xR = p1.transform.position.x + offset;
        }
    }

}
