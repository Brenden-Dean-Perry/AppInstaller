using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime;

namespace AppInstaller
{
    class Program
    {
        static void Main(string AppName, string AppVersion, string[] args)
        {
            int HeaderSize = 100;

            //Header
            Console.WriteLine(String.Concat(Enumerable.Repeat("_", HeaderSize)));
            Console.WriteLine(String.Concat(Enumerable.Repeat("/", HeaderSize)));
            Console.WriteLine(String.Concat(Enumerable.Repeat("/", HeaderSize)));
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("AppInstaller - " + AppName);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Installing version: " + AppVersion);
            Console.WriteLine(String.Concat(Enumerable.Repeat("/", HeaderSize)));
            Console.WriteLine(String.Concat(Enumerable.Repeat("/", HeaderSize)));
            Console.WriteLine(String.Concat(Enumerable.Repeat("_", HeaderSize)));
            Console.WriteLine("Beginning Install..");
            System.Threading.Thread.Sleep(2000);

            //End old process if its still running
            if (ProcessManager.CheckForRunningProcess(AppName))
            {
                Console.Write("Hold on. We need to shut down the old Applications background processes: ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error");
                Console.ForegroundColor = ConsoleColor.White;

                if(ProcessManager.KillRunningProcesses(AppName) >0)
                {
                    Console.WriteLine("No background processes have been shut down: ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Ok");
                    Console.ForegroundColor = ConsoleColor.White;
                }

                if (ProcessManager.CheckForRunningProcess(AppName))
                {
                    Console.Write("Processes are still running. Cannot continue: ");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error");
                    Console.ForegroundColor = ConsoleColor.White;
                    EndProgram();
                }
            }
            else
            {
                Console.WriteLine("No background processes are running. Good to proceed with the installation: ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Ok");
                Console.ForegroundColor = ConsoleColor.White;
            }

            //Check if there are new files available
            string Newpath = @"C:\Users\brend\OneDrive\Desktop\216\" + AppName + @"\Production\" + AppName + "_" + AppVersion + @"\";

            if (!System.IO.Directory.Exists(Newpath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Cannot find new version location: " + Newpath);
                Console.ForegroundColor = ConsoleColor.White;
                EndProgram();
            }
            else
            {
                string[] newFiles = System.IO.Directory.GetFiles(Newpath);
                List<string> newFileNames = new List<string>();
                foreach(string file in newFiles)
                {
                    newFileNames.Add(System.IO.Path.GetFileName(file));
                }

                Console.WriteLine("New file count: " + newFiles.Length.ToString());
                //Check if you have the bare minimum files
                if (newFiles.Length <= 2 || !Array.Exists(newFiles, x => x.EndsWith(".exe")))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Looks like you are missing files. Your application will not be updated.");
                    Console.ForegroundColor = ConsoleColor.White;
                    EndProgram();
                }

                //List Old Files
                string path = @"C:\Users\brend\OneDrive\Desktop\216\" + AppName + @"\Current Install\";
                string[] files = System.IO.Directory.GetFiles(path);
                foreach (string file in files)
                {
                    //Intial Report
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Deleting File: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(file);

                    if (System.IO.File.Exists(file))
                    {
                        System.IO.File.Delete(file);
                    }

                    //Delete old file
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Deleted File: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(file + ": ");

                    if (System.IO.File.Exists(file))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("OK");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }

                //Break
                Console.WriteLine("");

                //Copy over new files
                foreach (string newfile in newFileNames)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Copying File: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(path);

                    if (System.IO.Directory.Exists(path))
                    {
                        if (System.IO.File.Exists(Newpath + newfile) )
                        {
                            System.IO.File.Copy(Newpath + newfile, path + newfile);
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Cannot file directory: " + path);
                        Console.ForegroundColor = ConsoleColor.White;
                    }


                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Copyed File: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(path + newfile + ": ");
                    if (!System.IO.File.Exists(path + newfile))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("OK");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }

            EndProgram();
        }

        private static void EndProgram()
        {
            //Close
            Console.WriteLine("");
            Console.WriteLine("Everything seems fine. Wating 5 seconds, then closing.");
            System.Threading.Thread.Sleep(5000);
            Environment.Exit(1);
        }
    }
}
