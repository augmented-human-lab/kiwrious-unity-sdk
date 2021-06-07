using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kiwrious;

public class PluginTest : MonoBehaviour {

	public List<string> sensorIds = new List<string>();

	public float conductivity;

	// Use this for initialization
	void Start () {
		//KiwriousReader.Instance.StartReader();
		
	}
	
	// Update is called once per frame
	void Update () {
		//conductivity = KiwriousReader.Instance.conductivity;
	}
}
