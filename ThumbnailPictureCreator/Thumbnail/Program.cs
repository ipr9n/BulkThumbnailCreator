using System;
using Thumbnail.ThumbnailCreator;

namespace Thumbnail
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            ThumbnailMain thumbnail = new ThumbnailMain();
            thumbnail.Start();
            Console.ReadKey();
        }
    }
}
