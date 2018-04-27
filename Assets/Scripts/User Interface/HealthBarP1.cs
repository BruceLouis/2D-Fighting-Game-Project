using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBarP1 : MonoBehaviour {

	private Slider slider;
	private float maxHealth;
	private float sliderValue;
	private bool settingMaxHealth;	

	// Use this for initialization
	void Start () {
		slider = GetComponent<Slider>();
		settingMaxHealth = true;
	}
	
	public void SetHealth(float currentHealth){
		if (settingMaxHealth){
			maxHealth = currentHealth;
			settingMaxHealth = false;
		}
		sliderValue = currentHealth/maxHealth;
		slider.value = sliderValue;
	}
}