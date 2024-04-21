namespace Markwardt;

public interface IConverter<in TFrom, out TTo>
{
    TTo Convert(TFrom from);
}

public class Converter<TFrom, TTo>(Func<TFrom, TTo> convert) : IConverter<TFrom, TTo>
{
    public TTo Convert(TFrom from)
        => convert(from);
}