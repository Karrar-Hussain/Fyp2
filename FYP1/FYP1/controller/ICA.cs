using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace FYP1.controller
{
    class ICA
    {
        private double[][] rawData;
        private double[,] matrix;
        private double[,] eigenVectors;
        private double[,] finalizedData;
        double[][] transponsedData;
        private int EigenValuesLength = 5;
        private int size = 5, dicLen = 0;
        private int length = 0;
        private string[] components;
        Dictionary<string, int> featureCount;
        Dictionary<string, int> featureCountClassLabel;

        public double[][] readCSV(string csvFileName, string channelName = "IED_AF3")
        {
            
                var reader = new StreamReader(File.OpenRead(csvFileName + ".csv"));
                List<List<double>> listAF3 = new List<List<double>>();
                List<string> listB = new List<string>();
                double tempD = 0;

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    if (values.Length > 1)

                        if (values[5] == channelName)
                        {

                            List<string> l = values.ToList<string>();
                            var a = l.Count;
                            l.Insert(5, "Awaish");
                            l.RemoveRange(5, l.Count - 5);
                            a = l.Count;

                            Console.WriteLine(line);
                            List<double> l1 = l.Select(x => double.Parse(x)).ToList();

                            tempD = Double.Parse(values[0]);
                            if (tempD > 0)
                            {
                                //components[listAF3.Count] = values[5];
                                listAF3.Add(l1);
                            }
                        }

                }
                double[][] arrays = listAF3.Select(a => a.ToArray()).ToArray();
                return arrays;
           
        }
        public void eigenDecompositionW()
        {
            double[,] v;
            double[] a;
            double[] b;
            alglib.rmatrixevd(matrix, size, 3, out a, out b, out v, out eigenVectors);

        }
        public void buildCorpus(string docName)
        {

            string channelName1 = "IED_T8";
            //rawData = this.readCSV(subName + "Down", channelName1);
            rawData = this.readCSV(docName, channelName1);
            length = rawData.GetLength(0);
            
        }
        public void executeICA(string docName)
        {
            try {
                buildCorpus(docName);
                CovarianceMatrix();
                eigenDecompositionW();
                ICATransformation(docName);
                double[] d = null;
                double[] d1 = null;
                d = new double[length];
                d1 = new double[length];
                for (int i = 0; i < length; i++)
                {
                    d1[i] = rawData[i][0];
                    d[i] = finalizedData[i, 0];
                }
                icaVisualization(d1, d);
            }catch(Exception e)
            {

                MessageBox.Show("Invalid Filename, File doesnot exist!");
            }
        }
        public double[] readCSV(string csvFileName)
        {
            var reader = new StreamReader(File.OpenRead(csvFileName + ".csv"));
            List<double> listAF3 = new List<double>();
            List<string> listB = new List<string>();
            double tempD = 0;
            int n = 0;
            while (!reader.EndOfStream)
            {
                n++;
                var line = reader.ReadLine();
                Console.WriteLine(line);
                var values = line.Split(',');
                if (values.Length > 1&&n>1)

                //if (values[5] == channelName)
                {
                    tempD = Double.Parse(values[0]);
                    //if (tempD > 0 && tempD < 70)
                    listAF3.Add(tempD);
                }

            }
            return listAF3.ToArray();
        }
        public void icaVisualization(double[] Orignal, double[] Ica)
        {

            MLApp.MLApp matlb = new MLApp.MLApp();
            matlb.Execute("cd C:\\temp");

            object res = null;
            matlb.Feval("icaVisualizeFunc", 1, out res, Orignal, Ica);
        }
        public void CovarianceMatrix()
        {

            transponsedData = new double[size][];
            for (int i = 0; i < size; i++)
                transponsedData[i] = new double[length];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < length; j++)
                    transponsedData[i][j] = rawData[j][i];
            matrix = new double[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    matrix[i, j] = this.Covariance(transponsedData[i], transponsedData[j]);
                }
            }

        }
        public void ICATransformation(string docName)
        {
            StreamWriter fileWriter;
            fileWriter = new StreamWriter(docName + "ICA.csv", false);
            featureCount = new Dictionary<string, int>();
            featureCountClassLabel = new Dictionary<string, int>();
            finalizedData = new double[length, EigenValuesLength];
            Array.Clear(finalizedData, 0, finalizedData.Length);
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < EigenValuesLength; j++)
                {
                    for (int k = 0; k < size; k++)
                        finalizedData[i, j] += rawData[i][k] * eigenVectors[k, j];
                }
            }

            double deter = alglib.rmatrixdet(eigenVectors);
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < EigenValuesLength; j++)
                    fileWriter.Write(finalizedData[i, j] + ",");
                fileWriter.WriteLine("");
            }
        }
        public void ICA_MutualInfo()
        {
            string[] classes = null;
            double[] mi = new double[size];
            for (int i = 0; i < length; i++)
            {

                mi[i] = Shannon_EntropyX(i) - Shannon_RelativeEntropy(classes, i);
            }
        }
        public double Shannon_RelativeEntropy(string[] classes, int y)
        {
            int v = 0;
            double sum = 0, prob = 0, logprob = 0;
            for (int j = 0; j < size; j++)
            {
                featureCountClassLabel.TryGetValue(finalizedData[y, j].ToString() + "_" + components[y], out v);
                prob = v / dicLen;
                logprob = Math.Log(prob);
                sum += prob * logprob;
            }

            return sum;
        }
        public double Shannon_EntropyX(int n)
        {
            int v = 0;
            double sum = 0, prob = 0, logprob = 0;
            for (int j = 0; j < size; j++)
            {
                featureCount.TryGetValue(finalizedData[n, j].ToString(), out v);
                prob = v / dicLen;
                logprob = Math.Log(prob);
                sum += prob * logprob;
            }
            return -1 * sum;
        }
        public void Shannon_Counts()
        {

            dicLen = 0;
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < EigenValuesLength; j++)
                {
                    if (!featureCount.ContainsKey(finalizedData[i, j].ToString()))
                    {
                        dicLen++;
                        featureCount.Add(finalizedData[i, j].ToString(), 1);
                    }
                    else
                    {
                        featureCount[finalizedData[i, j].ToString()]++;

                        //featureCount.TryGetValue(finalizedData[i, j].ToString(), out v);
                        //v++;
                        //featureCount.Remove(finalizedData[i, j].ToString());
                        //featureCount.Add(finalizedData[i, j].ToString(), v);
                    }
                    if (!featureCountClassLabel.ContainsKey(finalizedData[i, j].ToString() + "_" + components[i]))
                        featureCountClassLabel.Add(finalizedData[i, j].ToString() + "_" + components[i], 1);
                    else
                        featureCountClassLabel[finalizedData[i, j] + "_" + components[i].ToString()]++;
                }

            }
        }
        public double Covariance(double[] arr1, double[] arr2)
        {
            int size = arr1.Length;
            double avg1 = 0, avg2 = 0;
            avg1 = arr1.Average();
            avg2 = arr2.Average();
            double cov = 0;
            for (int i = 0; i < size; i++)
            {
                cov += (arr1[i] - avg1) * (arr2[i] - avg2);
            }
            cov = cov / (arr1.Length - 1);

            return cov;
        }
    }
}
