using Assets.Kiwrious.Scripts;

namespace kiwrious {
	public interface IKiwriousReader
	{

		SensorData GetConductivity();


		SensorData GetVOC();


		SensorData GetUVLux();


		SensorData GetHumidityTemperature();


		SensorData GetColor();


		SensorData GetHeartRate();


		SensorData GetBodyTemperature();


		SensorData GetBodyTemperature2();

	}
}
