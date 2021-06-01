using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ConductivityController : KiwriousController {

    void Update()
    {
        sensor_value.enabled = KiwriousSerialReader.instance.conductivity.isOnline;
        sensor_graphic.enabled = KiwriousSerialReader.instance.conductivity.isOnline;
        if (sensor_value.enabled)
        {
            sensor_value.text = KiwriousSerialReader.instance.conductivity.values["Conductivity"].ToString("F1");
        }
    }

}
