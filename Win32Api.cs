using System;
using System.Runtime.InteropServices;
using System.Text;



namespace DLL_injector
{
    internal class Win32Api
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);
        public class OpenProcessFlags
        {
            public const uint PROCESS_ALL_ACCESS = (uint)(0x000F0000L | 0x00100000L | 0xFFFF);
            public const uint PROCESS_CREATE_PROCESS = (uint)0x0080;
            public const uint PROCESS_CREATE_THREAD = (uint)0x0002;
            public const uint PROCESS_DUP_HANDLE = (uint)0x0040;
            public const uint PROCESS_QUERY_INFORMATION = (uint)0x0400;
            public const uint PROCESS_QUERY_LIMITED_INFORMATION = (uint)0x1000;
            public const uint PROCESS_SET_QUOTA = (uint)0x0100;
            public const uint PROCESS_SUSPEND_RESUME = (uint)0x0800;
            public const uint PROCESS_TERMINATE = (uint)0x0001;
            public const uint PROCESS_VM_OPERATION = (uint)0x0008;
            public const uint PROCESS_VM_READ = (uint)0x0010;
            public const uint PROCESS_VM_WRITE = (uint)0x0020;
            public const uint SYNC = (uint)0x00100000L;
        }

        [DllImport("psapi.dll", SetLastError = true)]
        public static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, [In][MarshalAs(UnmanagedType.U4)] int nSize);


        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
        uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            byte[] lpBuffer,
            Int32 nSize,
            out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateRemoteThread(IntPtr hProcess,
        IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);


        [DllImport("ntdll.dll")]
        public static extern uint RtlAdjustPrivilege(int Privilege, bool bEnablePrivilege, bool IsThreadPrivilege,
                                                      out bool PreviousValue);

        [DllImport("ntdll.dll")]
        public static extern uint NtAllocateVirtualMemory(IntPtr hProcess, ref IntPtr baseAdress, UInt64 ZeroBits, ref ulong RegionSize, uint AllocationType, uint Protect);
    }
}
