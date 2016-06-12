using System.IO;
using System.Windows;

namespace cenzor
{
    class FileOperation
    {
        public static void Remove(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (IOException)
                {
                    MessageBoxResult result = MessageBox.Show("Cannot access file");
                }
            }
        }
    }
}
