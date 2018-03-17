using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace TienXuLy
{
    class Program
    {
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
                    List<Feature> fList = inFeatureList(featureList, documentList);
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

        public static void tienXuLy(String input, String output)
        {
            //System.IO.File.WriteAllText(@"C:\Users\Kim\Desktop\output.txt", string.Empty);
            //System.IO.File.WriteAllText("output.txt", string.Empty);

            EnglishPorter2Stemmer stemword = new EnglishPorter2Stemmer();

            string line = null;

            List<string> Texts = new List<string>();
            List<string> StopWords = new List<string>();

            bool flag;

            //Doc cac van ban tu file input
            using (StreamReader filein = new StreamReader(input))
            {
                while ((line = filein.ReadLine()) != null)
                {
                    Texts.Add(line);
                }
                filein.Close();
            }

            //Doc cac stop words tu file stop
            using (StreamReader filestop = new StreamReader("stop.txt"))
            {
                while ((line = filestop.ReadLine()) != null)
                {
                    StopWords.Add(line);
                }
                filestop.Close();
            }

            Console.WriteLine("===============Doc cac dong trong file input===============");
            foreach (string t in Texts)
            {
                Console.WriteLine(t);
            }

            Console.WriteLine("===============Doc cac dong trong file stop===============");
            foreach (string sw in StopWords)
            {
                Console.WriteLine(sw);
            }

            Console.WriteLine("===============Xu ly===============");

            //Xu ly co ban cho cac dong van ban
            for (int i = 0; i < Texts.Count(); i++)
            {
                Texts[i] = Texts[i].ToLower();
                Texts[i] = XoaKyTu(Texts[i]);
                Texts[i] = XoaTrang(Texts[i]);
            }

            //Xu ly co ban cho cac dong stop words
            for (int j = 0; j < StopWords.Count; j++)
            {
                StopWords[j] = StopWords[j].ToLower();
                StopWords[j] = XoaKyTu(StopWords[j]);
                StopWords[j] = XoaTrang(StopWords[j]);
                StopWords[j] = stemword.Stem(StopWords[j]).Value;
            }

            Console.WriteLine("=====Cac van ban sau khi xu ly=====");

            using (StreamWriter fileout = new StreamWriter(output))
            {
                foreach (string t in Texts)
                {
                    string[] temp = t.Split(' ');

                    for (int i = 0; i < temp.Length; i++)
                    {
                        flag = true;
                        temp[i] = stemword.Stem(temp[i]).Value;

                        foreach (string st in StopWords)
                        {
                            if (st == temp[i])
                                flag = false;
                        }

                        if (flag == true)
                        {
                            if (i == (temp.Length - 1))
                            {
                                Console.Write(temp[i]);
                                Console.WriteLine();
                                fileout.Write(temp[i]);
                                fileout.WriteLine();
                            }
                            else
                            {
                                Console.Write(temp[i] + " ");
                                fileout.Write(temp[i]);
                                fileout.Write(' ');
                            }
                        }
                    }
                }
                fileout.Close();
            }
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

        public static void timKiem(Document searchedDocument, int numberOfDocuments, String searchOutput)
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

            int numberOfLines = timSoLuongVanBan("output.txt");

            featureList.ForEach(delegate (Feature f)
            {
                double tf = (double)timWeightCuaFeatureTrongDocument(searchedDocument, f.feature) / timSoLanXuatHienNhieuNhat(searchedDocument);
                double idf = Math.Log10((double)numberOfLines / f.weight);
                searchedDocument_tfidf.featureList.Add(new Feature(f.feature, Math.Round(tf * idf, 4)));
            });

            searchedDocument_tfidf.featureList.ForEach(delegate (Feature f)
            {
                Console.Write(f.weight + "\t");
            });
        }

        public static void Main()
        {
            tienXuLy("input.txt", "output.txt");

            bow_tfdf("output.txt", "output2.txt", "featureList.txt", "round.txt");

            tienXuLy("searchInput.txt", "preprocessedSearchInput.txt");

            using (StreamReader sr = new StreamReader("preprocessedSearchInput.txt"))
            {
                List<Document> documentList = taoDocumentList(sr);

                timKiem(documentList.ElementAt(0),
                    Int32.Parse(documentList.ElementAt(1).featureList.ElementAt(0).feature),
                    "searchOutput.txt");
            }
        }
    }
}

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