# DynamicDB
DynamicDB是一种NoSQL数据库。一个实例下可以创建多种Database，一个Database中可以包含多个Collection（相当于关系型数据库中的Table），一个Collection中包含多个对象。在一个Collection中包含对象的对象可以为不同类型（但推荐使用同一类型，或具有共同的祖先）。DynamicDB提供Linq脚本的执行能力。并可执行C#、VB.Net等DotNet平台支持的语言脚本。DynamicDB由C#语言编写，可在Windows系统上运行。支持TCP/IP、Http、NamedPipe等传输方式。

创建数据库
Compete.DynamicDB.NetDBInstance instance = new Compete.DynamicDB.NetDBInstance();
instance.CreateDatabase("Database");

创建Collection
Compete.DynamicDB.NetDatabase database = new Compete.DynamicDB.NetDatabase() { Name = "Database" };
database.CreateCollection("Collection");

插入数据
database.Insert("Collection", new { Id = Guid.NewGuid(), Code = "10101", Name = "abc123" });

执行Linq脚本
var count = database.Query<int>("Database[\"Collection\"].Count()");


http://www.cnblogs.com/competesoft/p/6236840.html
