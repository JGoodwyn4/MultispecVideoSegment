using System.Windows;
using System.Windows.Controls;

/* Libraries Not Used
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for PathConfigBox.xaml
    /// </summary>
    public partial class PathConfigBox : Window
    {
        public string Path { get { return PathText.Text; } }
        private bool textChanged;

        public PathConfigBox(string inputPath)
        {
            InitializeComponent();

            PathText.Text = inputPath;
            textChanged = false;
        }

        private void SetPath_Click(object sender, RoutedEventArgs e)
        {
            // Make sure last character of path ends with '\'
            CheckPathEnd();

            this.DialogResult = true;
            this.Close();
        }
        
        private void PathText_TextChanged(object sender, TextChangedEventArgs e)
        {
            textChanged = true;
        }

        private void CheckPathEnd()
        {
            if(!PathText.Text[PathText.Text.Length - 1].Equals('\\'))
                PathText.Text += @"\";
        }

        private void SearchDirectory_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog searchDirectory = new System.Windows.Forms.FolderBrowserDialog();
            searchDirectory.ShowNewFolderButton = true;

            if(searchDirectory.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                PathText.Text = searchDirectory.SelectedPath;

                // Make sure last character of path ends with '\'
                CheckPathEnd();

                textChanged = false;
                this.DialogResult = true;
            }
        }
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(this.DialogResult != true)
            {
                if(textChanged)
                {
                    MessageBoxResult result = MessageBox.Show("Changes were made. Are you sure you still want to exit?","Close Window",MessageBoxButton.YesNo,MessageBoxImage.Exclamation);

                    if(result == MessageBoxResult.Yes)
                        this.DialogResult = false;
                    else
                        e.Cancel = true;
                }
                else
                {
                    this.DialogResult = false;
                }
            }
        }
    }
}
