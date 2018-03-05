using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SlumpaGrupper
{
    /// <summary>
    /// Interaction logic for Add.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        public AddWindow()
        {
            InitializeComponent();

            string[] fromTxtFile = TextReader.LoadFromFile();

            if (fromTxtFile.Length > 0)
            {
                var textBoxText = fromTxtFile.Aggregate((a, b) => a + "\n" + b);

                StudentsTxtBox.Text = textBoxText;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Restart the program to load in all text again.
            // FIXME find a way to load PopulatePersons from the main window so we don't have to restart the whole program. 
            TextReader.SaveToFile(StudentsTxtBox.Text);
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
