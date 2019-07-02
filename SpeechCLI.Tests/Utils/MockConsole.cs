using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit.Abstractions;

namespace SpeechCLI.Tests.Utils
{
    public class MockConsole : IConsole
    {
        public MockConsole(TextWriter outputWriter)
        {
            Out = outputWriter;
            Error = outputWriter;
        }

        public TextWriter Out { get; set; }

        public TextWriter Error { get; set; }

        public TextReader In => throw new NotImplementedException();

        public bool IsInputRedirected => throw new NotImplementedException();

        public bool IsOutputRedirected => true;

        public bool IsErrorRedirected => true;

        public ConsoleColor ForegroundColor { get; set; }
        public ConsoleColor BackgroundColor { get; set; }

        public event ConsoleCancelEventHandler CancelKeyPress
        {
            add { }
            remove { }
        }

        public void ResetColor()
        {
        }

        
    }

    public class MockTestWriter : TextWriter
    {
        private StringBuilder _sb = new StringBuilder();

        public override Encoding Encoding => Encoding.Unicode;

        public override void Write(char ch)
        {
            //if (ch == '\n')
            //{
            //    //_output.WriteLine(_sb.ToString());
            //    _sb.Clear();
            //}
            //else
            //{
            //    _sb.Append(ch);
            //}

            _sb.Append(ch);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_sb.Length > 0)
                {
                    //_output.WriteLine(_sb.ToString());
                    _sb.Clear();
                }
            }

            base.Dispose(disposing);
        }

        public string ReadAsString()
        {
            return _sb.ToString();
        }
    }
}
