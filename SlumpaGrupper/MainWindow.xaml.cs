using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SlumpaGrupper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Person> persons = new List<Person>();

        IGrouping<string, Person>[] groupDataContent;

        public MainWindow()
        {
            InitializeComponent();

            PopulatePersons();

            //Loaded += (s, e) => BindGroups();
        }

        public void PopulatePersons()
        {
            string[] personsFromFile = TextReader.LoadFromFile();

            if (personsFromFile.Length == 0 || personsFromFile[0] == "")
            {
                NameTable.ItemsSource = null;
                return;
            }

            foreach (var student in personsFromFile)
            {
                persons.Add(new Person(student));
            }
            NameTable.ItemsSource = persons;

            // Loads in last group from file
            // FIXME Json loading from file doesn't load expected values.
            //groupData.ItemsSource = TextReader.LoadLastGroup();
            //groupData.Visibility = Visibility.Visible;
        }

        private void GroupBtn_Click(object sender, RoutedEventArgs e)
        {
            BindGroups();
            var data = groupData.ItemsSource;

            // FIXME Json saving current group data is disabled until loading works
            //TextReader.SaveGroup(data);
        }

        private void BindGroups()
        {
            bool correctInput = false;

            correctInput = int.TryParse(GroupSizeTxtBox.Text, out int groupSize);

            if (!correctInput)
            {
                // Antal grupper har inte en siffra som inmatning.
                return;
            }

            var sortedPersons = persons
                .Where(p => p.IsParticipating == true)
                .OrderBy(p => Guid.NewGuid())
                .ToArray();

            int numberOfGroups = (int)Math.Round(persons.Count / (float)groupSize, 0);

            int groupNumber = 0;
            for (int i = 0; i < sortedPersons.Length; i++)
            {
                if (i % groupSize == 0)
                {
                    groupNumber++;
                }
                sortedPersons[i].Group = $"Grupp {groupNumber}";
            }

            var filteredSortedPersons = sortedPersons
                .GroupBy(o => o.Group)
                .ToArray();

            groupDataContent = filteredSortedPersons;

            // FIXME Json saving current group data is disabled until loading works
            //TextReader.SaveGroup(filteredSortedPersons);

            groupData.ItemsSource = filteredSortedPersons;
            groupPanel.Visibility = Visibility.Visible;
        }

        private void RebindGroups()
        {
            var content = groupData.ItemsSource;

            groupData.ItemsSource = null;
            groupData.ItemsSource = content;
            groupPanel.Visibility = Visibility.Visible;
        }

        // Unsuccessful hack to fix width during zooming down the font width
        private void DataGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //var dataGrid = sender as DataGrid;
            //if (dataGrid != null)
            //{
            //    dataGrid.UpdateLayout();
            //    foreach (var column in dataGrid.Columns)
            //    {
            //        column.Width = new DataGridLength(1.0, DataGridLengthUnitType.Auto);
            //    }
            //}
        }


        // New hack
        private bool dragStarted = false;

        private void Slider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            DoWork(((Slider)sender).Value);
            this.dragStarted = false;
        }

        private void Slider_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.dragStarted = true;
        }

        private void Slider_ValueChanged(
            object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            if (dragStarted)
            {
                DoWork(e.NewValue);
            }
        }

        private void DoWork(double newValue)
        {
            groupData.Width = 0;
            groupData.Width = double.NaN;

            RebindGroups();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddWindow win2 = new AddWindow();
            win2.Show();
        }

        private void PresentationBtn_Click(object sender, RoutedEventArgs e)
        {
            int groupDataContentSize = groupDataContent.Length;

            Random random = new Random();

            int randomGroup = random.Next(0, groupDataContentSize);

            var messageGroup = groupDataContent[randomGroup].ToList();
            string message = null;

            foreach (var item in messageGroup)
            {
                message += item.ToString() + Environment.NewLine;
            }

            MessageBox.Show(message.ToString());
        }
    }
}
