namespace Amusoft.Reflection.Emit
{
	public interface IPropertyDelegate
	{
		object Get(object target);
		void Set(object target, object value);
	}
}