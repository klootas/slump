using System.Collections.Generic;
using System.Windows;
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

            Person[] fromTxtFile = TextReader.LoadFromFile();


            if (fromTxtFile != null)
            {
                string textBoxText = "";
                foreach (Person person in fromTxtFile)
                {
                    textBoxText += $"{person.Name},";
                }

                StudentsTxtBox.Text = textBoxText;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            //TODO: Remove unecessary code.
            if (userCanceled)
                return;
            List<Person> tmp = new List<Person>();
            string[] students = StudentsTxtBox.Text.Split(',', '\n');
            foreach (var student in students)
            {

                tmp.Add(new Person(student));
            }

            MainWindow.persons.AddRange(tmp);
            MainWindow.persons.RemoveAll(p => !tmp.Contains(p));
            MainWindow.persons.RemoveAll(p => p.Name == "");
            //tmp = MainWindow.persons.GroupBy(x => x.Name).Select(x => x.First()).ToList();
            TextReader.SaveToFile(tmp);
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            userCanceled = false;
            this.Close();
        }
    }
}
