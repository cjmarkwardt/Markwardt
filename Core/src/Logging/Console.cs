namespace Markwardt;

[Singleton<SystemConsole>]
public interface IConsole : ITextOutput, ITextInput;