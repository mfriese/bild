//Console.WriteLine("Hello, World!");

//var files = Finder.FindFiles(new Bild.Core.Files.Dir("/Users/mfriese/Pictures/"));

//foreach (var file in files)
//{
//    Console.WriteLine(file.AbsolutePath);

//    if (file.ExifCreationDate is not null)
//        Console.WriteLine("Created exif: " + file.ExifCreationDate);
//    else
//        Console.WriteLine("Created file: " + file.FileCreationDate);

//    Console.WriteLine(file.ExifFileType);
//    Console.WriteLine(file.Filename);
//    Console.WriteLine();
//}

var program = new Bild.Core.Program();
program.Execute();
