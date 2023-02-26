// See https://aka.ms/new-console-template for more information
using Bild.Cmd;
using Bild.Core.Data;
using Bild.Core.Environment;
using Bild.Core.Files;
using Bild.Core.Importer;

Console.WriteLine("Hello, World!");

var p0 = new Meta(@"C:\tmp\img0001.jpg");
var p1 = new Meta(@"C:\tmp\img0002.jpg");
var p2 = new Meta(@"C:\tmp\img0003.jpg");

Console.WriteLine($@"Picture {p0.Filename} taken {p0.DateTime} type {p0.FileType}");
Console.WriteLine($@"Picture {p1.Filename} taken {p1.DateTime} type {p1.FileType}");
Console.WriteLine($@"Picture {p2.Filename} taken {p2.DateTime} type {p2.FileType}");

// var repo = new Repository(new FileConfig());

var album = new Album(new Settings()
{
	ProjectFolder = @"C:\tmp\proj"
});

album.PrepareDirImport(@"C:\tmp\import");
album.AdjustDirImport();
album.ExecuteDirImport();

Console.WriteLine(album.MediaType);
