using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

/* Libraries Not Used
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
*/

namespace Hyperspec_VideoProcessing
{
    /// <summary>
    /// Interaction logic for VideoProcessing.xaml
    /// </summary>
    public partial class VideoProcessing : Window
    {
        private DispatcherTimer videoTimer;
        private bool videoScrubbing;
        private string videoPath;
        private string outputDirectory;
        private string outputFileName;
        private bool windowCloseFromVideoError;
        private const int FrameStepCount = 10;

        public VideoProcessing(string video, string output, string file)
        {
            InitializeComponent();
            
            videoPath = video;
            outputDirectory = output;
            outputFileName = file;
            windowCloseFromVideoError = false;
            
            // Make sure that the video can be accessed
            try
            {
                VideoProcessPlayer.Source = new System.Uri(videoPath);
                videoTimer = new DispatcherTimer();
                videoTimer.Interval = TimeSpan.FromMilliseconds(1);
                videoTimer.Tick += VideoTick;

                videoScrubbing = false;

                VideoProcessPlayer.Play();
                VideoProcessPlayer.Stop();
            }
            catch
            {
                MessageBox.Show("The input video ( " + videoPath + " ) could not be found or accessed", "Video Error", MessageBoxButton.OK, MessageBoxImage.Error);
                windowCloseFromVideoError = true;
                this.Close();
            }
        }

        #region Video Player Functions

        private void VideoProcessPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            // Get the time duration from the video and set the slider values appropriately
            TimeSpan tsVideo = VideoProcessPlayer.NaturalDuration.TimeSpan;
            VideoTimeSlider.Maximum = tsVideo.TotalSeconds;
            VideoTimeSlider.SmallChange = .01;
            VideoTimeSlider.LargeChange = Math.Min(10, tsVideo.Milliseconds / 10);

            videoTimer.Start();
        }

        private void VideoProcessPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            VideoProcessPlayer.Pause();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            VideoProcessPlayer.Play();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            VideoProcessPlayer.Pause();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            VideoProcessPlayer.Stop();
        }

        #endregion

        #region Video Timer Functions

        private void VideoTick(object sender, EventArgs e)
        {
            if(VideoProcessPlayer.Source != null && VideoProcessPlayer.NaturalDuration.HasTimeSpan)
            {
                if (videoScrubbing)
                {
                    VideoProcessPlayer.Position = TimeSpan.FromSeconds(VideoTimeSlider.Value);
                    VideoTimeDisplay.Content = string.Format("{0} / {1}", TimeSpan.FromSeconds(VideoTimeSlider.Value).ToString(@"mm\:ss\.ff"), VideoProcessPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss\.ff"));
                }
                else
                {
                    VideoTimeSlider.Value = VideoProcessPlayer.Position.TotalSeconds;
                    VideoTimeDisplay.Content = string.Format("{0} / {1}", VideoProcessPlayer.Position.ToString(@"mm\:ss\.ff"), VideoProcessPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss\.ff"));
                }
            }
        }

        private void VideoTimeSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            videoScrubbing = true;
            VideoProcessPlayer.Pause();
        }

        private void VideoTimeSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            videoScrubbing = false;
            VideoProcessPlayer.Position = TimeSpan.FromSeconds(VideoTimeSlider.Value);
        }

        #endregion
        
        #region Set Start/Stop Times

        private void SetStart_0ToPos225_Click(object sender, RoutedEventArgs e)
        {
            Start_0ToPos225.Content = VideoProcessPlayer.Position.ToString(@"mm\:ss\.ff");
        }

        private void SetStop_0ToPos225_Click(object sender, RoutedEventArgs e)
        {
            Stop_0ToPos225.Content = VideoProcessPlayer.Position.ToString(@"mm\:ss\.ff");
        }

        private void SetStart_Pos225ToNeg225_Click(object sender, RoutedEventArgs e)
        {
            Start_Pos225ToNeg225.Content = VideoProcessPlayer.Position.ToString(@"mm\:ss\.ff");
        }

        private void SetStop_Pos225ToNeg225_Click(object sender, RoutedEventArgs e)
        {
            Stop_Pos225ToNeg225.Content = VideoProcessPlayer.Position.ToString(@"mm\:ss\.ff");
        }

        private void SetStart_Neg225To0_Click(object sender, RoutedEventArgs e)
        {
            Start_Neg225To0.Content = VideoProcessPlayer.Position.ToString(@"mm\:ss\.ff");
        }

        private void SetStop_Neg225To0_Click(object sender, RoutedEventArgs e)
        {
            Stop_Neg225To0.Content = VideoProcessPlayer.Position.ToString(@"mm\:ss\.ff");
        }

        #endregion

        #region Window Closing Behavior

        private void CloseVideoButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(!windowCloseFromVideoError && this.DialogResult != true && !WindowClosingUserCheck())
            {
                this.DialogResult = false;
                e.Cancel = true;
            }
        }

        private bool WindowClosingUserCheck()
        {
            MessageBoxResult result = MessageBox.Show("Closing the window will lose any progress made.\n\nWould you like to continue?","Close Window",MessageBoxButton.YesNo,MessageBoxImage.Exclamation);

            if(result == MessageBoxResult.Yes)
                return true;

            return false;
        }

        #endregion

        #region Capture Stills/Video Processing Functions

        private async void CaptureStillsButton_Click(object sender, RoutedEventArgs e)
        {
            // Set all appropriate buttons to not enabled
            ButtonEnabledFlip();
            
            // Initialize time duration strings
            string duration_0ToPos225 = string.Empty;
            string duration_Pos225ToNeg225 = string.Empty;
            string duration_Neg225To0 = string.Empty;

            bool noError = true;

            // Calculate difference in time and store into time duration strings
            if(!GetTimeDifference("0:" + Start_0ToPos225.Content.ToString(), "0:" + Stop_0ToPos225.Content.ToString(), out duration_0ToPos225))
                noError = false;
            if(!GetTimeDifference("0:" + Start_Pos225ToNeg225.Content.ToString(), "0:" + Stop_Pos225ToNeg225.Content.ToString(), out duration_Pos225ToNeg225))
                noError = false;
            if(!GetTimeDifference("0:" + Start_Neg225To0.Content.ToString(), "0:" + Stop_Neg225To0.Content.ToString(), out duration_Neg225To0))
                noError = false;

            // Check if no errors occurred while getting time difference
            if(noError)
            {
                busyPopup.IsOpen = true;
                //await Task.Delay(10);

                bool captureErrorFound = false;
                
                ((System.Windows.Controls.TextBlock)busyPopup.Child).Text = string.Format("Processing 0° --> +22.5° . . .\n\nPlease wait until finished");
                await Task.Delay(10);

                // 0 ---> +22.5
                captureErrorFound = CaptureStills(Start_0ToPos225.Content.ToString(), duration_0ToPos225, FrameStepCount, Stop_0ToPos225.Content.ToString());

                ((System.Windows.Controls.TextBlock)busyPopup.Child).Text = string.Format("Processing +22.5° --> -22.5° . . .\n\nPlease wait until finished");
                await Task.Delay(10);

                // 0 <--> +22.5
                captureErrorFound = CaptureStills(Start_Pos225ToNeg225.Content.ToString(), duration_Pos225ToNeg225, FrameStepCount, Stop_Pos225ToNeg225.Content.ToString());

                ((System.Windows.Controls.TextBlock)busyPopup.Child).Text = string.Format("Processing -22.5° --> 0° . . .\n\nPlease wait until finished");
                await Task.Delay(10);

                // -22.5 ---> 0
                captureErrorFound = CaptureStills(Start_Neg225To0.Content.ToString(), duration_Neg225To0, FrameStepCount, Stop_Neg225To0.Content.ToString());


                busyPopup.IsOpen = false;
                await Task.Delay(10);

                // FFmpeg returned no errors
                if(!captureErrorFound)
                {
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    ButtonEnabledFlip();
                }
            }
            else
            {
                MessageBox.Show("There was an error with times. Make sure all times are set and that start times are before end times before continuing", "Video Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ButtonEnabledFlip();
            }
        }

        private bool GetTimeDifference(string start, string stop, out string result)
        {
            result = string.Empty;

            bool noError = false;
            TimeSpan startTime = TimeSpan.Zero;
            TimeSpan endTime = TimeSpan.Zero;

            if(TimeSpan.TryParse(start,out startTime))
            {
                if(TimeSpan.TryParse(stop, out endTime))
                {
                    // Compare both timespans and see if startTime is not larger than endTime (indicating a negative difference)
                    if(TimeSpan.Compare(startTime,endTime) != 1)
                    {
                        result = endTime.Subtract(startTime).ToString(@"mm\:ss\.ff");
                        noError = true;
                    }
                }
            }
            
            return noError;
        }

        private bool CaptureStills(string startTime, string duration, int frameStep, string stopTime)
        {
            bool errorFound = false;
            string errorOutput = string.Empty;

            string ffmpegPath = AppDomain.CurrentDomain.BaseDirectory + @"\ffmpeg.exe";

            /* FFMPEG.EXE VERSION */
            string ffmpegCommand = string.Format("-hide_banner -loglevel warning -ss {0} -i \"{1}\" -t {2} -vf \"select=not(mod(n\\,{3}))\" -pix_fmt bgr24 -vsync vfr -q:v 3 -start_number {4} \"{5}\"", startTime, videoPath, duration, frameStep, GetLastNumber(outputDirectory), outputDirectory + outputFileName);
            Process ffmpeg = new Process
            {
                StartInfo =
                {
                    FileName = ffmpegPath ,
                    Arguments = ffmpegCommand ,
                    UseShellExecute = false ,
                    RedirectStandardOutput = true ,
                    RedirectStandardError = true ,
                    CreateNoWindow = true ,
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                }
            };

            ffmpeg.EnableRaisingEvents = true;
            ffmpeg.Start();

            errorOutput = ffmpeg.StandardError.ReadToEnd();

            ffmpeg.WaitForExit();

            if(errorOutput != string.Empty)
            {
                MessageBox.Show(errorOutput,"FFmpeg Error",MessageBoxButton.OK,MessageBoxImage.Error);
                errorFound = true;
            }


            /* Get Last Frame from Timespan */

            ffmpeg.StartInfo.Arguments = ffmpegCommand = string.Format("-hide_banner -loglevel warning -ss {0} -i \"{1}\" -pix_fmt bgr24 -vframes 1 -start_number {2} \"{3}\"", stopTime, videoPath, GetLastNumber(outputDirectory), outputDirectory + outputFileName);
            ffmpeg.Start();

            errorOutput = ffmpeg.StandardError.ReadToEnd();

            ffmpeg.WaitForExit();
            ffmpeg.Dispose(); // Remove all resources relating to ffmpeg process

            if(errorOutput != string.Empty)
            {
                MessageBox.Show(errorOutput,"FFmpeg Error",MessageBoxButton.OK,MessageBoxImage.Error);
                errorFound = true;
            }


            return errorFound;
        }

        private int GetLastNumber(string directory)
        {
            int lastNum = 0;

            try
            {
                string[] files = Directory.GetFiles(directory, "*.bmp", SearchOption.TopDirectoryOnly);
                // Sort files
                Array.Sort(files);
            
                string[] lastFileSplit = Path.GetFileNameWithoutExtension(files[files.Length - 1]).Split('_');

                Int32.TryParse(lastFileSplit[lastFileSplit.Length - 1], out lastNum);
            }
            catch { }
            
            return lastNum;
        }

        #endregion

        #region GUI Functions

        private void ButtonEnabledFlip()
        {
            SetStart_0ToPos225.IsEnabled = !SetStart_0ToPos225.IsEnabled;
            SetStart_Pos225ToNeg225.IsEnabled = !SetStart_Pos225ToNeg225.IsEnabled;
            SetStart_Neg225To0.IsEnabled = !SetStart_Neg225To0.IsEnabled;
            SetStop_0ToPos225.IsEnabled = !SetStop_0ToPos225.IsEnabled;
            SetStop_Pos225ToNeg225.IsEnabled = !SetStop_Pos225ToNeg225.IsEnabled;
            SetStop_Neg225To0.IsEnabled = !SetStop_Neg225To0.IsEnabled;
            CaptureStillsButton.IsEnabled = !CaptureStillsButton.IsEnabled;
            CloseVideoButton.IsEnabled = !CloseVideoButton.IsEnabled;
        }

        #endregion

    }
}
