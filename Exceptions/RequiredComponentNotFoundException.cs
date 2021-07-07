using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Frozen
{
	[Serializable]
	public class RequiredComponentNotFoundException : Exception
	{
		public RequiredComponentNotFoundException() { }

		public RequiredComponentNotFoundException(string message) : base(message) { }

		public RequiredComponentNotFoundException(string entityName, string componentType) : base($"Required component {componentType} not found in entity {entityName}") { }

		public RequiredComponentNotFoundException(string message, Exception innerException) : base(message, innerException) { }

		protected RequiredComponentNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
