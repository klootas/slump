using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SlumpaGrupper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static List<Person> persons = new List<Person>();

        IGrouping<string, Person>[] groupDataContent;

        public MainWindow()
        {
            InitializeComponent();

            PopulatePersons();

            UI_Controller.FileSaved += OnFileSaved;

            Loaded += (s, e) => GroupOnOpen();
        }

        private void OnFileSaved()
        {
            persons.Clear();
            PopulatePersons();

            groupData.ItemsSource = null;
            groupPanel.Visibility = Visibility.Hidden;
        }

        public void PopulatePersons()
        {
            NameTable.ItemsSource = null;

            Person[] personsFromFile = TextReader.LoadFromFile();

            if (personsFromFile == null)
            {
                return;
            }

            persons.AddRange(personsFromFile);
            NameTable.ItemsSource = persons;

            // Loads in last group from file
            // FIXME Json loading from file doesn't load expected values.
            //groupData.ItemsSource = TextReader.LoadLastGroup();
            //groupData.Visibility = Visibility.Visible;
        }

        private void GroupBtn_Click(object sender, RoutedEventArgs e)
        {
            BindGroups();

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

            foreach (var person in persons)
            {
                person.Presented = false;
            }

            var sortedPersons = persons
                .Where(p => p.IsParticipating == true)
                .OrderBy(p => Guid.NewGuid())
                .ToArray();

            int numberOfGroups = (int)Math.Round(sortedPersons.Length / (float)groupSize, 0);

            int groupNumber = 0;

            foreach (var person in sortedPersons)
            {
                int index = groupNumber++ % numberOfGroups;
                person.Group = $"Grupp {index + 1}";
            }

            var filteredSortedPersons = sortedPersons
                .GroupBy(o => o.Group)
                .ToArray();

            groupDataContent = filteredSortedPersons;

            // FIXME Json saving current group data is disabled until loading works
            //TextReader.SaveGroup(filteredSortedPersons);

            TextReader.SaveToFile(MainWindow.persons);

            groupData.ItemsSource = filteredSortedPersons;
            groupPanel.Visibility = Visibility.Visible;
        }

        private void GroupOnOpen()
        {
            var filteredSortedPersons = persons
                .GroupBy(o => o.Group)
                .ToArray();

            groupDataContent = filteredSortedPersons;

            // FIXME Json saving current group data is disabled until loading works
            //TextReader.SaveGroup(filteredSortedPersons);

            TextReader.SaveToFile(MainWindow.persons);

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
            AddWindow window = new AddWindow();
            window.ShowDialog();
        }

        private void PresentationBtn_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Remove unecessary code.
            int groupDataContentSize = groupDataContent.Length;

            int i = 0;
            Random random = new Random();

            int randomGroup = random.Next(0, groupDataContentSize);



            for (int j = 0; j < groupDataContentSize; j++)
            {
                foreach (var person in groupDataContent[j])
                {
                    if (person.Presented)
                    {
                        i++;
                        break;
                    }
                }
            }

            if (i == groupDataContentSize)
            {
                MessageBox.Show("Alla grupper har presenterat.");
                return;
            }

            var messageGroup = groupDataContent[randomGroup].ToList();

            string message = null;
            while (messageGroup[0].Presented == true)
            {
                randomGroup = random.Next(0, groupDataContentSize);
                messageGroup = groupDataContent[randomGroup].ToList();

            }


            foreach (var item in messageGroup)
            {
                message += item.ToString() + Environment.NewLine;
                item.Presented = true;
            }

            TextReader.SaveToFile(persons);

            MessageBox.Show(message.ToString());
        }
    }
}
