using Lab2.Model;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lab2.View
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Frame frame = (this.Content as StackPanel).Children[0] as Frame;
            try
            {
                frame.Source = new Uri("ThreatPageView.xaml", UriKind.Relative);
            }
            catch (Exception ex1)
            {
                MessageWindow messageWindow = new MessageWindow("Oopsie", ex1.Message);
                messageWindow.Show();
            }
        }
    }
}
