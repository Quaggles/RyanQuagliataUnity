using QFSW.QC;
using UnityEngine;

namespace RyanQuagliataUnity.Extensions.QuantumConsole {
	[CommandPrefix("SystemInfo")]
	public class SystemInfoCommands {
		[Command] public string DeviceModel => SystemInfo.deviceModel;
		[Command] public string DeviceName => SystemInfo.deviceName;
		[Command] public string OperatingSystem => SystemInfo.operatingSystem;
		[Command] public string ProcessorType => SystemInfo.processorType;
		[Command] public string UnsupportedIdentifier => SystemInfo.unsupportedIdentifier;
		[Command] public string DeviceUniqueIdentifier => SystemInfo.deviceUniqueIdentifier;
		[Command] public string GraphicsDeviceName => SystemInfo.graphicsDeviceName;
		[Command] public string GraphicsDeviceVendor => SystemInfo.graphicsDeviceVendor;
		[Command] public string GraphicsDeviceVersion => SystemInfo.graphicsDeviceVersion;
		[Command] public float BatteryLevel => SystemInfo.batteryLevel;
		[Command] public int ProcessorCount => SystemInfo.processorCount;
		[Command] public int ProcessorFrequency => SystemInfo.processorFrequency;
	}
}