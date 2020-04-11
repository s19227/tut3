using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace tut3.Logging
{
    class Logger
    {
        private readonly string m_destinationPath;

        public Logger(string destinationPath)
        {
            m_destinationPath = destinationPath;

            if (File.Exists(m_destinationPath))
                File.Delete(m_destinationPath);
        }

        public void PrintMessage(string message)
        {
            using (var writer = new StreamWriter(m_destinationPath, true))
                writer.WriteLine(message);
        }
    }
}
