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

namespace Lab2.View
{
    /// <summary>
    /// Логика взаимодействия для MessageWindow.xaml
    /// </summary>
    public partial class MessageWindow : Window
    {
        public MessageWindow(string msg, string description)
        {
            InitializeComponent();
            ((this.Content as Panel).Children[0] as TextBlock).Text = msg;
            (((this.Content as Panel).Children[1] as ScrollViewer).Content as TextBlock).Text = description;
        }
        public void ButtonClick(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
