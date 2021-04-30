namespace RyanQuagliataUnity.Configurable {
	public interface IConfig<T> {
		public T GetConfig { get; }
	}
}