using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace FileManager
{
    public partial class Form1 : Form
    {
        List<string> FileList = new List<string>();

        Thread hThread = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Все файлы|*.*";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach(string filename in ofd.FileNames)
                {
                    if(!FileList.Contains(filename))
                        FileList.Add(filename);
                }
            }
            label2.Text = String.Format("Всего файлов: {0}", FileList.Count);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (FileList.Count == 0)
            {
                MessageBox.Show("Файлы не выбраны", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (String.IsNullOrEmpty(textBox1.Text))
            {
                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
                textBox1.Enabled = false;
                listView1.Items.Clear();
                hThread = new Thread(Compare);
                hThread.Start();
            }
            else
            {
                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
                textBox1.Enabled = false;
                listView1.Items.Clear();
                hThread = new Thread(Find);
                hThread.Start();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(hThread != null)
                if (hThread.IsAlive)
                    hThread.Abort();
        }

        private void Find()
        {
            try
            {
                pictureBox1.Invoke(new ThreadStart(delegate
                {
                    pictureBox1.Visible = true;
                }));

                foreach(string FileName in FileList)
                {
                    int cnt = 0;
                    StreamReader sr = new StreamReader(FileName, Encoding.Default);
                    string line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        cnt++;
                        if (line.IndexOf(textBox1.Text,StringComparison.InvariantCultureIgnoreCase) >= 0)
                        {
                            if (this.InvokeRequired)
                                listView1.Invoke(new ThreadStart(delegate
                                {
                                    listView1.Items.Add(new ListViewItem(new string[] { line, FileName, cnt.ToString() }));
                                }));
                            else
                                listView1.Items.Add(new ListViewItem(new string[] { line, FileName, cnt.ToString() }));
                        }
                    }
                    sr.Close();
                }
                pictureBox1.Invoke(new ThreadStart(delegate
                {
                    pictureBox1.Visible = false;
                }));
                textBox1.Invoke(new ThreadStart(delegate
                {
                    textBox1.Enabled = true;
                }));
                button1.Invoke(new ThreadStart(delegate
                {
                    button1.Enabled = true;
                }));
                button2.Invoke(new ThreadStart(delegate
                {
                    button2.Enabled = true;
                }));
                button3.Invoke(new ThreadStart(delegate
                {
                    button3.Enabled = true;
                }));
                MessageBox.Show("Поиск завершён!");
            }
            catch(ThreadAbortException)
            {

            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Ошибка");
            }
        }

        private void Compare()
        {
            try
            {
                List<string> ReadFiles = new List<string>();
                List<string> ReadAll = new List<string>();

                pictureBox1.Invoke(new ThreadStart(delegate
                {
                    pictureBox1.Visible = true;
                }));

                foreach (string FileName in FileList)
                {
                    List<int> ReadStrings = new List<int>();

                    if(!ReadFiles.Contains(FileName))
                    {
                        int cnt = 0;
                        StreamReader sr = new StreamReader(FileName, Encoding.Default);
                        string line;

                        while ((line = sr.ReadLine()) != null)
                        {
                            cnt++;

                            bool ToRead = false;

                            string ReplaceLine = Regex.Replace(line, @"[ _,.:;-]+", "");

                            if (!ReadAll.Contains(ReplaceLine))
                            {
                                if (!String.IsNullOrEmpty(ReplaceLine))
                                {
                                    foreach (string _File in FileList)
                                    {
                                        List<int> _ReadStrings = new List<int>();

                                        if (!ReadFiles.Contains(_File))
                                        {
                                            if (_File != FileName)
                                            {
                                                int _cnt = 0;
                                                StreamReader _sr = new StreamReader(_File, Encoding.Default);
                                                string _line;
                                                while ((_line = _sr.ReadLine()) != null)
                                                {
                                                    _cnt++;

                                                    if (!String.IsNullOrEmpty(Regex.Replace(_line, @"[ _,.:;-]+", "")))
                                                    {
                                                        if (String.Equals(Regex.Replace(_line, @"[ _,.:;-]+", ""), ReplaceLine, StringComparison.InvariantCultureIgnoreCase))
                                                        {
                                                            ToRead = true;
                                                            if (this.InvokeRequired)
                                                            {
                                                                if (!ReadStrings.Contains(cnt))
                                                                {
                                                                    listView1.Invoke(new ThreadStart(delegate
                                                                    {
                                                                        listView1.Items.Add(new ListViewItem(new string[] { line, FileName, cnt.ToString() }));
                                                                    }));
                                                                    ReadStrings.Add(cnt);
                                                                }
                                                                if (!_ReadStrings.Contains(_cnt))
                                                                {
                                                                    listView1.Invoke(new ThreadStart(delegate
                                                                    {
                                                                        listView1.Items.Add(new ListViewItem(new string[] { _line, _File, _cnt.ToString() }));
                                                                    }));
                                                                    _ReadStrings.Add(_cnt);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (!ReadStrings.Contains(cnt) && !_ReadStrings.Contains(_cnt))
                                                                {
                                                                    listView1.Invoke(new ThreadStart(delegate
                                                                    {
                                                                        listView1.Items.Add(new ListViewItem(new string[] { line, FileName, cnt.ToString() }));
                                                                    }));
                                                                    ReadStrings.Add(cnt);
                                                                    _ReadStrings.Add(_cnt);
                                                                }
                                                                listView1.Invoke(new ThreadStart(delegate
                                                                {
                                                                    listView1.Items.Add(new ListViewItem(new string[] { _line, _File, _cnt.ToString() }));
                                                                }));
                                                            }
                                                        }
                                                    }
                                                }
                                                _sr.Close();
                                            }
                                        }
                                    }
                                }
                                if(ToRead)
                                    ReadAll.Add(Regex.Replace(line, @"\s+", ""));
                            }
                        }
                        sr.Close();
                    }
                    if (!ReadFiles.Contains(FileName))
                        ReadFiles.Add(FileName);
                }
                pictureBox1.Invoke(new ThreadStart(delegate
                {
                    pictureBox1.Visible = false;
                }));
                textBox1.Invoke(new ThreadStart(delegate
                {
                    textBox1.Enabled = true;
                }));
                button1.Invoke(new ThreadStart(delegate
                {
                    button1.Enabled = true;
                }));
                button2.Invoke(new ThreadStart(delegate
                {
                    button2.Enabled = true;
                }));
                button3.Invoke(new ThreadStart(delegate
                {
                    button3.Enabled = true;
                }));
                MessageBox.Show("Поиск завершён!");
            }
            catch (ThreadAbortException)
            {

            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Ошибка");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FileList.Clear();
            label2.Text = "Всего файлов: 0";
        }
    }
}
