using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DLL_injector
{
    /// <summary>
    /// Logica di interazione per FilesSelect.xaml
    /// </summary>
    public partial class FilesSelect : UserControl
    {
        public ObservableCollection<string> SelectedFiles { get; set; }
        public ObservableCollection<string> AllFilesCollection { get; set; }
        public FilesSelect()
        {
            InitializeComponent();
            SelectedFiles = new ObservableCollection<string>();
            AllFilesCollection = new ObservableCollection<string>();
            this.DataContext = this;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e?.AddedItems != null)
            {
                foreach (string item in e.AddedItems)
                {
                    if (!SelectedFiles.Contains(item)) SelectedFiles.Add((string)item);
                }
            }

            if (e?.RemovedItems != null)
            {
                foreach (string item in e.RemovedItems)
                {
                    if (SelectedFiles.Contains(item)) SelectedFiles.Remove((string)item);
                }
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".dll";
            dlg.Filter = "Dynamic Link Libraries (*.dll)|*.dll";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                var files = dlg.FileNames.ToList();
                foreach (string file in files)
                {
                    if (!AllFilesCollection.Contains(file)) AllFilesCollection.Add(file);
                }
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            var lst = this.SelectedFiles.ToList();
            if (this.SelectedFiles.Count > 0)
            {
                foreach (var item in lst)
                {
                    this.AllFilesCollection.Remove(item);
                }
                this.SelectedFiles.Clear();
            }
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    if (!AllFilesCollection.Contains(file)) AllFilesCollection.Add(file);
                }
            }
        }
    }
}
