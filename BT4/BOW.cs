using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PhanLop
{
    class BOW
    {
        BOW()
        {

        }

        public static List<Feature> inFeatureList(String featureList, List<Document> documentList)
        {
            Console.WriteLine("======In Feature List======");
            try
            {
                int round_num = XuLyFile.docRound();
                int doc_length = documentList.Count;

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
                                Double IDF = 0;
                                Double round = 0;
                                int doc_count = Document.DemDocChuaI(f.feature, documentList);
                                IDF = Math.Log10((float)doc_length / (float)doc_count);
                                round = Math.Round(IDF, round_num);
                                sw.WriteLine(f.feature + " " + round);
                                //Console.WriteLine(f.feature + " " + round);
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

        public static void bow_tfdf(List<Document> documentList, String output, String featureList)
        {
            Console.WriteLine("======Tao BOW====");
            try
            {
                List<Document> TF_IDF_List = new List<Document>();
                // đọc lấy số làm tròn . 
                int round_num = XuLyFile.docRound();


                    //In danh sách features ra file (danh sách này chứa các feature duy nhất dù feature
                    //đó xuất hiện trong nhiều document khách nhau
                    //List<Feature> fList = inFeatureList(featureList, documentList);
                    List<Feature> fList = inFeatureList(featureList, documentList);
                    // doclength là số lượng văn bản trong file 
                    int doc_length = documentList.Count;

                    using (StreamWriter fileout = new StreamWriter(output))
                    {
                        documentList.ForEach(delegate (Document x)
                        {
                            TF_IDF_List.Add(new Document());
                            // tìm trọng số  lớn nhất trong 1 document
                            double max = Document.timSoLanXuatHienNhieuNhat(x);
                            fList.ForEach(delegate (Feature j)
                            {
                                //lấy số lượng văn bản có string j
                                int doc_count = Document.DemDocChuaI(j.feature, documentList);
                                double tf = 0;
                                Double IDF = 0;
                                Double TF_IDF = 0;
                                // Lấy trọng số của string j trong document  
                                double weigthj = Document.timWeightCuaFeatureTrongDocument(x, j.feature);
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
            catch (Exception e)
            {
                Console.WriteLine("Có lỗi trong lúc đọc file: " + e.Message);
            }
        }

        public static List<double[]> docBOW_tfidf()
        {
            using (StreamReader sr = new StreamReader("output.txt"))
            {
                List<double[]> bow_tfidf = new List<double[]>();
                String line = null;

                while ((line = sr.ReadLine()) != null)
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
    }
}
