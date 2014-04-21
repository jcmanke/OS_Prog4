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
        public ObservableCollection<Process> Processes { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Process p1 = new Process(1, 0, 10, 3);
            Process p2 = new Process(2, 0, 1, 1);
            Process p3 = new Process(3, 0, 2, 4);
            Process p4 = new Process(4, 0, 1, 5);
            Process p5 = new Process(5, 0, 5, 2);

            Processes = new ObservableCollection<Process>();
            Processes.Add(p1);
            Processes.Add(p2);
            Processes.Add(p3);
            Processes.Add(p4);
            Processes.Add(p5);
            ProcessScheduler pScheduler = new ProcessScheduler(Scheduler.Priority, Processes);

            ObservableCollection<Process> reordered = pScheduler.ReorderByRoundRobin(2);

            for (int i = 0; i < reordered.Count; i++)
            {
                Console.WriteLine(reordered[i].ToString());
            }
        }
    }
}
