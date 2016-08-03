using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.RocketAPI.GUI.Helpers
{
    public static class ErrorReportCreator
    {
        public static void Create(string filename, string message, Exception ex)
        {
            try
            {
                // Creates the Log Directory
                string logFolderName = "Logs";
                DirectoryInfo logDirectory = Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), logFolderName));

                // Creates the Log Name
                string logFile = $"{filename}.{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt";

                // Creates the Log File
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("@@ CUSTOM ERROR MESSAGE @@@@@@@@@@@@@@@@@@@@");
                sb.AppendLine(message);
                sb.AppendLine("@@ EXCEPTION ERROR MESSAGE @@@@@@@@@@@@@@@@@");
                sb.AppendLine(ex.Message);
                sb.AppendLine("@@ STACK TRACE @@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
                sb.AppendLine(ex.ToString());
                sb.AppendLine("@@ INNER EXCEPTIONS @@@@@@@@@@@@@@@@@@@@@@@@@");

                while (ex != null)
                {
                    sb.AppendLine(ex.Message);
                    sb.AppendLine(ex.StackTrace);

                    ex = ex.InnerException;
                    sb.AppendLine("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
                }

                File.WriteAllText(Path.Combine(logDirectory.FullName, logFile), sb.ToString());
            }
            catch (Exception e)
            {
                Logger.Write("Unable to Create Log File.");
                Logger.Write(e.Message);
            }
        }
    }
}
