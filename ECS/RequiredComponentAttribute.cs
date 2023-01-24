using System;

namespace Frozen.ECS
{
	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class RequiredComponentAttribute : Attribute
	{ }
}
