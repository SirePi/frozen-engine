namespace Frozen.ECS.Components
{
	public class SoundListener : Component
	{
		[RequiredComponent]
		internal Transform Transform { get; set; }

		[RequiredComponent]
		internal Camera Camera { get; set; }

		public float CutOffDistance { get; set; } = 100000;

		public float FullVolumeDistance { get; set; } = 1000;
	}
}
