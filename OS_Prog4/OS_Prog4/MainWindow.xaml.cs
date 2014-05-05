using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OS_Prog4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ProcessScheduler _scheduler;
        private PageTable _pageTable;
        private PageReplacement _pageReplacement;
        ObservableCollection<ObservableCollection<string>> _currentAlgorithm;

        //*******************************************************************//
        //Author: Joe Manke, Josh Schultz
        //
        //Date: March 31, 2014
        //
        //Description: Constructor for the MainWindow
        //
        //Parameters: (none)
        //
        //Returns:  (nothing)
        //*******************************************************************//
        public MainWindow()
        {
            //Quantum default value of 2
            _scheduler = new ProcessScheduler(2, this);

            _pageTable = new PageTable();

            _pageReplacement = new PageReplacement();

            _currentAlgorithm = new ObservableCollection<ObservableCollection<string>>();

            InitializeComponent();
        }


        //*******************************************************************//
        //Author: Joe Manke
        //
        //Date: May 3, 2014
        //
        //Description: Opens a dialog to add a new process. If the input is
        //             accepted, it is added to the scheduler and the table
        //             and gantt charts are updated.
        //
        //Parameters:  sender - The GUI button object
        //                  e - The event object
        //
        //Returns:  (nothing)
        //*******************************************************************//
        private void AddProcessButton_Click_1(object sender, RoutedEventArgs e)
        {
            uint pId = (uint)_scheduler.Processes.Count + 1;

            //Make a window to take in the start time, duration, and priority
            AddProcessDialog dialog = new AddProcessDialog(pId);

            dialog.ShowDialog();
            
            uint priority, startTime, duration;

            if (UInt32.TryParse(dialog.Priority.Text, out priority) &&
                UInt32.TryParse(dialog.StartTime.Text, out startTime) &&
                UInt32.TryParse(dialog.Duration.Text, out duration))
            {
                //Add the process to the scheduler
                Process process = new Process(pId, startTime, duration, priority);
                _scheduler.AddProcess(process);
            }
            else if(!dialog.Cancelled)
            {
                MessageBox.Show("Error parsing inputs. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        //*******************************************************************//
        //Author: Josh Schultz, Joe Manke
        //
        //Date: May 1, 2014
        //
        //Description: Once the Generate Processes button is pressed, this callback
        //             creates 7 random processes.
        //
        //Parameters:  sender - The GUI button object
        //                  e - The event object
        //
        //Returns:  (nothing)
        //*******************************************************************//
        private void GenerateProcessButton_Click_1(object sender, RoutedEventArgs e)
        {
            //clear existing processes from GUI
            SJFPanel.Children.Clear();
            PriorityPanel.Children.Clear();
            RRPanel.Children.Clear();

            //Generate 7 random processes
            _scheduler.GenerateProcesses(7);
        }


        //*******************************************************************//
        //Author: Josh Schultz, Joe Manke
        //
        //Date: May 1, 2014
        //
        //Description: Once the Clear Processes button is pressed, this function
        //             removed all of the processes from the list.
        //
        //Parameters:  sender - The GUI button object
        //                  e - The event object
        //
        //Returns: (nothing)
        //*******************************************************************//
        private void ClearProcessButton_Click_1(object sender, RoutedEventArgs e)
        {
            //Clear all of the processes
            _scheduler.ClearProcesses();

            SJFPanel.Children.Clear();
            PriorityPanel.Children.Clear();
            RRPanel.Children.Clear();
        }

        //*******************************************************************//
        //Author: Joe Manke
        //
        //Date: May 3, 2014
        //
        //Description: Changes the data context to match which tab is selected.
        //
        //Parameters:  sender - The GUI element that triggered the event
        //                  e - The event object
        //
        //Returns:  (nothing)
        //*******************************************************************//
        private void TabControl_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (ProcessTab.IsSelected)
            {
                DataContext = _scheduler;
            }
            else if (MemoryTab.IsSelected)
            {
                DataContext = _pageTable;
            }
            else if (PageTab.IsSelected)
            {
                DataContext = _pageReplacement; 
            }
        }

        private void GoButton_Click_1(object sender, RoutedEventArgs e)
        {
            int page, offset;

            try
            {
                if (Int32.TryParse(PageTextbox.Text, out page) && Int32.TryParse(FrameTextbox.Text, out offset))
                {
                    valueReturned.Content = "Value " + _pageTable.goPushed(page, offset);
                    if (_pageTable.foundInTLB)
                    {
                        valueFoundWhere.Content = " was found in TLB";
                    }
                    else
                    {
                        valueFoundWhere.Content = " was found in Page Table";
                    }
                }
                else
                {
                    MessageBox.Show("Error parsing inputs. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch
            {
                MessageBox.Show("Error parsing inputs. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            RadioButton button = sender as RadioButton;

            //switch to current algorithm
            switch (button.Content.ToString())
            {
                case "FIFO":
                    _currentAlgorithm = _pageReplacement.FirstInFirstOut();
                    break;
                case "LRU":
                    _currentAlgorithm = _pageReplacement.LeastRecentlyUsed();
                    break;
                case "LFU":
                    _currentAlgorithm = _pageReplacement.LeastFrequentlyUsed();
                    break;
                case "Optimal":
                    _currentAlgorithm = _pageReplacement.Optimal();
                    break;
                case "Second Chance":
                    _currentAlgorithm = _pageReplacement.SecondChance();
                    break;
                case "Clock":
                    _currentAlgorithm = _pageReplacement.SecondChance();
                    break;
            }

            DrawPageStack();
        }

        private void DrawPageStack()
        {
            if (PageStack != null)
            {
                PageStack.Children.OfType<Border>().ToList().ForEach(o => PageStack.Children.Remove(o));
                PageStack.Children.OfType<TextBlock>().ToList().ForEach(o => PageStack.Children.Remove(o));
                PageStack.ColumnDefinitions.Clear();
                PageStack.ColumnDefinitions.Add(LabelsColumn);

                for (int i = 0; i < _pageReplacement.ReferenceString.Count; i++)
                {
                    ColumnDefinition colDef = new ColumnDefinition();
                    colDef.Width = new GridLength((PageStack.Width - LabelsColumn.Width.Value) / _pageReplacement.ReferenceString.Count);
                    PageStack.ColumnDefinitions.Add(colDef);

                    TextBlock tb = new TextBlock();
                    tb.Text = _pageReplacement.ReferenceString[i].ToString();
                    tb.SetValue(Grid.RowProperty, 0);
                    tb.SetValue(Grid.ColumnProperty, i + 1);
                    PageStack.Children.Add(tb);
                }

                for (int i = 0; i < _currentAlgorithm.Count; i++)
                {
                    ObservableCollection<string> row = _currentAlgorithm[i];
                    for (int j = 0; j < row.Count; j++)
                    {
                        string frame = row[j];

                        TextBlock tb = new TextBlock();
                        tb.Text = frame;

                        Border border = new Border();
                        border.Child = tb;

                        border.SetValue(Grid.RowProperty, i + 1);
                        border.SetValue(Grid.ColumnProperty, j + 1);

                        PageStack.Children.Add(border);
                    }
                }
            }
        }

        private void GenerateRefStringButton_Click_1(object sender, RoutedEventArgs e)
        {
            int length, pageCount;
            try
            {
                if (Int32.TryParse(RefStringLength.Text, out length) && Int32.TryParse(NumPages.Text, out pageCount))
                {
                    _pageReplacement.Length = length;
                    _pageReplacement.MaxPageValue = pageCount;
                    _pageReplacement.GenerateReferenceString();
                }
                else
                {
                    MessageBox.Show("Error parsing inputs. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch
            {
                MessageBox.Show("Error parsing inputs. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            //forces GUI to update
            var checkRadioButton = RadioGroup.Children.OfType<RadioButton>().First(o => (bool)o.IsChecked);
            RadioButton_Checked_1(checkRadioButton, e);
        }

        private void SetQuantumButton_Click_1(object sender, RoutedEventArgs e)
        {
            uint quantum;

            if (UInt32.TryParse(QuantumTextbox.Text, out quantum))
            {
                _scheduler.Quantum = quantum;
                _scheduler.ReorderByRoundRobin(quantum);
            }
            else
            {
                MessageBox.Show("Error parsing input. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
