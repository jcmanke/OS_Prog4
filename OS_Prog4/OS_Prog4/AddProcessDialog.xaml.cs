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

namespace OS_Prog4
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class AddProcessDialog : Window
    {
        public bool Cancelled = false;

        public AddProcessDialog(uint pid)
        {
            InitializeComponent();
            PID.Text = pid.ToString();
        }

        private void OKButton_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CancelButton_Click_1(object sender, RoutedEventArgs e)
        {
            StartTime.Text = string.Empty;
            Duration.Text = string.Empty;
            Priority.Text = string.Empty;
            Cancelled = true;

            this.Close();
        }
    }
}
