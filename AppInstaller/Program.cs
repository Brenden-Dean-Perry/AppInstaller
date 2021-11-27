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
            string AppName = "DataManager_216";
            string AppVersion = "1_0_0_1";
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
            System.Threading.Thread.Sleep(2000);

            //Check if there are new files available
            string Newpath = @"C:\Users\brend\OneDrive\Desktop\216\DataManager\Production\" + AppName + "_" + AppVersion + @"\";

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
                string path = @"C:\Users\brend\OneDrive\Desktop\216\DataManager\Current Install\";
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
