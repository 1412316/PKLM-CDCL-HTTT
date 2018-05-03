using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PhanLop
{
    class Program
    {
        static void Main(string[] args)
        {
            String preprocessedFilePath = "D:\\PhanLop\\bin\\Debug\\preprocessedClasses\\";
            String[] filePaths = Directory.GetFiles("D:\\PhanLop\\bin\\Debug\\classes", "*.txt");
            List<Class> classList = new List<Class>();

            for (int i = 0; i < filePaths.Length; i++) { 
                Class c = new Class();
                c.className = filePaths[i].Split('\\').Last().Split('.').First();
                TienXuLy.tienXuLy(filePaths[i], preprocessedFilePath + c.className + ".txt");
                c.layCacDocuments(preprocessedFilePath + c.className + ".txt", c.className);
                classList.Add(c);
            }

            // Gộp tất cả các document của các class lại thành 1 documentList
            List<Document> documentListOfAllClasses = Document.gopDocuments(classList);
            //Tính bow tfidf
            BOW.bow_tfdf(documentListOfAllClasses, "output.txt", "featureList.txt");

            // Tiền xử lý các document cần phân lớp
            TienXuLy.tienXuLy("test.txt", "preprocessedTest.txt");

            // Các document cần được phân lớp
            List<Document> testList = Document.layCacDocuments("preprocessedTest.txt");

            testList.ForEach(delegate (Document d)
            {
                d.className = Document.phanLop(d, XuLyFile.knn(), documentListOfAllClasses.Count, documentListOfAllClasses, classList);
                Console.WriteLine(d.className);
                Console.WriteLine(d.index);
            });

            List<String> testDocuments = Document.layChuoiCacDocuments("test.txt");

            using (StreamWriter sw = new StreamWriter("result.txt"))
            {
                for (int i = 0; i < testDocuments.Count; i++)
                {
                    testList.ForEach(delegate (Document d)
                    {
                        if (d.index == i)
                        {
                            sw.WriteLine(testDocuments.ElementAt(i) + "\tclass:" + d.className);
                        }
                    });
                }
            }
        }
    }
}
