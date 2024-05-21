namespace Markwardt;

public class DataSegmentTests
{
    [Fact]
    public void Test()
    {
        TypeSetSource types = new([typeof(ITestSegment), typeof(ISubSegment), typeof(ISubSegment1), typeof(ISubSegment1A), typeof(ISubSegment1B), typeof(ISubSegment2)]);
        ITestSegment test = new DataDictionary().AsSegment<ITestSegment>(new DataSegmentTyper() { Types = types }, new DataHandler());

        Assert.Null(test.ValueA);
        Assert.Null(test.ValueB);
        Assert.Null(test.ValueC);
        Assert.Equal(0, test.List.Count);
        Assert.Equal(0, test.Dictionary.Count);
        Assert.False(test.MainSub.HasValue);
        Assert.Equal(0, test.Subs.Count);
        Assert.Equal(0, test.Other.Count);

        test.ValueA = "test";
        test.ValueB = 50;
        test.ValueC = new DateTime(50000, DateTimeKind.Local);
        test.List.Add(5.5f);
        test.List.Add(1);
        test.Dictionary.Add(2, "test1");
        test.Dictionary.Add(4, "test2");
        test.Dictionary.Add(5, "test3");
        ISubSegment mainSub = test.MainSub.Set();
        ISubSegment1A sub1 = test.Subs.Add<ISubSegment1A>("key1");
        ISubSegment1B sub2 = test.Subs.Add<ISubSegment1B>("key2");
        ISubSegment2 sub3 = test.Subs.Add<ISubSegment2>("key3");
        test.Other.Add<ISubSegment>().Name = "blah";

        Assert.Equal("test", test.ValueA);
        Assert.Equal(50, test.ValueB);
        Assert.Equal(new DateTime(50000, DateTimeKind.Local), test.ValueC);
        Assert.Equal(2, test.List.Count);
        Assert.Equal(5.5f, test.List[0]);
        Assert.Equal(1, test.List[1]);
        Assert.Equal(3, test.Dictionary.Count);
        Assert.Equal("test1", test.Dictionary[2]);
        Assert.Equal("test2", test.Dictionary[4]);
        Assert.Equal("test3", test.Dictionary[5]);
        Assert.True(test.MainSub.HasValue);
        Assert.True(test.MainSub.Is<ISubSegment>());
        Assert.Equal(3, test.Subs.Count);
        Assert.NotNull(test.Subs.Get<ISubSegment1A>("key1"));
        Assert.NotNull(test.Subs.Get<ISubSegment1B>("key2"));
        Assert.NotNull(test.Subs.Get<ISubSegment2>("key3"));
        Assert.Equal("blah", test.Other.Get<ISubSegment>(0).Name);
        Assert.True(test.Other.Is<ISubSegment>(0));
        Assert.False(test.Other.Is<ISubSegment1A>(0));
        Assert.True(test.Other.Is<object>(0));

        mainSub.Name = "mainSub";
        sub1.Name = "sub1";
        sub1.Value = 5;
        sub1.List.Add("s1");
        sub1.List.Add("s2");
        sub2.Name = "sub2";
        sub2.Value = 10;
        sub2.Test.Set().ValueA = "sub2Test";
        sub2.Subs.Add(1000).Value = 50;
        sub2.Subs.Add(2000).Value = 55;
        sub3.Name = "sub3";
        sub3.Subs.Add().Name = "sub3Sub";

        Assert.Equal("mainSub", test.MainSub.Get().Name);
        Assert.Equal("sub1", test.Subs.Get<ISubSegment>("key1").Name);
        Assert.Equal((byte)5, test.Subs.Get<ISubSegment1>("key1").Value);
        Assert.Equal(["s1", "s2"], test.Subs.Get<ISubSegment1A>("key1").List);
        Assert.Equal("sub2", test.Subs.Get<ISubSegment>("key2").Name);
        Assert.Equal((byte)10, test.Subs.Get<ISubSegment1>("key2").Value);
        Assert.Equal("sub2Test", test.Subs.Get<ISubSegment1B>("key2").Test.Get().ValueA);
        Assert.Equal((byte)50, test.Subs.Get<ISubSegment1B>("key2").Subs.Get<ISubSegment1>(1000).Value);
        Assert.Equal((byte)55, test.Subs.Get<ISubSegment1B>("key2").Subs.Get<ISubSegment1>(2000).Value);
        Assert.Equal("sub3", test.Subs.Get<ISubSegment>("key3").Name);
        Assert.Equal("sub3Sub", test.Subs.Get<ISubSegment2>("key3").Subs.Get(0).Name);

        test.ValueA = null;
        test.ValueB = null;
        test.List.Clear();
        test.Dictionary.Clear();
        test.MainSub.Clear();
        test.Subs.Clear();

        Assert.Null(test.ValueA);
        Assert.Null(test.ValueB);
        Assert.NotNull(test.ValueC);
        Assert.Equal(0, test.List.Count);
        Assert.Equal(0, test.Dictionary.Count);
        Assert.False(test.MainSub.HasValue);
        Assert.Equal(0, test.Subs.Count);
    }

    [Segment("Test")]
    public interface ITestSegment
    {
        public string? ValueA { get; set; }
        public int? ValueB { get; set; }
        public DateTime? ValueC { get; set; }

        public IList<float> List { get; }
        public IDictionary<int, string> Dictionary { get; }

        public ISegmentSlot<ISubSegment> MainSub { get; }
        public ISegmentDictionary<string, ISubSegment> Subs { get; }
        public ISegmentList<object> Other { get; }
    }

    [Segment("Sub")]
    public interface ISubSegment
    {
        public string? Name { get; set; }
    }

    [Segment("Sub1")]
    public interface ISubSegment1 : ISubSegment
    {
        public byte? Value { get; set; }
    }

    [Segment("Sub1A")]
    public interface ISubSegment1A : ISubSegment1
    {
        public IList<string> List { get; }
    }

    [Segment("Sub1B")]
    public interface ISubSegment1B : ISubSegment1
    {
        public ISegmentSlot<ITestSegment> Test { get; }
        public ISegmentDictionary<long, ISubSegment1> Subs { get; }
    }

    [Segment("Sub2")]
    public interface ISubSegment2 : ISubSegment
    {
        public ISegmentList<ISubSegment> Subs { get; }
    }
}