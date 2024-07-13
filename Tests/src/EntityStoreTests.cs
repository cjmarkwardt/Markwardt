namespace Markwardt;

public class EntityStoreTests
{
    /*[Fact]
    public async Task Test()
    {
        TypeSetSource types = new([typeof(ITestSection), typeof(ITestItem)]);
        await using EntityStore store = new(new DataSegmentTyper() { Types = types }, new DataHandler(), new EntityIdCreator(), new DataStore());

        IEntityClaim entity = store.Create("test");
        Assert.False(entity.HasFlag("testFlag"));
        entity.SetFlag("testFlag");
        Assert.True(entity.HasFlag("testFlag"));
        entity.Dispose();

        entity = await store.Load("test");
        Assert.True(entity.HasFlag("testFlag"));
        entity.ClearFlag("testFlag");
        Assert.False(entity.HasFlag("testFlag"));
        entity.Dispose();

        entity = store.Create();
        EntityId id = entity.Id;
        Assert.False(entity.ContainsSection<ITestSection>());
        ITestSection section = entity.GetSection<ITestSection>();
        Assert.True(entity.ContainsSection<ITestSection>());
        Assert.Null(section.Name);
        Assert.Equal(0, section.Values.Count);
        Assert.Equal(0, section.Items.Count);

        section.Name = "value";
        section.Values.Add(1);
        section.Values.Add(5);
        section.Values.Add(21);
        section.Items.Add();
        section.Items.Add().Value = "item";

        Assert.Equal("value", section.Name);
        Assert.Equal(3, section.Values.Count);
        Assert.Equal([1, 5, 21], section.Values);
        Assert.Equal(2, section.Items.Count);
        Assert.Null(section.Items.Get(0).Value);
        Assert.Equal("item", section.Items.Get(1).Value);

        entity.Dispose();

        entity = await store.Load(id);
        section = entity.GetSection<ITestSection>();

        Assert.True(entity.ContainsSection<ITestSection>());
        Assert.Equal("value", section.Name);
        Assert.Equal(3, section.Values.Count);
        Assert.Equal([1, 5, 21], section.Values);
        Assert.Equal(2, section.Items.Count);
        Assert.Null(section.Items.Get(0).Value);
        Assert.Equal("item", section.Items.Get(1).Value);

        section.Name = null;
        section.Values.Clear();
        section.Items.Clear();

        Assert.Null(section.Name);
        Assert.Equal(0, section.Values.Count);
        Assert.Equal(0, section.Items.Count);

        entity.DeleteSection<ITestSection>();
        Assert.False(entity.ContainsSection<ITestSection>());

        entity.Dispose();
    }

    [Segment("Test")]
    public interface ITestSection
    {
        string? Name { get; set; }
        IList<int> Values { get; }
        ISegmentList<ITestItem> Items { get; }
    }

    [Segment("Item")]
    public interface ITestItem
    {
        string? Value { get; set; }
    }*/
}