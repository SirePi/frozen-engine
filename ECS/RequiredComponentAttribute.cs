using System;
using System.Collections.Generic;
using System.Text;

namespace FrozenEngine.ECS
{
	[global::System.AttributeUsageAttribute(global::System.AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class RequiredComponentAttribute : Attribute { }
}
