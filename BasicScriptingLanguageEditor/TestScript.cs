﻿using FastColoredTextBoxNS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BasicScriptingLanguageEditor
{
    public partial class TestScript : Form
    {
        Process InterProc = new Process();
        string _fileToPass;
        string DebugGUID = "df58b608-1bc8-4a4a-bd1f-b4f5205658ab";

        public TestScript()
        {
            Font = SystemFonts.MessageBoxFont;
            InitializeComponent();
        }

        public TestScript(string file)
        {
            Font = SystemFonts.MessageBoxFont;
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            _fileToPass = file;
        }

        private void InitializeInterpreter()
        {
            InterProc.StartInfo.UseShellExecute = false;
            InterProc.StartInfo.FileName = "debug.exe";
            InterProc.StartInfo.Arguments = "\"" + _fileToPass + "\"";
            InterProc.StartInfo.RedirectStandardInput = true;
            InterProc.StartInfo.RedirectStandardOutput = true;
            InterProc.StartInfo.RedirectStandardError = true;
            InterProc.StartInfo.CreateNoWindow = true;
            InterProc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            InterProc.OutputDataReceived += new DataReceivedEventHandler(InterProcOutputHandler);

            bool started = InterProc.Start();

            InterProc.BeginOutputReadLine();

        }

        private void AppendTextInBox(FastColoredTextBoxNS.FastColoredTextBox box, string text)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke((Action<FastColoredTextBoxNS.FastColoredTextBox, string>)AppendTextInBox, fastColoredTextBox1, text);
                }
                catch
                { /*Script was prematurely murdered*/ }
            }
            else
            {
                box.Text += text;
            }
        }

        private void InterProcOutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            AppendTextInBox(fastColoredTextBox1, outLine.Data + Environment.NewLine);
        }

        private void Enterbutton_Click(object sender, EventArgs e)
        {
            InterProc.StandardInput.WriteLine(inputTextBox.Text);
            inputTextBox.Text = "";
        }

        private void TestScript_Load(object sender, EventArgs e)
        {
            Thread t = new Thread(InitializeInterpreter);
            t.Start();

            inputTextBox.Select();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            InterProc.StandardInput.WriteLine();
        }

        Style EndOfScriptStyle = new TextStyle(Brushes.Red, Brushes.Transparent, FontStyle.Regular);
        Style ErrorStyle = new TextStyle(Brushes.White, Brushes.DarkRed, FontStyle.Bold);
        private void fastColoredTextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            e.ChangedRange.ClearStyle(EndOfScriptStyle);
            e.ChangedRange.SetStyle(EndOfScriptStyle, "(END OF SCRIPT)", System.Text.RegularExpressions.RegexOptions.Multiline);

            e.ChangedRange.ClearStyle(ErrorStyle);
            e.ChangedRange.SetStyle(ErrorStyle, "(ERROR EXECUTING SCRIPT)", System.Text.RegularExpressions.RegexOptions.Multiline);
        }

        private void inputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyValue == (int)Keys.Enter)
            {
                InterProc.StandardInput.WriteLine(inputTextBox.Text);
                inputTextBox.Text = "";
            }
        }

        private void TestScript_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                InterProc.Kill();
            }
            catch
            { /*debug actually killed itself like it's supposed to*/ }
        }

    }
}
