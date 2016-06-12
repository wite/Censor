using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cenzor
{
    class DialogBoxes
    {
        //Show dialog box to save or load file
        public Nullable<bool> Result
        {
            get; private set;
        }
        public string FileName
        {
            get;  private set;
        }
        private string defaultExtension = ".avi";
        private string defaultFilter = "AVI Files (*.avi)|*.avi|ALL Files (*.*)|*.*";
        public void saveFile()
        {
            SaveFileDialog DialogBox = new SaveFileDialog();
            DialogBox.DefaultExt = defaultExtension;
            DialogBox.Filter = defaultFilter;
            this.Result = DialogBox.ShowDialog();
            this.FileName = DialogBox.FileName;
            
        }

        public void loadFile()
        { 
            OpenFileDialog DialogBox = new OpenFileDialog();
            DialogBox.DefaultExt = defaultExtension;
            DialogBox.Filter = defaultFilter;
            this.Result = DialogBox.ShowDialog();
            this.FileName = DialogBox.FileName;
        }
    }
}