using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

/* Libraries Not Used
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
*/

namespace Hyperspec_VideoProcessing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string DefaultPath  = @"X:\MultispectralFace_2016\Session_2_SecCam\";
        private string postProcessPath;
        private string RID;
        private string date;
        private string dateFolder;
        private string sessionName;
        private string basePath;
    
        public MainWindow()
        {
            InitializeComponent();

            RID = string.Empty;
            date = string.Empty;
            sessionName = string.Empty;

            postProcessPath = DefaultPath;
        }

        #region GUI and Barcode Functions

        private void AxisCameraSelector_Checked(object sender, RoutedEventArgs e)
        {
            ObservableCollection<string> axisVideoList = new ObservableCollection<string>();
            axisVideoList.Add("Ground Truth");
            axisVideoList.Add("670nm10");
            axisVideoList.Add("690nm10");
            axisVideoList.Add("710nm10");
            axisVideoList.Add("730nm10");

            VideoComboBox.ItemsSource = axisVideoList;
            VideoComboBox.SelectedIndex = 0;
            VideoComboBox.IsEnabled = true;

            PictureStatusCheck();
        }

        private void EOSCameraSelector_Checked(object sender, RoutedEventArgs e)
        {
            ObservableCollection<string> eosVideoList = new ObservableCollection<string>();
            eosVideoList.Add("Rotation 1");
            eosVideoList.Add("Rotation 2");
            eosVideoList.Add("Rotation 3");
            eosVideoList.Add("Rotation 4");
            eosVideoList.Add("Rotation 5");

            VideoComboBox.ItemsSource = eosVideoList;
            VideoComboBox.SelectedIndex = 0;
            VideoComboBox.IsEnabled = true;

            PictureStatusCheck();
        }

        private async void PictureStatusCheck()
        {
            await Task.Run(() => CheckStillFiles());
        }

        private async Task CheckStillFiles()
        {
            // If you want the picture status block to temporarily have a different color when looking for files, uncomment invoke block below
            /*
            this.Dispatcher.Invoke(() => {
                PictureStatus.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0x4A, 0x4A, 0x4A));
            });
            //*/
            await Task.Delay(10);

            // Get path to stills and check if any files are inside folder
            string stillPath = basePath;

            this.Dispatcher.Invoke(() => {
                // Axis camera is currently selected
                if (AxisCameraSelector.IsChecked != null && AxisCameraSelector.IsChecked == true)
                {
                    stillPath += @"Axis V5914\Still\" + RID + @"\";
                    switch(VideoComboBox.SelectedIndex)
                    {
                        case 0:
                            stillPath += @"GT\";
                            break;
                        case 1:
                            stillPath += @"670nm10\";
                            break;
                        case 2:
                            stillPath += @"690nm10\";
                            break;
                        case 3:
                            stillPath += @"710nm10\";
                            break;
                        case 4:
                            stillPath += @"730nm10\";
                            break;
                    }
                }

                // Cannon camera is currently selected
                else
                {
                    stillPath += @"EOS 5DS R\Still\" + RID + @"\";
                    switch(VideoComboBox.SelectedIndex)
                    {
                        case 0:
                            stillPath += @"Rotation_1\";
                            break;
                        case 1:
                            stillPath += @"Rotation_2\";
                            break;
                        case 2:
                            stillPath += @"Rotation_3\";
                            break;
                        case 3:
                            stillPath += @"Rotation_4\";
                            break;
                        case 4:
                            stillPath += @"Rotation_5\";
                            break;
                    }
                }
            });

            if (Directory.Exists(stillPath))
            {
                // Directory contains some files
                if (Directory.GetFileSystemEntries(stillPath).Length > 0)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        PictureStatus.Background = System.Windows.Media.Brushes.LightGreen;
                    });
                }

                // Directory does not have any files
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        PictureStatus.Background = System.Windows.Media.Brushes.LightCoral;
                    });
                }
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    PictureStatus.Background = System.Windows.Media.Brushes.LightCoral;
                });
            }
        }

        private void VideoComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(VideoComboBox.HasItems)
                PictureStatusCheck();
        }

        private void ClearSessionButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear out information on main window
            RIDTextBox.Text = string.Empty;
            DateTextBox.Text = string.Empty;
            RID = string.Empty;
            date = string.Empty;
            dateFolder = string.Empty;

            RIDTextBox.IsEnabled = true;
            DateTextBox.IsEnabled = true;
            ScanSubjectButton.IsEnabled = true;
            
            AxisCameraSelector.IsEnabled = false;
            AxisCameraSelector.IsChecked = false;
            EOSCameraSelector.IsEnabled = false;
            EOSCameraSelector.IsChecked = false;

            VideoComboBox.ItemsSource = null;
            VideoComboBox.IsEnabled = false;
            PictureStatus.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0x4A, 0x4A, 0x4A));

            OpenVideoButton.IsEnabled = false;
            ClearSessionButton.IsEnabled = false;
        }

        private void ScanSubjectButton_Click(object sender, RoutedEventArgs e)
        {
            if(ReadSubjectInfo())
            {
                RIDTextBox.IsEnabled = false;
                DateTextBox.IsEnabled = false;
                ScanSubjectButton.IsEnabled = false;

                AxisCameraSelector.IsEnabled = true;
                AxisCameraSelector.IsChecked = true;
                EOSCameraSelector.IsEnabled = true;

                OpenVideoButton.IsEnabled = true;
                ClearSessionButton.IsEnabled = true;

                // Check and notify if one or both of the segmented folders are missing
                if(!Directory.Exists(basePath + @"\Axis V5914\Video\Segmented\" + RID))
                {
                    // Dialog box to inform can't find segmented RID for AXIS
                    MessageBox.Show("The Axis segmented folder could not be found\n\nPlease check the path configurations", "Segmented Folder Not Found", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                if(!Directory.Exists(basePath + @"\EOS 5DS R\Video\Segmented\" + RID))
                {
                    // Dialog box to inform can't find segmented RID for EOS
                    MessageBox.Show("The EOS segmented folder could not be found\n\nPlease check the path configurations", "Segmented Folder Not Found", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        private bool ReadSubjectInfo()
        {
            bool result = true;

            // Check RID length
            if(RIDTextBox.Text.Length == 0 || RIDTextBox.Text.Length != 7)
            {
                MessageBox.Show("Enter a valid RID before scanning", "RID Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                result = false;
            }

            // Check Date length
            else if(DateTextBox.Text.Length == 0 || DateTextBox.Text.Length != 8)
            {
                MessageBox.Show("Enter a valid date before scanning", "Date Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                result = false;
            }

            else
            {
                RID = RIDTextBox.Text;
                string[] dateSplit = DateTextBox.Text.Split('/'); // Split date by '/' just in case user entered date as "MM/DD/YYYY"

                // Save concatenated string into date and set the dateFolder (which is formatted as "YYYYMMDD")
                if(dateSplit.Length == 1)
                {
                    date = dateSplit[0];
                    dateFolder = date.Substring(4,4) + date.Substring(0,4);
                }
                else if(dateSplit.Length == 3)
                {
                    date = dateSplit[0] + dateSplit[1] + dateSplit[2];
                    dateFolder = dateSplit[2] + dateSplit[0] + dateSplit[1];
                }
                else
                {
                    MessageBox.Show("Date format is incorrect", "Date Format Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    result = false;
                }

                // Get the session name and check if not an error
                if(!GetSessionName())
                {
                    MessageBox.Show("Could not find the session for the RID on the given date\n\nMake sure that the date entered is correct", "Session Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    result = false;
                }

            }
            return result;
        }

        private bool GetSessionName()
        {
            bool sessionFound = false;
            sessionName = "Error";

            // Search through each session folder and check if RID exists within session
            basePath = postProcessPath + dateFolder + @"\";

            // Check Noon session
            if(Directory.Exists(basePath + @"Noon\Axis V5914\Video\Segmented\" + RID) || Directory.Exists(basePath + @"Noon\EOS 5DS R\Video\Segmented\" + RID))
            {
                basePath += @"Noon\";
                sessionName = "Noon";
                sessionFound = true;
            }

            // Check Afternoon session
            else if(Directory.Exists(basePath + @"Afternoon\Axis V5914\Video\Segmented\" + RID) || Directory.Exists(basePath + @"Afternoon\EOS 5DS R\Video\Segmented\" + RID))
            {
                basePath += @"Afternoon\";
                sessionName = "Afternoon";
                sessionFound = true;
            }

            // Check Night Session
            else if(Directory.Exists(basePath + @"Night\Axis V5914\Video\Segmented\" + RID) || Directory.Exists(basePath + @"Night\EOS 5DS R\Video\Segmented\" + RID))
            {
                basePath += @"Night\";
                sessionName = "Night";
                sessionFound = true;
            }

            return sessionFound;
        }

        #endregion

        #region Open Video Functions

        private void OpenVideoButton_Click(object sender, RoutedEventArgs e)
        {
            string videoPath = GetVideoPath();

            if(videoPath != string.Empty)
            {
                VideoProcessing videoWindow = new VideoProcessing(videoPath, GetOutputDirectory(), GetFileName());
                videoWindow.ShowDialog();
                PictureStatusCheck();
            }
            else
            {
                MessageBox.Show("The selected video could not be found or accessed", "Video Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetVideoPath()
        {
            string videoPath = string.Empty;

            if(AxisCameraSelector.IsChecked != null && AxisCameraSelector.IsChecked == true)
            {
                videoPath = basePath + @"Axis V5914\Video\Segmented\" + RID + @"\";
                switch(VideoComboBox.SelectedIndex)
                {
                    case 0:
                        videoPath = Directory.GetFiles(videoPath,"*_GT.mp4",SearchOption.TopDirectoryOnly).FirstOrDefault();
                        break;
                    case 1:
                        videoPath = Directory.GetFiles(videoPath,"*_670nm10.mp4",SearchOption.TopDirectoryOnly).FirstOrDefault();
                        break;
                    case 2:
                        videoPath = Directory.GetFiles(videoPath,"*_690nm10.mp4",SearchOption.TopDirectoryOnly).FirstOrDefault();
                        break;
                    case 3:
                        videoPath = Directory.GetFiles(videoPath,"*_710nm10.mp4",SearchOption.TopDirectoryOnly).FirstOrDefault();
                        break;
                    case 4:
                        videoPath = Directory.GetFiles(videoPath,"*_730nm10.mp4",SearchOption.TopDirectoryOnly).FirstOrDefault();
                        break;
                }
            }
            else if(EOSCameraSelector.IsChecked != null && EOSCameraSelector.IsChecked == true)
            {
                videoPath = basePath + @"EOS 5DS R\Video\Segmented\" + RID + @"\";
                switch(VideoComboBox.SelectedIndex)
                {
                    case 0:
                        videoPath = Directory.GetFiles(videoPath,"*_Rotation1.mp4",SearchOption.TopDirectoryOnly).FirstOrDefault();
                        break;
                    case 1:
                        videoPath = Directory.GetFiles(videoPath,"*_Rotation2.mp4",SearchOption.TopDirectoryOnly).FirstOrDefault();
                        break;
                    case 2:
                        videoPath = Directory.GetFiles(videoPath,"*_Rotation3.mp4",SearchOption.TopDirectoryOnly).FirstOrDefault();
                        break;
                    case 3:
                        videoPath = Directory.GetFiles(videoPath,"*_Rotation4.mp4",SearchOption.TopDirectoryOnly).FirstOrDefault();
                        break;
                    case 4:
                        videoPath = Directory.GetFiles(videoPath,"*_Rotation5.mp4",SearchOption.TopDirectoryOnly).FirstOrDefault();
                        break;
                }
            }

            if(videoPath == null)
                videoPath = string.Empty;

            return videoPath;
        }

        private string GetOutputDirectory()
        {
            string outputPath = string.Empty;

            if(AxisCameraSelector.IsChecked != null && AxisCameraSelector.IsChecked == true)
            {
                outputPath = basePath + @"Axis V5914\Still\" + RID + @"\";
                switch(VideoComboBox.SelectedIndex)
                {
                    case 0:
                        outputPath += @"GT\";
                        break;
                    case 1:
                        outputPath += @"670nm10\";
                        break;
                    case 2:
                        outputPath += @"690nm10\";
                        break;
                    case 3:
                        outputPath += @"710nm10\";
                        break;
                    case 4:
                        outputPath += @"730nm10\";
                        break;
                }
            }
            else if(EOSCameraSelector.IsChecked != null && EOSCameraSelector.IsChecked == true)
            {
                outputPath = basePath + @"EOS 5DS R\Still\" + RID + @"\";
                switch(VideoComboBox.SelectedIndex)
                {
                    case 0:
                        outputPath += @"Rotation_1\";
                        break;
                    case 1:
                        outputPath += @"Rotation_2\";
                        break;
                    case 2:
                        outputPath += @"Rotation_3\";
                        break;
                    case 3:
                        outputPath += @"Rotation_4\";
                        break;
                    case 4:
                        outputPath += @"Rotation_5\";
                        break;
                }
            }

            try
            {
                // Make sure path is created
                Directory.CreateDirectory(outputPath); 
            }
            catch
            {
                // Maybe create message box informing that directory was unable to be created + exception name
            }

            return outputPath;
        }

        private string GetFileName()
        {
            string fileName = string.Empty;

            if(AxisCameraSelector.IsChecked != null && AxisCameraSelector.IsChecked == true)
            {
                switch(VideoComboBox.SelectedIndex)
                {
                    case 0:
                        fileName = RID + "_" + date + @"_22_AXIS_GT_%03d.bmp";
                        break;
                    case 1:
                        fileName = RID + "_" + date + @"_22_AXIS_670nm10_%03d.bmp";
                        break;
                    case 2:
                        fileName = RID + "_" + date + @"_22_AXIS_690nm10_%03d.bmp";
                        break;
                    case 3:
                        fileName = RID + "_" + date + @"_22_AXIS_710nm10_%03d.bmp";
                        break;
                    case 4:
                        fileName = RID + "_" + date + @"_22_AXIS_730nm10_%03d.bmp";
                        break;
                }
            }
            else if(EOSCameraSelector.IsChecked != null && EOSCameraSelector.IsChecked == true)
            {
                switch(VideoComboBox.SelectedIndex)
                {
                    case 0:
                        fileName = RID + "_" + date + @"_22_5DSR_R1_%03d.bmp";
                        break;
                    case 1:
                        fileName = RID + "_" + date + @"_22_5DSR_R2_%03d.bmp";
                        break;
                    case 2:
                        fileName = RID + "_" + date + @"_22_5DSR_R3_%03d.bmp";
                        break;
                    case 3:
                        fileName = RID + "_" + date + @"_22_5DSR_R4_%03d.bmp";
                        break;
                    case 4:
                        fileName = RID + "_" + date + @"_22_5DSR_R5_%03d.bmp";
                        break;
                }
            }

            return fileName;
        }

        #endregion

        #region Path Configuration and Connection Status Functions

        private void PathConfigButton_Click(object sender, RoutedEventArgs e)
        {
            // Open Path Config Dialog
            PathConfigBox pathConfig = new PathConfigBox(postProcessPath);

            if(pathConfig.ShowDialog() == true)
                postProcessPath = pathConfig.Path;

            // Recheck the connection status to post processing folder
            CheckConnection_Click(null,null);

            // Recheck if still able to read stills
            // First check if the scan button is enabled
            //      true = no subject scanned
            //      false = subject scanned
            if(!ScanSubjectButton.IsEnabled)
                PictureStatusCheck();
        }

        private async Task CheckServerConnection()
        {
            // Change refresh button to "blue" state and change status
            this.Dispatcher.Invoke(() => {
                CheckConnection.Background = System.Windows.Media.Brushes.LightSkyBlue;
                StatusText.Text = "Attempting to Connect to Post Processing";
            });
            await Task.Delay(500);
            
            if(Directory.Exists(postProcessPath))
            {
                // Set refresh to green and update status
                this.Dispatcher.Invoke(() => {
                    CheckConnection.Background = System.Windows.Media.Brushes.LightGreen;
                    StatusText.Text = "Connected to Post Processing";
                });
            }
            else
            {
                // Set refresh to red and update status
                this.Dispatcher.Invoke(() => {
                    CheckConnection.Background = System.Windows.Media.Brushes.LightCoral;
                    StatusText.Text = "Not Connected to Post Processing";
                });
            }
        }

        private async void CheckConnection_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => CheckServerConnection());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CheckConnection_Click(null,null);
        }

        private void ProcessFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("explorer.exe",postProcessPath);
            }
            catch
            {
                MessageBox.Show("An error occurred opening the post processing directory.\nPlease make sure the post processing path config is correct.","Open Window Error",MessageBoxButton.OK,MessageBoxImage.Exclamation);
            }
        }

        #endregion
    }
}
