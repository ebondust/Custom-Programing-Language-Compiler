using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CompilerR
{
    public partial class Form1 : Form
    {
        public static string code;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           // new Parser(richTextBox1.Text);
            LexicalAnalyzerB lex = new LexicalAnalyzerB();
            lex.Read(new Preprocessor(richTextBox1.Text).getCode());
            richTextBox3.Text = lex.getTypeCode();
            ParserB parser = new ParserB();
            parser.Parse(richTextBox1.Text);
            richTextBox2.Text = code;
            EndPoint endPoint = new EndPoint(parser.pAsmCode);
            richTextBox4.Text = endPoint.getCode();
        }
    }
}
