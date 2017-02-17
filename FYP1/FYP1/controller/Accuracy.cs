using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace FYP1.controller
{
    class Accuracy
    {
        List<List<double>> down;
        List<List<double>> up;
        List<List<double>> right;
        List<List<double>> left;
        List<List<double>> neutral;

        double[][] rightTrainData;
        double[][] leftTrainData;
        double[][] downTrainData;
        double[][] upTrainData;

        double[][] rightTestData;
        double[][] leftTestData;
        double[][] downTestData;
        double[][] upTestData;
        private double[][] neutralTrainData;
        private double[][] neutralTestData;
        bool fileExistance;
        public Accuracy()
        {       
            down = new List<List<double>>();
            up = new List<List<double>>();
            left = new List<List<double>>();
            right= new List<List<double>>();
            neutral = new List<List<double>>();
            rightTestData = leftTestData = upTestData = downTestData = rightTrainData = leftTrainData = upTrainData = downTrainData =neutralTestData=neutralTrainData= null;
        }
        public bool buildCrossValidationCorpus(string subName)
        {
            fileExistance = false;
            if(File.Exists(subName+".csv"))
            {
                this.readSingalFile(subName);
                fileExistance = true;
            }
            if (File.Exists(subName + "Neutral.csv"))
            {
                this.readData(subName + "Neutral");
                fileExistance = true;
            }
            if (File.Exists(subName + "Up.csv"))
            {
                this.readData(subName + "Up");
                fileExistance = true;
            }
            if (File.Exists(subName + "Down.csv"))
            {
                this.readData(subName + "Down");
                fileExistance = true;
            }
            if (File.Exists(subName + "Left.csv"))
            {
                this.readData(subName + "Left");
                fileExistance = true;
            }
            if (File.Exists(subName + "Right.csv"))
            {
                this.readData(subName + "Right");
                fileExistance = true;
            }
            return fileExistance;
        }
        public void readSingalFile(string filename)
        {
            var reader = new StreamReader(File.OpenRead(filename + ".csv"));
            List<string> listB = new List<string>();
            double tempD = 0;
            List<string> vectorLabel = new List<string>();
            var line2 = reader.ReadLine();
            string line = "",label="";
            var line1 = reader.ReadLine();
            while (!reader.EndOfStream)
            {
                line = "";
                line = reader.ReadLine();
                var values = line.Split(',');
                if (values.Length > 1)
                {
                    if (values.Length > 10)
                    {
                        List<string> tempList = values.ToList<string>();
                        var tempListCount = tempList.Count;
                        tempList.RemoveRange(tempList.Count - 1, 1);
                        tempListCount = tempList.Count;
                        List<double> l1 = tempList.Select(x => double.Parse(x)).ToList();
                        tempD = Double.Parse(values[0]);
                        label = values[values.Length - 1];
                        if (tempD > 0)
                        {
                            vectorLabel.Add(label);
                            if (label.Equals("Down"))
                                down.Add(l1);
                            else if (label.Equals("Up"))
                                up.Add(l1);
                            else if (label.Equals("Left"))
                                left.Add(l1);
                            else if (label.Equals("Right"))
                                right.Add(l1);
                            else if (label.Equals("Neutral"))
                                neutral.Add(l1);
                        }
                    }
                }

            }
            foreach (var lbl in vectorLabel)
            {
                if (lbl.Equals("Up"))
                {
                    int testPercentage = Convert.ToInt32(0.1 * up.Count);
                    upTrainData = new double[up.Count - testPercentage][];
                    upTestData = new double[testPercentage][];
                    Random rand = new Random();
                    int indx = rand.Next() % (up.Count - testPercentage);

                    for (int i = 0, j = 0, k = indx, n = 0; i < up.Count; i++)
                        if (i == k && i < indx + testPercentage)
                        {
                            upTestData[n] = up.ElementAt(i).ToArray();
                            k++;
                            n++;
                        }
                        else
                        {
                            upTrainData[j] = up.ElementAt(i).ToArray();
                            j++;
                        }
                }
                if (lbl.Equals("Left"))
                {
                    int testPercentage = Convert.ToInt32(0.1 * left.Count);
                    leftTrainData = new double[left.Count - testPercentage][];
                    leftTestData = new double[testPercentage][];
                    Random rand = new Random();
                    int indx = rand.Next() % (left.Count - testPercentage);

                    for (int i = 0, j = 0, k = indx, n = 0; i < left.Count; i++)
                        if (i == k && i < indx + testPercentage)
                        {
                            leftTestData[n] = left.ElementAt(i).ToArray();
                            k++;
                            n++;
                        }
                        else
                        {
                            leftTrainData[j] = left.ElementAt(i).ToArray();
                            j++;
                        }
                }
                if (lbl.Equals("Right"))
                {
                    int testPercentage = Convert.ToInt32(0.1 * right.Count);
                    rightTrainData = new double[right.Count - testPercentage][];
                    rightTestData = new double[testPercentage][];
                    Random rand = new Random();
                    int indx = rand.Next() % (right.Count - testPercentage);

                    for (int i = 0, j = 0, k = indx, n = 0; i < right.Count; i++)
                        if (i == k && i < indx + testPercentage)
                        {
                            rightTestData[n] = right.ElementAt(i).ToArray();
                            k++;
                            n++;
                        }
                        else
                        {
                            rightTrainData[j] = right.ElementAt(i).ToArray();
                            j++;
                        }
                }
                if (lbl.Equals("Down"))
                {
                    int testPercentage = Convert.ToInt32(0.1 * down.Count);
                    downTrainData = new double[down.Count - testPercentage][];
                    downTestData = new double[testPercentage][];
                    Random rand = new Random();
                    int indx = rand.Next() % (down.Count - testPercentage);

                    for (int i = 0, j = 0, k = indx, n = 0; i < down.Count; i++)
                        if (i == k && i < indx + testPercentage)
                        {
                            downTestData[n] = down.ElementAt(i).ToArray();
                            k++;
                            n++;
                        }
                        else
                        {
                            downTrainData[j] = down.ElementAt(i).ToArray();
                            j++;
                        }
                }
                if (lbl.Equals("Neutral"))
                {
                    int testPercentage = Convert.ToInt32(0.1 * neutral.Count);
                    neutralTrainData = new double[neutral.Count - testPercentage][];
                    neutralTestData = new double[testPercentage][];
                    Random rand = new Random();
                    int indx = rand.Next() % (neutral.Count - testPercentage);

                    for (int i = 0, j = 0, k = indx, n = 0; i < neutral.Count; i++)
                        if (i == k && i < indx + testPercentage)
                        {
                            neutralTestData[n] = neutral.ElementAt(i).ToArray();
                            k++;
                            n++;
                        }
                        else
                        {
                            neutralTrainData[j] = neutral.ElementAt(i).ToArray();
                            j++;
                        }
                }
            }
        }
        public void readData(string filename)
        {
            var reader = new StreamReader(File.OpenRead(filename + ".csv"));
            List<string> listB = new List<string>();
            double tempD = 0;

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
                    tempList.RemoveRange(tempList.Count-1, 1);
                    tempListCount = tempList.Count;
                    List<double> l1 = tempList.Select(x => double.Parse(x)).ToList();
                    tempD = Double.Parse(values[0]);
                    if (tempD > 0)
                    {
                        if (filename.Contains("Down"))
                            down.Add(l1);
                        else if (filename.Contains("Up"))
                            up.Add(l1);
                        else if (filename.Contains("Left"))
                            left.Add(l1);
                        else if (filename.Contains("Right"))
                            right.Add(l1);
                        else if (filename.Contains("Neutral"))
                            neutral.Add(l1);
                    }

                }

            }
            //Thread.Sleep(100);
            if (filename.Contains("Up"))
            {
                int testPercentage = Convert.ToInt32(0.1 * up.Count);
                upTrainData = new double[up.Count - testPercentage][];
                upTestData = new double[testPercentage][];
                Random rand = new Random();
                int indx=rand.Next()%(up.Count-testPercentage);

                for (int i = 0, j = 0, k = indx,n=0; i < up.Count; i++)
                    if (i==k &&i<indx+testPercentage)
                    {
                        upTestData[n] = up.ElementAt(i).ToArray();
                        k++;
                        n++;
                    }
                    else
                    {
                        upTrainData[j] = up.ElementAt(i).ToArray();
                        j++;
                    }
            }
            if (filename.Contains("Left"))
            {
                int testPercentage = Convert.ToInt32(0.1 * left.Count);
                leftTrainData = new double[left.Count - testPercentage][];
                leftTestData = new double[testPercentage][];
                Random rand = new Random();
                int indx = rand.Next() % (left.Count - testPercentage);

                for (int i = 0, j = 0, k = indx, n = 0; i < left.Count; i++)
                    if (i == k && i < indx + testPercentage)
                    {
                        leftTestData[n] = left.ElementAt(i).ToArray();
                        k++;
                        n++;
                    }
                    else
                    {
                        leftTrainData[j] = left.ElementAt(i).ToArray();
                        j++;
                    }
            }
            if (filename.Contains("Right"))
            {
                int testPercentage = Convert.ToInt32(0.1 * right.Count);
                rightTrainData = new double[right.Count - testPercentage][];
                rightTestData = new double[testPercentage][];
                Random rand = new Random();
                int indx = rand.Next() % (right.Count - testPercentage);

                for (int i = 0, j = 0, k = indx, n = 0; i < right.Count; i++)
                    if (i == k && i < indx + testPercentage)
                    {
                        rightTestData[n] = right.ElementAt(i).ToArray();
                        k++;
                        n++;
                    }
                    else
                    {
                        rightTrainData[j] = right.ElementAt(i).ToArray();
                        j++;
                    }
            }
            if (filename.Contains("Down"))
            {
                int testPercentage = Convert.ToInt32(0.1 * down.Count);
                downTrainData = new double[down.Count - testPercentage][];
                downTestData = new double[testPercentage][];
                Random rand = new Random();
                int indx = rand.Next() % (down.Count - testPercentage);

                for (int i = 0, j = 0, k = indx, n = 0; i < down.Count; i++)
                    if (i == k && i < indx + testPercentage)
                    {
                        downTestData[n] = down.ElementAt(i).ToArray();
                        k++;
                        n++;
                    }
                    else
                    {
                        downTrainData[j] = down.ElementAt(i).ToArray();
                        j++;
                    }
            }
            if (filename.Contains("Neutral"))
            {
                int testPercentage = Convert.ToInt32(0.1 * neutral.Count);
                neutralTrainData = new double[neutral.Count - testPercentage][];
                neutralTestData = new double[testPercentage][];
                Random rand = new Random();
                int indx = rand.Next() % (neutral.Count - testPercentage);

                for (int i = 0, j = 0, k = indx, n = 0; i < neutral.Count; i++)
                    if (i == k && i < indx + testPercentage)
                    {
                        neutralTestData[n] = neutral.ElementAt(i).ToArray();
                        k++;
                        n++;
                    }
                    else
                    {
                        neutralTrainData[j] = neutral.ElementAt(i).ToArray();
                        j++;
                    }
            }
        }
        public double calculateKNNAccuracy()
        {
            if (fileExistance)
            {
                KNN knn = new KNN();

                if (rightTestData != null)

                    knn.setRight(rightTrainData);

                if (leftTestData != null)

                    knn.setLeft(leftTrainData);

                if (upTestData != null)

                    knn.setUp(upTrainData);

                if (downTestData != null)

                    knn.setDown(downTrainData);

                if (neutralTestData != null)

                    knn.setNeutral(neutralTrainData);

               double tp = 0, fp = 0;
                if (rightTestData != null)
                {
                    for (int i = 0; i < rightTestData.Length; i++)
                    {
                        if (knn.knnClassifier(rightTestData[i]) == "Right")
                        {
                            tp++;
                        }
                        else
                        {
                            fp++;
                        }
                    }
                }
                if (leftTestData != null)
                {
                    for (int i = 0; i < leftTestData.Length; i++)
                    {
                        if (knn.knnClassifier(leftTestData[i]) == "Left")
                        {
                            tp++;
                        }
                        else
                        {
                            fp++;
                        }
                    }
                }
                if (upTestData != null)
                {
                    for (int i = 0; i < upTestData.Length; i++)
                    {
                        if (knn.knnClassifier(upTestData[i]) == "Up")
                        {
                            tp++;
                        }
                        else
                        {
                            fp++;
                        }
                    }
                }
                if (downTestData != null)
                {
                    for (int i = 0; i < downTestData.Length; i++)
                    {
                        if (knn.knnClassifier(downTestData[i]) == "Down")
                        {
                            tp++;
                        }
                        else
                        {
                            fp++;
                        }
                    }
                }
                if (neutralTestData != null)
                {
                    for (int i = 0; i < neutralTestData.Length; i++)
                    {
                        if (knn.knnClassifier(neutralTestData[i]) == "Neutral")
                        {
                            tp++;
                        }
                        else
                        {
                            fp++;
                        }
                    }
                }
                double accuracy = (tp / (tp + fp))*100;
                return accuracy;
            }
            else
                return 0;

        }
    }
}
