namespace Markwardt;

public enum DataKind : byte
{
    Null,
    False,
    True,
    Integer,
    Single,
    Double,
    String,
    Block,
    Stop,
    Sequence,
    PairSequence,
    PropertySequence,
}