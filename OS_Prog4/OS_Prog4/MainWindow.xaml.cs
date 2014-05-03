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
            InitializeComponent();
           
            //Quantum default value of 2
            _scheduler = new ProcessScheduler(2, this);
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
            }
            else if (PageTab.IsSelected)
            {

            }
        }
    }
}
