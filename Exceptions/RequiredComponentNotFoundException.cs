using System;
using System.Runtime.Serialization;

namespace Frozen
{
	[Serializable]
	public class RequiredComponentNotFoundException : Exception
	{
		protected RequiredComponentNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{ }

		public RequiredComponentNotFoundException()
		{ }

		public RequiredComponentNotFoundException(string message) : base(message)
		{ }

		public RequiredComponentNotFoundException(string entityName, string componentType) : base($"Required component {componentType} not found in entity {entityName}")
		{ }

		public RequiredComponentNotFoundException(string message, Exception innerException) : base(message, innerException)
		{ }
	}
}
