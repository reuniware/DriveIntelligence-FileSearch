using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DriveIntelligence
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives)
            {
                string path = d.Name;
                //TreeScan("C:\\");
                checkedListBox1.Items.Add(path, CheckState.Checked);
            }

            checkBox1.Checked = false;

            log("Current username : " + Environment.UserName);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            nbfiles = 0;
            fileList.Clear();

            //TreeScan("F:\\AUDIO");

            long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
            {
                string path = checkedListBox1.CheckedItems[i].ToString();
                log("Scanning path = " + path);
                TreeScan(path);
            }
            long milliseconds2 = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            log("Scanning Done : " + nbfiles + " files in " + (milliseconds2-milliseconds) + " ms.");

            // définir répertoire de sortie de effacer tout dans le répertoire de sortie
            String outputFolder = "D:\\tmp\\";
            System.IO.DirectoryInfo di = new DirectoryInfo(outputFolder);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            // créer un rapport et y mettre tous les fichiers scannés (liste globale selon les drives qui sont cochés)
            DateTime dt = DateTime.Now;
            log(dt.ToString());
            string strDate = dt.Year.ToString() + dt.Month.ToString("00") + dt.Day.ToString("00");
            strDate += dt.Hour.ToString("00") + dt.Minute.ToString("00") + dt.Second.ToString("00");
            string strOutputFilePath = strDate + "_di_output.txt";
            log("Output report file : " + outputFolder + " \\" + strOutputFilePath);
            FileStream fs = File.Create(outputFolder + "\\" + strOutputFilePath);
            StreamWriter sw = new StreamWriter(fs);
            foreach (string s in fileList)
            {
                sw.WriteLine(s);
            }
            sw.Close();

            // rechercher des fichiers selon critères
            listBox1.Visible = false;
            int resultCount = 0;
            for (int i = 0; i < fileList.Count(); i++)
            {
                string f = fileList[i];
               if (
                        (textBox1.Text != "" && f.ToLower().Contains(textBox1.Text) && (checkBox2.CheckState == CheckState.Checked))
                    )
                {
                    resultCount++;

                    FileInfo fi = null;
                    try
                    {
                        fi = new System.IO.FileInfo(f);
                        long length = fi.Length;
                        string fname = fi.Name;
                        fi = null;

                        double lenMb = ((double)length) / 1024 / 1024;
                        log(f + " " + lenMb.ToString("N2") + " Mb (" + length + " b)");

                        /*FileStream fs = File.Open(f, FileMode.Open);
                        double lenMb = ((double) length) / 1024 / 1024;
                        log(f + " " + lenMb.ToString("N2") + " Mb (" + length + " b)");
                        Byte[] byteArray = new Byte[length];
                        fs.Read(byteArray, 0, (int) length);
                        fs.Close();

                        FileStream outfile = File.Create(outputFolder + fname, (int) length);
                        outfile.Write(byteArray, 0, (int) length);
                        outfile.Close();*/

                        //File.Delete(f);

                        /*Uri uri = new Uri(@"https://investdata.000webhostapp.com/fsmonitor/?uploadfile");
                        WebRequest h = WebRequest.Create(uri);
                        h.Timeout = 1000;
                        WebResponse r = h.GetResponse();

                        Stream s = r.GetResponseStream();
                        Byte[] bArray = new Byte[s.Length];
                        s.Read(bArray, 0, (int) s.Length);

                        s.Close();
                        r.Close();*/

                        //MessageBox.Show("001");


                    }
                    catch (Exception)
                    {
                        //log(ex.Message);
                    }

                    #region Copier fichiers sources vers répertoire spécifique
                    //log(f);
                    /*try
                    {
                        long len = 0;
                        FileStream fs = File.Open(f, FileMode.Open);
                        long length = new System.IO.FileInfo(f).Length;
                        string fname = new System.IO.FileInfo(f).Name;

                        Byte[] byteArray = new Byte[length];
                        long indexByteArray = 0;

                        while (true)
                        {
                            int ii = fs.ReadByte();
                            if (ii == -1)
                            {
                                log(f + " : End of file ; len = " + len + " " + length);
                                break;
                            } else
                            {
                                len++;
                                byteArray[indexByteArray] = (Byte)ii;
                                indexByteArray++;
                            }
                        }
                        fs.Close();

                        Boolean disableCopy = true;
                        String outputFolder = "D:\\tmp\\";

                        try
                        {
                            if (!disableCopy)
                            {
                                // Copie du fichier vers D:\TMP
                                FileStream ff = new FileStream(outputFolder + fname, FileMode.CreateNew);
                                for (long jj = 0; jj < length; jj++)
                                {
                                    ff.WriteByte(byteArray[jj]);
                                }
                                ff.Close();
                            }
                        } catch(Exception ex)
                        {
                            log(fname + " : Duplication exception : " + ex.Message);
                        }

                    }
                    catch (UnauthorizedAccessException)
                    {
                        log(f + " : UnauthorizedAccessException");
                    }
                    catch (IOException)
                    {
                        log(f + " : IOException");
                    }*/
                    #endregion
                }

            }

            log(resultCount + " files found.");

            listBox1.Visible = true;

        }

        private void log(string str)
        {
            listBox1.Items.Add(str);
        }

        int nbfiles = 0;
        List<String> fileList = new List<string>();

        private void TreeScan(string sDir)
        {
            bool authorized = true;
            try
            {
                Directory.GetFiles(sDir);
            }
            catch (UnauthorizedAccessException)
            {
                authorized = false;
                //log("Unauthorized : " + sDir);
            }

            if (authorized == false)
            {
                //log(sDir + " : Unauthorized");
                return;
            }

            string[] files = Directory.GetFiles(sDir);
            if (files.Count() == 0)
            { 
                /*if (new DirectoryInfo(sDir).GetDirectories().Count() == 0)
                {
                    log("no files and no folders in folder : " + sDir);
                    try
                    {
                        Directory.Delete(sDir);
                        log("deleted " + sDir);
                    }
                    catch (Exception)
                    {

                    }
                }*/
                return;
            }

            foreach (string f in Directory.GetFiles(sDir))
            {
                nbfiles++;
                fileList.Add(f);
            }

            foreach (string d in Directory.GetDirectories(sDir))
                TreeScan(d); // recursive call to get files of directory
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (openFolderOnClick)
            {
                try
                {
                    string path = listBox1.SelectedItem.ToString();
                    FileInfo fi = new System.IO.FileInfo(path);
                    Process proc = new Process();
                    proc.StartInfo.FileName = "explorer.exe";
                    proc.StartInfo.Arguments = fi.DirectoryName;
                    proc.Start();
                }
                catch (Exception)
                {

                }
            }
        }

        Boolean openFolderOnClick = false;
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.CheckState == CheckState.Checked)
            {
                openFolderOnClick = true;
            } else
            {
                openFolderOnClick = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
