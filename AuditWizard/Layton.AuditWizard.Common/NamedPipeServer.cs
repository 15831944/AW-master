using System; 
using System.Collections.Generic; 
using System.Text; 
using System.Runtime.InteropServices; 
using System.Diagnostics; 
using System.ComponentModel; 
using System.IO; 
using System.Threading; 
using Microsoft.Win32.SafeHandles;

namespace Layton.AuditWizard.Common
{
    /// <summary> 
    /// Imported named-pipe entry points for P/Invoke into native code 
    /// </summary> 
    public class NamedPipeInterop 
    {
        // #defines related to named-pipe processing 
		public const int PIPE_ACCESS_OUTBOUND = 0x00000002; 
		public const int PIPE_ACCESS_DUPLEX = 0x00000003; 
		public const int PIPE_ACCESS_INBOUND = 0x00000001;

		public const int PIPE_WAIT = 0x00000000; 
		public const int PIPE_NOWAIT = 0x00000001; 
		public const int PIPE_READMODE_BYTE = 0x00000000; 
		public const int PIPE_READMODE_MESSAGE = 0x00000002; 
		public const int PIPE_TYPE_BYTE = 0x00000000; 
		public const int PIPE_TYPE_MESSAGE = 0x00000004;

		public const int PIPE_CLIENT_END = 0x00000000; 
		public const int PIPE_SERVER_END = 0x00000001;

		public const int PIPE_UNLIMITED_INSTANCES = 255;

		public const uint NMPWAIT_WAIT_FOREVER = 0xffffffff; 
		public const uint NMPWAIT_NOWAIT = 0x00000001; 
		public const uint NMPWAIT_USE_DEFAULT_WAIT = 0x00000000;

		public const uint GENERIC_READ = (0x80000000); 
		public const uint GENERIC_WRITE = (0x40000000); 
		public const uint GENERIC_EXECUTE = (0x20000000); 
		public const uint GENERIC_ALL = (0x10000000);

		public const int CREATE_NEW = 1;
		public const int CREATE_ALWAYS = 2;
		public const int OPEN_EXISTING = 3;
		public const int OPEN_ALWAYS = 4;
		public const int TRUNCATE_EXISTING = 5;

		public static IntPtr INVALID_HANDLE_VALUE = (IntPtr)(-1);
		public const int ERROR_PIPE_BUSY = 231;
		public const int ERROR_NO_DATA = 232;
		public const int ERROR_PIPE_NOT_CONNECTED = 233;
		public const int ERROR_MORE_DATA = 234;
		public const int ERROR_PIPE_CONNECTED = 535;
		public const int ERROR_PIPE_LISTENING = 536;

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool CallNamedPipe(
            string lpNamedPipeName,
            byte[] lpInBuffer,
            uint nInBufferSize,
            byte[] lpOutBuffer,
            uint nOutBufferSize,
            byte[] lpBytesRead,
            uint nTimeOut);

		[DllImport("kernel32.dll", SetLastError = true)] 
		public static extern bool CloseHandle(SafeFileHandle hObject);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool ConnectNamedPipe( 
            SafeFileHandle hNamedPipe,// Handle to named pipe 
            IntPtr lpOverlapped // Overlapped structure 
            );

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern SafeFileHandle CreateNamedPipe( 
            String lpName,       // Pipe name 
            uint dwOpenMode,     // Pipe open mode 
            uint dwPipeMode,     // Pipe-specific modes 
            uint nMaxInstances,  // Maximum number of instances 
            uint nOutBufferSize,     // Output buffer size 
            uint nInBufferSize,  // Input buffer size 
            uint nDefaultTimeOut,    // Time-out interval 
            //SecurityAttributes attr 
            IntPtr pipeSecurityDescriptor  // Security descriptor 
            );

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern SafeFileHandle CreatePipe(
            SafeFileHandle hReadPipe,
            SafeFileHandle hWritePipe,
            IntPtr lpPipeAttributes,
            uint nSize);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern SafeFileHandle CreateFile( 
            String lpFileName,// File name 
            uint dwDesiredAccess,// Access mode 
            uint dwShareMode,// Share mode 
            IntPtr attr,     // Security descriptor 
            uint dwCreationDisposition,  // How to create 
            uint dwFlagsAndAttributes,   // File attributes 
            uint hTemplateFile);// Handle to template file

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool DisconnectNamedPipe(SafeFileHandle hNamedPipe);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool FlushFileBuffers(SafeFileHandle hFile);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool GetNamedPipeHandleState( 
            SafeFileHandle hNamedPipe, 
            IntPtr lpState, 
            IntPtr lpCurInstances, 
            IntPtr lpMaxCollectionCount, 
            IntPtr lpCollectDataTimeout, 
            string lpUserName, 
            uint nMaxUserNameSize);

		[DllImport("KERNEL32.DLL", SetLastError = true)]
		public static extern bool GetNamedPipeInfo( 
            SafeFileHandle hNamedPipe, 
            out uint lpFlags, 
            out uint lpOutBufferSize, 
            out uint lpInBufferSize, 
            out uint lpMaxInstances);

		[DllImport("KERNEL32.DLL", SetLastError = true)]
		public static extern bool PeekNamedPipe( 
            SafeFileHandle hNamedPipe, 
            byte[] lpBuffer, 
            uint nBufferSize, 
            byte[] lpBytesRead, 
            out uint lpTotalBytesAvail, 
            out uint lpBytesLeftThisMessage);

		[DllImport("KERNEL32.DLL", SetLastError = true)]
		public static extern bool SetNamedPipeHandleState( 
            SafeFileHandle hNamedPipe, 
            ref int lpMode, 
            IntPtr lpMaxCollectionCount, 
            IntPtr lpCollectDataTimeout);

		[DllImport("KERNEL32.DLL", SetLastError = true)]
		public static extern bool TransactNamedPipe( 
            SafeFileHandle hNamedPipe, 
            byte[] lpInBuffer, 
            uint nInBufferSize, 
            [Out] byte[] lpOutBuffer, 
            uint nOutBufferSize, 
            IntPtr lpBytesRead, 
            IntPtr lpOverlapped);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool WaitNamedPipe( 
            string name, 
            uint timeout);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool ReadFile( 
            SafeFileHandle hFile,           // Handle to file 
            byte[] lpBuffer,        // Data buffer 
            uint nNumberOfBytesToRead,  // Number of bytes to read 
            byte[] lpNumberOfBytesRead, // Number of bytes read 
            uint lpOverlapped       // Overlapped buffer 
            );

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool WriteFile(
            SafeFileHandle hFile,              // Handle to file
            byte[] lpBuffer,           // Data buffer
            uint nNumberOfBytesToWrite,    // Number of bytes to write
            byte[] lpNumberOfBytesWritten,  // Number of bytes written
            uint lpOverlapped         // Overlapped buffer
            );
    }


    /// <summary> 
    /// NamedPipeServer - An implementation of a synchronous, message-based, 
    /// named-pipe server 
    /// 
    /// </summary> 
    public class NamedPipeServer : IDisposable 
    {
        #region Private Members
        /// <summary>
        /// The pipe handle
        /// </summary>
        private SafeFileHandle _handle = new SafeFileHandle(NamedPipeInterop.INVALID_HANDLE_VALUE, true);

		/// <summary>
        /// The name of the pipe
        /// </summary>
        private string _pipeName = "";

        /// <summary>
        /// Default size of message buffer to read
        /// </summary>
        private int _receiveBufferSize = 1024;

        /// <summary>
        /// Track if dispose has been called
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// PIPE_SERVER_BUFFER_SIZE set to 8192 by default
        /// </summary>
        private const int PIPE_SERVER_BUFFER_SIZE = 8192;
        #endregion

        #region Construction / Cleanup
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="pipeBaseName">the base name of the pipe</param>
        /// <param name="msgReceivedDelegate">delegate to be notified when
        /// a message is received</param>
        public NamedPipeServer(string pipeBaseName)
        {
            // Assemble the pipe name
            _pipeName = "\\\\.\\PIPE\\" + pipeBaseName;
            Trace.WriteLine("NamedPipeServer using pipe name " + _pipeName);
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~NamedPipeServer()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (!this.disposed) 
            {
                ClosePipe();
            }
            disposed = true;
        }

        private void ClosePipe() 
        { 
            Trace.WriteLine("NamedPipeServer closing pipe");

            if (!_handle.IsInvalid)
            {
                _handle.Close();
            }
        }
        /// <summary>
        /// Close - because it is more intuitive than Dispose…
        /// </summary>
        public void Close()
        {
            ClosePipe();
        }
        #endregion

        #region Properties
        /// <summary>
        /// PipeName
        /// </summary>
        /// <returns>the composed pipe name</returns>
        public string PipeName
        { get { return _pipeName; } }

        /// <summary> 
        /// ReceiveBufferSize Property - the size used to create receive buffers 
        /// for messages received using WaitForMessage 
        /// </summary> 
        public int ReceiveBufferSize 
        {
            get { return _receiveBufferSize; }
            set { _receiveBufferSize = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// CreatePipe - create the named pipe
        /// </summary>
        /// <returns>true is pipe created</returns>
        public bool CreatePipe()
        {
            // Make a named pipe in message mode
            _handle = NamedPipeInterop.CreateNamedPipe(_pipeName, 
                NamedPipeInterop.PIPE_ACCESS_DUPLEX, 
                NamedPipeInterop.PIPE_TYPE_MESSAGE | NamedPipeInterop.PIPE_READMODE_MESSAGE | NamedPipeInterop.PIPE_WAIT, 
                NamedPipeInterop.PIPE_UNLIMITED_INSTANCES, 
                PIPE_SERVER_BUFFER_SIZE, 
                PIPE_SERVER_BUFFER_SIZE, 
                NamedPipeInterop.NMPWAIT_WAIT_FOREVER, 
                IntPtr.Zero);

            // Make sure we got a good one
            if (_handle.IsInvalid)
            {
                Debug.WriteLine("Could not create the pipe (" + _pipeName + ") - os returned " + Marshal.GetLastWin32Error());
                return false;
            }
            return true;
        }

        /// <summary>
        /// WaitForClientConnect - wait for a client to connect to this pipe
        /// </summary>
        /// <returns>true if connected, false if timed out</returns>
        public bool WaitForClientConnect()
        {
            // Wait for someone to talk to us. 
            return NamedPipeInterop.ConnectNamedPipe(_handle, IntPtr.Zero); 
        }

        /// <summary>
        /// WaitForMessage - have the server wait for a message
        /// </summary>
        /// <returns>a non-null MessageStream if it got a message, null if timed out or error 
        /// </returns>
        public MemoryStream WaitForMessage() 
        {
				bool fullyRead = false;
            string errMsg = "";
            int errCode = 0;

			// They want to talk to us, read their messages and write replies
            MemoryStream receiveStream = new MemoryStream();
            byte[] buffer = new byte[_receiveBufferSize];
            byte[] _numReadWritten = new byte[4];

            // Need to read the whole message and put it in one message byte buffer
            do
            {
                // Read the response from the pipe
                if (!NamedPipeInterop.ReadFile(
                    _handle,    // Pipe handle
                    buffer,   // Buffer to receive reply
                    (uint)_receiveBufferSize,     // Size of buffer
                    _numReadWritten,  // Number of bytes read
                    0)) // Not overlapped
                {
                    // Failed, not just more data to come
                    errCode = Marshal.GetLastWin32Error( );
                    if (errCode != NamedPipeInterop.ERROR_MORE_DATA)
					{
                        break;
					}
                    else
                    {
                        errMsg = string.Format("Could not read from pipe with error {0}", errCode);
                        Trace.WriteLine(errMsg);
                        throw new Win32Exception(errCode, errMsg);
                    }
                }
                else
                {
                    // We succeeded and no more data is coming
                    fullyRead = true;
                }

				// Concat the message bytes to the stream
                receiveStream.Write(buffer, 0, buffer.Length);

            } while (!fullyRead);

			// Return the message we received if any
            if (receiveStream.Length > 0)
                return receiveStream;
            else // Didn't receive anything
                return null;
        }
        #endregion
    }
}