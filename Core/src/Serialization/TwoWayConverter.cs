namespace Markwardt;

public interface ITwoWayConverter<TFrom, TTo> : IConverter<TFrom, TTo>
{
    TFrom ConvertBack(TTo to);
}

public static class TwoWayConverterExtensions
{
    public static ITwoWayConverter<TTo, TFrom> Reverse<TTo, TFrom>(this ITwoWayConverter<TFrom, TTo> converter)
        => new TwoWayConverter<TTo, TFrom>(converter.ConvertBack, converter.Convert);
}

public class TwoWayConverter<TFrom, TTo>(Func<TFrom, TTo> convert, Func<TTo, TFrom> convertBack) : ITwoWayConverter<TFrom, TTo>
{
    public TwoWayConverter(IConverter<TFrom, TTo> convert, IConverter<TTo, TFrom> convertBack)
        : this(convert.Convert, convertBack.Convert) { }

    public TTo Convert(TFrom from)
        => convert(from);

    public TFrom ConvertBack(TTo to)
        => convertBack(to);
}