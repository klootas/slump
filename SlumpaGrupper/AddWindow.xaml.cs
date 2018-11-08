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
        bool userCanceled = true;

        public AddWindow()
        {
            InitializeComponent();

            string[] fromTxtFile = TextReader.LoadFromFile();

            if (fromTxtFile.Length > 0)
            {
                var textBoxText = fromTxtFile.Aggregate((a, b) => a + ", " + b);

                StudentsTxtBox.Text = textBoxText;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (userCanceled)
                return;

            TextReader.SaveToFile(StudentsTxtBox.Text.Replace(", ", Environment.NewLine));
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            userCanceled = false;
            this.Close();
        }
    }
}
