using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace cenzor
{
    class ModifyImage : IDisposable
    { 
        public int pixelateSize
        { get; set; } = 7;
        public Bitmap BitmapToOutput
        { get; set; }
        public Color customColorValue
        { get; set; }
        private int beginX;
        private int endX;
        private int beginY;
        private int endY;
        private int actualSelection;
        private string temporaryFilesPath;
        private Color pixelColor;
        private Bitmap bitmapFromInput;
        private LoadBitmap loadedBitmap;
        private LockBitmap lockBitmap;
        enum SetColor
        {
            fromImage,
            custom
        }
        private SetColor setColor;


        public ModifyImage(int actualSelection, Selections selectionData, bool customColor)
        {

            this.beginX = selectionData.beginX;
            this.endX = selectionData.endX;
            this.beginY = selectionData.beginY;
            this.endY = selectionData.endY;
            this.temporaryFilesPath = selectionData.temporaryFilesPath;
            this.actualSelection = actualSelection;

            if (customColor == true)
            {
                setColor = SetColor.custom;
            }
            else
            {
                setColor = SetColor.fromImage;
            }
            
            loadedBitmap = new LoadBitmap(actualSelection, temporaryFilesPath);
            this.bitmapFromInput = loadedBitmap.BitmapFromStream;
        }

        public void StartModification()
        {
            lockBitmap = new LockBitmap(bitmapFromInput);
            lockBitmap.LockBits();
            try
            {
                IterateThroughSelection();
            }
            catch (IndexOutOfRangeException ex)
            {
                PassException.exceptionThrown = new IndexOutOfRangeException("Pixelate is too big",ex);
            }

            lockBitmap.UnlockBits();
            this.BitmapToOutput = (Bitmap)bitmapFromInput.Clone();
            bitmapFromInput.Dispose();
            loadedBitmap.Dispose();
            Save();
        }

        private void Save()
        {
            try
            {
                BitmapToOutput.Save(temporaryFilesPath + actualSelection + ".bmp", System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (ExternalException ex)
            {  
                PassException.exceptionThrown = new ExternalException("No access to file. Change tmp directory", ex);
            }
            finally
            {
                BitmapToOutput.Dispose();
            }
        }

        private void pixelColorSet(int actualX, int actualY)
        {
            switch (setColor)
            {
                case SetColor.fromImage:
                    pixelColor = lockBitmap.GetPixel(actualX, actualY);
                    break;
                case SetColor.custom:                    
                    pixelColor = customColorValue;
                    break;
            }        
        }
        private void IterateThroughSelection()
        {
            for (int actualY = beginY; actualY < endY; actualY = actualY + pixelateSize)
            {
                if (actualY + pixelateSize > lockBitmap.Height)
                {
                    break;
                }
                for (int actualX = beginX; actualX < endX; actualX = actualX + pixelateSize)
                {
                    if (actualX + pixelateSize > lockBitmap.Width)
                    {
                        break;
                    }
                    pixelColorSet(actualX, actualY);
                    PixelateAreaOfPixelateSize(actualX, actualY);
                }
            }
        }
        private void PixelateAreaOfPixelateSize(int actualX, int actualY)
        {
            for (int i = 0; i < pixelateSize; i++)
            {
                for (int j = 0; j < pixelateSize; j++)
                {
                    lockBitmap.GetPixel(actualX + i, actualY + j);
                    lockBitmap.SetPixel(actualX + i, actualY + j, pixelColor);
                }
            }
        }

        public void Dispose()
        {
            bitmapFromInput.Dispose();
            BitmapToOutput.Dispose();
        } 
    }
}
