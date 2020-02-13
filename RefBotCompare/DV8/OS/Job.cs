using System;
using System.Diagnostics;

namespace RefBotCompare.DV8.OS
{
    public class Job : IDisposable
    {
        public bool Completed { get; private set; } = false;

        private readonly Process _process;

        public Job(string exec, string args, string dir, JobOutputHandler handler)
        {
            _process = new Process();
            _process.StartInfo.FileName = exec;
            _process.StartInfo.Arguments = args;
            _process.StartInfo.WorkingDirectory = dir;
            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.RedirectStandardOutput = true;
            _process.StartInfo.RedirectStandardError = true;
            _process.EnableRaisingEvents = true;
            _process.OutputDataReceived += handler.OutputDataReceived;
            _process.ErrorDataReceived += handler.ErrorDataReceived;
            _process.Exited += Exited;
            _process.Exited += handler.Exited;
        }

        private void Exited(object sender, EventArgs e)
        {
            Completed = true;
        }

        public void Exec()
        {
            _process.Start();
            _process.BeginErrorReadLine();
            _process.BeginOutputReadLine();
            // _process.WaitForExit();
        }

        public void Dispose()
        {
            _process.Dispose();
        }
    }
}
