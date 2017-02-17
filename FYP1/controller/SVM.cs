using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using libsvm;
using System.Windows.Forms;

namespace FYP1.controller
{
    class SVM
    {
        static double C = 35;
        static double gamma = 0.000030518125;
        Dictionary<int, string> predictionDictionary;
        svm_problem _prob;
        C_SVC svm;
        SVMScale scale;
        bool fileExistance;
        svm_node[] svmnode;
        public SVM()
        {
            fileExistance = false;
            predictionDictionary = new Dictionary<int, string> { { 1, "Neutral" }, { 2, "Up" }, { 3, "Down" }, { 4, "Left" }, { 5, "Right" } };
            scale = new SVMScale();
            svmnode = new svm_node[25];
            int i = 0;
            for(;i<25;i++)
            {
                svmnode[i] = new svm_node();
                svmnode[i].index = i + 1;
            }
        }
        public bool buildSVMCorpus(string filename)
        {
            string trainDataPath = filename+"SimpleScaledTrainSVM.txt";
            if (File.Exists(trainDataPath))
            {
                _prob = ProblemHelper.ReadAndScaleProblem(trainDataPath);
                svm = new C_SVC(_prob, KernelHelper.LinearKernel(), C);
                fileExistance = true;

                var reader = new StreamReader(File.OpenRead(filename + "MinMax.txt"));
                string[] minMax = reader.ReadLine().Split(',');
                scale.min = Convert.ToDouble(minMax[0]);
                scale.max = Convert.ToDouble(minMax[1]);
            }

                return fileExistance;
        }
        public double[] scaleData(double[] testData)
        {
            double[][] test = new double[1][];
            test[0] = new double[testData.Length];
            test[0] = testData;
            scale.minDouble(test);
            scale.scaleData(test);
            scale.maxDouble(test);
            scale.scale2Data(test);
            return test[0];
        }
        public double DoCrossValidationTest()
        {
            var cva = svm.GetCrossValidationAccuracy(5);
            return cva;
        }
        public double moldingSVM(string filename)
        {
            double acc = 0, vc = C, cv=C;
            double[] ac = new double[5];
            int cn = (int) C;
            for (int i = cn; i < cn + 5; i++)
            {
                SVM svm = new SVM();
                svm.buildSVMCorpus(filename);
                ac[i-cn]=svm.DoCrossValidationTest();
                C++;
            }
            acc = ac[0];
            for(int i=0;i<5;i++)
            {
                if (acc < ac[i])
                {
                    acc = ac[i];
                    vc = i;
                }
            }
            MessageBox.Show("Highest Accuracy : " + (acc * 100) + " With Value of C: " + (cv + vc + 1));
            return acc;
        }
        public int svmAccuracy(double[][] testData,string label)
        {
            int tp = 0;
            if(testData!= null)
            {
                for(int i=0;i<testData.Length;i++)
                {
                    if(label.Equals(svmRealTimeTest(testData[i])))
                        tp++;
                }
            }
            return tp;
        }
        public string svmRealTimeTest(double[] testData)
        {
            //testData=scaleData(testData);
            for (int i = 0; i < 25; i++)
                svmnode[i].value = testData[i];
            var predictY = svm.Predict(svmnode);
            return predictionDictionary[(int)predictY];
        }
    }
}
