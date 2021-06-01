﻿
using System.Collections;
using System.Linq;
using UnityEngine;

public class LuxController : KiwriousController {

	void Update () {
		sensor_value.enabled = KiwriousSerialReader.instance.uv_lux.isOnline;
		sensor_graphic.enabled = KiwriousSerialReader.instance.uv_lux.isOnline; 
		if (sensor_value.enabled)
		{
			sensor_value.text = KiwriousSerialReader.instance.uv_lux.values["Lux"].ToString("F0");
		}
	}

}
