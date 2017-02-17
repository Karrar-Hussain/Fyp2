using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using FYP1.controller;
using System.IO;

namespace FYP1
{
    public partial class Form1 : Form
    {
        Thread td1;
        Thread td2,startRealtime;
        Signal training = new Signal();
        ICA ica = new ICA();
        PCA pca = new PCA();
        Signal signal = new Signal();

        public Form1()
        {
            InitializeComponent();
            instance = this;
        }
        private static Form1 instance;

        private Form1(int i=0) { }

        public static Form1 Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Form1(1);
                }
                return instance;
            }
        }
        private void btnTrain_Click(object sender, EventArgs e)
        {
            pnlTrain.Enabled = true;
            pnlTrain.BackColor = Color.White;
            pnlTest.BackColor = Color.Silver;
            pnlTest.Enabled = false;
            lblIDTrain.Text = Properties.Settings.Default.id.ToString();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            pnlTest.Enabled = true;
            pnlTest.BackColor = Color.White;
            pnlTrain.BackColor = Color.Silver;
            pnlTrain.Enabled = false;

            dirPic.Image = Properties.Resources.Picture2;

            int x = 270, y = 255;
            dirPic.Location = new Point(x, y);
            dirPic.Visible = true;
            this.Refresh();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNameTrain.Text))
            {
                MessageBox.Show("Please enter your name!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtNameTrain.Focus();
                return;
            }
            if (cmbTrainType.SelectedIndex == 0)
            {
                MessageBox.Show("Please select any mental command!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cmbTrainType.Focus();
                return;
            }
            
            string name, id, type;
            name = txtNameTrain.Text;
            id = lblIDTrain.Text;
            type=cmbTrainType.SelectedItem.ToString();
            /*if(isDataExist(name,type))
            {
                MessageBox.Show("Data already exist", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            }
            else
            {*/
                td1 = new Thread(() => training.start(id, name, type));
                td1.Start();
                pnlTrain.Enabled = false;
                moveObj();
                pnlTrain.Enabled = true;
                Properties.Settings.Default.Save();

                Properties.Settings.Default.id = Properties.Settings.Default.id += 1;
                

            //}

        }
        public void moveObject(string direction)
        {
            //MessageBox.Show(direction);
            if (direction == "Down")
            {
                int x = dirPic.Location.X, y = dirPic.Location.Y;
                if (y < 510)
                {
                    y +=5;

                    dirPic.Invoke((MethodInvoker)(() => dirPic.Location = new Point(x, y)));
                    this.Invoke((MethodInvoker)(() => this.Refresh()));
                    //System.Threading.Thread.Sleep(30);
                }
                else
                {
                    y = 255;
                    dirPic.Invoke((MethodInvoker)(() => dirPic.Location = new Point(x, y)));
                    this.Invoke((MethodInvoker)(() => this.Refresh()));
                    //System.Threading.Thread.Sleep(30);
                }
            }
            else if (direction == "Up")
            {
                int x = dirPic.Location.X, y = dirPic.Location.Y;
                if (y > 77)
                {
                    y-=5;
                    //dirPic.Location=new Point(x, y);
                    dirPic.Invoke((MethodInvoker)(() => dirPic.Location = new Point(x, y)));
                    this.Invoke((MethodInvoker)(() => this.Refresh()));

                }
                else
                {
                    y = 255;
                    dirPic.Invoke((MethodInvoker)(() => dirPic.Location = new Point(x, y)));
                    this.Invoke((MethodInvoker)(() => this.Refresh()));
                    //System.Threading.Thread.Sleep(30);
                }
            }
            else if (direction == "Left")
            {
                int x = dirPic.Location.X, y = dirPic.Location.Y;
                if (x > 77)
                {
                    x -= 5;
                    //dirPic.Location=new Point(x, y);
                    dirPic.Invoke((MethodInvoker)(() => dirPic.Location = new Point(x, y)));
                    this.Invoke((MethodInvoker)(() => this.Refresh()));
                }
                else
                {
                    x = 255;
                    dirPic.Invoke((MethodInvoker)(() => dirPic.Location = new Point(x, y)));
                    this.Invoke((MethodInvoker)(() => this.Refresh()));
                    //System.Threading.Thread.Sleep(30);
                }
            }
            else if (direction == "Right")
            {
                int x = dirPic.Location.X, y = dirPic.Location.Y;
                if (x <500)
                {
                    x += 5;
                    //dirPic.Location=new Point(x, y);
                    dirPic.Invoke((MethodInvoker)(() => dirPic.Location = new Point(x, y)));
                    this.Invoke((MethodInvoker)(() => this.Refresh()));
                }
                else
                {
                    x = 255;
                    dirPic.Invoke((MethodInvoker)(() => dirPic.Location = new Point(x, y)));
                    this.Invoke((MethodInvoker)(() => this.Refresh()));
                    //System.Threading.Thread.Sleep(30);
                }
            }
            else if (direction == "Neutral")
            {
                int x = dirPic.Location.X, y = dirPic.Location.Y;

                this.Invoke((MethodInvoker)(() => this.Refresh()));

            }
        }
        private void moveObj()
        {
            if (cmbTrainType.SelectedIndex == 1)
            {
                dirPic.Image = Properties.Resources.Actions_go_up_icon;
                
                int x = 540, y = 250;
                Point np = new Point(x, y);
                dirPic.Location = np;
                dirPic.Visible = true;
                while (y > 77)
                {
                    y-=2;
                    np = new Point(x, y);
                    dirPic.Location = np;
                    this.Refresh();
                    System.Threading.Thread.Sleep(70);
                }
            }
            else if (cmbTrainType.SelectedIndex == 2)
            {
                dirPic.Image = Properties.Resources.Actions_go_down_icon;
                int x = 540, y = 77;
                Point np = new Point(x, y);
                dirPic.Location = np;
                dirPic.Visible = true;
                while (y < 300)
                {
                    y+=2;
                    np = new Point(x, y);
                    dirPic.Location = np;
                    this.Refresh();
                    System.Threading.Thread.Sleep(70);
                }
            }
            else if (cmbTrainType.SelectedIndex == 3)
            {
                dirPic.Image = Properties.Resources.Actions_go_previous_icon;
                int x = 510, y = 250;
                Point np = new Point(x, y);
                dirPic.Location = np;
                dirPic.Visible = true;
                while (x > 250)
                {
                    x-=2;
                    np = new Point(x, y);
                    dirPic.Location = np;
                    this.Refresh();
                    System.Threading.Thread.Sleep(15);
                }
            }
            else if (cmbTrainType.SelectedIndex == 4)
            {
                dirPic.Image = Properties.Resources.Actions_go_next_icon;
                int x = 100, y = 250;
                Point np = new Point(x, y);
                dirPic.Location = np;
                dirPic.Visible = true;
                while (x < 350)
                {
                    x+=2;
                    np = new Point(x, y);
                    dirPic.Location = np;
                    this.Refresh();
                    System.Threading.Thread.Sleep(15);
                }
            }
            else if (cmbTrainType.SelectedIndex == 5)
            {
                dirPic.Image = Properties.Resources.Picture2;
                int x = 250, y = 250;
                Point np = new Point(x, y);
                dirPic.Location = np;
                dirPic.Visible = true;
                while (x < 500)
                {
                    x += 2;
                    dirPic.Enabled = false;
                    this.Refresh();
                    System.Threading.Thread.Sleep(15);
                    dirPic.Enabled = true;
                }
                }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This is the final year project\n of team Brain Readers","About Us",MessageBoxButtons.OK,MessageBoxIcon.Information);
           

        }

        private void btnStartTest_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNameTest.Text))
            {
                MessageBox.Show("Please enter your name!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtNameTest.Focus();
                return;
            }
            else
            if (cmbClassifier.SelectedIndex == 0)
            {
                MessageBox.Show("Please select any mental command!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cmbClassifier.Focus();
                return;
            }
            else if (cmbClassifier.SelectedIndex == 1)
            {
                //newThread = new Thread(() => signal.startRealtime("", txtNameTest.Text.ToString(), "KNN"));
                //newThread.Start();
                /*new Thread(signal.startRealtime("", txtNameTest.Text.ToString(), "KNN"))
                    MessageBox.Show("Record Data not found!\n There is no recorded data with provided subject name.");
                txtNameTest.Focus();*/
                startRealtime = new Thread(() => signal.startRealtimeKNN("", txtNameTest.Text.ToString()));
                startRealtime.Start();
            }
            else
            {
                startRealtime = new Thread(() => signal.startRealtimeSVM("", txtNameTest.Text.ToString()));
                startRealtime.Start();
            }
            /*
            Shell32.Shell objShel = new Shell32.Shell();
            // Show the desktop
            ((Shell32.IShellDispatch4)objShel).ToggleDesktop();
            Cursor.Position = new Point(50, 50);
            */
            }

        private void pCAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name=Microsoft.VisualBasic.Interaction.InputBox("Enter name.", "File Name", "");
            //td1=new Thread(() => bol=pca.ExecutePCA1(name));
            //td1.Start();
            if (!pca.ExecutePCA1(name))
            {
                MessageBox.Show("The Record for the provided name does not exist!");
            }
            else
            {
                MessageBox.Show("PCA Done!");

                timer1.Interval = 50;
                progress.Visible = true;
                timer1.Start();
            }
            //use time bar here
        }
        
        private void iCPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = Microsoft.VisualBasic.Interaction.InputBox("Enter name with command.", "File Name", "");
            td2 = new Thread(() => ica.executeICA(name));
            td2.Start();

            //use time bare here
        }

        private void dataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string train = Microsoft.VisualBasic.Interaction.InputBox("Enter Train Filename.", "File Name", "");
            string test = Microsoft.VisualBasic.Interaction.InputBox("Enter Test Filename.", "File Name", "");
            SVMScale svmScale = new SVMScale();
            bool ready = false;
            SVM svm = new SVM();
            ready = svm.buildSVMCorpus(train);
            if (!ready)
            {
                svmScale.buildSVMCorpus(train);
                svmScale.scaleSVMData(train);
            }
            SVMScale svmtest = new SVMScale();
            if(ready)
            {
                svmtest.buildSVMCorpus(test);
                //svmtest.noTestData(test);
                double total=0, tp=0;
                tp+=svm.svmAccuracy(svmtest.downTrainData,"Down");
                tp+=svm.svmAccuracy(svmtest.upTrainData,"Up");
                tp+=svm.svmAccuracy(svmtest.neutralTrainData,"Neutral");
                tp += svm.svmAccuracy(svmtest.leftTrainData,"Left");
                tp += svm.svmAccuracy(svmtest.rightTrainData,"Right");
                if(svmtest.leftTrainData!=null)
                total+=svmtest.leftTrainData.Length;
                if (svmtest.neutralTrainData != null)
                    total += svmtest.neutralTrainData.Length;
                if (svmtest.downTrainData != null)
                    total += svmtest.downTrainData.Length;
                if (svmtest.upTrainData != null)
                    total += svmtest.upTrainData.Length;
                if (svmtest.rightTrainData != null)
                    total += svmtest.rightTrainData.Length;
                MessageBox.Show("Accuracy SVM"+(tp / total * 100) , "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            progress.Minimum = 0;
            progress.Maximum = 160;
            if (progress.Value < progress.Maximum)
            {
                progress.Value = progress.Value + 1;
            }
            if (progress.Value == 159)
            {
                progress.Visible = false;
            }
        }

        private void cmbTrainType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void accurayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = Microsoft.VisualBasic.Interaction.InputBox("Enter name.", "File Name", "");
            
            double accuracy = 0; int fold = 0;
            for (fold = 0; fold < 5; fold++)
            {
                Accuracy acc = new Accuracy();
                if (acc.buildCrossValidationCorpus(name))
                {
                    accuracy += acc.calculateKNNAccuracy();
                    //MessageBox.Show((fold+1)+" Accuracy: " + accuracy);
                }else
                {
                    fold = 6;
                }
            }
            if (fold != 7)
            {
                accuracy /= fold;
                if (accuracy > 0)
                {
                    MessageBox.Show("Current Classifier is K-Nearest Neighbours (KNN) its Accuracy: " + accuracy + "%");
                }
            }
            else
            {
                MessageBox.Show("Record Data not found!\n There is no recorded data with provided subject name.\n You must take training! ");
            }
        }

        private void sVMScaleToolStripMenuItem_Click(object sender, EventArgs e)
        {

            string name = Microsoft.VisualBasic.Interaction.InputBox("Enter name.", "File Name", "");
            SVMScale svmScale = new SVMScale();

            svmScale.buildSVMCorpus(name);
            if (svmScale.fileExistance)
            {
                svmScale.scaleSVMData(name);
                MessageBox.Show("The data is successfully transformed for SVM application.");
            }
            else
                MessageBox.Show("Record Data not found!\n There is no recorded data with provided subject name");

        }

        private void sVMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = Microsoft.VisualBasic.Interaction.InputBox("Enter name.", "File Name", "");
            SVM svm = new SVM();
            //svm.moldingSVM(name);
            svmClassifierPerformance(svm, name);
        }
        private void svmClassifierPerformance(SVM svm,string name)
        {
            double accuracy = 0;
            if (svm.buildSVMCorpus(name))
            {
                accuracy = svm.DoCrossValidationTest();
                if (accuracy > 0)
                {
                    MessageBox.Show("Current Classifier is Support Vector Machine (SVM), its Accuracy: " + accuracy * 100 + "%");
                }
            }
            else
            {
                SVMScale svmScale = new SVMScale();
                if (svmScale.buildSVMCorpus(name))
                {
                    if (svmScale.fileExistance)
                    {
                        svmScale.scaleSVMData(name);
                        svmClassifierPerformance(svm,name);
                        MessageBox.Show("The data is successfully transformed for SVM application.");
                    }
                }
                else
                    MessageBox.Show("Record Data not found!\n There is no recorded data with provided subject name");

            }
        }
        private void cmbClassifer_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private bool isDataExist(string name,string type)
        {
            string data = name + " " + type;//this string store data to file
            string[] fileData = File.ReadAllLines(@"Data.txt");
            foreach (string entry in fileData)
            {
                if (string.Equals(data, entry))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
