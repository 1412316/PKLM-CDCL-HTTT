using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace TienXuLy
{
    class Program
    {
        //========================================Phần xử lý TF-IDF========================================
        //Đọc và tạo danh sách các document. Moi mot phan tu la 1 dong trong file text
        public static List<Document> taoDocumentList(StreamReader sr)
        {
            String line = null;
            List<Document> documentList = new List<Document>();
            int j = 0; //Cho biết document thứ bao nhiêu trong documentList

            //Mỗi một line (dòng) là một document
            while ((line = sr.ReadLine()) != null)
            {
                //Thêm một document rỗng vào danh sách documents
                documentList.Add(new Document());

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

            //Trả về danh sách documents
            return documentList;
        }

        //Hàm in danh sách features xuống file. Liet ke cac feature co trong file 
        public static List<Feature> inFeatureList(String featureList, List<Document> documentList)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(featureList))
                {
                    //Đây là danh sách các feature trong tất cả các document (một feature nằm trong 
                    //nhiều documents thì cũng chỉ có một phần tử trong danh sách features này
                    List<Feature> features = new List<Feature>();

                    //Với mỗi document
                    documentList.ForEach(delegate (Document d)
                    {
                        //Với mỗi feature trong document đó
                        d.featureList.ForEach(delegate (Feature f)
                        {
                            int exist = 0;

                            //Tìm xem trong danh sách các feature đã có feature nay chưa
                            foreach (Feature feature in features)
                            {
                                //Nếu có rồi thì ghi nhận đã exist (=1)
                                if (feature.feature == f.feature)
                                {
                                    exist = 1;
                                    feature.weight++;
                                    break;
                                }
                            }

                            //Nếu chưa tồn tại feature này trong danh sách features thì thêm vào
                            if (exist == 0)
                            {
                                features.Add(new Feature(f.feature, 1));
                            }
                        });
                    });

                    //Với mỗi feature in ra file
                    foreach (Feature feature in features)
                    {
                        sw.WriteLine(feature.feature + " " + feature.weight);
                    }

                    return features;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Có lỗi trong lúc viết file: " + e.Message);
                return null;
            }
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

        //Tìm feature xuất hiện nhiều nhất trong Dj 
        public static double TinhMaxDj(Document a)
        {
            double max_temp = 0;
            a.featureList.ForEach(delegate (Feature i)
            {
                if (i.weight > max_temp)
                {
                    max_temp = i.weight;
                }
            });
            return max_temp;
        }

        public void PrintOutPut(List<Document> TF_IDF, string ouput)
        {
            using (StreamWriter sw = new StreamWriter(ouput))
            {
                TF_IDF.ForEach(delegate (Document x)
                {
                    x.featureList.ForEach(delegate (Feature y)
                    {
                        sw.Write(y.weight);
                        sw.Write("\t");

                    });
                    sw.Write("\n");
                });
                sw.Close();
            }
        }

        public static double getweigth(Document d, string a)
        {
            double weight = 0;
            d.featureList.ForEach(delegate (Feature x)
            {
                if (x.feature == a)
                {
                    weight = x.weight;
                }
            });
            return weight;
        }

        public static void bow_tfdf(String input, String output, String featureList, string round)
        {
            try
            {
                //List chứa các trị số mới TF_IDF
                int round_num = 0;
                List<Document> TF_IDF_List = new List<Document>();
                // đọc lấy số làm tròn . 
                using (StreamReader fileround = new StreamReader(round))
                {
                    round_num = Int32.Parse(fileround.ReadLine().ToString());
                }
                using (StreamReader sr = new StreamReader(input))
                {
                    //Tạo danh sách các features theo các documents
                    List<Document> documentList = taoDocumentList(sr);
                    //In ra màn hình xem thử kết quả
                    String final = "";
                    documentList.ForEach(delegate (Document d)
                    {
                        d.featureList.ForEach(delegate (Feature f)
                        {
                            final += f.feature + f.weight + " ";
                        });

                        final += "\n";
                    });

                    Console.WriteLine(final);
                    //In danh sách features ra file (danh sách này chứa các feature duy nhất dù feature
                    //đó xuất hiện trong nhiều document khách nhau
                    //List<Feature> fList = inFeatureList(featureList, documentList);
                    List<Feature> fList = inFeatureList2(featureList, documentList);
                    // doclength là số lượng văn bản trong file 
                    int doc_length = documentList.Count;

                    using (StreamWriter fileout = new StreamWriter(output))
                    {
                        documentList.ForEach(delegate (Document x)
                        {

                            TF_IDF_List.Add(new Document());
                            // tìm trọng số  lớn nhất trong 1 document
                            double max = TinhMaxDj(x);
                            fList.ForEach(delegate (Feature j)
                            {
                                //lấy số lượng văn bản có string j
                                int doc_count = DemDocChuaI(j.feature, documentList);
                                double tf = 0;
                                Double IDF = 0;
                                Double TF_IDF = 0;
                                // Lấy trọng số của string j trong document  
                                double weigthj = getweigth(x, j.feature);
                                tf = weigthj / max;
                                IDF = Math.Log10((float)doc_length / (float)doc_count);
                                TF_IDF = Math.Round(tf * IDF, round_num);
                                fileout.Write(TF_IDF);
                                fileout.Write('\t');
                            });

                            fileout.WriteLine();
                        });

                        fileout.Close();
                        Console.WriteLine("Thao tác thành công!!");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Có lỗi trong lúc đọc file: " + e.Message);
            }
        }

        //========================================Phần tiền xử lý văn bản========================================
        public static string XoaTrang(string str)
        {
            //Xoa trang thua o dau va cuoi chuoi
            str = str.Trim();

            //Xoa trang thua o giua cac ky tu
            while (str.IndexOf("  ") >= 0)
            {
                str = str.Replace("  ", " ");
            }
            return str;
        }

        public static string XoaKyTu(string str)
        {
            str = Regex.Replace(str, @"[^\w\s]", "");
            return str;
        }

        public static string BoStopWords(string str)
        {
            string stwline = null;
            string templine = null;

            List<string> StopWords = new List<string>();

            bool flag;

            //Doc cac stop words tu file stop
            using (StreamReader filestop = new StreamReader("stop.txt"))
            {
                while ((stwline = filestop.ReadLine()) != null)
                {
                    StopWords.Add(stwline);
                }
                filestop.Close();
            }

            //Xu ly co ban cho cac dong stop words
            for (int j = 0; j < StopWords.Count; j++)
            {
                StopWords[j] = StopWords[j].ToLower();
                StopWords[j] = XoaKyTu(StopWords[j]);
                StopWords[j] = XoaTrang(StopWords[j]);
            }

            string[] temp = str.Split(' ');
            for (int i = 0; i < temp.Length; i++)
            {
                flag = true;
                foreach (string st in StopWords)
                {
                    if (st == temp[i])
                        flag = false;
                }

                if (flag == true)
                {
                    if (i == (temp.Length - 1))
                        templine = templine + temp[i];
                    else
                        templine = templine + temp[i] + " ";
                }
            }
            return XoaTrang(templine);
        }

        public static string PorterStemming(string str)
        {
            EnglishPorter2Stemmer stemword = new EnglishPorter2Stemmer();
            string templine = null;

            List<string> StopWords = new List<string>();

            string[] temp = str.Split(' ');
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = stemword.Stem(temp[i]).Value;
                if (i == (temp.Length - 1))
                    templine = templine + temp[i];
                else
                    templine = templine + temp[i] + " ";
            }
            return templine;
        }

        public static void tienXuLy(string input, string output)
        {
            EnglishPorter2Stemmer stemword = new EnglishPorter2Stemmer();

            string line = null;

            List<string> Texts = new List<string>();

            //Doc cac van ban tu file input
            using (StreamReader filein = new StreamReader(input))
            {
                while ((line = filein.ReadLine()) != null)
                {
                    Texts.Add(line);
                }
                filein.Close();
            }

            Console.WriteLine("===============Cac dong trong file input===============");
            foreach (string t in Texts)
            {
                Console.WriteLine(t);
            }

            Console.WriteLine("===============Cac dong trong file output===============");
            using (StreamWriter fileout = new StreamWriter(output))
            {
                for (int i = 0; i < Texts.Count(); i++)
                {
                    Texts[i] = Texts[i].ToLower();
                    Texts[i] = XoaKyTu(Texts[i]);
                    Texts[i] = XoaTrang(Texts[i]);
                    Texts[i] = BoStopWords(Texts[i]);
                    //Texts[i] = PorterStemming(Texts[i]);

                    if (i == (Texts.Count - 1))
                    {
                        Console.WriteLine(Texts[i]);
                        fileout.Write(Texts[i]);
                    }
                    else
                    {
                        Console.WriteLine(Texts[i]);
                        fileout.WriteLine(Texts[i]);
                    }
                }
                fileout.Close();
            }
        }

        //========================================Phần xử lý tìm kiếm văn bản========================================
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

        public static int timSoLuongVanBan(String input)
        {
            String line = null;
            int numberOfLines = 0;
            using (StreamReader sr = new StreamReader(input))
            {
                while((line = sr.ReadLine()) != null)
                {
                    numberOfLines++;
                }
            }

            return numberOfLines;
        }

        public static int docRound()
        {
            using (StreamReader sr = new StreamReader("Round.txt"))
            {
                return Int32.Parse(sr.ReadLine());
            }
        }

        public static List<double[]> docBOW_tfidf()
        {
            using (StreamReader sr = new StreamReader("output2.txt"))
            {
                List<double[]> bow_tfidf = new List<double[]>();
                String line = null;
                
                while((line = sr.ReadLine()) != null)
                {
                    String[] str = line.Split('\t');
                    double[] d = new double[str.Length];
                    int i = 0;

                    for (i = 0; i < str.Length - 1; i++)
                    {
                        d[i] = Double.Parse(str[i]);
                    }

                    bow_tfidf.Add(d);
                }

                return bow_tfidf;
            }
        }

        public static List<String> layDocuments()
        {
            using (StreamReader sr = new StreamReader("dataSearch.txt"))
            {
                String line = null;
                List<String> lstr = new List<string>();

                while((line = sr.ReadLine()) != null)
                {
                    lstr.Add(line);
                }

                return lstr;
            }
        }

        public static void timKiem(Document searchedDocument, int numberOfReturnDocuments, String searchOutput)
        {
            Document searchedDocument_tfidf = new Document();
            List<Feature> featureList = new List<Feature>();

            using (StreamReader sr = new StreamReader("featureList.txt"))
            {
                String line = null;
                while((line = sr.ReadLine()) != null)
                {
                    String[] feature = line.Split(' ');

                    featureList.Add(new Feature(feature[0], Int32.Parse(feature[1])));
                }
            }

            int numberOfDocuments = timSoLuongVanBan("output.txt");

            featureList.ForEach(delegate (Feature f)
            {
                double tf = (double)timWeightCuaFeatureTrongDocument(searchedDocument, f.feature) / timSoLanXuatHienNhieuNhat(searchedDocument);
                double idf = Math.Log10((double)numberOfDocuments / f.weight);
                searchedDocument_tfidf.featureList.Add(new Feature(f.feature, Math.Round(tf * idf, docRound())));
            });

            searchedDocument_tfidf.featureList.ForEach(delegate (Feature f)
            {
                Console.Write(f.weight + "\t");
            });

            List<double[]> bow_tfidf = docBOW_tfidf();
            SimilarityMeasure[] similarityMeasures = new SimilarityMeasure[numberOfDocuments];
            int i, j;

            for (i = 0; i < numberOfDocuments; i++)
            {
                double value = 0;

                for (j = 0; j < searchedDocument_tfidf.featureList.Count; j++)
                {
                    value += Math.Pow(bow_tfidf.ElementAt(i)[j] - searchedDocument_tfidf.featureList.ElementAt(j).weight, 2);
                }

                similarityMeasures[i] = new SimilarityMeasure(Math.Sqrt(value), i);
            }

            SimilarityMeasure[] sortedSimilarityMeasures = similarityMeasures.OrderBy(sm => sm.value).ToArray();

            List<String> lstr = layDocuments();

            using (StreamWriter sw = new StreamWriter("searchOutput.txt"))
            {
                for (int k = 0; k < numberOfReturnDocuments; k++)
                {
                    sw.WriteLine(lstr.ElementAt(sortedSimilarityMeasures[k].index));
                }
            }
        }

        public static List<Feature> inFeatureList2(String featureList, List<Document> documentList)
        {
            try
            {
                int round_num = 0;
                int doc_length = documentList.Count;

                using (StreamReader fileround = new StreamReader("round.txt"))
                {
                    round_num = Int32.Parse(fileround.ReadLine().ToString());
                }

                using (StreamWriter sw = new StreamWriter(featureList))
                {
                    //Đây là danh sách các feature trong tất cả các document (một feature nằm trong 
                    //nhiều documents thì cũng chỉ có một phần tử trong danh sách features này
                    List<Feature> features = new List<Feature>();

                    //Với mỗi document
                    documentList.ForEach(delegate(Document d)
                    {
                        //Với mỗi feature trong document đó
                        d.featureList.ForEach(delegate(Feature f)
                        {
                            int exist = 0;

                            //Tìm xem trong danh sách các feature đã có feature nay chưa
                            foreach (Feature feature in features)
                            {
                                //Nếu có rồi thì ghi nhận đã exist (=1)
                                if (feature.feature == f.feature)
                                {
                                    exist = 1;
                                    feature.weight++;
                                    break;
                                }
                            }

                            //Nếu chưa tồn tại feature này trong danh sách features thì thêm vào
                            if (exist == 0)
                            {
                                features.Add(new Feature(f.feature, 1));
                                Double IDF = 0;
                                Double round = 0;
                                int doc_count = DemDocChuaI(f.feature, documentList);
                                IDF = Math.Log10((float)doc_length / (float)doc_count);
                                round = Math.Round(IDF, round_num);
                                sw.WriteLine(f.feature + " " + round);
                                Console.WriteLine(f.feature + " " + round);
                            }
                        });
                    });

                    //Với mỗi feature in ra file
                    //foreach (String feature in features)
                    //{
                    //    sw.WriteLine(feature);
                    //    Console.WriteLine(feature);
                    //}

                    return features;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Có lỗi trong lúc viết file: " + e.Message);
                return null;
            }
        }

        public static void timKiem2(Document searchedDocument, int numberOfReturnDocuments, String searchOutput)
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

            int numberOfDocuments = timSoLuongVanBan("output.txt");

            featureList.ForEach(delegate(FeatureIDF f)
            {
                double tf = (double)timWeightCuaFeatureTrongDocument(searchedDocument, f.feature) / timSoLanXuatHienNhieuNhat(searchedDocument);
                //double idf = Math.Log10((double)numberOfDocuments / f.weight);
                searchedDocument_tfidf.featureList.Add(new Feature(f.feature, Math.Round(tf * f.idf, docRound())));
            });

            //searchedDocument_tfidf.featureList.ForEach(delegate(Feature f)
            //{
            //    Console.Write(f.weight + "\t");
            //});

            List<double[]> bow_tfidf = docBOW_tfidf();
            SimilarityMeasure[] similarityMeasures = new SimilarityMeasure[numberOfDocuments];
            int i, j;

            for (i = 0; i < numberOfDocuments; i++)
            {
                double value = 0;

                for (j = 0; j < searchedDocument_tfidf.featureList.Count; j++)
                {
                    value += Math.Pow(bow_tfidf.ElementAt(i)[j] - searchedDocument_tfidf.featureList.ElementAt(j).weight, 2);
                }

                similarityMeasures[i] = new SimilarityMeasure(Math.Round(Math.Sqrt(value), docRound()), i);
            }

            SimilarityMeasure[] sortedSimilarityMeasures = similarityMeasures.OrderBy(sm => sm.value).ToArray();

            List<String> lstr = layDocuments();

            using (StreamWriter sw = new StreamWriter("searchOutput.txt"))
            {
                for (int k = 0; k < numberOfReturnDocuments; k++)
                {
                    sw.WriteLine(lstr.ElementAt(sortedSimilarityMeasures[k].index) + "\t{0}", sortedSimilarityMeasures[k].value);
                }
            }
        }

        //========================================Phần gọi hàm main========================================
        public static void Main()
        {
            tienXuLy("dataSearch.txt", "output.txt");
            
            bow_tfdf("output.txt", "output2.txt", "featureList.txt", "round.txt");
            
            tienXuLy("searchInput.txt", "preprocessedSearchInput.txt");

            using (StreamReader sr = new StreamReader("preprocessedSearchInput.txt"))
            {
                List<Document> documentList = taoDocumentList(sr);
                timKiem2(documentList.ElementAt(0),
                    Int32.Parse(documentList.ElementAt(1).featureList.ElementAt(0).feature),
                    "searchOutput.txt");
            }

        }
    }
}

//========================================Phần tạo các class cần thiết để sử dụng========================================
public class Document
{
    public List<Feature> featureList { get; set; }

    public Document()
    {
        this.featureList = new List<Feature>();
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

}

public class Feature
{
    public String feature { get; set; }
    public double weight { get; set; }

    public void tangWeight()
    {
        this.weight++;
    }

    public Feature()
    {
        this.feature = "";
        this.weight = 0;
    }

    public Feature(String feature, double weight)
    {
        this.feature = feature;
        this.weight = weight;
    }
}

public class SimilarityMeasure
{
    public double value { get; set; }
    public int index { get; set; }

    public SimilarityMeasure(double value, int index)
    {
        this.value = value;
        this.index = index;
    }
}

public class FeatureIDF
{
    public String feature { get; set; }
    public double idf { get; set; }

    public FeatureIDF()
    {
        this.feature = "";
        this.idf = 0;
    }

    public FeatureIDF(String feature, double idf)
    {
        this.feature = feature;
        this.idf = idf;
    }
}