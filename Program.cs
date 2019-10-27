using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.ServiceModel;
using workWithGetDTsClassesFile.ISca;

namespace workWithGetDTsClassesFile
{
    class Program
    {
        static void Main(string[] args)
        {
            long inputedPA = 0;
            string lookedSrv = null;
            string sortTDsType = null;
            string sortSrvsType = null;

            bool lookSrv = false;
            bool sortIdx = false;
            bool writeFile = false;
            bool writeConsole = true;

            string pathToFile = null;
            string fileName = null;

            if (args.Length == 0)
                exitError();

            if (args.Length == 1 && args[0] == "/?")
                writeAppHelp();

            for (int i = 0; i < args.Length; i++)
            {
                if (i == 0 && args[i].Length != 12)
                    exitError();
                else if (i == 0 && !long.TryParse(args[0], out inputedPA))
                    exitError();

                if (i > 0 && !string.Equals(args[i].ToLower(), "asc") && !string.Equals(args[i].ToLower(), "desc") 
                    && !string.Equals(args[i].ToLower(), "/f") && !string.Equals(args[i].ToLower(), "/c") 
                    && !string.Equals(args[i].ToLower(), "/st") && !string.Equals(args[i].ToLower(), "/ss")
                    && !args[i].Contains(":\\"))
                {
                    foreach (char c in args[i])
                        if (!char.IsLetterOrDigit(c))
                            exitError();
                    lookedSrv = Convert.ToString(args[i]).ToUpper();
                    lookSrv = true;
                }

                if (i > 0 && string.Equals(args[i].ToLower(), "/st"))
                {
                    sortTDsType = "asc";
                    sortIdx = true;
                }
                if (i > 0 && string.Equals(args[i-1].ToLower(), "/st") && (string.Equals(args[i].ToLower(), "asc") || string.Equals(args[i].ToLower(), "desc")))
                {
                    sortTDsType = Convert.ToString(args[i].ToLower());
                    sortIdx = true;
                }

                if (i > 0 && string.Equals(args[i].ToLower(), "/ss"))
                {
                    sortSrvsType = "asc";
                    sortIdx = true;
                }
                if (i > 0 && string.Equals(args[i-1].ToLower(), "/ss") && (string.Equals(args[i].ToLower(), "asc") || string.Equals(args[i].ToLower(), "desc")))
                {
                    sortSrvsType = Convert.ToString(args[i].ToLower());
                    sortIdx = true;
                }

                if (i > 0 && string.Equals(args[i].ToLower(), "/f"))
                {
                    pathToFile = @"c:\";
                    writeFile = true;
                }

                if (i > 0 && string.Equals(args[i-1].ToLower(), "/f") && args[i].Contains(":\\"))
                {
                    pathToFile = Convert.ToString(args[i].ToLower());
                    writeFile = true;
                }

                if (i > 0 && string.Equals(args[i].ToLower(), "/c"))
                    writeConsole = false;
            }
            Console.Clear();

            GetTDsOutput scaOutput = null;
            try
            {
                ScaClient scaClient = new ScaClient("ConfigurationService_ISca", new EndpointAddress("http://msk-dev-foris:8106/SCA"));
                scaOutput = scaClient.GetTDs(new GetTDsInput() { PANumber = inputedPA });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                System.Environment.Exit(1);
            }

            fileName = @inputedPA + ".txt";
            FileStream fileStream = null;
            StreamWriter streamWriter = null;

            if (writeFile)
            {
                try
                {
                    if (File.Exists(pathToFile + fileName))
                        fileStream = new FileStream(pathToFile + fileName, FileMode.Truncate);
                    else
                        fileStream = new FileStream(pathToFile + fileName, FileMode.CreateNew);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    writeFile = false;
                    if (!writeConsole)
                        System.Environment.Exit(1);
                }

                if (writeFile)
                    streamWriter = new StreamWriter(fileStream);
            }

            PAid searchedPA = new PAid(inputedPA, scaOutput);

            if (lookSrv)
                searchedPA.lookupSrv(lookedSrv);
            if (sortIdx)
                searchedPA.sortTDsList(sortTDsType, sortSrvsType);
            if (writeConsole)
            {
                searchedPA.writePAtoConsole();
                searchedPA.writeTDwithSrvtoConsole(lookedSrv);
            }
            if (writeFile)
            {
                searchedPA.writePAtoFile(ref streamWriter);
                searchedPA.writeTDwithSrvtoFile(lookedSrv, ref streamWriter);
                streamWriter.Close();
                fileStream.Close();
                fileWritePath(pathToFile + fileName);
            }

            Console.Write("\nPress Enter for exit...");
            Console.Read();
        }

        static void exitError()
        {
            Console.WriteLine("\nWrong input, please use 'tworkWithGetDTsClassesFile.exe /?' for help.");
            System.Environment.Exit(1);
        }
        static void fileWritePath(string fp)
        {
            Console.WriteLine($"\nOutput saved as \"{fp}\"");
        }
        static void writeAppHelp()
        {
            Console.WriteLine("\nUsage:");
            Console.WriteLine("\tworkWithGetDTsClassesFile.exe PA [Srv] [/f {Path}] [/c] " +
                            "\n\t\t\t\t\t [/st {asc|desc}] [/ss {asc|desc}]");
            Console.WriteLine("\nInput:");
            Console.WriteLine("\tPA   - Personal Account (must be 12 digits long).");
            Console.WriteLine("\nOptional:");
            Console.WriteLine("\tSrv  - Service code which you looking for" +
                            "\n\t       (must not contain special symbols).");
            Console.WriteLine("\t/st  - {asc|desc} sorting for Terminal Devices" +
                            "\n\t       (default \"asc\").");
            Console.WriteLine("\t/ss  - {asc|desc} sorting for Service codes" +
                            "\n\t       (default \"asc\").");
            Console.WriteLine("\t/c   - decline write ouput to console.");
            Console.WriteLine("\t/f   - {Path} path to file where result will be save" +
                            "\n\t       (by deafault will be used \"c:\\out.txt\").");
            Console.WriteLine("\n\t/?   - for this help.");
            Console.WriteLine("\nExample:");
            Console.WriteLine("\tworkWithGetDTsClassesFile.exe 277300065848 TEST666 /f c:\\ /c /st desc /ss desc");
            System.Environment.Exit(0);
        }
    }
}