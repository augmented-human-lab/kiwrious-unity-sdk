using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PluginTest : MonoBehaviour {

	public List<string> sensorIds = new List<string>();

	public float conductivity;

	// Use this for initialization
	void Start () {
		//IKiwriousReader.Instance.StartReader();
		
	}
	
	// Update is called once per frame
	void Update () {
		//conductivity = IKiwriousReader.Instance.conductivity;
	}
}
