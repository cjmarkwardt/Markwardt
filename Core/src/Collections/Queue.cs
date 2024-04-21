namespace Markwardt;

public interface IQueue<T> : IInsertionQueue<T>, IConsumptionQueue<T>;