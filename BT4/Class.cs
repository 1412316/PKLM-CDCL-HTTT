using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PhanLop
{
    class Class
    {
        public List<Document> documentList { get; set; }
        public String className { get; set; }

        public Class()
        {
            documentList = new List<Document>();
            className = "";
        }

        public void layCacDocuments(String filePath, String className)
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                String line = null;
                int j = 0; //Cho biết document thứ bao nhiêu trong documentList

                //Mỗi một line (dòng) là một document
                while ((line = sr.ReadLine()) != null)
                {
                    //Thêm một document rỗng vào danh sách documents
                    documentList.Add(new Document());
                    documentList.ElementAt(j).className = className;
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
        }
    }
}
