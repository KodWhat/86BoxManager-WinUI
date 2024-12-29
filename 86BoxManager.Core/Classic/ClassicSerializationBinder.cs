using System;
using System.Runtime.Serialization;

namespace EightySixBoxManager.Core.Classic;

public class ClassicSerializationBinder : SerializationBinder
{
	public override Type? BindToType(string assemblyName, string typeName)
	{
		if (assemblyName.StartsWith("86Manager") && typeName == "_86boxManager.VM")
		{
			return typeof(VM);
		}
		return null;
	}
}
