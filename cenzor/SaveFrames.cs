using AForge.Video.FFMPEG;
using System.Drawing;

namespace cenzor
{
    class SaveFrames : VideoFileWriter
    {
        //Makes video file from frames(bitmaps saved on disk) and delete used frames.
        private string temporaryFilesPath;

        public SaveFrames(
            string filePath,
            int frameRate,
            int frameWidth,
            int frameHeight,
            string temporaryFilesPath,
            int bitRate
            )
        {
            this.temporaryFilesPath = temporaryFilesPath;
            Open(filePath, frameWidth, frameHeight, frameRate, VideoCodec.MPEG4, bitRate);
        }
        public void AddFrame(int actualFrame)
        {
            Bitmap frame = new Bitmap(temporaryFilesPath + actualFrame + ".bmp");
            WriteVideoFrame(frame);
            frame.Dispose();
        }
        public void RemoveFile(int actualFrame)
        {
            FileOperation.Remove(temporaryFilesPath + actualFrame + ".bmp");
        }
    }
}
