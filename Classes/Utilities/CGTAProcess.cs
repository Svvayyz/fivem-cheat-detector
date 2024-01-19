using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fivemhackdetector.Classes
{
    internal class CGTAProcess
    {
        public bool bFound = false;

        public List<Process> Get()
        {
            List<Process> result = new List<Process>();

            foreach (Process process in Process.GetProcesses())
            {
                if (process.ProcessName.Contains("GTAProcess"))
                {
                    bFound = true;

                    result.Add(process);
                }
            }

            return result;
        }
    }
}
