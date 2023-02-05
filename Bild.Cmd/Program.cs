// See https://aka.ms/new-console-template for more information
using Bild.Cmd;
using Bild.Core.Context;
using Bild.Core.Data;
using Bild.Core.Environment;

Console.WriteLine("Hello, World!");

var p0 = new Pic(@"C:\tmp\img0001.jpg");
var p1 = new Pic(@"C:\tmp\img0002.jpg");
var p2 = new Pic(@"C:\tmp\img0003.jpg");

Console.WriteLine($@"Picture {p0.Filename} taken {p0.DateTime}");
Console.WriteLine($@"Picture {p1.Filename} taken {p1.DateTime}");
Console.WriteLine($@"Picture {p2.Filename} taken {p2.DateTime}");

var repo = new Repository(new FileConfig());

var pool = new PicPool(new Repository(new FileConfig()));

pool.Import("C:\\tmp\\");
