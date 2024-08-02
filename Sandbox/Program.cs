using System.Numerics;
using System.Text;
using Markwardt;

DataTransformer transformer = new() { Target = new MemoryStream() };
transformer.WriteType("List[int]");

Console.WriteLine(transformer.Target.Length);

transformer.Target.Position = 0;
Console.WriteLine(transformer.Read());

/*DataSpace space = new(File.Open(@"C:\Users\cjmar\Markwardt\Sandbox\Test.db", FileMode.OpenOrCreate), 10);
int id = await space.Create(Encoding.UTF8.GetBytes("hello hello hello"));
Console.WriteLine(id);
DynamicBuffer buffer = new();
await space.Load(id, buffer);
Console.WriteLine(Encoding.UTF8.GetString(buffer.Data.Span));*/