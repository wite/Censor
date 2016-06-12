using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System;

namespace cenzor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private string temporaryFilesPath = "c:/cenzor/TmpCenzor/";
        //private string temporaryFilesPath = AppDomain.CurrentDomain.BaseDirectory + "TmpCenzor/";
        private string savePathName;
        public string loadPathName;
        private int firstFrame = 1;
        private long framesAmount;
        private int frameRate;
        private int bitRate = 200000;
        private int pixelate = 7;
        private int width;
        private int height;
        private Point mouseDownFrame;
        private bool isDragging = false;
        public ObservableCollection<Selections> censoredSelections = new ObservableCollection<Selections>();
        private LoadFrames Frames;

        private enum Work
        {
            LoadFrames,
            MakeCensor,
            MakeVideo
        };

        private Work workToDo;

        BackgroundWorker bw = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();
            // disable operators to prevent crashes
            buttonMakeCensor.IsEnabled = false;
            buttonMakeVideo.IsEnabled = false;
            buttonSavePath.IsEnabled = false;
            sliderCensorStart.IsEnabled = false;
            sliderCensorStop.IsEnabled = false;

            listBox.ItemsSource = censoredSelections;

            bw.WorkerSupportsCancellation = true;
            bw.WorkerReportsProgress = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
        }

        //-----------------------------------------------------------------
        //Sliders
        private void sliderCensorStart_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // ... Get Slider reference.
            Slider slider = sender as Slider;
            // ... Get Value
            int frameNumber = (int)slider.Value;

            LoadBitmapImage img = new LoadBitmapImage(frameNumber, temporaryFilesPath);
            imageVideoPreview.Source = img.LoadedBitmapImage;
            img.Dispose();
        }

        //-----------------------------------------------------------------
        //TextBox
        private void textBoxBitrate_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9]+"); 
            return !regex.IsMatch(text);
        }
        private void textBoxPixelate_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        //-----------------------------------------------------------------
        //Buttons
        private void buttonLoadVideo_Click(object sender, RoutedEventArgs e)
        {
            DialogBoxes dialogBox = new DialogBoxes();
            dialogBox.loadFile();

            if (dialogBox.Result == true)
            {
                if (bw.IsBusy != true)
                {
                    loadPathName = dialogBox.FileName;
                    workToDo = Work.LoadFrames;
                    bw.RunWorkerAsync();
                }
            }
        }

        private void buttonBitRate_Click(object sender, RoutedEventArgs e)
        {
            bitRate = Convert.ToInt32(textBoxBitrate.Text);
            MessageBoxResult result = MessageBox.Show("Bitrate set to " + bitRate);
        }
        private void buttonPixelateSet_Click(object sender, RoutedEventArgs e)
        {
            pixelate = Convert.ToInt32(textBoxPixelate.Text);
            MessageBoxResult result = MessageBox.Show("Pixelate set to " + pixelate);
        }
        
        private void buttonMakeCensor_Click(object sender, RoutedEventArgs e)
        {
            //translate global positioning into imageVideoPreview positioning
            Point selectionStart = SelectionRectangle.TranslatePoint(
                new Point(0, 0), imageVideoPreview);

            int beginY = (int)selectionStart.Y;
            int endY = beginY + (int)SelectionRectangle.Height;
            int beginX = (int)selectionStart.X;
            int endX = beginX + (int)SelectionRectangle.Width;
            int beginSelection = (int)sliderCensorStart.Value;
            int endSelection = (int)sliderCensorStop.Value;


            if (sliderCensorStop.Value > sliderCensorStart.Value)
            {
                Selections selectedCensor = new Selections(
                beginX,
                endX,
                beginY,
                endY,
                beginSelection,
                endSelection,
                temporaryFilesPath);

                buttonMakeCensor.IsEnabled = false;
                censoredSelections.Add(selectedCensor);
                workToDo = Work.MakeCensor;
                bw.RunWorkerAsync();
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Censor start must be greater than Censor stop");
            }

        }

        private void buttonSavePath_Click(object sender, RoutedEventArgs e)
        {
            DialogBoxes dialogBox = new DialogBoxes();
            dialogBox.saveFile();
            if (dialogBox.Result == true)
            {
                this.savePathName = dialogBox.FileName;
                buttonSavePath.Content = savePathName;
                buttonMakeVideo.IsEnabled = true;
            }
        }

        private void buttonMakeVideo_Click(object sender, RoutedEventArgs e)
        {
            //disable operators cause files was deleted
            sliderCensorStart.IsEnabled = false;
            sliderCensorStop.IsEnabled = false;
            textBoxSliderCensorStart.IsEnabled = false;
            textBoxSliderCensorStop.IsEnabled = false;
            buttonMakeCensor.IsEnabled = false;
            buttonSavePath.IsEnabled = false;
            buttonMakeVideo.IsEnabled = false;
            workToDo = Work.MakeVideo;
            bw.RunWorkerAsync();
        }

        //-----------------------------------------------------------------
        //Mouse

        private void imageVideoPreview_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //reseting selection
            ResetSelectionRectangle();
            //setting starting position
            this.mouseDownFrame.X = e.GetPosition(Selection).X;
            this.mouseDownFrame.Y = e.GetPosition(Selection).Y;
            this.isDragging = true;
        }

        private void imageVideoPreview_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                //selection starting position
                SelectionRectangle.SetValue(Canvas.LeftProperty, mouseDownFrame.X);
                SelectionRectangle.SetValue(Canvas.TopProperty, mouseDownFrame.Y);

                //selection draging position
                double x = e.GetPosition(Selection).X;
                double y = e.GetPosition(Selection).Y;

                //setting width
                double xWidth = x - mouseDownFrame.X;
                if (xWidth <= 0)
                {
                    xWidth = 0;
                }

                double yWidth = y - mouseDownFrame.Y;
                if (yWidth <= 0)
                {
                    yWidth = 0;
                }

                SelectionRectangle.Width = xWidth;
                SelectionRectangle.Height = yWidth;

                //making selection visible
                if (SelectionRectangle.Visibility != Visibility.Visible)
                {
                    SelectionRectangle.Visibility = Visibility.Visible;
                }

            }
        }

        private void Selection_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.isDragging = false;
        }

        private void ResetSelectionRectangle()
        {
            this.isDragging = false;
            SelectionRectangle.Visibility = Visibility.Collapsed;
        }

        //-----------------------------------------------------------------
        //Background worker - thread

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            switch (workToDo)
            {
                case Work.LoadFrames:
                    LoadFramesProcess(worker);
                    break;
                case Work.MakeCensor:
                    MakeCensorProcess(worker);
                    break;
                case Work.MakeVideo:
                    MakeVideoProcess(worker);
                    break;
            }
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.pbStatus.Value = (e.ProgressPercentage);
        }
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            switch (workToDo)
            {
                case Work.LoadFrames:
                    LoadFramesProcessCompleted();
                    break;
                case Work.MakeCensor:
                    MakeCensorProcessCompleted();
                    break;
                case Work.MakeVideo:
                    MakeVideoProcessCompleted();
                    break;
            }
        }

        //-----------------------------------------------------------------
        //Functions to Background worker

        private void LoadFramesProcess(BackgroundWorker worker)
        {
            Frames = new LoadFrames(loadPathName);
            Frames.CreateTmp(temporaryFilesPath);
            this.framesAmount = Frames.FrameCount;
            for (int frameNumber = firstFrame; frameNumber <= Frames.FrameCount; frameNumber++)
            {
                Frames.Load();
                int progressCalculated = CalculateProgress.Calculate(frameNumber, framesAmount);
                worker.ReportProgress(progressCalculated);
            }
            width = Frames.Width;
            height = Frames.Height;
            frameRate = Frames.FrameRate;
            Frames.Close();
        }

        private void MakeCensorProcess(BackgroundWorker worker)
        {
            Selections lastSelection = censoredSelections[censoredSelections.Count -1];
            for (int actualSelection = lastSelection.BeginSelection; 
                actualSelection <= lastSelection.EndSelection; actualSelection++)
            {
                ModifyImage modification = new ModifyImage(actualSelection, lastSelection, false);
                modification.pixelateSize = pixelate;
                modification.StartModification();
                if (PassException.exceptionThrown != null)
                {
                    break;
                }
                int framesAmount = lastSelection.EndSelection - lastSelection.BeginSelection;
                int frameNumber = actualSelection - lastSelection.BeginSelection;
                int progressCalculated = CalculateProgress.Calculate(frameNumber, framesAmount);
                worker.ReportProgress(progressCalculated);
            }
        }

        private void MakeVideoProcess(BackgroundWorker worker)
        {
            SaveFrames file = new SaveFrames(
                savePathName, 
                frameRate, 
                width, 
                height, 
                temporaryFilesPath, 
                bitRate);

            for (int frameNumber = firstFrame; frameNumber <= framesAmount; frameNumber++)
            {
                file.AddFrame(frameNumber);
                int progressCalculated = CalculateProgress.Calculate(frameNumber, framesAmount);
                worker.ReportProgress(progressCalculated);
                file.RemoveFile(frameNumber);
            }
            file.Close();
        }
        //-----------------------------------------------------------------
        //Functions to Background worker Completed

        private void LoadFramesProcessCompleted()
        {
            sliderCensorStart.IsEnabled = true;
            sliderCensorStop.IsEnabled = true;
            buttonMakeCensor.IsEnabled = true;

            //set slider to frames amount
            sliderCensorStart.Minimum = 1;
            sliderCensorStart.Maximum = framesAmount;
            sliderCensorStart.Value = 1;
            sliderCensorStop.Minimum = 1;
            sliderCensorStop.Maximum = framesAmount;
            textBoxSliderCensorStart.IsEnabled = true;
            textBoxSliderCensorStop.IsEnabled = true;
            buttonSavePath.IsEnabled = true;
            imageVideoPreview.Width = width;
            imageVideoPreview.Height = height;
            pbStatus.Value = 0;
            censoredSelections.Clear();
        }

        private void MakeCensorProcessCompleted()
        {
            if (PassException.exceptionThrown != null)
            {
                MessageBoxResult result = MessageBox.Show(PassException.exceptionThrown.Message);
                PassException.exceptionThrown = null;   
            }
            else
            { 
                MessageBoxResult result = MessageBox.Show("Censor Applied");
                buttonMakeCensor.IsEnabled = true;
            }
            pbStatus.Value = 0;
        }

        private void MakeVideoProcessCompleted()
        {
            MessageBoxResult result = MessageBox.Show("Video censored");
            pbStatus.Value = 0;
        }
    }
}
