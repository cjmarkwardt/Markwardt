using System.Numerics;
using System.Text;
using Markwardt;

BigInteger value = -5000000000000000000;
byte[] buffer = new byte[16];
value.TryWriteBytes(buffer, out int written, true);
value = new(buffer, true);

Console.WriteLine(value);
return;

MemoryStream stream = new();
DataTransformer writer = new(stream);
writer.WriteInteger(5000000000000000000);

DataReader reader = new(stream.ToArray());
Console.WriteLine(reader.Read());

/*DataSpace space = new(File.Open(@"C:\Users\cjmar\Markwardt\Sandbox\Test.db", FileMode.OpenOrCreate), 10);
int id = await space.Create(Encoding.UTF8.GetBytes("hello hello hello"));
Console.WriteLine(id);
DynamicBuffer buffer = new();
await space.Load(id, buffer);
Console.WriteLine(Encoding.UTF8.GetString(buffer.Data.Span));*/