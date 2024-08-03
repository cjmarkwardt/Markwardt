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
    Sequence,
    PairSequence,
    PropertySequence,
    SequenceStop
}