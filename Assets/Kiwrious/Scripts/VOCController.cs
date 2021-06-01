using System.Collections;
using System.Linq;
using UnityEngine;

public class VOCController : KiwriousController {

    void Update () {
		sensor_value.enabled = KiwriousSerialReader.instance.voc.isOnline;
		sensor_graphic.enabled = KiwriousSerialReader.instance.voc.isOnline;
		if (sensor_value.enabled) {
			sensor_value.text = KiwriousSerialReader.instance.voc.values["VOC"].ToString("F0");
		}
	}

}
