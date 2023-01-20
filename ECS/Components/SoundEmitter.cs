namespace Frozen.ECS.Components
{
	public class SoundEmitter : Component
	{
		[RequiredComponent]
		internal Transform Transform { get; set; }

		public float Volume { get; set; } = 1;
	}
}
