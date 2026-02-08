using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace GamblersMod.config
{
	// Token: 0x02000015 RID: 21
	internal class SerializerHelper
	{
		// Token: 0x06000065 RID: 101 RVA: 0x00005064 File Offset: 0x00003264
		public static byte[] GetSerializedSettings<T>(T valToSerialize)
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			try
			{
				binaryFormatter.Serialize(memoryStream, valToSerialize);
			}
			catch (SerializationException ex)
			{
				Plugin.mls.LogError("Config serialization failed: " + ex.Message);
			}
			byte[] array = memoryStream.ToArray();
			memoryStream.Close();
			return array;
		}

		// Token: 0x06000066 RID: 102 RVA: 0x000050D8 File Offset: 0x000032D8
		public static T GetDeserializedSettings<T>(byte[] settingsAsBytes)
		{
			MemoryStream memoryStream = new MemoryStream();
			memoryStream.Write(settingsAsBytes, 0, settingsAsBytes.Length);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			try
			{
				object obj = binaryFormatter.Deserialize(memoryStream);
				memoryStream.Close();
				return (T)((object)obj);
			}
			catch (SerializationException ex)
			{
				Plugin.mls.LogError("Config deserialization failed: " + ex.Message);
			}
			memoryStream.Close();
			return default(T);
		}
	}
}
