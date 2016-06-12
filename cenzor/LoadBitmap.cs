using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace cenzor
{
    class LoadBitmap : IDisposable
    {
        //Make Bitmap from file and load into stream. Now Bitmap is disposable
        //and possible to remove from drive. which is made.
        private Bitmap savedBitmap;
        private MemoryStream streamBitmap;
        public Bitmap BitmapFromStream { get; private set; }
        public LoadBitmap(int frameNumber, string filePath)
        {
            savedBitmap = new Bitmap(filePath + frameNumber + ".bmp");
            streamBitmap = new MemoryStream();
            savedBitmap.Save(streamBitmap, ImageFormat.Bmp);
            savedBitmap.Dispose();
            BitmapFromStream = new Bitmap(streamBitmap);
            FileOperation.Remove(filePath + frameNumber + ".bmp");
        }
        public void Dispose()
        {
            streamBitmap.Close();
            BitmapFromStream.Dispose();
        }
    }
}
