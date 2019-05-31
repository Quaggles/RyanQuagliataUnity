namespace RyanQuagliata.GlobalShaderPropertySetter {
	public interface IGlobalShaderPropertySettable {
		bool Enabled { get; set; }

		string SourceTypeName { get; }

		string GlobalShaderPropertyName { get; set; }

		void Set();

		void UpdateValuePreview();
	}
}