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
            //Nhập lựa chọn
            Console.WriteLine("====Nhap so 0 đe phan lop theo Euclidean Similarity====");
            Console.WriteLine("====Nhap so 1 đe phan lop theo Cosine Similarity====");
            Console.WriteLine("Nhap vao lua chon: ");
            string choice = Console.ReadLine();
            int choicenum = Int32.Parse(choice);

            String preprocessedFilePath = "C:\\Users\\nhkim\\Documents\\Visual Studio 2017\\Projects\\PhanLop\\PhanLop\\bin\\Debug\\preprocessedClasses\\";
            String[] filePaths = Directory.GetFiles("C:\\Users\\nhkim\\Documents\\Visual Studio 2017\\Projects\\PhanLop\\PhanLop\\bin\\Debug\\classes", "*.txt");
            List<Class> classList = new List<Class>();

            for (int i = 0; i < filePaths.Length; i++)
            {
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

            if (choicenum == 0)
            {
                testList.ForEach(delegate (Document d)
                {
                    d.className = Document.phanLop(d, XuLyFile.knn(), documentListOfAllClasses.Count, documentListOfAllClasses, classList);
                    Console.WriteLine(d.className);
                    Console.WriteLine(d.index);
                });
            }

            if (choicenum == 1)
            {
                testList.ForEach(delegate (Document d)
                {
                    d.className = Document.phanLopCosine(d, XuLyFile.knn(), documentListOfAllClasses.Count, documentListOfAllClasses, classList);
                    Console.WriteLine(d.className);
                    Console.WriteLine(d.index);
                });
            }

            List<String> testDocuments = Document.layChuoiCacDocuments("test.txt");

            using (StreamWriter sw = new StreamWriter("result.txt"))
            {
                for (int i = 0; i < testDocuments.Count; i++)
                {
                    testList.ForEach(delegate (Document d)
                    {
                        if (d.index == i)
                        {
                            sw.WriteLine(testDocuments.ElementAt(i) + "  " + d.className);
                        }
                    });
                }
            }

            //=========================================Phần tính thông tin của các class=========================================
            List<ClassInfo> listClass = ClassInfo.ThongTinClass("true.txt", "result.txt");

            for (int k = 0; k < listClass.Count; k++)
            {
                Console.WriteLine("==========Class " + k + "==========");
                Console.WriteLine("Ten class: " + listClass[k].ClassName);
                Console.WriteLine("So van ban duoc phan dung vao class " + listClass[k].ClassName + ": " + listClass[k].NumberRightofClass);
                Console.WriteLine("So van ban bi phan sai vao class " + listClass[k].ClassName + ": " + listClass[k].NumberWrongofClass);
                Console.WriteLine("So van ban class " + listClass[k].ClassName + " bi phan sai vao class khac: " + listClass[k].NumberWrongoutClass);
                Console.WriteLine("Do chinh xac cua class " + listClass[k].ClassName + ": " + listClass[k].Precision);
                Console.WriteLine("Do bao phu cua class " + listClass[k].ClassName + ": " + listClass[k].Recall);
                Console.WriteLine("Do do tong hop cua class " + listClass[k].ClassName + ": " + listClass[k].Fscore);
                Console.WriteLine();
            }

            double f_macro = ClassInfo.F_Macro(listClass);
            double f_micro = ClassInfo.F_Micro(listClass, Document.demSoDocument("true.txt"));

            Console.WriteLine("Do do F-macro: " + f_macro);
            Console.WriteLine("Do do F-micro: " + f_micro);

        }
    }
}
