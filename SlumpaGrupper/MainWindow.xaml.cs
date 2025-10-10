﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace SlumpaGrupper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static List<Person> persons = new List<Person>();

        IGrouping<string, Person>[] groupDataContent;

        int _groupSize;

        IEnumerable<Person> Participating => persons.Where(p => p.IsParticipating);
        int NumberOfGroups => (int)Math.Ceiling(NumberOfParticipants / (float)_groupSize);
        int NumberOfParticipants => Participating.Count();

        public MainWindow()
        {
            InitializeComponent();


            this.Top = Properties.Settings.Default.Top;
            this.Left = Properties.Settings.Default.Left;
            this.Height = Properties.Settings.Default.Height;
            this.Width = Properties.Settings.Default.Width;
            GroupSizeTxtBox.Text = Properties.Settings.Default.GroupSize.ToString();
            fontSlider.Value = Properties.Settings.Default.Slider;
            DoWork(fontSlider.Value);
            // Very quick and dirty - but it does the job
            if (Properties.Settings.Default.WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Maximized;
            }


            PopulatePersons();
            UpdateRandomizeBtnText();

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

            if (personsFromFile.Length == 0 || personsFromFile == null)
            {
                HideUnhide(true);
                return;
            }

            persons.Clear();

            persons.AddRange(personsFromFile);
            HideUnhide(false);


            NameTable.ItemsSource = persons.OrderBy(p => p.Name).ToList();
            //NameTable.ItemsSource = persons.OrderBy(p => p.Name);

            // Loads in last group from file
            // FIXME Json loading from file doesn't load expected values.
            //groupData.ItemsSource = TextReader.LoadLastGroup();
            //groupData.Visibility = Visibility.Visible;
        }

        private void NameTable_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void NameTable_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            UpdateRandomizeBtnText();
        }

        private void HideUnhide(bool b)
        {
            if (b)
            {

                PresentationBtn.Visibility = Visibility.Hidden;
                groupPanel.Visibility = Visibility.Hidden;
                fontSlider.Visibility = Visibility.Hidden;
                groupData.ItemsSource = null;
            }
            else
            {
                PresentationBtn.Visibility = Visibility.Visible;
                groupPanel.Visibility = Visibility.Visible;
                fontSlider.Visibility = Visibility.Visible;
            }
        }

        private void GroupBtn_Click(object sender, RoutedEventArgs e)
        {
            BindGroups();
            // FIXME Json saving current group data is disabled until loading works
            //TextReader.SaveGroup(data);
        }

        private void GroupSize_Changed(object sender, RoutedEventArgs e)
        {

            bool correctInput = false;

            string input = GroupSizeTxtBox.Text;
            if (input == string.Empty)
            {
                return;
            }
            correctInput = int.TryParse(input, out int groupSize);

            if (correctInput)
            {
                _groupSize = groupSize;
            }
            UpdateRandomizeBtnText();
        }

        private void NameTable_CurrentCellChanged(object sender, EventArgs e)
        {
            UpdateRandomizeBtnText();
        }

        private void UpdateRandomizeBtnText()
        {
            GroupSizeTxtBox.Text = _groupSize.ToString();
            if (GroupBtn != null)
            {
                if (NumberOfGroups > 0)
                {
                    string btnText;
                    if (_groupSize > NumberOfParticipants)
                    {
                        btnText = $"(1x{NumberOfParticipants})";
                    }
                    else if (NumberOfParticipants % _groupSize == 0)
                    {
                        btnText = $"({NumberOfGroups}x{_groupSize})";
                    }
                    else
                    {
                        btnText = $"({NumberOfGroups - 1}x{_groupSize} + {NumberOfParticipants % _groupSize})";
                    }
                    GroupBtnText.Text = btnText;
                }
            }
        }

        private void BindGroups()
        {
            foreach (var person in persons)
            {
                person.Presented = false;
            }

            var sortedPersons = Participating.OrderBy(p => Guid.NewGuid())
                .ToArray();

            for (int groupNumber = 0; groupNumber < NumberOfGroups; groupNumber++)
            {
                for (int groupMembers = 0; groupMembers < _groupSize; groupMembers++)
                {
                    int index = groupNumber * _groupSize + groupMembers;
                    if (index >= NumberOfParticipants)
                    {
                        break;
                    }
                    sortedPersons[index].Group = $"Grupp {groupNumber + 1}";
                }
            }

            var filteredSortedPersons = sortedPersons
                .OrderBy(o => o.Name)
                .GroupBy(o => o.Group)
                .OrderBy(g => int.Parse(g.Key.Split(' ')[1]))
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
                .Where(o => o.Group != null && o.IsParticipating)
                .OrderBy(o => o.Name)
                .GroupBy(o => o.Group)
                .OrderBy(g => int.Parse(g.Key.Split(' ')[1]))
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
            PopulatePersons();
            UpdateRandomizeBtnText();
        }

        private void PresentationBtn_Click(object sender, RoutedEventArgs e)
        {
        Restart:
            var notPresented = groupDataContent.Where(p => !p.ToArray()[0].Presented).ToArray();
            int groupDataContentSize = notPresented.Length;

            if (groupDataContentSize == 0)
            {
                persons.ForEach(p => p.Presented = false);
                goto Restart;
            }

            Random random = new Random();
            int randomGroup = random.Next(0, groupDataContentSize);

            var messageGroup = notPresented[randomGroup].ToList();

            string message = null;

            foreach (var item in messageGroup)
            {
                message += item.ToString() + Environment.NewLine;
                item.Presented = true;
            }

            TextReader.SaveToFile(persons);

            MessageBox.Show(message.ToString());
        }

        private Point? _startPoint;
        private void NameTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dg = sender as DataGrid;
            persons.Where(x => x.Name == dg.Items[0].ToString()).First().IsParticipating = false;

        }

        private void GroupBoxes_PreviewMouseMove(object sender, MouseEventArgs e)
        {

            // No drag operation
            if (_startPoint == null)
                return;

            var dg = sender as DataGrid;
            if (dg == null) return;
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = _startPoint.Value - mousePos;
            // test for the minimum displacement to begin the drag
            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {

                // Get the dragged DataGridRow
                var DataGridRow =
                    FindAnchestor<DataGridRow>((DependencyObject)e.OriginalSource);

                if (DataGridRow == null)
                    return;
                // Find the data behind the DataGridRow
                var dataTodrop = (Person)dg.ItemContainerGenerator.
                    ItemFromContainer(DataGridRow);

                if (dataTodrop == null) return;

                // Initialize the drag & drop operation
                var dataObj = new DataObject(dataTodrop);
                dataObj.SetData("DragSource", sender);
                DragDrop.DoDragDrop(dg, dataObj, DragDropEffects.Copy);
                _startPoint = null;
            }
        }

        private void GroupBoxes_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
        }

        private void GroupBoxes_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _startPoint = null;
        }

        private void GroupBoxes_Drop(object sender, DragEventArgs e)
        {
            var dg = sender as DataGrid;
            if (dg == null) return;
            var dgSrc = e.Data.GetData("DragSource") as DataGrid;
            var data = e.Data.GetData(typeof(Person));
            if (dgSrc == null || data == null) return;
            // Implement move data here, depends on your implementation
            MoveDataFromSrcToDest(dgSrc, dg, data);
            // OR
            //MoveDataFromSrcToDest(dgSrc.ItemsSource, dg.ItemsSource, data);
        }

        private void MoveDataFromSrcToDest(DataGrid dgSrc, DataGrid dg, object data)
        {
            Person tmp = (Person)data;
            tmp.Group = persons.Where(x => x.Name == dg.Items[0].ToString()).First().Group;
            TextReader.SaveToFile(persons);
            GroupOnOpen();
        }

        private void GroupBoxes_PreviewDragOver(object sender, DragEventArgs e)
        {
            //if false e.Effects = DragDropEffects.None;
        }

        private static T FindAnchestor<T>(DependencyObject current)
        where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        private void TheWindow_Closing(object sender, CancelEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                // Use the RestoreBounds as the current values will be 0, 0 and the size of the screen
                Properties.Settings.Default.Top = RestoreBounds.Top;
                Properties.Settings.Default.Left = RestoreBounds.Left;
                Properties.Settings.Default.Height = RestoreBounds.Height;
                Properties.Settings.Default.Width = RestoreBounds.Width;
                Properties.Settings.Default.WindowState = WindowState.Maximized;

            }
            else
            {
                Properties.Settings.Default.Top = this.Top;
                Properties.Settings.Default.Left = this.Left;
                Properties.Settings.Default.Height = this.Height;
                Properties.Settings.Default.Width = this.Width;
                Properties.Settings.Default.WindowState = WindowState.Normal;

            }
            Properties.Settings.Default.Slider = fontSlider.Value;
            Properties.Settings.Default.GroupSize = _groupSize;
            Properties.Settings.Default.Save();
        }

    }
}
