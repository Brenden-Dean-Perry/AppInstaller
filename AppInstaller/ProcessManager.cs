using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace AppInstaller
{
    static class ProcessManager
    {
        public static bool CheckForRunningProcess(string AppName)
        {
            bool isRunning = false;
            int processCounter = 0;
            Process[] processList = Process.GetProcesses();

            foreach(Process process in processList)
            {
                if(process.ProcessName.ToLower() == AppName.ToLower())
                {
                    processCounter++;
                    isRunning = true;
                }
            }

            return isRunning;
        }

        public static int KillRunningProcesses(string AppName)
        {
            int processCounter = 0;
            Process[] processList = Process.GetProcesses();

            foreach (Process process in processList)
            {
                if (process.ProcessName.ToLower() == AppName.ToLower())
                {

                    try
                    {
                        process.Kill();
                        processCounter++;
                    }
                    catch { }
                }
            }

            return processCounter;
        }
    }
}
