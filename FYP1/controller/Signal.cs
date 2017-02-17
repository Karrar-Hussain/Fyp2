using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Emotiv;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

namespace FYP1.controller
{
    class Signal
    {
        int userID = -1;
        string filename;// = "AverageBandPowers.csv";
        PCA pca ;
        KNN knn ;
        SVM svm ;
        SVMScale svmscale ;
        public Signal()
        {
            pca = new PCA();
            knn = new KNN();
            svm = new SVM();
            svmscale = new SVMScale();
        }
        void engine_UserAdded_Event(object sender, EmoEngineEventArgs e)
        {
            Console.WriteLine("User Added Event has occured");
            userID = (int)e.userId;
            EmoEngine.Instance.IEE_FFTSetWindowingType((uint)userID, EdkDll.IEE_WindowingTypes.IEE_HAMMING);
        }
        public bool startRealtimeKNN(string id, string name)
        {
            EmoEngine engine = EmoEngine.Instance;
            engine.UserAdded += new EmoEngine.UserAddedEventHandler(engine_UserAdded_Event);
            engine.Connect();
            EdkDll.IEE_DataChannel_t[] channelList = new EdkDll.IEE_DataChannel_t[5] { EdkDll.IEE_DataChannel_t.IED_AF3, EdkDll.IEE_DataChannel_t.IED_AF4, EdkDll.IEE_DataChannel_t.IED_T7,
                                               EdkDll.IEE_DataChannel_t.IED_T8, EdkDll.IEE_DataChannel_t.IED_O1 };
            bool ready = false;
             ready = knn.buildCorpusSimple(name);
           
            IntPtr state=new IntPtr(1);
            Thread newThread;
            int itr = 0;
            double[] alpha = new double[1];
            double[] low_beta = new double[1];
            double[] high_beta = new double[1];
            double[] gamma = new double[1];
            double[] theta = new double[1];
            double[] data = new double[25];
            while (ready)
            {
                itr++;
                //if (EdkDll.IS_FacialExpressionIsLeftWink(state))
                  //  MessageBox.Show("Left Eye Wink Detected!");

                //if (EdkDll.IS_FacialExpressionIsRightWink(state))
                   // MessageBox.Show("Right Eye Wink Detected!");
                engine.ProcessEvents(10);
                
                if (userID < 0)  continue;                
                for (int i = 0,j=0; i < 5; i++)
                {
                    engine.IEE_GetAverageBandPowers((uint)userID, channelList[i], theta, alpha, low_beta, high_beta, gamma);
                    data[j++] = theta[0];
                    data[j++] = alpha[0];
                    data[j++] = low_beta[0];
                    data[j++] = high_beta[0];
                    data[j++] = gamma[0];
                }
                //up simple data taken form karrarUp.csv
                //
                //data = new double[] { 4.651620563, 5.212359266, 10.90409395, 5.157308481, 4.170539408, 5.10950104, 6.035994062, 12.41738389, 4.743290343, 3.866878481, 2.873242245, 4.77619567, 2.151664297, 1.760807141, 3.082304127, 3.986381183, 10.31494602, 9.029601134, 2.233667016, 2.991145428, 0.244647851, 0.618714049, 0.239897275, 0.36874583, 0.333496459 };
                //data = new double[] { 2.254629381, 1.60659665, 1.328094785, 1.882520841, 1.027082428, 2.57410163, 2.209505082, 1.17652227, 0.9200791, 0.524185866, 2.047865372, 1.76506681, 1.091739745, 0.690766561, 0.820688245, 1.643871005, 5.476966707, 1.084435047, 1.055108772, 0.556177084, 335.9026309, 186.3244237, 140.337729, 67.65089061, 25.64029729 };

                if (data[0] != 0 && data[24] != 0)
                {
                        newThread = new Thread(() => Form1.Instance.moveObject(knn.knnClassifier(data)));
                        newThread.Start();
                      //newThread = new Thread(() => Form1.Instance.moveObject(knn.knnClassifier(pca.testDataPCA(data))));
                    }
                    Thread.Sleep(2000);
            }
            return ready;
        }
        public bool startRealtimeSVM(string id, string name)
        {
            EmoEngine engine = EmoEngine.Instance;
            engine.UserAdded += new EmoEngine.UserAddedEventHandler(engine_UserAdded_Event);
            engine.Connect();
            EdkDll.IEE_DataChannel_t[] channelList = new EdkDll.IEE_DataChannel_t[5] { EdkDll.IEE_DataChannel_t.IED_AF3, EdkDll.IEE_DataChannel_t.IED_AF4, EdkDll.IEE_DataChannel_t.IED_T7,
                                               EdkDll.IEE_DataChannel_t.IED_T8, EdkDll.IEE_DataChannel_t.IED_O1 };
            bool ready = false;
            
                ready = svm.buildSVMCorpus(name);
                if (!ready)
                {
                    svmscale.buildSVMCorpus(name);
                    svmscale.scaleSVMData(name);
                    startRealtimeSVM(id, name);
                }
            IntPtr state = new IntPtr(1);
            Thread newThread;
            int itr = 0;
            double[] alpha = new double[1];
            double[] low_beta = new double[1];
            double[] high_beta = new double[1];
            double[] gamma = new double[1];
            double[] theta = new double[1];
            double[] data = new double[25];
            while (ready)
            {
                itr++;
                //if (EdkDll.IS_FacialExpressionIsLeftWink(state))
                //  MessageBox.Show("Left Eye Wink Detected!");

                //if (EdkDll.IS_FacialExpressionIsRightWink(state))
                // MessageBox.Show("Right Eye Wink Detected!");
                engine.ProcessEvents(10);
                
                if (userID < 0)  continue;                
                for (int i = 0,j=0; i < 5; i++)
                {
                    engine.IEE_GetAverageBandPowers((uint)userID, channelList[i], theta, alpha, low_beta, high_beta, gamma);
                    data[j++] = theta[0];
                    data[j++] = alpha[0];
                    data[j++] = low_beta[0];
                    data[j++] = high_beta[0];
                    data[j++] = gamma[0];
                }
                //up simple data taken form karrarUp.csv
                //
                
                //data = new double[] { 4.651620563, 5.212359266, 10.90409395, 5.157308481, 4.170539408, 5.10950104, 6.035994062, 12.41738389, 4.743290343, 3.866878481, 2.873242245, 4.77619567, 2.151664297, 1.760807141, 3.082304127, 3.986381183, 10.31494602, 9.029601134, 2.233667016, 2.991145428, 0.244647851, 0.618714049, 0.239897275, 0.36874583, 0.333496459 };
                //data = new double[] { 3.465019915, 1.931854818, 3.448097045, 1.47996222, 1.137614759, 5.230779949, 2.01443672, 2.395593998, 1.310546988, 0.988700891, 6.74993337, 5.037963203, 1.074775069, 0.615915165, 0.920866746, 6.755586104, 5.014666624, 2.568192279, 1.08015653, 0.931508695, 500, 500, 500, 269.375212, 85.55572135 };
                //data=new double [] { 2.254629381, 1.60659665,  1.328094785, 1.882520841, 1.027082428, 2.57410163,  2.209505082, 1.17652227,  0.9200791,   0.524185866, 2.047865372, 1.76506681,  1.091739745, 0.690766561, 0.820688245, 1.643871005, 5.476966707, 1.084435047, 1.055108772, 0.556177084, 335.9026309, 186.3244237, 140.337729,  67.65089061, 25.64029729 };

                if (data[0] != 0 && data[24] != 0)
                {
                        newThread = new Thread(() => Form1.Instance.moveObject(svm.svmRealTimeTest(data)));
                        newThread.Start();
                    //newThread = new Thread(() => Form1.Instance.moveObject(knn.knnClassifier(pca.testDataPCA(data))));
                }
                Thread.Sleep(2000);
                //ready = false;
            }
            return ready;
        }
        public void start(string id,string name,string type)
        {
            /*using (TextWriter tsw = new StreamWriter(@"Data.txt", true))
            {
                tsw.WriteLine(name + " " + type);                
            }*/
                //Console.WriteLine(TimeSpan.Compare(t1, System.DateTime.Now.TimeOfDay));
                //int a = 0;
                string header = "Theta, Alpha, Low_beta, High_beta, Gamma, Channel";        
            name = name+type + ".csv";
            filename = name;
            if (!File.Exists(filename))
            {
                using (StreamWriter file = File.CreateText(filename))
                {
                    file.WriteLine(header);
                    writeDataFile(file);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filename))
                {
                    writeDataFile(sw);
                }
            }
            //file = new StreamWriter(filename, false);
            //file.WriteLine(header);
            //AverageBandPowers p = new AverageBandPowers();                        
        }
        public void writeDataFile(StreamWriter file)
        {
            EmoEngine engine = EmoEngine.Instance;
            engine.UserAdded += new EmoEngine.UserAddedEventHandler(engine_UserAdded_Event);
            engine.Connect();
            EdkDll.IEE_DataChannel_t[] channelList = new EdkDll.IEE_DataChannel_t[5] { EdkDll.IEE_DataChannel_t.IED_AF3, EdkDll.IEE_DataChannel_t.IED_AF4, EdkDll.IEE_DataChannel_t.IED_T7, EdkDll.IEE_DataChannel_t.IED_T8, EdkDll.IEE_DataChannel_t.IED_O1 };
            var t1 = System.DateTime.Now.TimeOfDay.Add(new TimeSpan(300000000));
            int itr = 0;
            while (TimeSpan.Compare(t1, System.DateTime.Now.TimeOfDay) == 1)
            {
                engine.ProcessEvents(10);
                if (userID < 0) continue;
                double[] alpha = new double[1];
                double[] low_beta = new double[1];
                double[] high_beta = new double[1];
                double[] gamma = new double[1];
                double[] theta = new double[1];
                for (int i = 0; i < 5; i++)
                {
                    engine.IEE_GetAverageBandPowers((uint)userID, channelList[i], theta, alpha, low_beta, high_beta, gamma);
                    if (itr > 3)
                    {
                        file.Write(theta[0] + ",");
                        file.Write(alpha[0] + ",");
                        file.Write(low_beta[0] + ",");
                        file.Write(high_beta[0] + ",");
                        file.Write(gamma[0] + ",");
                    }
                    //      Console.WriteLine("Theta: " + theta[0]);
                }
                if (itr > 3)
                {
                    if (filename.Contains("Up"))
                        file.WriteLine("Up");
                    else if (filename.Contains("Down"))
                        file.WriteLine("Down");
                    else if (filename.Contains("Left"))
                        file.WriteLine("Left");
                    else if (filename.Contains("Right"))
                        file.WriteLine("Right");
                    else if (filename.Contains("Neutral"))
                        file.WriteLine("Neutral");
                }
                itr++;
                Thread.Sleep(500);
            }
            //Console.WriteLine("done");
            file.Close();
            engine.Disconnect();
        }
    }
}
