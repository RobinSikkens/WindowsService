using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace TestService
{
    /// <summary>
    /// The program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main()
        {
            ServiceBase[] servicesToRun = new ServiceBase[]
            {
                new TestService()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
