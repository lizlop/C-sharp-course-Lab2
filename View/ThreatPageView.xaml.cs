using Lab2.Model;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для ThreatPageView.xaml
    /// </summary>
    public partial class ThreatPageView : Page
    {
        private ThreatTableModel tableModel;
        private Grid grid;
        public ThreatPageView()
        {
            InitializeComponent();

            tableModel = new ThreatTableModel() { Path = "thrlist.xlsx" };

            if (File.Exists("thrlist.xlsx")) tableModel.FillTableFromFile("thrlist.xlsx"); 
            else if (MessageBox.Show("Couldn't find the file \"thrlist.xlsx\". Do you want to download it?", "File is not found", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                tableModel.DownloadTable();
                tableModel.FillTableFromFile("thrlist.xlsx");
            }
            
            Title = "All threats";
            grid = ConstructGrid();
            (this.Content as StackPanel).Children.Add(grid);
        }
        private Grid ConstructGrid()
        {
            string[][] table = tableModel.GetPageTable();
            Grid grid = new Grid();
            for (int i = 0; i < ThreatTableModel.shortFieldsCount; i++) grid.ColumnDefinitions.Add(new ColumnDefinition());
            for (int i = 0; i < ThreatTableModel.entriesPerPage + 2; i++) grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions[0].Width = new GridLength(200);
            //header
            for (int j = 0; j < ThreatTableModel.shortFieldsCount; j++)
            {
                TextBlock block = new TextBlock();
                block.Text = table[0][j];
                block.Background = Brushes.AliceBlue;
                block.FontSize = 14;
                block.Padding = new Thickness(15, 10, 15, 10);
                block.TextAlignment = TextAlignment.Center;
                Grid.SetRow(block, 0);
                Grid.SetColumn(block, j);
                grid.Children.Add(block);
            }
            //cells
            Button cell;
            for (int i = 1; i < table.Length; i++)
            {
                for (int j = 0; j < ThreatTableModel.shortFieldsCount; j++)
                {
                    cell = new Button();
                    cell.BorderThickness = new Thickness(0);
                    cell.Content = table[i][j];
                    cell.HorizontalContentAlignment = HorizontalAlignment.Left;
                    cell.Tag = ThreatTableModel.ParseId(table[i][0]);
                    cell.Background = i % 2 > 0 ? Brushes.White : Brushes.AliceBlue;
                    cell.FontSize = 13;
                    cell.Padding = new Thickness(15, 10, 15, 10);
                    cell.Click += new RoutedEventHandler(OpenThreatEntryView);
                    Grid.SetRow(cell, i);
                    Grid.SetColumn(cell, j);
                    grid.Children.Add(cell);
                }
            }
            //change page buttons
            if (tableModel.PagePointer < tableModel.PageCount)
            {
                Button button = new Button();
                button.Content = "Next Page";
                button.Tag = tableModel.PagePointer;
                button.HorizontalAlignment = HorizontalAlignment.Right;
                button.VerticalAlignment = VerticalAlignment.Bottom;
                button.Background = Brushes.AliceBlue;
                button.Margin = new Thickness(50,20,50,20);
                button.Padding = new Thickness(5);
                button.FontSize = 14; button.Width = 100;
                button.Click += new RoutedEventHandler(NextPageEvent);
                Grid.SetRow(button, table.Length);
                Grid.SetColumn(button, 1);
                grid.Children.Add(button);
            }
            if (tableModel.PagePointer > 1)
            {
                Button button = new Button();
                button.Content = "Previous Page";
                button.Tag = tableModel.PagePointer;
                button.HorizontalAlignment = HorizontalAlignment.Left;
                button.VerticalAlignment = VerticalAlignment.Bottom;
                button.Background = Brushes.AliceBlue;
                button.Margin = new Thickness(50, 20, 50, 20);
                button.Padding = new Thickness(5);
                button.FontSize = 14; button.Width = 100;
                button.Click += new RoutedEventHandler(PreviousPageEvent);
                Grid.SetRow(button, table.Length);
                Grid.SetColumn(button, 0);
                grid.Children.Add(button);
            }
            return grid;
        }
        public void NextPageEvent(object sender, EventArgs e)
        {
            tableModel.PagePointer = tableModel.PagePointer + 1;
            FlushGrid();
        }
        public void PreviousPageEvent(object sender, EventArgs e)
        {
            tableModel.PagePointer = tableModel.PagePointer - 1;
            FlushGrid();
        }
        private void FlushGrid()
        {
            (this.Content as StackPanel).Children.Remove(grid);
            grid = ConstructGrid();
            (this.Content as StackPanel).Children.Add(grid);
        }
        public void OpenThreatEntryView(object sender, EventArgs e)
        {
            NavigationService.GetNavigationService(this).Navigate(new ThreatEntryView(tableModel.GetEntryTable((int)(sender as Button).Tag)));
        }
        public void UpdateTable(object sender, EventArgs e)
        {
            try
            {
                int count = tableModel.UpdateTable();
                (this.Content as StackPanel).Children.Remove(grid);
                grid = ConstructGrid();
                (this.Content as StackPanel).Children.Add(grid);
                StringBuilder str = new StringBuilder(count.ToString());
                str.Append(" entries changed\n");
                int i = 0; int m = tableModel.Messages.Count;
                foreach (string s in tableModel.Messages)
                {
                    str.Append(s);
                    str.Append('\n');
                    i++; m--;
                    if (i > 50) break;
                }
                if (m > 0) str.Append(m + " messages more...");
                MessageWindow messageWindow = new MessageWindow("Table is successfully updated", str.ToString());
                messageWindow.Show();
            }
            catch (Exception ex)
            {
                MessageWindow messageWindow = new MessageWindow("Some errors occurred during updating", ex.Message);
                messageWindow.Show();
            }
        }
        public void SaveTable(object sender, EventArgs e)
        {
            try 
            {
                string path = "thrlist.xlsx";
                tableModel.SaveTableToFile(path);
                MessageWindow messageWindow = new MessageWindow("File is successfully saved", System.IO.Directory.GetCurrentDirectory() + "\\" + path);
                messageWindow.Show();
            }
            catch (Exception ex)
            {
                MessageWindow messageWindow = new MessageWindow("Some errors occurred during saving", ex.Message);
                messageWindow.Show();
            }
        }
    }
}
