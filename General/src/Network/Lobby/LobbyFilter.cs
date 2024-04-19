namespace Markwardt;

public abstract record LobbyFilter
{
    public record Distance(float Value) : LobbyFilter;
    public record PropertyEqual(string Key, string? Value) : LobbyFilter;
    public record PropertyNotEqual(string Key, string? Value) : LobbyFilter;
    public record PropertyMaximum(string Key, float Value) : LobbyFilter;
    public record PropertyMinimum(string Key, float Value) : LobbyFilter;
    public record ResultMaximum(int Value) : LobbyFilter;
    public record SlotMinimum(int Value) : LobbyFilter;
}