using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataReader
{
	BinaryReader reader;

	public DataReader(BinaryReader reader)
	{
		this.reader = reader;
	}

	public float ReadFloat()
	{
		return reader.ReadSingle();
	}

	public int ReadInt()
	{
		return reader.ReadInt32();
	}

	public short ReadShort()
	{
		return reader.ReadInt16();
	}

	public Quaternion ReadQuaternion()
	{
		Quaternion value;
		value.x = reader.ReadSingle();
		value.y = reader.ReadSingle();
		value.z = reader.ReadSingle();
		value.w = reader.ReadSingle();
		return value;
	}

	public Vector3 ReadVector3()
	{
		Vector3 value;
		value.x = reader.ReadSingle();
		value.y = reader.ReadSingle();
		value.z = reader.ReadSingle();
		return value;
	}


}
