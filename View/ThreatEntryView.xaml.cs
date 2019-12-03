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
    /// Логика взаимодействия для ThreatEntryView.xaml
    /// </summary>
    public partial class ThreatEntryView : Page
    {
        public ThreatEntryView(string[][] entry)
        {
            InitializeComponent();
            Title = "Информация об угрозе: " + entry[1][1];
            ConstructGrid(entry);
        }
        private void ConstructGrid(string[][] entry)
        {
            Grid grid = (Grid)this.Content;
            for (int i = 0; i < entry[0].Length + 1; i++) grid.RowDefinitions.Add(new RowDefinition());
            //fill in
            TextBlock cell = new TextBlock();
            //header
            cell.Text = "Информация об угрозе";
            cell.Padding = new Thickness(0,10,0,10);
            cell.Background = Brushes.AliceBlue;
            cell.TextAlignment = TextAlignment.Center;
            cell.FontSize = 16;
            Grid.SetRow(cell, 0);
            Grid.SetColumnSpan(cell, 2);
            grid.Children.Add(cell);
            //cells
            for (int i = 0; i < entry[0].Length; i++)
            {
                grid.RowDefinitions[i].Height = GridLength.Auto;
                for (int j = 0; j < 2; j++)
                {
                    cell = new TextBlock();
                    cell.TextWrapping = TextWrapping.Wrap;
                    cell.Padding = new Thickness(20,10,20,10);
                    cell.Text = entry[j][i];
                    cell.FontSize = 14;
                    Grid.SetRow(cell, i + 1);
                    Grid.SetColumn(cell, j);
                    grid.Children.Add(cell);
                }
            }
        }
    }
}
