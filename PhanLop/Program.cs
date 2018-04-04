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
            String preprocessedFilePath = "C:\\Users\\ThienPhu\\source\\repos\\Chuyên_Đề_Chọn_Lọc_Trong_HTTT\\PhanLop\\bin\\Debug\\preprocessedClasses\\";
            String[] filePaths = Directory.GetFiles("C:\\Users\\ThienPhu\\source\\repos\\Chuyên_Đề_Chọn_Lọc_Trong_HTTT\\PhanLop\\bin\\Debug\\classes", "*.txt");
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

            // Tiền xử lý các document cần phan lớp
            TienXuLy.tienXuLy("test.txt", "preprocessedTest.txt");

            // Các document cần được phân lớp
            List<Document> testList = Document.layCacDocuments("preprocessedTest.txt");

            testList.ForEach(delegate (Document d)
            {
                Document.phanLop(d, 4, documentListOfAllClasses.Count);
            });
        }
    }
}
