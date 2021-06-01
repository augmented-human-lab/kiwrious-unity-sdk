using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using UnityEngine;

public class SerialReader : MonoBehaviour{

	public static SerialReader instance;

	[SerializeField]
	public Dictionary<int, bool> sensorEvents = new Dictionary<int, bool>();

	public float voc1;
	public float voc2;
	public float conductivity;
	public float uv;
	public float lux;
	public float humidity;
	public float temperature;
	public float color_h;
	public float color_s;
	public float color_v;
	public Dictionary<string, float> hsv = new Dictionary<string, float>
	{
		{ "hue", 0 },
		{ "sat", 0 },
		{ "val", 0 }
	};

	public bool listen;
	Coroutine serialListener;
	public string[] connectedSerialPorts;

	[SerializeField]
	public List<KiwriousSensor> kiwriousSensorRegistry = new List<KiwriousSensor>();
	[SerializeField]
	public List<KiwriousSensor> connectedKiwriousSensors = new List<KiwriousSensor>();

    public List<Thread> sensorReaders = new List<Thread>();
	public List<SerialPort> activePorts = new List<SerialPort>();
	public Dictionary<SENSOR_TYPE, Action<string, byte[]>> readMethods = new Dictionary<SENSOR_TYPE, Action<string, byte[]>>();

	void Awake() {
		instance = this;
	}

    void Start () {
        sensorEvents[(int)SENSOR_TYPE.Conductivity] = false;
        sensorEvents[(int)SENSOR_TYPE.Humidity] = false;
        sensorEvents[(int)SENSOR_TYPE.VOC] = false;
        sensorEvents[(int)SENSOR_TYPE.Uv] = false;
        sensorEvents[(int)SENSOR_TYPE.Color] = false;
        serialListener = StartCoroutine(ScanPorts());
		readMethods[SENSOR_TYPE.Conductivity] = ReadConductivity;
        readMethods[SENSOR_TYPE.Humidity] = ReadHumidity;
        readMethods[SENSOR_TYPE.Uv] = ReadUV;
        readMethods[SENSOR_TYPE.VOC] = ReadVOC;
        readMethods[SENSOR_TYPE.Color] = ReadColor;
    }

	IEnumerator ScanPorts() {
		while (listen) {
			yield return new WaitForSeconds(1);
			string[] ports = SerialPort.GetPortNames();
			if (ports.Length > connectedSerialPorts.Length)
			{
				string[] newDevices = ports.Where(p => !connectedSerialPorts.Contains(p)).ToArray();
				foreach (string port in newDevices)
				{
					ProcessConnectedSerialDevice(port);
				}
			}
			else if(ports.Length < connectedSerialPorts.Length) {
				string[] removedDevices = connectedSerialPorts.Where(p => !ports.Contains(p)).ToArray();
				foreach (string port in removedDevices)
				{
					ProcessDisconnectedSerialDevice(port);
				}
			}
			connectedSerialPorts = ports;
		}
	}

	private void ProcessConnectedSerialDevice(string port) {
		if (kiwriousSensorRegistry.Any(s => s.Port == port))
		{
			KiwriousSensor connectedSensor = kiwriousSensorRegistry.Where(s => s.Port == port).FirstOrDefault();
			Debug.Log($"{connectedSensor.Name} sensor connected!");
			connectedKiwriousSensors.Add(connectedSensor);
			StartKiwriousSensorReader(port);
			return;
		}
		Thread newSensorScanner = new Thread(() => IdentifySerialDevice(port))
		{
			Name = port,
			IsBackground = true
		};
		newSensorScanner.Start();
		newSensorScanner.Join();
		if (IsKiwriousSensorConnectedAt(port)) {
			StartKiwriousSensorReader(port);
		}
	}

	private void StartKiwriousSensorReader(string port) {
		Thread newsensorReader = new Thread(() => ReadSensor(port))
		{
			Name = port,
			IsBackground = true
		};
		newsensorReader.Start();
		sensorReaders.Add(newsensorReader);
	}

	private void ProcessDisconnectedSerialDevice(string port) {
		if (connectedKiwriousSensors.Any(s => s.Port == port))
		{
			KiwriousSensor disconnectedSensor = connectedKiwriousSensors.Where(s => s.Port == port).FirstOrDefault();
			Debug.Log($"{disconnectedSensor.Name} sensor disconnected!");
			connectedKiwriousSensors.Remove(disconnectedSensor);
			sensorEvents[disconnectedSensor.Type] = (false);
			SerialPort activePort = activePorts.Where(s => s.PortName == port).FirstOrDefault();
			activePort.Close();
			activePort.Dispose();
			activePorts.Remove(activePort);
			Thread reader = sensorReaders.Where(r => r.Name == port).FirstOrDefault();
			reader.Join();
			sensorReaders.Remove(reader);
		}
		else {
			Debug.Log($"Disconnected device at port [{port}] is not a kiwrious sensor");
		}
	}

	private void ReadSensor(string port) {
		Debug.Log($"Read {port}");
		SENSOR_TYPE sensorType = (SENSOR_TYPE)GetSensorTypeByPort(port);
		sensorEvents[(int)sensorType] = (true);
		byte[] data = new byte[25];
		SerialPort stream = new SerialPort(port);
		activePorts.Add(stream);
		if (!stream.IsOpen)
		{
			stream.Open();
		}
		while (stream.IsOpen)
		{
			try {
				stream.Read(data, 0, 25);
				readMethods[sensorType](port, data);
			}
			catch (Exception ex) {
				Debug.LogError(ex.Message);
				stream.Close();
			}
		}
		stream.Dispose();

	}

	private void IdentifySerialDevice(string port) {
		Debug.Log("Identify");
		byte[] data = new byte[25];
		SerialPort serialPort = new SerialPort(port);
		serialPort.Open();
        serialPort.ReadTimeout = 500;
        data[1] = 0;
		while (data[1] == 0)
		{
			try
			{
				serialPort.Read(data, 0, 25);
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				break;
			}
		}
		if (Enum.IsDefined(typeof(SENSOR_TYPE), (SENSOR_TYPE)data[1]))
		{
			string sensorName = Enum.GetName(typeof(SENSOR_TYPE), data[1]);
			kiwriousSensorRegistry.Add(new KiwriousSensor(sensorName, data[1], port));
			connectedKiwriousSensors.Add(new KiwriousSensor(sensorName, data[1], port));
			Debug.Log($"{sensorName} sensor connected!");
		}
		else {
			Debug.Log($"type [{data?[1]} is not registered as a kiwrious sensor]");
		};
        serialPort.Close();
	}

	void OnDisable() {
		if (serialListener != null) {
			StopCoroutine(serialListener);
		}
		foreach (SerialPort s in activePorts) {
			s.Close();
			s.Dispose();
		}
        foreach (Thread reader in sensorReaders)
        {
			SerialPort port = new SerialPort(reader.Name);
			port.Close();
			port.Dispose();
			Debug.Log(reader.IsAlive);
            try
            {
                reader.Join();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }
    }

	#region Decode methods
	private void ReadConductivity(string port, byte[] data)
	{
		string d0a = Convert.ToString(data[6], 16);
		string d0b = Convert.ToString(data[5], 16);
		string d1a = Convert.ToString(data[8], 16);
		string d1b = Convert.ToString(data[7], 16);
		if (d0b.Length < 2)
		{
			d0b = "0" + d0b;
		}
		if (d1b.Length < 2)
		{
			d1b = "0" + d1b;
		}
		string d0 = d0a + d0b;
		string d1 = d1a + d1b;
		int d0i = int.Parse(d0, NumberStyles.HexNumber);
		int d1i = int.Parse(d1, NumberStyles.HexNumber);
		if (d0i == 65535)
		{
			conductivity = 0;
		}
		else
		{
			conductivity = d0i * d1i;
			conductivity = (1 / conductivity) * Mathf.Pow(10, 6);
		}
	}

	private void ReadHumidity(string port, byte[] data)
	{
		string temp_a = Convert.ToString(data[6], 16);
		string temp_b = Convert.ToString(data[5], 16);
		string hum_a = Convert.ToString(data[8], 16);
		string hum_b = Convert.ToString(data[7], 16);
		if (temp_b.Length < 2)
		{
			temp_b = "0" + temp_b;
		}
		if (hum_b.Length < 2)
		{
			hum_b = "0" + hum_b;
		}
		string temp = temp_a + temp_b;
		string hum = hum_a + hum_b;
		int d0i = int.Parse(temp, NumberStyles.HexNumber);
		int d1i = int.Parse(hum, NumberStyles.HexNumber);
		temperature = d0i / 100;
		humidity = d1i / 100;
	}

	private void ReadUV(string port, byte[] data)
	{
		string data0_a = Convert.ToString(data[8], 16);
		string data0_b = Convert.ToString(data[7], 16);
		string data0_c = Convert.ToString(data[6], 16);
		string data0_d = Convert.ToString(data[5], 16);
		string data1_a = Convert.ToString(data[12], 16);
		string data1_b = Convert.ToString(data[11], 16);
		string data1_c = Convert.ToString(data[10], 16);
		string data1_d = Convert.ToString(data[9], 16);
		if (data0_b.Length < 2)
		{
			data0_b = "0" + data0_b;
		}
		if (data0_c.Length < 2)
		{
			data0_c = "0" + data0_c;
		}
		if (data0_d.Length < 2)
		{
			data0_d = "0" + data0_d;
		}
		if (data1_b.Length < 2)
		{
			data1_b = "0" + data1_b;
		}
		if (data1_c.Length < 2)
		{
			data1_c = "0" + data1_c;
		}
		if (data1_d.Length < 2)
		{
			data1_d = "0" + data1_d;
		}
		string data0 = data0_a + data0_b + data0_c + data0_d;
		string data1 = data1_a + data1_b + data1_c + data1_d;
		lux = HexToFloat(data0);
		uv = HexToFloat(data1);
	}

	void ReadColor(string port, byte[] data)
	{
		string data0_a = Convert.ToString(data[6], 16);
		string data0_b = Convert.ToString(data[5], 16);
		string data1_a = Convert.ToString(data[8], 16);
		string data1_b = Convert.ToString(data[7], 16);
		string data2_a = Convert.ToString(data[10], 16);
		string data2_b = Convert.ToString(data[9], 16);
		string data3_a = Convert.ToString(data[12], 16);
		string data3_b = Convert.ToString(data[11], 16);
		if (data0_b.Length < 2)
		{
			data0_b = "0" + data0_b;
		}
		if (data1_b.Length < 2)
		{
			data1_b = "0" + data1_b;
		}
		if (data2_b.Length < 2)
		{
			data2_b = "0" + data2_b;
		}
		if (data3_b.Length < 2)
		{
			data3_b = "0" + data3_b;
		}
		string data0 = data0_a + data0_b;
		string data1 = data1_a + data1_b;
		string data2 = data2_a + data2_b;
		string data3 = data3_a + data3_b;
		float r = Mathf.Sqrt(int.Parse(data0, NumberStyles.HexNumber));
		float g = Mathf.Sqrt(int.Parse(data1, NumberStyles.HexNumber));
		float b = Mathf.Sqrt(int.Parse(data2, NumberStyles.HexNumber));
		float w = Mathf.Sqrt(int.Parse(data3, NumberStyles.HexNumber));
		color_h = Mathf.FloorToInt(r);
		color_s = Mathf.FloorToInt(g);
		color_v = Mathf.FloorToInt(b);
		hsv["hue"] = color_h;
		hsv["sat"] = color_s;
		hsv["val"] = color_v;
		//color = new Color32((byte)reading1, (byte)reading2, (byte)reading3, 255);
	}

	private void ReadVOC(string port, byte[] data)
	{
		string data0_a = Convert.ToString(data[6], 16);
		string data0_b = Convert.ToString(data[5], 16);
		string data1_a = Convert.ToString(data[8], 16);
		string data1_b = Convert.ToString(data[7], 16);
		if (data0_b.Length < 2)
		{
			data0_b = "0" + data0_b;
		}
		if (data1_b.Length < 2)
		{
			data1_b = "0" + data1_b;
		}
		string data0 = data0_a + data0_b;
		string data1 = data1_a + data1_b;
		int d0i = int.Parse(data0, NumberStyles.HexNumber);
		int d1i = int.Parse(data1, NumberStyles.HexNumber);
		voc1 = d0i;
		voc2 = d1i;
	}
	#endregion

	#region Helper methods
	private int GetSensorTypeByPort(string port)
	{
		KiwriousSensor sensor = kiwriousSensorRegistry.Where(s => s.Port == port).FirstOrDefault();
		if (sensor != null)
		{
			return sensor.Type;
		}
		return 0;
	}

	private float HexToFloat(string hex)
	{
		Int32 IntRep = Int32.Parse(hex, NumberStyles.AllowHexSpecifier);
		return BitConverter.ToSingle(BitConverter.GetBytes(IntRep), 0);
	}

	private bool IsKiwriousSensorConnectedAt(string port)
	{
		return kiwriousSensorRegistry.Any(s => s.Port == port) && connectedKiwriousSensors.Any(s => s.Port == port);
	}
	#endregion

}

public enum SENSOR_TYPE
{
	Uv = 1,
	Color = 3,
	Conductivity = 4,
	Humidity = 7,
	VOC = 6,
	BODY_TEMP = 2,
	SOUND = 8,
	HEART_RATE = 5
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
