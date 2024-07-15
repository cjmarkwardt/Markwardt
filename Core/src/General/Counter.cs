namespace Markwardt;

public interface IIncrementer
{
    void Increment(int count = 1);
}

public interface IDecrementer
{
    void Decrement(int count = 1);
}

public interface ICounter : IIncrementer, IDecrementer
{
    int Value { get; set; }

    void IIncrementer.Increment(int count)
        => Value += count;

    void IDecrementer.Decrement(int count)
        => Value -= count;
}

public class Counter : ICounter
{
    public int Value { get; set; }
}