using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace FYP1.controller
{
    class PCA
    {
        static TextWriter fileWriter;
        private string[] channelNames = { "IED_AF3", "IED_T8", "IED_T7", "IED_AF4", "IED_O1" };
        private double[][] rawData;
        private double[,] matrix;
        private double[,] eigenVectors;
        private double[,] data;
        private double[,] finalizedData;
        double[][] transponsedData;
        private int EigenValuesLength = 3;
        private int size = 5;
        private int length = 0;
        private int cUp = 0;
        private int cDown = 0;
        private int cLeft = 0;
        private int cRight = 0;
        public bool fileExistance;
        private int cNeutral;
        private List<string>sequence;
        private List<int> sequenceCount;
       /* public bool buildPCACorpus(string subName)
        {
            sequence = "";
            sequenceCount = new List<int>();
            fileExistance = false;
            double[][] tempData = null;
            if (File.Exists(subName + "Neutral.csv"))
            {
                if (!fileExistance)
                    rawData = this.readData(subName + "Neutral");
                cNeutral = rawData.GetLength(0);
                fileExistance = true;
                sequence = "Neutral";
                sequenceCount.Add(cNeutral);
            }
            if (File.Exists(subName + "Up.csv"))
            {
                if (!fileExistance)
                    rawData = this.readData(subName + "Up");
                else
                    tempData = this.readData(subName + "Up");
                rawData = rawData.Concat(tempData).ToArray();
                cUp = tempData.GetLength(0);
                fileExistance = true;
                sequence = sequence + ",Up";
                sequenceCount.Add(cUp);
            }
            if (File.Exists(subName + "Down.csv"))
            {
                if (!fileExistance)
                    rawData = this.readData(subName + "Down");
                else
                    tempData = this.readData(subName + "Down");
                cDown = tempData.GetLength(0);
                rawData = rawData.Concat(tempData).ToArray();
                fileExistance = true;
                sequence = sequence + ",Down";
                sequenceCount.Add(cDown);
            }
            if (File.Exists(subName + "Left.csv"))
            {
                if (!fileExistance)
                    rawData = this.readData(subName + "Left");
                else
                    tempData = this.readData(subName + "Left"); ;
                cLeft = tempData.GetLength(0);
                rawData = rawData.Concat(tempData).ToArray();
                fileExistance = true;
                sequence = sequence + ",Left";
                sequenceCount.Add(cLeft);
            }
            if (File.Exists(subName + "Right.csv"))
            {
                if (!fileExistance)
                    rawData = this.readData(subName + "Right");
                else
                    tempData = this.readData(subName + "Right"); ;
                cRight = tempData.GetLength(0);
                rawData = rawData.Concat(tempData).ToArray();
                fileExistance = true;
                sequence = sequence + ",Right";
                sequenceCount.Add(cRight);
            }
            length = rawData.GetLength(0);
            return fileExistance;
        }*/
        public double[][] readData(string filename)
        {
            var reader = new StreamReader(File.OpenRead(filename + ".csv"));
            List<string> listB = new List<string>();
            double tempD = 0;
            List<List<double>> temp2dList = new List<List<double>>();
            var line2 = reader.ReadLine();
            string line = "";
            var line1 = reader.ReadLine();
            while (!reader.EndOfStream)
            {
                line = "";
                line = reader.ReadLine();
                var values = line.Split(',');
                if (values.Length > 1)
                {
                    List<string> tempList = values.ToList<string>();
                    var tempListCount = tempList.Count;
                    tempList.RemoveRange(25, 1);
                    tempListCount = tempList.Count;
                    List<double> l1 = tempList.Select(x => double.Parse(x)).ToList();
                    tempD = Double.Parse(values[0]);
                    if (tempD > 0)
                    {
                        temp2dList.Add(l1);
                    }
                }

            }
            double[][] arrays = temp2dList.Select(a => a.ToArray()).ToArray();
            return arrays;
        }
        public void applyPCA(string subName)
        {
            string[] str = null;
            double[] d = null;
            double[] d1 = null;
            int g = 0;
            fileWriter = new StreamWriter(subName + "PCA.csv", false);
            if (this.fileExistance)
            {
                this.CovarianceMatrix();
                this.findEigenMatrix();
                this.CentredData();
                this.UpdatedData();
                if (g == 0)
                {
                    d = new double[length];
                    d1 = new double[length];
                    str = new string[length];
                    for (int i = 0; i < length; i++)
                        str[i] = "";
                }
                g++;
                for (int i = 0; i < length; i++)
                {
                    d1[i] = data[i, 0];
                    d[i] = finalizedData[i, 0];
                    for (int j = 0; j < EigenValuesLength; j++)
                    {
                        str[i] += finalizedData[i, j].ToString() + ",";
                    }
                }
            }
            /*
            string[] seq = sequence.Split(',');
            for (int j = 0; j < seq.Length; j++)
                for (int i = 0; i < sequenceCount[j]; i++)
                {
                    str[i] = str[i] + seq[j];
                    fileWriter.WriteLine(str[i]);
                }
            
                for (int i = 0; i < length; i++)
            {
                if (File.Exists(subName + "Neutral.csv"))
                    {
                    if (i < cNeutral)
                        str[i] += "Neutral";
                    }
                if (File.Exists(subName + "Up.csv"))
                {
                    if (i < cUp)
                        str[i] += "Up";
                }
                else if (i < cUp + cDown)
                    str[i] += "Down";
                else if (i < cUp + cDown + cLeft)
                    str[i] += "Left";
                else if (i < cUp + cDown + cLeft + cRight)
                    str[i] += "Right";

                }
                */
            //pcaVisualization(d1, d); 
        }
        public double[] testDataPCA(double[] testData)
        {
            rawData = new double[1][];
            rawData[0] = new double[25];
            rawData[0] = testData;
            length = rawData.GetLength(0);
            this.CovarianceMatrix();
            this.findEigenMatrix();
            this.CentredData();
            this.UpdatedData();
            double[] pcaConvertedData = new double[this.EigenValuesLength];
            for (int j = 0; j < EigenValuesLength; j++)
            {
                pcaConvertedData[j] += finalizedData[0, j];
            }
            MessageBox.Show(pcaConvertedData[0] + "");
            return pcaConvertedData;
        }




        /// <summary>
        /// //////////////////////////
        /// Awaish updates
        /// </summary>
        /// <param name="cName"></param>
        /// <param name="docName"></param>
        public void buildCorpus(string cName, string docName)
        {
            double[][] tempData = null;
            tempData = this.readCSV(docName + "Down", cName);
            cDown = tempData.GetLength(0);
            rawData = this.readCSV(docName + "Up", cName);
            cUp = rawData.GetLength(0);
            rawData = rawData.Concat(tempData).ToArray();
            tempData = this.readCSV(docName + "Left", cName);
            cLeft = tempData.GetLength(0);
            rawData = rawData.Concat(tempData).ToArray();
            tempData = this.readCSV(docName + "Right", cName);
            cRight = tempData.GetLength(0);
            rawData = rawData.Concat(tempData).ToArray();
            length = rawData.GetLength(0);
        }
        public void buildCorpus1(string docName, int cn)
        {
            double[][] tempData = null;
            tempData = this.readCSV1(docName + "Down", cn);
            cDown = tempData.GetLength(0);
            rawData = this.readCSV1(docName + "Up", cn);
            cUp = rawData.GetLength(0);
            rawData = rawData.Concat(tempData).ToArray();
            tempData = this.readCSV1(docName + "Left", cn);
            cLeft = tempData.GetLength(0);
            rawData = rawData.Concat(tempData).ToArray();
            tempData = this.readCSV1(docName + "Right", cn);
            cRight = tempData.GetLength(0);
            rawData = rawData.Concat(tempData).ToArray();
            length = rawData.GetLength(0);
        }
        public bool buildPCACorpus(string subName, int cn)
        {
            sequence = new List<string>();
            sequenceCount = new List<int>();
            fileExistance = false;
            double[][] tempData = null;
            if (File.Exists(subName + "Neutral.csv"))
            {
                if (!fileExistance)
                    rawData = this.readCSV1(subName + "Neutral", cn);
                cNeutral = rawData.GetLength(0);
                fileExistance = true;
                sequence.Add( "Neutral");
                sequenceCount.Add(cNeutral);
            }
            if (File.Exists(subName + "Up.csv"))
            {
                if (!fileExistance)
                {
                    rawData = this.readCSV1(subName + "Up", cn);
                    sequence.Add( "Up");
                }
                else
                {
                    tempData = this.readCSV1(subName + "Up", cn);
                    sequence .Add( "Up");
                    rawData = rawData.Concat(tempData).ToArray();
                }
                cUp = tempData.GetLength(0);
                fileExistance = true;
                sequenceCount.Add(cUp);
            }
            if (File.Exists(subName + "Down.csv"))
            {
                if (!fileExistance)
                {
                    rawData = this.readCSV1(subName + "Down", cn);
                    sequence.Add("Down");
                }
                else
                {
                    tempData = this.readCSV1(subName + "Down", cn);
                    sequence.Add("Down");
                    rawData = rawData.Concat(tempData).ToArray();
                }
                cDown = tempData.GetLength(0);
                fileExistance = true;
                sequenceCount.Add(cDown);
            }
            if (File.Exists(subName + "Left.csv"))
            {
                if (!fileExistance)
                {
                    rawData = this.readCSV1(subName + "Left", cn);
                    sequence.Add("Left");
                }
                else
                {
                    tempData = this.readCSV1(subName + "Left", cn);
                    sequence.Add("Left");
                    rawData = rawData.Concat(tempData).ToArray();
                }
                cLeft = tempData.GetLength(0);
                fileExistance = true;
                sequenceCount.Add(cLeft);
            }
            if (File.Exists(subName + "Right.csv"))
            {
                if (!fileExistance)
                {
                    rawData = this.readCSV1(subName + "Right", cn);
                    sequence.Add("Right");
                }
                else
                {
                    tempData = this.readCSV1(subName + "Right", cn);
                    sequence.Add("Right");
                    rawData = rawData.Concat(tempData).ToArray();
                }
                cRight = tempData.GetLength(0);
                fileExistance = true;
                sequenceCount.Add(cRight);
            }
            if(fileExistance)
            length = rawData.GetLength(0);
            return fileExistance;
        }
        public double[][] readCSV(string csvFileName, string channelName = "IED_AF3")
        {
            if (File.Exists(csvFileName + ".csv"))
            {
                var reader = new StreamReader(File.OpenRead(csvFileName + ".csv"));
                List<List<double>> listAF3 = new List<List<double>>();
                List<string> listB = new List<string>();
                double tempD = 0;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    Console.WriteLine(line);
                    var values = line.Split(',');
                    if (values.Length > 1)
                        if (values[5] == channelName)
                        {
                            List<string> l = values.ToList<string>();
                            var a = l.Count;
                            l.RemoveRange(5, l.Count - 5);
                            a = l.Count;
                            List<double> l1 = l.Select(x => double.Parse(x)).ToList();
                            tempD = Double.Parse(values[0]);
                            if (tempD > 0)
                                listAF3.Add(l1);
                        }
                }
                double[][] arrays = listAF3.Select(a => a.ToArray()).ToArray();
                return arrays;
            }
            else
            {
                return null;
            }
        }
        public double[][] readCSV1(string csvFileName, int cn)
        {
            if (File.Exists(csvFileName + ".csv"))
            {
                var reader = new StreamReader(File.OpenRead(csvFileName + ".csv"));
                List<List<double>> chList = new List<List<double>>();
                List<string> listB = new List<string>();
                double tempD = 0;
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    Console.WriteLine(line);
                    var values = line.Split(',');
                    if (values.Length > 1)
                    {
                        List<string> l = new List<string>();
                        for (int n = (cn - 1) * 5; n < (cn - 1) * 5 + 5; n++)
                            l.Add(values[n]);
                        List<double> l1 = l.Select(x => double.Parse(x)).ToList();
                        tempD = Double.Parse(values[0]);
                        if (tempD > 0)
                            chList.Add(l1);
                    }
                }
                double[][] arrays = chList.Select(a => a.ToArray()).ToArray();
                return arrays;
            }
            else
            {
                return null;
            }
        }
        public bool ExecutePCA1(string docName)
        {
            string[] str = null;
            double[] d = null;
            double[] d1 = null;
            fileWriter = new StreamWriter(docName + "PCA.csv", false);
            int cn = 1;
            for ( cn = 1; cn < 6; cn++)
            {
                if (this.buildPCACorpus(docName, cn))
                {
                    this.CovarianceMatrix();
                    this.findEigenMatrix();
                    this.CentredData();
                    this.UpdatedData();
                    if (cn == 1)
                    {
                        d = new double[length];
                        d1 = new double[length];
                        str = new string[length];
                        for (int i = 0; i < length; i++)
                            str[i] = "";
                    }
                    for (int i = 0; i < length; i++)
                    {
                        d1[i] = data[i, 0];
                        d[i] = finalizedData[i, 0];
                        for (int j = 0; j < EigenValuesLength; j++)
                        {
                            str[i] += finalizedData[i, j].ToString() + ",";
                        }
                    }
                }else
                {
                    cn = 6;
                }
            }
            if (cn != 7)
            {
                    for (int i = 0; i < str.Length; i++)
                    {
                        if(i<sequenceCount[0])
                        str[i] = str[i] +sequence[0];
                        else if (i < sequenceCount[1]+ sequenceCount[0])
                            str[i] = str[i] + sequence[1];
                        else if (i < sequenceCount[1] + sequenceCount[0]+sequenceCount[2])
                            str[i] = str[i] + sequence[2];
                        else if (i < sequenceCount[1] + sequenceCount[0] + sequenceCount[2]+sequenceCount[3])
                            str[i] = str[i] + sequence[3];
                        else if (i < sequenceCount[1] + sequenceCount[0] + sequenceCount[3]+sequenceCount[2]+sequenceCount[4])
                            str[i] = str[i] + sequence[4];
                        fileWriter.WriteLine(str[i]);
                    }
                for (int i = 0; i < eigenVectors.GetLength(0); i++)
                {
                    for (int j = 0; j < eigenVectors.GetLength(1); j++)
                    {
                        fileWriter.Write(eigenVectors[i, j] + ",");
                    }
                    fileWriter.WriteLine();
                }
            }
            fileWriter.Close();
            return fileExistance;
            //pcaVisualization(d1, d);
        }
        public void findEigenMatrix()
        {
            double[,] v;
            double[] a;
            double[] b;
            alglib.rmatrixevd(matrix, size, 3, out a, out b, out v, out eigenVectors);
        }
        public void CentredData()
        {
            data = new double[length, size];
            finalizedData = new double[length, EigenValuesLength];
            Array.Clear(finalizedData, 0, finalizedData.Length);
            for (int i = 0; i < size; i++)
            {
                double avg = transponsedData[i].Average();
                for (int j = 0; j < length; j++)
                {
                    data[j, i] = rawData[j][i] - avg;
                }
            }
        }
        public void UpdatedData()
        {
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < EigenValuesLength; j++)
                {
                    for (int k = 0; k < size; k++)
                        finalizedData[i, j] += data[i, k] * eigenVectors[k, j];
                }
            }
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
            if (arr1.Length > 1)
                cov = cov / (arr1.Length - 1);
            return cov;
        }
        public void ExecutePCA(string docName)
        {
            string[] str = null;
            double[] d = null;
            double[] d1 = null;
            int g = 0;
            try
            {
                fileWriter = new StreamWriter(docName + "PCA.csv", false);
                foreach (string cName in channelNames)
                {
                    this.buildCorpus(cName, docName);
                    this.CovarianceMatrix();
                    this.findEigenMatrix();
                    this.CentredData();
                    this.UpdatedData();
                    if (g == 0)
                    {
                        d = new double[length];
                        d1 = new double[length];
                        str = new string[length];
                        for (int i = 0; i < length; i++)
                            str[i] = "";
                    }
                    g++;
                    for (int i = 0; i < length; i++)
                    {
                        d1[i] = data[i, 0];
                        d[i] = finalizedData[i, 0];
                        for (int j = 0; j < EigenValuesLength; j++)
                        {
                            str[i] += finalizedData[i, j].ToString() + ",";
                        }
                    }
                }
                for (int i = 0; i < length; i++)
                {
                    if (i < cUp)
                        str[i] += "Up";
                    else if (i < cUp + cDown)
                        str[i] += "Down";
                    else if (i < cUp + cDown + cLeft)
                        str[i] += "Left";
                    else if (i < cUp + cDown + cLeft + cRight)
                        str[i] += "Right";
                    fileWriter.WriteLine(str[i]);
                }
                //pcaVisualization(d1, d);
            }
            catch (Exception e)
            {
                MessageBox.Show("Invalid Filename, File doest not exist!");
            }
        }
        public void icaVisualization(double[] Orignal, double[] Ica)
        {
            MLApp.MLApp matlb = new MLApp.MLApp();
            matlb.Execute("cd C:\\temp");
            object res = null;
            matlb.Feval("icaVisualizeFunc", 1, out res, Orignal, Ica);
        }
        public void pcaVisualization(double[] Orignal, double[] Pca)
        {
            MLApp.MLApp matlb = new MLApp.MLApp();
            matlb.Execute("cd C:\\temp");
            object res = null;
            matlb.Feval("pcaVisualizeFunc", 1, out res, Orignal, Pca);
        }
        public void signalVisualization(double[] Orignal, double[] Pca, double[] Ica)
        {
            MLApp.MLApp matlb = new MLApp.MLApp();
            matlb.Execute("cd C:\\temp");

            object res = null;
            matlb.Feval("myfunc2", 1, out res, Orignal, Pca, Ica);
        }
    }
}
