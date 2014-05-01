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

        public MainWindow()
        {
            InitializeComponent();
            //DataContext = this;
           
            //Quantum default value of 2
            _scheduler = new ProcessScheduler(2);

            //Set the datacontext to this single scheduler
            DataContext = _scheduler;

        }


        //*******************************************************************//
        //Author: 
        //Date: 
        //Description:
        //Parameters:
        //Returns:
        //*******************************************************************//
        private void AddProcessButton_Click_1(object sender, RoutedEventArgs e)
        {
            //Make a window to take in the start time, duration, and priority

            uint pId = (uint)_scheduler.Processes.Count + 1;
            //uint priority =
            //uint startTime =
            //uint duration =

            //Add the process to the scheduler
            //Process process = new Process(pId, startTime, duration, priority);
            //_scheduler.AddProcess(process);
        }


        //*******************************************************************//
        //Author: Josh Schultz/Joe Manke
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
            //Generate 7 random processes
            _scheduler.GenerateProcesses(7);
        }


        //*******************************************************************//
        //Author: Josh Schultz/Joe Manke
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
        }
    }
}
