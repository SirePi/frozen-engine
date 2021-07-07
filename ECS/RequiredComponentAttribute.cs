using System;
using System.Collections.Generic;
using System.Text;

namespace Frozen.ECS
{
	[global::System.AttributeUsageAttribute(global::System.AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class RequiredComponentAttribute : Attribute { }
}
