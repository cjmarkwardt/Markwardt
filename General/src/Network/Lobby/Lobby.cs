namespace Markwardt;

public interface ILobbyProperty
{
    ILobby Lobby { get; }

    string Key { get; }
    string Value { get; set; }

    void Delete();
}

public interface ILobbyMemberProperty
{
    ILobbyMember Member { get; }

    string Key { get; }
    bool IsRestricted { get; set; }
    IObservableList<string> SuggestedValues { get; }

    string Value { get; set; }
    
    void Delete();
}

public interface ILobbyMember
{
    ILobby Lobby { get; }

    string Name { get; }
    bool IsSelf { get; }
    bool IsPresent { get; }
    bool IsOwner { get; }

    IObservableReadOnlyLookupList<string, ILobbyMemberProperty> Properties { get; }

    ILobbyMemberProperty AddProperty(string name);
    void Kick();
    void Ban();
    void Promote();
}

public interface ILobbyProfile
{
    string Name { get; }
    bool IsLocked { get; }
    int Members { get; }
    int MaxMembers { get; }

    IObservableReadOnlyDictionary<string, string> Properties { get; }

    ValueTask<Failable<ILobby>> Join(string? password = null, CancellationToken cancellation = default);
    void Refresh();
}

public interface ILobby
{
    bool IsLocked { get; }

    string Name { get; set; }
    int MaxMembers { get; set; }
    bool IsVisible { get; set; }

    ILobbyMember Self { get; }
    IObservableReadOnlyList<ILobbyMember> Members { get; }
    IObservableReadOnlyLookupList<string, ILobbyProperty> Properties { get; }

    IObservable<(ILobbyMember Member, string Message)> Received { get; }
    IObservable Disconnected { get; }

    void Send(string message);
    ILobbyProperty AddProperty(string name);
    void SetLock(string? password);
    void Leave();
}