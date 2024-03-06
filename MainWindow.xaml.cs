using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;

namespace DLL_injector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public MyProcess? SelectedProcess
        {
            get { return this.SelectProcessComboBox.SelectedProcess; }
        }
        public string[] Files { get { return this.FilesMenu.AllFilesCollection.ToArray(); } }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            bool previousValue;
            Win32Api.RtlAdjustPrivilege(19, true, false, out previousValue);

        }

        private void Inject_Click(object sender, RoutedEventArgs e)
        {
            if (this.SelectedProcess == null) { return; }
            if (this.Files == null) { return; }
            if (this.Files.Length == 0) { return; }

            // attach dll files to project
            foreach (string d in this.Files)
            {
                InjectDll(d);
            }


        }

        private void InjectDll(string file)
        {
            var bytes = Encoding.Default.GetBytes(file);
            // antivirus alert, need to implement NtApi
            //IntPtr address = Win32Api.VirtualAllocEx((IntPtr)this.SelectedProcess.MyHandle, IntPtr.Zero, 0x1000, 0x3000, 0x4);
            var processHandle = this.SelectedProcess.MyHandle;
            IntPtr baseAddress = IntPtr.Zero;
            ulong RegionSize = 0xF000;
            UIntPtr status = Win32Api.NtAllocateVirtualMemory(
                processHandle,
                ref baseAddress,
                (UInt64)0,
                ref RegionSize,
                0x3000,
                0x4
                );
            if (status != 0)
            {
                Trace.WriteLine("Error with NtAllocateVirtualMemory()");
            }


            Trace.WriteLine("baseAddress = " + baseAddress);
            IntPtr outSize;
            Boolean res = Win32Api.WriteProcessMemory(processHandle, baseAddress, bytes, file.Length, out outSize);

            IntPtr loadLibAddr = Win32Api.GetProcAddress(Win32Api.GetModuleHandle("kernel32.dll"), "LoadLibraryA");

            IntPtr hThread = Win32Api.CreateRemoteThread(processHandle, IntPtr.Zero, 0, loadLibAddr, baseAddress, 0, IntPtr.Zero);
        }
    }







}
