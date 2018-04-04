using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PhanLop
{
    class Document
    {
        public List<Feature> featureList { get; set; }
        public String className { get; set; }
        public int index { get; set; }

        public Document()
        {
            this.featureList = new List<Feature>();
            this.className = "";
            this.index = -1;
        }

        public int timIndexCuaFeatureTrongDocument(String feature)
        {
            return this.featureList.FindIndex(x => x.feature == feature);
        }

        public void tangWeightChoFeatureCuaDocument(int featureIndex)
        {
            this.featureList.ElementAt(featureIndex).tangWeight();
        }

        public void themFeatureVaoDocument(String feature)
        {
            featureList.Add(new Feature(feature, 1));
        }

        public void themfeature(String feature, int weigth)
        {
            featureList.Add(new Feature(feature, weigth));
        }

        //Tìm feature xuất hiện nhiều nhất trong Document
        public static double timSoLanXuatHienNhieuNhat(Document document)
        {
            double max = 0;

            document.featureList.ForEach(delegate (Feature f)
            {
                if (f.weight > max)
                {
                    max = f.weight;
                }
            });

            return max;
        }

        //Tinh so luong van ban co tu feature i bat ki 
        public static int DemDocChuaI(string i, List<Document> documentlist)
        {
            int count = 0;
            documentlist.ForEach(delegate (Document x)
            {
                x.featureList.ForEach(delegate (Feature y)
                {
                    if (y.feature == i)
                    {
                        count++;
                    }
                });
            });
            return count;
        }

        public static double timWeightCuaFeatureTrongDocument(Document document, String feature)
        {
            double weight = 0;

            document.featureList.ForEach(delegate (Feature f)
            {
                if (f.feature == feature)
                {
                    weight = f.weight;
                }
            });

            return weight;
        }

        public static List<Document> gopDocuments(List<Class> classList)
        {
            List<Document> documentList = new List<Document>();

            classList.ForEach(delegate (Class c)
            {
                c.documentList.ForEach(delegate (Document d)
                {
                    documentList.Add(d);
                });
            });

            return documentList;
        }

        public static List<Document> layCacDocuments(String filePath)
        {
            List<Document> documentList = new List<Document>();

            using (StreamReader sr = new StreamReader(filePath))
            {
                String line = null;
                int j = 0; //Cho biết document thứ bao nhiêu trong documentList

                //Mỗi một line (dòng) là một document
                while ((line = sr.ReadLine()) != null)
                {
                    //Thêm một document rỗng vào danh sách documents
                    documentList.Add(new Document());
                    documentList.ElementAt(j).index = j;

                    //Tạo ra các feature từ line mới đọc (dấu khoảng cách ngăn cách 2 features)
                    String[] features = line.Split(' ');

                    //Với mỗi feature kiem tra xem da co feature nay chua. Neu co roi thi tang trong so len. 
                    for (int i = 0; i < features.Length; i++)
                    {
                        //Tìm index của feature (tìm xem có feature đó trong document chưa)
                        int featureIndex = documentList.ElementAt(j).timIndexCuaFeatureTrongDocument(features[i]);

                        if (featureIndex >= 0)
                        {
                            //Nếu có rồi thì tăng weight cho feature đó trong document này
                            documentList.ElementAt(j).tangWeightChoFeatureCuaDocument(featureIndex);
                        }
                        else
                        {
                            //Nếu chưa có thì thêm feature đó vào document này
                            documentList.ElementAt(j).themFeatureVaoDocument(features[i]);
                        }
                    }

                    j++;
                }
            }

            return documentList;
        }

        public static String phanLop(Document searchedDocument, int numberOfReturnDocuments, int numberOfDocuments, List<Document> documentListOfAllClasses, List<Class> classList)
        {
            Document searchedDocument_tfidf = new Document();
            List<FeatureIDF> featureList = new List<FeatureIDF>();

            using (StreamReader sr = new StreamReader("featureList.txt"))
            {
                String line = null;
                while ((line = sr.ReadLine()) != null)
                {
                    String[] feature = line.Split(' ');

                    featureList.Add(new FeatureIDF(feature[0], double.Parse(feature[1])));
                }
            }

            featureList.ForEach(delegate (FeatureIDF f)
            {
                double tf = (double)timWeightCuaFeatureTrongDocument(searchedDocument, f.feature) / timSoLanXuatHienNhieuNhat(searchedDocument);
                searchedDocument_tfidf.featureList.Add(new Feature(f.feature, Math.Round(tf * f.idf, XuLyFile.docRound())));
            });

            List<double[]> bow_tfidf = BOW.docBOW_tfidf();
            SimilarityMeasure[] similarityMeasures = new SimilarityMeasure[numberOfDocuments];
            int i, j;

            for (i = 0; i < numberOfDocuments; i++)
            {
                double value = 0;

                for (j = 0; j < searchedDocument_tfidf.featureList.Count; j++)
                {
                    value += Math.Pow(bow_tfidf.ElementAt(i)[j] - searchedDocument_tfidf.featureList.ElementAt(j).weight, 2);
                }

                similarityMeasures[i] = new SimilarityMeasure(Math.Round(Math.Sqrt(value), XuLyFile.docRound()), i);
            }
            
            //Sắp xếp độ tương tự tăng dần
            SimilarityMeasure[] sortedSimilarityMeasures = similarityMeasures.OrderBy(sm => sm.value).ToArray();
            
            Dictionary<String, int> classesOfDocument = new Dictionary<string, int>();
            for (int k = 0; k < numberOfReturnDocuments; k++)
            {
                for (int f = 0; f < documentListOfAllClasses.Count; f++)
                {
                    if (f == sortedSimilarityMeasures[k].index)
                    {
                        Console.WriteLine(documentListOfAllClasses.ElementAt(f).className + " " + documentListOfAllClasses.ElementAt(f).index);
                        if (classesOfDocument.ContainsKey(documentListOfAllClasses.ElementAt(f).className))
                        {
                            classesOfDocument[documentListOfAllClasses.ElementAt(f).className]++;
                        } 
                        else
                        {
                            classesOfDocument.Add(documentListOfAllClasses.ElementAt(f).className, 1);
                        }
                    }
                }
            }

            foreach (var val in classesOfDocument)
            {
                if (val.Value > numberOfReturnDocuments / 2)
                {
                    return val.Key;
                }
            }

            return "";
        }

        public static List<String> layChuoiCacDocuments(String input)
        {
            using (StreamReader sr = new StreamReader(input))
            {
                String line = null;
                List<String> lstr = new List<string>();

                while ((line = sr.ReadLine()) != null)
                {
                    lstr.Add(line);
                }

                return lstr;
            }
        }
    }
}
