using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KiwriousController : MonoBehaviour {

	public bool isSensorOnline;

	public Text sensor_value;
	public Image sensor_graphic;

	void Awake () {
		sensor_value = GetComponentInChildren<Text>();
		sensor_graphic = GetComponent<Image>();
	}
	
}
