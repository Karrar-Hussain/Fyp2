using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace FYP1.controller
{
    class KNN
    {
        private int K = 5;
        List<List<double>> down;
        List<List<double>> up;
        List<List<double>> right;
        List<List<double>> left;
        List<List<double>> neutral;

        private double[][] neutralArr;
        private double[][] downArr;
        private double[][] upArr;
        private double[][] rightArr;
        private double[][] leftArr;
        private double[][] downT7;
        private double[][] upT7;
        private double[][] rightT7;
        private double[][] leftT7;
        private double[][] downT8;
        private double[][] upT8;
        private double[][] rightT8;
        private double[][] leftT8;
        bool fileExistance;
        public KNN()
        {
            down = new List<List<double>>();
            up = new List<List<double>>();
            left = new List<List<double>>();
            right= new List<List<double>>();
            neutral = new List<List<double>>();
        }
        public void setRight(double[][] right) { rightArr = right; }
        public void setLeft(double[][] left) { leftArr = left; }
        public void setUp(double[][] up) { upArr = up; }
        public void setDown(double[][] down) { downArr = down; }
        public void setNeutral(double[][] neutral) { neutralArr = neutral; }

        public KNN(double[][] right,double[][] left,double[][] up,double[][] down)
        {
            rightArr = right;
            leftArr = left;
            upArr = up;
            downArr = down;
        }
        public double[][] readCSV(string csvFileName, string channelName = "IED_AF3")
        {
            var reader = new StreamReader(File.OpenRead("C:\\temp\\" + csvFileName + ".csv"));
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
                        l.Insert(5, "Awaish");
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
        public void buildCorpus(string subName="karrar")
        {
            KNN P = new KNN();
            string channelName1 = "IED_T8";
            downT8 = P.readCSV(subName + "Down", channelName1);
            upT8 = P.readCSV(subName + "Up", channelName1);
            rightT8 = P.readCSV(subName + "Right", channelName1);
            leftT8 = P.readCSV(subName + "Left", channelName1);

            string channelName2 = "IED_T7";
            downT7 = P.readCSV(subName + "Down", channelName2);
            upT7 = P.readCSV(subName + "Up", channelName2);
            rightT7 = P.readCSV(subName + "Right", channelName2);
            leftT7 = P.readCSV(subName + "Left", channelName2);

        }
        public double[] EuclideanDistance(double[][] train1, double[][] train2, double[] test1, double[] test2)
        {
            int size = train1.GetLength(0) == train2.GetLength(0) ? train2.GetLength(0) : 0;
            double[] distance = new double[size];
            for (int i = 0; i < size; i++)
            {
                double sum = 0;
                for (int j = 0; j < 5; j++)
                {
                    sum += Math.Pow(train1[i][j] - test1[j], 2);
                }
                for (int j = 0; j < 5; j++)
                {
                    sum += Math.Pow(train2[i][j] - test2[j], 2);
                }
                distance[i] = Math.Sqrt(sum);
            }
            return distance;
        }
        public Tuple<double[], double[], double[], double[]> getDistance(KNN obj, double[] test1, double[] test2)
        {
            double[] distancedown = obj.EuclideanDistance(downT7, downT8, test1, test2);
            double[] distanceup = obj.EuclideanDistance(upT7, upT8, test1, test2);
            double[] distanceleft = obj.EuclideanDistance(leftT7, leftT8, test1, test2);
            double[] distanceright = obj.EuclideanDistance(rightT7, rightT8, test1, test2);

            return new Tuple<double[], double[], double[], double[]>(distancedown, distanceup, distanceleft, distanceright);
        }

        /*public void classify()
        {
            KNN obj = new KNN();
            
            obj.buildCorpus();
            double[] test1 = { 2.614298424, 1.616061408, 2.714335497, 2.243712362, 1.594955009 };
            double[] test2 = { 2.662744246, 5.598855241, 8.885490631, 2.777243629, 2.348678793 };
            var distance = obj.getDistance(obj, test1, test2);
            //var Count = obj.getcount(distance);
            //string label = obj.label(Count);
            Console.WriteLine(label);
        }*/
        /// <summary>
        /// ///updated by Karrar Hussain 2-Feb-2017
        /// </summary>
        /// <param name="For pca input data and classifier for realtime eeg data"></param>
        /// <returns></returns>
        ///////////////////////////////////////////////////////////////////////


        public bool buildCorpusSimple(string subName="karrar")
        {
            fileExistance = false;
            if (File.Exists(subName + ".csv"))
            {
                this.readPCAConverted(subName);
                fileExistance = true;
            }
            else
            {
                if (File.Exists(subName + "Neutral.csv"))
                {
                    this.readSimpleData(subName + "Neutral");
                    fileExistance = true;
                }
                if (File.Exists(subName + "Up.csv"))
                {
                    this.readSimpleData(subName + "Up");
                    fileExistance = true;
                }
                if (File.Exists(subName + "Down.csv"))
                {
                    this.readSimpleData(subName + "Down");
                    fileExistance = true;
                }
                if (File.Exists(subName + "Left.csv"))
                {
                    this.readSimpleData(subName + "Left");
                    fileExistance = true;
                }
                if (File.Exists(subName + "Right.csv"))
                {
                    this.readSimpleData(subName + "Right");
                    fileExistance = true;
                }
            }
            return fileExistance;
        }
        public void readSimpleData(string filename)
        {
            var reader = new StreamReader(File.OpenRead(filename + ".csv"));
            List<string> listB = new List<string>();
            double tempD = 0;            
            var line2 = reader.ReadLine();
            string line="";
            var line1 = reader.ReadLine();
            while (!reader.EndOfStream)
            {
                line = "";
                line = reader.ReadLine();  
                var values=line.Split(',');
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
            if (filename.Contains("Right"))
                rightArr = right.Select(a => a.ToArray()).ToArray();

            if (filename.Contains("Left"))
                leftArr = left.Select(a => a.ToArray()).ToArray();

            if (filename.Contains("Up"))
                upArr = up.Select(a => a.ToArray()).ToArray();

            if (filename.Contains("Down"))
                downArr = down.Select(a => a.ToArray()).ToArray();

            if (filename.Contains("Neutral"))
                neutralArr = neutral.Select(a => a.ToArray()).ToArray();
        }
        public void readPCAConverted(string pcaFileName)
        {
            var reader = new StreamReader(File.OpenRead(pcaFileName + ".csv"));
            List<string> listB = new List<string>();
            List<string> vectorLabel = new List<string>();
            double[][] eigenValues = new double[5][];
            double tempD = 0;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                if (values.Length > 1)
                {
                    List<string> tempList = values.ToList<string>();
                    var tempListCount = tempList.Count;
                    int count = tempList.Count - 1;
                    if (values.Length > 10)
                    {
                        tempList.RemoveRange(count, 1);
                    tempListCount = tempList.Count;
                    List<double> l1 = tempList.Select(x => double.Parse(x)).ToList();
                    tempD = Double.Parse(values[0]);

                    if (tempD > 0)
                    {
                        
                            if (values[count].Equals("Down"))
                                down.Add(l1);
                            else if (values[count].Equals("Up"))
                                up.Add(l1);
                            else if (values[count].Equals("Right"))
                                left.Add(l1);
                            else if (values[count].Equals("Left"))
                                right.Add(l1);
                            else
                                neutral.Add(l1);
                            vectorLabel.Add(values[count]);
                        }
                    }
                    else
                    {

                    }
                }
            }
            foreach (var lbl in vectorLabel.Distinct())
            {
                if (lbl.Equals("Up"))
                {
                    upArr = up.Select(a => a.ToArray()).ToArray();
                }
                else
                if (lbl.Equals("Right"))
                {
                    rightArr = right.Select(a => a.ToArray()).ToArray();
                }
                else
                if (lbl.Equals("Left"))
                {
                    leftArr = left.Select(a => a.ToArray()).ToArray();
                }
                else
                if (lbl.Equals("Down"))
                {
                    downArr = down.Select(a => a.ToArray()).ToArray();
                }
                else
                if (lbl.Equals("Neutral"))
                {
                    neutralArr = neutral.Select(a => a.ToArray()).ToArray();
                }
            }
        }        
        public double[] neoEuclideanDistance(double[][] trainData, double[] testData)
        {
            int size = trainData.GetLength(0);
            double[] distance = new double[size];
            for (int i = 0; i < size; i++)
            {
                double sum = 0;
                for (int j = 0; j < 15; j++)
                {
                    sum += Math.Pow(trainData[i][j] - testData[j], 2);
                }
                distance[i] = Math.Sqrt(sum);
            }
            return distance;
        }
        public string classify(double[] testData)
        {
            var distance = this.getDistancePCA(testData);
            var Count = this.getcount(distance);
            string label = this.label(Count);
            return label;
        }
        private double[] pcaConversion(double[] testData)
        {

            return null;
        }
        public string knnClassifier(double[] testData)
        {

            var distance = this.getDistancePCA(testData);
            var Count = this.getcount(distance);
            string label = this.label(Count);
            return label;
        }
        public string label(Tuple<int, int, int, int,int> Count)
        {
            int[] doubleArr = { Count.Item1, Count.Item2, Count.Item3 , Count.Item4 , Count.Item5 };
            int maxIdx = maxInteger(doubleArr);
            if (maxIdx==0)
            {
                return "Down";
            }
            else if (maxIdx==1)
            {
                return "Up";
            }
            else if (maxIdx==2)
            {
                return "Left";
            }
            else if (maxIdx == 3)
            {
                return "Right";
            }
            else
                return "Neutral";
        }
        public Tuple<double[], double[], double[], double[],double[]> getDistancePCA( double[] testData)
        {
            double[] downDistance=null;
            double[] upDistance=null;
            double[] rightDistance=null; double[] leftDistance=null;
            double[] neutralDistance = null;
            if (downArr!= null)
            downDistance = this.neoEuclideanDistance(downArr,testData);
            if(upArr!= null)
            upDistance = this.neoEuclideanDistance(upArr, testData);
            if(rightArr!= null)
            rightDistance = this.neoEuclideanDistance(rightArr, testData);
            if(leftArr!= null)
            leftDistance = this.neoEuclideanDistance(leftArr, testData);
            if(neutralArr!= null)
                neutralDistance= this.neoEuclideanDistance(neutralArr, testData);
            return new Tuple<double[], double[], double[], double[],double[]>(downDistance, upDistance, leftDistance, rightDistance,neutralDistance);
        }
        public int maxInteger(int[] arr)
        {
            int max = arr[0];
            int index = 0;
            for (int i = 1; i < arr.Length; i++)
                if (max < arr[i])
                {
                    max = arr[i];
                    index = i;
                }
            return index;
        }
        public int minDouble(double[] arr)
        {
            double min = arr[0];
            int index = 0;
            for (int i = 1; i < arr.Length; i++)
                if (min > arr[i])
                {
                    min = arr[i];
                    index = i;
                }
            return index;
        }
        public Tuple<int, int, int, int,int> getcount(Tuple<double[], double[], double[], double[],double[]> distance)
        {
            int countd = 0;
            int countu = 0;
            int countl = 0;
            int countr = 0;
            int countn = 0;
            List<double> distancedown = null, distanceneutral = null, distanceup =null, distanceleft=null, distanceright=null;
            if (distance.Item1!=null)
            distancedown = distance.Item1.ToList<double>();
            if (distance.Item2!= null)
                distanceup = distance.Item2.ToList<double>();
            if (distance.Item3 != null)
                distanceleft = distance.Item3.ToList<double>();
            if (distance.Item4 != null)
                distanceright = distance.Item4.ToList<double>();
            if (distance.Item5 != null)
                distanceneutral = distance.Item5.ToList<double>();

            for (int i = 0; i < K; i++)
            {
                double distanced=10000, distancel=10000, distancer=10000, distanceu=10000, distancen = 10000;
                if(distancedown!=null)
                distanced = distancedown.Min();
                if (distanceup != null)
                    distanceu = distanceup.Min();
                if (distanceleft != null)
                    distancel = distanceleft.Min();
                if (distanceright != null)
                    distancer = distanceright.Min();
                if (distanceneutral != null)
                    distancen = distanceneutral.Min();
                double[] doubleArr = { distanced, distancel, distancer, distanceu, distancen };
                int minIdx=minDouble(doubleArr);
                if (minIdx==0)
                {
                    countd += 1;
                    distancedown.Remove(distanced);
                }
                else if (minIdx==3)
                {
                    countu += 1;
                    distanceup.Remove(distanceu);
                }
                else if (minIdx==1)
                {
                    countl += 1;
                    distanceleft.Remove(distancel);
                }
                else if (minIdx==2)
                {
                    countr += 1;
                    distanceright.Remove(distancer);
                }
                else
                {
                    countn += 1;
                    distanceneutral.Remove(distancen);
                }
            }
            return new Tuple<int, int, int, int,int>(countd, countu, countl, countr,countn);
        }

    }
}
