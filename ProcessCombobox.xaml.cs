using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DLL_injector
{
    /// <summary>
    /// Logica di interazione per ProcessCombobox.xaml
    /// </summary>
    public partial class ProcessCombobox : UserControl, INotifyPropertyChanged, INotifyCollectionChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        private void OnPropertyChange(string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private void OnCollectionChange(NotifyCollectionChangedAction action = NotifyCollectionChangedAction.Add)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action));
        }
        public ObservableCollection<MyProcess> ProcessList { get; set; } = new ObservableCollection<MyProcess>();
        private MyProcess? _SelectedProcess;
        public MyProcess? SelectedProcess
        {
            get { return _SelectedProcess; }
            set
            {
                if (_SelectedProcess != value)
                {
                    _SelectedProcess = value;
                    OnPropertyChange(nameof(SelectedProcess));
                }
            }
        }
        public ProcessCombobox()
        {
            InitializeComponent();
            this.RefreshProcessList();
            this.DataContext = this;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                this.SelectedProcess = e.AddedItems[0] as MyProcess;
            }
            catch (Exception)
            {
            }

        }

        public void RefreshProcessList()
        {
            var rawProcesses = Process.GetProcesses();
            ProcessList.Clear();
            foreach (Process process in rawProcesses)
            {
                MyProcess proc = new MyProcess(process);
                if (proc.MyHandle != (IntPtr)0)
                    ProcessList.Add(proc);
            }
        }



        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            this.RefreshProcessList();
        }
    }

    public class MyProcess
    {
        public IntPtr MyHandle { get; } = 0x0;
        public Process? Process { get; }
        public ImageSource? ImageSource { get; }
        public MyProcess()
        {
            MyHandle = (IntPtr)0;
            Process = new Process();
            Process curr = Process.GetCurrentProcess();
            if (curr is null) throw new Exception("Can't get CurrentProcess()");
            ImageSource = Imaging.CreateBitmapSourceFromHIcon(
                Icon.ExtractAssociatedIcon(Process?.MainModule?.FileName).Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()
                );

        }
        public MyProcess(Process p)
        {
            this.Process = p;
            this.MyHandle = Win32Api.OpenProcess(Win32Api.OpenProcessFlags.PROCESS_ALL_ACCESS, false, (uint)this.Process.Id);

            if (MainModuleExists(this.Process))
            {
                Icon icon;
                var filename = this.Process?.MainModule?.FileName;
                if (filename != null)
                {
                    try
                    {
                        icon = Icon.ExtractAssociatedIcon(filename);
                        this.ImageSource = Imaging.CreateBitmapSourceFromHIcon(
                            icon.Handle,
                            Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions()
                        );
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                }
            }

        }

        public bool MainModuleExists(Process p)
        {
            if (p == null) return false;
            ProcessModule module;
            try
            {
                module = p?.MainModule;
            }
            catch (Exception)
            {
                return false;
            }
            return module != null;
        }
    }
}
