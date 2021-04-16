using QFSW.QC;

namespace RyanQuagliataUnity.Extensions.QuantumConsole {
	[CommandPrefix]
	public static class Namespace {
		[Command] public static void Reset() => QuantumParser.ResetNamespaceTable();
		[Command] public static void Add(string name) => QuantumParser.AddNamespace(name);
		[Command] public static void Remove(string name) => QuantumParser.RemoveNamespace(name);
		[Command] public static void List() => QuantumParser.GetAllNamespaces();
	}
}