using PokemonGo.RocketAPI.Logging;
using System;
using System.Windows.Forms;

namespace PokemonGo.RocketAPI.GUI
{
    public class GUILogger : ILogger
    {
        private LogLevel maxLogLevel;
        private TextBox loggingBox;

        public void setLoggingBox(TextBox boxRef)
        {
            this.loggingBox = boxRef;
        }
        
        public GUILogger(LogLevel maxLogLevel)
        {
            this.maxLogLevel = maxLogLevel;
        }
        
        public void Write(string message, LogLevel level = LogLevel.Info, ConsoleColor color = ConsoleColor.White)
        {
            if (level > maxLogLevel)
                return;

            loggingBox.AppendText(Environment.NewLine + $"[{ DateTime.Now.ToString("HH:mm:ss")}] { message}");
        }
    }
}
