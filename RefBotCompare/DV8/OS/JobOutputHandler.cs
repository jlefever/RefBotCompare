using System;
using System.Diagnostics;
using System.IO;

namespace RefBotCompare.DV8.OS
{
    public class JobOutputHandler
    {
        private readonly TextWriter _writer;

        public JobOutputHandler(TextWriter writer)
        {
            _writer = writer;
        }

        public void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            _writer.WriteLine(e.Data);
        }

        public void ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            _writer.WriteLine(e.Data);
        }

        public void Exited(object sender, EventArgs e)
        {
            // TODO
        }
    }
}
