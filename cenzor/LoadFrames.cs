using System.Drawing;
using AForge.Video.FFMPEG;
using System.IO;

namespace cenzor
{
    class LoadFrames : VideoFileReader
    {
        //Read frame(image) from video file and save them into
        //temporary folder
        public string TemporaryFilesPath { get; private set; }

        public int FrameNumber { get; private set; } = 0;

        public LoadFrames(string videoFilePath)
        {
            Open(videoFilePath);
        }

        public void CreateTmp(string temporaryFilesPath)
        {
            TemporaryFilesPath = temporaryFilesPath;
            if (!Directory.Exists(TemporaryFilesPath))
            {
                Directory.CreateDirectory(TemporaryFilesPath);
            }
        }
        public void Load()
        {
            Bitmap videoFrame = ReadVideoFrame();
            FrameNumber++;
            videoFrame.Save(TemporaryFilesPath + FrameNumber + ".bmp",
            System.Drawing.Imaging.ImageFormat.Jpeg);
            videoFrame.Dispose();
        }
    }
}