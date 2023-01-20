namespace Frozen.Utilities
{
	public interface IPoolable
	{
		void OnPickup();

		void OnReturn();
	}
}
