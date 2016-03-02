using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;

namespace CompilerR
{
    public partial class Gui : Form
    {
        int tSize = 0;
        public Regex keyWords = new Regex("num|void|string|bool|");
        public Regex keyWords2 = new Regex("^[A-z]");

        public string directory = "";

        public Gui()
        {
            InitializeComponent();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            int buffor = tSize;
            tSize = richTextBox1.Text.Length;


            if (richTextBox1.Text[richTextBox1.Text.Length - 1] == ' ' || richTextBox1.Text[richTextBox1.Text.Length - 1] == '\n' || tSize - buffor != 1)
            {
                int selPos = richTextBox1.SelectionStart;
                RichTextBox txt = new RichTextBox();
                txt.Text = richTextBox1.Text;

                //For each match from the regex, highlight the word.



                foreach (Match keyWordMatch in keyWords.Matches(txt.Text))
                {

                    txt.Select(keyWordMatch.Index, keyWordMatch.Length);
                    txt.SelectionColor = Color.Blue;
                    richTextBox1.SelectionStart = selPos;
                    richTextBox1.SelectionColor = Color.Black;
                }
                richTextBox1.Rtf = txt.Rtf;
                richTextBox1.SelectionStart = selPos;
            }
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LexicalAnalyzerB lex = new LexicalAnalyzerB();
            lex.Read(new Preprocessor(richTextBox1.Text).getCode());
            ParserB parser = new ParserB();
            parser.Parse(new Preprocessor(richTextBox1.Text).getCode());
            richTextBox2.Text = "";
            foreach (String error in parser.errorList)
            {
                richTextBox2.Text += error;
            }
            if (parser.errorList.Count < 1)
            {
                EndPoint endPoint = new EndPoint(parser.pAsmCode);
                System.IO.StreamWriter file = new System.IO.StreamWriter("test.asm");
                file.Write(endPoint.getCode());
                file.Close();
                setFasm();
                var process = Process.Start("fasm.exe", "\"" + directory + "\\test.asm\" " + "\"" + directory + "\\test.exe\"");
                process.WaitForExit();
                Process.Start("test.exe");
            }

        }


        public void setFasm()
        {
            directory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string path = directory + "\\INCLUDE";

            System.Environment.SetEnvironmentVariable("INCLUDE", path);
        }

        private void compileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LexicalAnalyzerB lex = new LexicalAnalyzerB();
            lex.Read(new Preprocessor(richTextBox1.Text).getCode());
            ParserB parser = new ParserB();
            parser.Parse(new Preprocessor(richTextBox1.Text).getCode());
            richTextBox2.Text = "";
            foreach (String error in parser.errorList)
            {
                richTextBox2.Text += error;
            }
            if (parser.errorList.Count < 1)
            {
                EndPoint endPoint = new EndPoint(parser.pAsmCode);
                System.IO.StreamWriter file = new System.IO.StreamWriter("test.asm");
                file.Write(endPoint.getCode());
                file.Close();
                setFasm();
                var process = Process.Start("fasm.exe", "\"" + directory + "\\test.asm\" " + "\"" + directory + "\\test.exe\"");
                process.WaitForExit();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            listBox1.Items.Clear();
            string file = openFileDialog1.FileName;
            try
            {
                string text = File.ReadAllText(file);
                richTextBox1.Text = text;
                listBox1.Items.Add(openFileDialog1.SafeFileName);
            }
            catch (IOException)
            {
            }

        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            string name = saveFileDialog1.FileName;
            File.WriteAllText(name,richTextBox1.Text);
            string[] split = name.Split('\\');
            listBox1.Items.Clear();
            listBox1.Items.Add(split[split.Length-1]);
        }



    }
}
