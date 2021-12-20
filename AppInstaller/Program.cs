using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime;

namespace AppInstaller
{
    class Program
    {
        static void Main(string[] args)
        {
            string ParentPath = @"C:\Users\brend\OneDrive\Desktop\216\";

            //Paramater check
            ParamaterValidation(args);

            string AppName = args[0];
            string AppVersion = args[1];
            int HeaderSize = 100;

            //Header
            BuildHeader(AppName, AppVersion, HeaderSize);

            //End old process if its still running
            KillRunningProcesses(AppName);

            //Check if there are new files available
            string Newpath = ParentPath + AppName + @"\Production\" + AppName + "_" + AppVersion + @"\";

            if (!System.IO.Directory.Exists(Newpath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Cannot find new version location: " + Newpath);
                Console.ForegroundColor = ConsoleColor.White;
                EndProgram(false);
            }
            else
            {
                //Build list of new file names to copy over
                string[] newFiles = System.IO.Directory.GetFiles(Newpath);
                List<string> newFileNames = new List<string>();
                foreach (string file in newFiles)
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
                    EndProgram(false);
                }

                //List Old Files
                string path = ParentPath + AppName + @"\Current Install\";
                string[] files = System.IO.Directory.GetFiles(path);
                foreach (string file in files)
                {
                    //Intial Report
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Deleting File: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(file);

                    if (System.IO.File.Exists(file) && file.ToLower() != "version.txt")
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
                CopyOverNewFiles(Newpath, newFileNames, path);

                //Update version
                UpdateVersionFile(ParentPath + AppName + @"\Current Install\", AppVersion);

            }

            EndProgram(true);
        }

        private static void UpdateVersionFile(string FilePath,string appVersion)
        {
            string path = FilePath + "version.txt";
            Console.WriteLine("Updating version file");
            System.IO.File.WriteAllText(path, appVersion);
            Console.Write("Updated version file: ");
            if (System.IO.File.ReadAllText(path) == appVersion)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Ok");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error");
                Console.ForegroundColor = ConsoleColor.White;
                EndProgram(false);
            }
        }

        private static void CopyOverNewFiles(string SourcePath, List<string> newFileNames, string Targetpath)
        {
            foreach (string newfile in newFileNames)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Copying File: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(Targetpath);

                if (System.IO.Directory.Exists(Targetpath))
                {
                    if (System.IO.File.Exists(SourcePath + newfile))
                    {
                        System.IO.File.Copy(SourcePath + newfile, Targetpath + newfile);
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Cannot file directory: " + Targetpath);
                    Console.ForegroundColor = ConsoleColor.White;
                }


                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Copyed File: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(Targetpath + newfile + ": ");
                if (!System.IO.File.Exists(Targetpath + newfile))
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

        private static void KillRunningProcesses(string AppName)
        {
            if (ProcessManager.CheckForRunningProcess(AppName))
            {
                Console.Write("Hold on. We need to shut down the old Applications background processes: ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error");
                Console.ForegroundColor = ConsoleColor.White;

                int ProccessesKilled = ProcessManager.KillRunningProcesses(AppName);
                if (ProccessesKilled > 0)
                {
                    Console.Write(ProccessesKilled.ToString() + " Background processes have been shut down: ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Ok");
                    Console.ForegroundColor = ConsoleColor.White;
                }

                //If processes are still running. End update
                System.Threading.Thread.Sleep(3000); //Give it a few seconds to clear
                bool continueResponse = true;
                if (ProcessManager.CheckForRunningProcess(AppName))
                {
                    string result = String.Empty;
                    while (continueResponse == true)
                    {
                        Console.WriteLine("Would you like to try again? (Y/N)");
                        result = Console.ReadLine().Trim().ToLower();
                        if (result == "y")
                        {
                            if (!ProcessManager.CheckForRunningProcess(AppName))
                            {
                                Console.Write("No background processes are running. Good to proceed with the installation: ");
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("Ok");
                                Console.ForegroundColor = ConsoleColor.White;
                                continueResponse = false;
                            }
                        }
                        else
                        {
                            continueResponse = false;
                            Console.Write("Processes are still running. Cannot continue: ");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Error");
                            Console.ForegroundColor = ConsoleColor.White;
                            EndProgram(false);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("No background processes are running. Good to proceed with the installation: ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Ok");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private static void ParamaterValidation(string[] args)
        {
            if (args.Length < 2)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: No paramaters were passes to this program.");
                Console.WriteLine("Required paramaters: (string) AppName, (string) InstallVersion (format: 1_0_2_5)");
                Console.ReadLine();
                EndProgram(false);
            }
        }

        private static void BuildHeader(string AppName, string AppVersion, int HeaderSize)
        {
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
        }

        private async static void EndProgram(bool Successful)
        {
            //Close
            if(Successful == true)
            {
                Console.WriteLine("");
                Console.WriteLine("Everything seems fine. Wating 4 seconds, then closing.");
                await System.Threading.Tasks.Task.Run(() => System.Threading.Thread.Sleep(4000));
            }
            else
            {
                Console.ReadLine();
            }
            Environment.Exit(0);
        }
    }
}
