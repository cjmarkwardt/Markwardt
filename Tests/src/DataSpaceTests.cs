namespace Markwardt;

public class DataSpaceTests
{
    private static DataStore CreateTestSpace(int blockSize)
        => new(new MemoryStream(), blockSize);

    [Fact]
    public async Task CreateAndLoad()
    {
        DataStore space = CreateTestSpace(10);
        string data = "some test data";
        int id = await space.Create(Encoding.UTF8.GetBytes(data));

        DynamicBuffer buffer = new();
        await space.Load(id, buffer);
        Assert.Equal(data, Encoding.UTF8.GetString(buffer.Data.Span));
    }

    [Fact]
    public async Task SaveAndLoadRoot()
    {
        DataStore space = CreateTestSpace(10);
        string data = "some other test data";
        await space.SaveRoot(Encoding.UTF8.GetBytes(data));

        DynamicBuffer buffer = new();
        await space.LoadRoot(buffer);
        Assert.Equal(data, Encoding.UTF8.GetString(buffer.Data.Span));
    }

    [Fact]
    public async Task Replacement()
    {
        DataStore space = CreateTestSpace(10);
        string initialData2 = "test data!";
        string finalDataRoot = "test test test test test test test test";
        string finalData1 = "yet another test data";
        string finalData2 = "more testing of data parts for testing";
        int id1 = await space.Create();
        int id2 = await space.Create(Encoding.UTF8.GetBytes(initialData2));

        DynamicBuffer buffer = new();

        await space.LoadRoot(buffer);
        Assert.Equal(0, buffer.Data.Length);

        buffer.Clear();
        await space.Load(id1, buffer);
        Assert.Equal(0, buffer.Data.Length);

        buffer.Clear();
        await space.Load(id2, buffer);
        Assert.Equal(initialData2, Encoding.UTF8.GetString(buffer.Data.Span));

        await space.SaveRoot(Encoding.UTF8.GetBytes(finalDataRoot));
        await space.Save(id1, Encoding.UTF8.GetBytes(finalData1));
        await space.Save(id2, Encoding.UTF8.GetBytes(finalData2));

        buffer.Clear();
        await space.LoadRoot(buffer);
        Assert.Equal(finalDataRoot, Encoding.UTF8.GetString(buffer.Data.Span));

        buffer.Clear();
        await space.Load(id1, buffer);
        Assert.Equal(finalData1, Encoding.UTF8.GetString(buffer.Data.Span));

        buffer.Clear();
        await space.Load(id2, buffer);
        Assert.Equal(finalData2, Encoding.UTF8.GetString(buffer.Data.Span));

        await space.Delete(id2);
    }
}