namespace Markwardt;

public enum NetworkDelivery
{
    /// <summary> Data may be dropped, may arrive out of order, and may have duplicates. </summary>
    Simple,

    /// <summary> Data will not be dropped, may arrive out of order, and will not have duplicates. </summary>
    Reliable,

    /// <summary> Data may be dropped, will not arrive out of order, and will not have duplicates. </summary>
    Ordered,

    /// <summary> Data will not be dropped, will not arrive out of order, and will not have duplicates. </summary>
    Full
}