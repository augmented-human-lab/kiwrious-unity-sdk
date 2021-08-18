using Assets.Kiwrious.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using UnityEngine;
using static Assets.Kiwrious.Scripts.Constants;

public class SerialReader : MonoBehaviour{

	public static SerialReader instance;

	private const int PACKET_SIZE = 26;
	private const int BAUD_RATE = 115200;
	private const int READ_TIMEOUT = 1500;
	private const int MAX_RETRY_ATTEMPTS = 128;
	private const int PACKET_HEADER_BYTE = 0X0a;
	private const int PACKET_FOOTER_BYTE = 0X0b;


	public Dictionary<SENSOR_TYPE, bool> sensorEvents = new Dictionary<SENSOR_TYPE, bool>();

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
	public float a_temperature;
	public float d_temperature;

	public bool listen;
	public bool autoStart;
	private Coroutine serialListener;

	[SerializeField]
	private string[] connectedSerialPorts;
	[SerializeField]
	private List<KiwriousSensor> kiwriousSensorRegistry = new List<KiwriousSensor>();
	[SerializeField]
	private List<KiwriousSensor> connectedKiwriousSensors = new List<KiwriousSensor>();

	private List<Thread> sensorReaders = new List<Thread>();
	private List<SerialPort> activePorts = new List<SerialPort>();
	private Dictionary<SENSOR_TYPE, Action<string, byte[]>> decodeMethods = new Dictionary<SENSOR_TYPE, Action<string, byte[]>>();

	void Awake() {
		instance = this;
	}

    void Start () {
        sensorEvents[SENSOR_TYPE.EC] = false;
        sensorEvents[SENSOR_TYPE.CLIMATE] = false;
        sensorEvents[SENSOR_TYPE.VOC] = false;
        sensorEvents[SENSOR_TYPE.LIGHT] = false;
        sensorEvents[SENSOR_TYPE.COLOR] = false;
		sensorEvents[SENSOR_TYPE.CARDIO] = false;
		sensorEvents[SENSOR_TYPE.THERMAL] = false;
		decodeMethods[SENSOR_TYPE.EC] = DecodeConductivity;
        decodeMethods[SENSOR_TYPE.CLIMATE] = DecodeHumidity;
        decodeMethods[SENSOR_TYPE.LIGHT] = DecodeUV;
        decodeMethods[SENSOR_TYPE.VOC] = DecodeVOC;
        decodeMethods[SENSOR_TYPE.COLOR] = DecodeColor;
		decodeMethods[SENSOR_TYPE.CARDIO] = DecodeCardio;
		decodeMethods[SENSOR_TYPE.THERMAL] = DecodeThermal;
		decodeMethods[SENSOR_TYPE.THERMAL2] = DecodeThermal2;
		if (autoStart) {
			StartSerialReader();
		}
	}

	public void StartSerialReader() {
		serialListener = StartCoroutine(ScanPorts());
	}

	public void StopSerialReader() {
		if (serialListener != null)
		{
			StopCoroutine(serialListener);
		}
		foreach (SerialPort s in activePorts)
		{
			if (s != null && s.IsOpen) {
				s.Close();
			}
		}
		foreach (Thread reader in sensorReaders)
		{
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
			sensorEvents[(SENSOR_TYPE)disconnectedSensor.Type] = false;
			SerialPort activePort = activePorts.Where(s => s.PortName == port).FirstOrDefault();
			activePort.Close();
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
		sensorEvents[sensorType] = (true);
		SerialPort stream = new SerialPort(port, BAUD_RATE);
		activePorts.Add(stream);
		if (!stream.IsOpen)
		{
			stream.Open();
		}
		int attempts = 0;
		bool headerFound = false;
		List<int> buffer = new List<int>();
		while (stream.IsOpen)
		{
			// new reading method
			while (!headerFound && attempts < MAX_RETRY_ATTEMPTS)
			{
				attempts++;
				Debug.Log($"Searching for the header {port}");

				buffer.Add(stream.BaseStream.ReadByte());
				if (buffer.Count > PACKET_SIZE)
				{
					buffer.RemoveAt(0);
				}
				if (buffer.Count == PACKET_SIZE)
				{
					if (buffer.ElementAt(0) == PACKET_HEADER_BYTE &&
						buffer.ElementAt(1) == PACKET_HEADER_BYTE &&
						buffer.ElementAt(PACKET_SIZE - 1) == PACKET_FOOTER_BYTE &&
						buffer.ElementAt(PACKET_SIZE - 2) == PACKET_FOOTER_BYTE)
					{
						headerFound = true;
						break;
					}
				}
			}
			if (!headerFound)
			{
				Debug.Log("No header");
				return;
			}
			try
			{
				byte[] packet = new byte[PACKET_SIZE];
				for (int i = 0; i < PACKET_SIZE; i++)
				{
					packet[i] = (byte)stream.BaseStream.ReadByte();
				}
				string temp = "";
				foreach (byte b in packet) {
					temp += $"{b} ";
				}
				decodeMethods[sensorType](port, packet);
			}
			catch (Exception ex) {
				Debug.LogError(ex.Message);
				stream.Close();
			}
		}
		stream.Close();

	}

	private void IdentifySerialDevice(string port) {
		Debug.Log("Identify");
		byte[] data = new byte[PACKET_SIZE];
		SerialPort serialPort = new SerialPort(port, BAUD_RATE);
		serialPort.ReadTimeout = READ_TIMEOUT;
		serialPort.Open();
		int attempts = 0;

		if (!serialPort.BaseStream.CanRead)
		{
			Debug.Log($"Can't read {port}");
		}

		List<int> buffer = new List<int>();
		bool headerFound = false;
        while (attempts < MAX_RETRY_ATTEMPTS)
        {
            attempts++;
            Debug.Log($"Searching for the header {port}");
			
			buffer.Add(serialPort.BaseStream.ReadByte());
			if (buffer.Count > PACKET_SIZE) 
			{
				buffer.RemoveAt(0);
			}
			if (buffer.Count == PACKET_SIZE) {
				if (buffer.ElementAt(0) == PACKET_HEADER_BYTE && 
					buffer.ElementAt(1) == PACKET_HEADER_BYTE && 
					buffer.ElementAt(PACKET_SIZE-1) == PACKET_FOOTER_BYTE && 
					buffer.ElementAt(PACKET_SIZE-2) == PACKET_FOOTER_BYTE)
				{
					headerFound = true;
					break;
				}
			}
		}
		if (!headerFound) 
		{ 
			Debug.Log("No header"); 
			return; 
		}
		try
        {
			for (int i=0; i<PACKET_SIZE; i++) {
				data[i] = (byte)serialPort.BaseStream.ReadByte();
			}
		}
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

        Debug.Log("Identify done");
		if (Enum.IsDefined(typeof(SENSOR_TYPE), (SENSOR_TYPE)data[2]))
		{
			string sensorName = Enum.GetName(typeof(SENSOR_TYPE), data[2]);
			kiwriousSensorRegistry.Add(new KiwriousSensor(sensorName, data[2], port));
			connectedKiwriousSensors.Add(new KiwriousSensor(sensorName, data[2], port));
			Debug.Log($"{sensorName} sensor connected!");
		}
		else {
			Debug.Log($"type [{data?[2]} is not registered as a kiwrious sensor]");
		};
        serialPort.Close();
	}

	void OnDisable() {
		StopSerialReader();
	}

	#region Decode methods
	private void DecodeConductivity(string port, byte[] data)
	{
		string d0a = Convert.ToString(data[7], 16);
		string d0b = Convert.ToString(data[6], 16);
		string d1a = Convert.ToString(data[9], 16);
		string d1b = Convert.ToString(data[8], 16);
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
			conductivity = (float)((1 / conductivity) * Math.Pow(10, 6));
		}
	}

	private void DecodeHumidity(string port, byte[] data)
	{
		string temp_a = Convert.ToString(data[7], 16);
		string temp_b = Convert.ToString(data[6], 16);
		string hum_a = Convert.ToString(data[9], 16);
		string hum_b = Convert.ToString(data[8], 16);
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

	private void DecodeUV(string port, byte[] data)
	{
		string data0_a = Convert.ToString(data[9], 16);
		string data0_b = Convert.ToString(data[8], 16);
		string data0_c = Convert.ToString(data[7], 16);
		string data0_d = Convert.ToString(data[6], 16);
		string data1_a = Convert.ToString(data[13], 16);
		string data1_b = Convert.ToString(data[12], 16);
		string data1_c = Convert.ToString(data[11], 16);
		string data1_d = Convert.ToString(data[10], 16);
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

	private void DecodeColor(string port, byte[] data)
	{
		string data0_a = Convert.ToString(data[7], 16);
		string data0_b = Convert.ToString(data[6], 16);
		string data1_a = Convert.ToString(data[9], 16);
		string data1_b = Convert.ToString(data[8], 16);
		string data2_a = Convert.ToString(data[11], 16);
		string data2_b = Convert.ToString(data[10], 16);
		string data3_a = Convert.ToString(data[13], 16);
		string data3_b = Convert.ToString(data[12], 16);
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
		double r = Math.Sqrt(int.Parse(data0, NumberStyles.HexNumber));
		double g = Math.Sqrt(int.Parse(data1, NumberStyles.HexNumber));
		double b = Math.Sqrt(int.Parse(data2, NumberStyles.HexNumber));
		double w = Math.Sqrt(int.Parse(data3, NumberStyles.HexNumber));
		color_h = (float)Math.Floor(r);
        color_s = (float)Math.Floor(g);
        color_v = (float)Math.Floor(b);
    }

	private void DecodeCardio(string port, byte[] data) {
		tempx = data;
		uint data0 = BitConverter.ToUInt32(data.Skip(6).Take(4).ToArray(), 0);
		uint data1 = BitConverter.ToUInt32(data.Skip(10).Take(4).ToArray(), 0);
		uint data2 = BitConverter.ToUInt32(data.Skip(14).Take(4).ToArray(), 0);
		uint data3 = BitConverter.ToUInt32(data.Skip(18).Take(4).ToArray(), 0);
	}
	public byte[] tempx = new byte[26];

	private void DecodeThermal(string port, byte[] data)
	{
		
		d_temperature = BitConverter.ToInt16(data.Skip(6).Take(2).ToArray(), 0) / 100;
		a_temperature = BitConverter.ToInt16(data.Skip(8).Take(2).ToArray(), 0) / 100;
	}

	private void DecodeThermal2(string port, byte[] data)
	{
		a_temperature = BitConverter.ToInt16(data.Skip(6).Take(2).ToArray(), 0) / 100;
		ushort x = BitConverter.ToUInt16(data.Skip(8).Take(2).ToArray(), 0);
		float a = BitConverter.ToSingle(data.Skip(10).Take(4).ToArray(), 0);
		float b = BitConverter.ToSingle(data.Skip(14).Take(4).ToArray(), 0);
		float c = BitConverter.ToSingle(data.Skip(18).Take(4).ToArray(), 0);
		d_temperature = (float)(Math.Round(((a * Math.Pow(x, 2)) / Math.Pow(10, 5) + b * x + c)));
	}

	private void DecodeVOC(string port, byte[] data)
	{
		string data0_a = Convert.ToString(data[7], 16);
		string data0_b = Convert.ToString(data[6], 16);
		string data1_a = Convert.ToString(data[9], 16);
		string data1_b = Convert.ToString(data[8], 16);
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


