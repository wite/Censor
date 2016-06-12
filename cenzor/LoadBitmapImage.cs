using System.IO;
using System.Windows.Media.Imaging;

namespace cenzor
{
    class LoadBitmapImage : IDisposable
    {
        ////Make BitmapImage from file through stream which is disposable
        public BitmapImage LoadedBitmapImage
        {
            get; private set;
        }
        private FileStream bitmapStream;
        public LoadBitmapImage(int frameNumber, string filePath)
        {
            LoadedBitmapImage = new BitmapImage();
            bitmapStream = File.OpenRead(filePath + frameNumber + ".bmp");
            LoadedBitmapImage.BeginInit();
            LoadedBitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            LoadedBitmapImage.StreamSource = bitmapStream;
            LoadedBitmapImage.EndInit();
        }
        public void Dispose()
        {
            bitmapStream.Close();
            bitmapStream.Dispose();
        }
    }
}