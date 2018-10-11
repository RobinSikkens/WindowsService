namespace TestService
{
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.ServiceProcess;
    using System.Timers;

    /// <summary>
    /// The service state.
    /// </summary>
    public enum ServiceState
    {
        SERVICE_STOPPED = 0x0000_0001,
        SERVICE_START_PENDING = 0x0000_0002,
        SERVICE_STOP_PENDING = 0x0000_0003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
        public int dwServiceType;
        public ServiceState dwCurrentState;
        public int dwControlsAccepted;
        public int dwWin32ExitCode;
        public int dwServiceSpecificExitCode;
        public int dwCheckPoint;
        public int dwWaitHint;
    }


    /// <summary>
    /// Runs a <see cref="TestService"/> on windows that does absolutely
    /// nothing.
    /// </summary>
    /// <inheritdoc />
    public partial class TestService : ServiceBase
    {

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:TestService.TestService" /> class.
        /// </summary>
        public TestService()
        {
            this.InitializeComponent();

            this.eventLog = new EventLog();
            if (!EventLog.SourceExists("TestServiceSource"))
            {
                EventLog.CreateEventSource(
                    "TestServiceSource", "TestServiceLog");
            }

            this.eventLog.Source = "TestServiceSource";
            this.eventLog.Log = "TestServiceLog";
        }

        /// <summary>
        /// Gets or sets the event id.
        /// </summary>
        private int EventId { get; set; } = 1;

        /// <inheritdoc />
        /// <summary>
        /// Sets up the service when it is started.
        /// </summary>
        /// <param name="args">The <see cref="M:TestService.TestService.OnStart(System.String[])" /> arguments.</param>
        protected override void OnStart(string[] args)
        {
            // Update the service state to Start Pending.
            var serviceStatus =
                new ServiceStatus { dwCurrentState = ServiceState.SERVICE_START_PENDING, dwWaitHint = 100000 };
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            this.eventLog.WriteEntry("Started Service");

            // Set up timer that triggers every minute
            var timer = new Timer { Interval = 60_000 };
            timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
            timer.Start();

            // Update the service state to Running.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        /// <summary>
        /// Runs whenever the service timer goes off.
        /// </summary>
        /// <param name="sender">The <paramref name="sender" /> .</param>
        /// <param name="args">The args.</param>
        protected void OnTimer(object sender, ElapsedEventArgs args)
        {
            // TODO: Monitoring
            this.eventLog.WriteEntry("Monitoring", EventLogEntryType.Information, this.EventId++);
        }

        /// <inheritdoc />
        /// <summary>
        /// Break-down logic when stopping the service.
        /// </summary>
        protected override void OnStop()
        {
            // Update the service state to Start Pending.
            var serviceStatus =
                new ServiceStatus { dwCurrentState = ServiceState.SERVICE_STOP_PENDING, dwWaitHint = 100000 };
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            this.eventLog.WriteEntry("Stopping Service");

            // Update the service state to Running.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        // ReSharper disable once PrivateMembersMustHaveComments
        private static extern bool SetServiceStatus(System.IntPtr handle, ref ServiceStatus serviceStatus);
    }
}
