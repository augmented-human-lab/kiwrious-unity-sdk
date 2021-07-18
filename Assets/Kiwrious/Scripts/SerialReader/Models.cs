using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Kiwrious.Scripts
{
	[Serializable]
	public class SensorData
	{
		public bool isOnline { get; set; } // to be removed, use status
		public int status { get; set; }
		public Dictionary<string, float> values { get; set; }
	}

	[Serializable]
	public class KiwriousSensor
	{
		public KiwriousSensor(string name, int type, string port)
		{
			Name = name;
			Type = type;
			Port = port;
		}
		public int Type { get; set; }
		public string Name { get; set; }
		public string Port { get; set; }
	}

}
