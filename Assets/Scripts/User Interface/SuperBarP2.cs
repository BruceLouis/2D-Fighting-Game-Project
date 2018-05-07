using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuperBarP2 : MonoBehaviour {

	public float timeBetweenFlashes;
	
	private Image[] fill;
	private Slider slider;
	private float maxSuper, sliderValue;
	private bool isFlashing;
	private Color defaultColor;
	
	// Use this for initialization
	void Start () {
		slider = GetComponent<Slider>();
		fill = GetComponentsInChildren<Image>();
		defaultColor = fill[1].color;
		maxSuper = 100f;
	}
	
	void Update () {
		if (slider.value >= slider.maxValue && !isFlashing){
			StartCoroutine(SuperBarFlash(timeBetweenFlashes));
		}
	}
	
	public void SetSuper(float currentSuper){
		sliderValue = currentSuper/maxSuper;
		slider.value = sliderValue;
	}
	
	IEnumerator SuperBarFlash(float time){
		isFlashing = true;
		fill[1].color = Color.clear;
		yield return new WaitForSecondsRealtime(time*0.5f);
		fill[1].color = defaultColor;
		yield return new WaitForSecondsRealtime(time);
		isFlashing = false;
	}
}
