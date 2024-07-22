using System.Text;
using Markwardt;

DataSpace space = new(File.Open(@"C:\Users\cjmar\Markwardt\Sandbox\Test.db", FileMode.OpenOrCreate), 10);
int id = await space.Create(Encoding.UTF8.GetBytes("hello hello hello"));
Console.WriteLine(id);
DynamicBuffer buffer = new();
await space.Load(id, buffer);
Console.WriteLine(Encoding.UTF8.GetString(buffer.Data.Span));