using System;
using System.Collections.Generic;
using System.Text;

namespace FrozenEngine.ECS
{
	[System.AttributeUsage(System.AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	sealed class RequiredComponentAttribute : Attribute { }
}
