using System.Configuration;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace Thumbnail.ThumbnailCreator
{
    class ThumbnailMain
    {
        readonly string _pathWithImage = ConfigurationManager.AppSettings["Path"];
        readonly string _pathToSave = ConfigurationManager.AppSettings["NewPath"];

        private readonly Point _newSize = GetSizeFromConfig();

        private int ThreadCount { get; }

        public ThumbnailMain() => ThreadCount = 1;

        public ThumbnailMain(int threads) => ThreadCount = threads;

        private static Point GetSizeFromConfig()
        {
            try
            {
                return new Point(Convert.ToInt32(ConfigurationManager.AppSettings["NewSize"].Split(',')[0]),
                    Convert.ToInt32(ConfigurationManager.AppSettings["NewSize"].Split(',')[1]));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Error with get size. Set Default size 600,800");
                return new Point(600,800);
            }
         
        }

        public void Start()
        {
            for (int i = 0; i < ThreadCount; i++)
            {
                Console.WriteLine("Try");
                Task<Bitmap> resizeTask = new Task<Bitmap>(() => Resize(GetRandomImage(_pathWithImage)));
                Task renameAndSaveTask = resizeTask.ContinueWith(RenameAndSave);

                resizeTask.Start();
            }
        }

        private Bitmap Resize(Image image)
        {
            if (image != null)
            {
                Console.WriteLine($"Try to resize in Task: {Task.CurrentId}");

                Bitmap myImageBitmap = new Bitmap(image, _newSize.X, _newSize.Y);
                myImageBitmap.SetResolution(_newSize.X, _newSize.Y);

                return myImageBitmap;
            }
            else
            {
                return null;
            }
        }

        private void RenameAndSave(Task<Bitmap> image)
        {
            if (image.Result != null)
            {
                Console.WriteLine($"Try to save in Task: {Task.CurrentId}(Previously: {image.Id}");

                Bitmap myImage = image.Result;
                myImage.Save($"{_pathToSave}{new Random().Next(9999999)}.jpg");

                if (IsJpgExist(_pathWithImage)) Start();
            }
        }

        private Image GetRandomImage(string path)
        {
            try
            {
                string randomImagePath =
                    Directory.GetFiles(path, "*.jpg")[
                        new Random().Next(Directory.GetFiles(path, "*.jpg").Length - 1)];
                Image myImage;

                using (var imgStream = File.OpenRead(randomImagePath))
                    myImage = Image.FromStream(imgStream);

                File.Delete(randomImagePath);

                return myImage;
            }
            catch (Exception e)
            {
                Console.WriteLine($"end {Task.CurrentId}");
                return null;
            }
        }

        private static bool IsJpgExist(string path)
        {
            try
            {
                if (Directory.GetFiles(path, "*.jpg").Length > 0) return true;
                else return false;
            }
            catch (Exception e)
            {
                Console.WriteLine($"The process failed: {e.ToString()}");
                return false;
            }
        }
    }
}
