using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace PhanLop
{
    class Program
    {
        static void Main(string[] args)
        {
            String preprocessedFilePath = "D:\\PhanLop\\PhanLop\\bin\\Debug\\preprocessedClasses\\";
            String[] filePaths = Directory.GetFiles("D:\\PhanLop\\PhanLop\\bin\\Debug\\classes", "*.txt");
            String splitPath = "D:\\PhanLop\\PhanLop\\bin\\Debug\\splitFiles\\"; // Đường dẫn chứa các file train và text

            // Chọn chế độ thực thi
            Console.WriteLine("====Nhap so 1 đe chon che do 1====");
            Console.WriteLine("====Nhap so 2 đe chon che do 2====");
            Console.WriteLine("Nhap vao lua chon: ");
            string chedo = Console.ReadLine();
            int chedonum = Int32.Parse(chedo);

            if (chedonum == 1)
            {
                //Nhập lựa chọn
                Console.WriteLine("====Nhap so 0 đe phan lop theo Euclidean Similarity====");
                Console.WriteLine("====Nhap so 1 đe phan lop theo Cosine Similarity====");
                Console.WriteLine("Nhap vao lua chon: ");
                string choice = Console.ReadLine();
                int choicenum = Int32.Parse(choice);

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

                // Tạo một file text đã xóa các class trước khi chạy knn
                DocumentDaPhanLop.removeClassDocument("true.txt", "test.txt");

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

                using (StreamWriter sw = new StreamWriter("temp.txt"))
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
                List<ClassInfo> listClass = ClassInfo.ThongTinClass("true.txt", "temp.txt", filePaths);

                using (StreamWriter sw = new StreamWriter("result.txt"))
                {
                    // Ghi lại sersult và class vào file
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

                    // Ghi thông tin các độ đo 
                    for (int k = 0; k < listClass.Count; k++)
                    {
                        //Ghi ra file infoclass
                        sw.WriteLine("P(" + listClass[k].ClassName + ") = " + listClass[k].Precision);
                        sw.WriteLine("R(" + listClass[k].ClassName + ") = " + listClass[k].Recall);
                        sw.WriteLine("F-score(" + listClass[k].ClassName + ") = " + listClass[k].Fscore);
                    }

                    double f_macro = ClassInfo.F_Macro(listClass);
                    double f_micro = ClassInfo.F_Micro(listClass, DocumentDaPhanLop.demSoDocument("true.txt"));

                    //Ghi ra file infoclass
                    sw.WriteLine("F-macro = " + f_macro);
                    sw.WriteLine("F-micro = " + f_micro);

                }
                File.Delete("temp.txt");
                Console.WriteLine("Hoan tat!!!");
            }

            if (chedonum == 2)
            {
                //Nhập lựa chọn
                Console.WriteLine("====Nhap so 0 đe phan lop theo Euclidean Similarity====");
                Console.WriteLine("====Nhap so 1 đe phan lop theo Cosine Similarity====");
                Console.WriteLine("Nhap vao lua chon: ");
                string choice = Console.ReadLine();
                int choicenum = Int32.Parse(choice);

                // Chia file
                DocumentDaPhanLop.chiaFile(filePaths, splitPath);

                //=========================================Phần xử lý các document=========================================
                // File resultfinal.txt sẽ lưu lại các lần tính otan1 và trung bình F-macro, F-micro
                using (StreamWriter final = new StreamWriter(splitPath + "resultfinal.txt"))
                {
                    double temp_F_macro = 0;
                    double temp_F_micro = 0;

                    //Dùng phương pháp cross-validation lấy 1 file làm Test, 9 file làm Train
                    for (int t = 1; t <= XuLyFile.k(); t++)
                    {
                        // Truyền chỉ số t = bao nhiêu thì các file có số != t sẽ làm train
                        DocumentDaPhanLop.taoTrainClasstxt(t, splitPath, filePaths);

                        List<Class> classList = new List<Class>();

                        for (int i = 0; i < filePaths.Length; i++)
                        {
                            Class c = new Class();
                            c.className = filePaths[i].Split('\\').Last().Split('.').First();
                            TienXuLy.tienXuLy(splitPath + c.className + ".txt", preprocessedFilePath + c.className + ".txt");
                            c.layCacDocuments(preprocessedFilePath + c.className + ".txt", c.className);
                            classList.Add(c);
                        }

                        // Gộp tất cả các document của các class lại thành 1 documentList
                        List<Document> documentListOfAllClasses = Document.gopDocuments(classList);
                        //Tính bow tfidf
                        BOW.bow_tfdf(documentListOfAllClasses, "output.txt", "featureList.txt");

                        DocumentDaPhanLop.removeClassDocument(splitPath + "file" + t + ".txt", "test.txt");

                        // Tiền xử lý các document cần phân lớp
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
                                Console.WriteLine();
                            });
                        }

                        if (choicenum == 1)
                        {
                            testList.ForEach(delegate (Document d)
                            {
                                d.className = Document.phanLopCosine(d, XuLyFile.knn(), documentListOfAllClasses.Count, documentListOfAllClasses, classList);
                                Console.WriteLine(d.className);
                                Console.WriteLine(d.index);
                                Console.WriteLine();
                            });
                        }

                        List<String> testDocuments = Document.layChuoiCacDocuments("test.txt");

                        using (StreamWriter sw = new StreamWriter(splitPath + "result" + t + ".txt"))
                        {
                            for (int i = 0; i < testDocuments.Count; i++)
                            {
                                for (int k = 0; k < testList.Count; k++)
                                {
                                    if (testList[k].index == i)
                                    {
                                        if (i != testDocuments.Count - 1)
                                            sw.WriteLine(testDocuments.ElementAt(i) + "  " + testList[k].className);
                                        else
                                            sw.Write(testDocuments.ElementAt(i) + "  " + testList[k].className);
                                    }
                                }
                            }
                        }

                        //=========================================Phần tính thông tin của các class=========================================
                        List<ClassInfo> listClass = ClassInfo.ThongTinClass(splitPath + "file" + t + ".txt", splitPath + "result" + t + ".txt", filePaths);

                        /*final.WriteLine("====================File" + t + "====================");
                        for (int k = 0; k < listClass.Count; k++)
                        {
                            //Ghi ra file
                            final.WriteLine("==========Class " + k + "==========");
                            final.WriteLine("Ten class: " + listClass[k].ClassName);
                            final.WriteLine("So van ban duoc phan dung vao class " + listClass[k].ClassName + ": " + listClass[k].NumberRightofClass);
                            final.WriteLine("So van ban bi phan sai vao class " + listClass[k].ClassName + ": " + listClass[k].NumberWrongofClass);
                            final.WriteLine("So van ban class " + listClass[k].ClassName + " bi phan sai vao class khac: " + listClass[k].NumberWrongoutClass);
                            final.WriteLine("Do chinh xac cua class " + listClass[k].ClassName + ": " + listClass[k].Precision);
                            final.WriteLine("Do bao phu cua class " + listClass[k].ClassName + ": " + listClass[k].Recall);
                            final.WriteLine("Do do tong hop cua class " + listClass[k].ClassName + ": " + listClass[k].Fscore);
                            final.WriteLine();
                        }*/

                        double f_macro = ClassInfo.F_Macro(listClass);
                        double f_micro = ClassInfo.F_Micro(listClass, DocumentDaPhanLop.demSoDocument(splitPath + "file" + t + ".txt"));

                        temp_F_macro += f_macro;
                        temp_F_micro += f_micro;

                        final.WriteLine("F-macro[" + t + "] = " + f_macro);
                        final.WriteLine("F-micro[" + t + "] = " + f_micro);
                        final.WriteLine();
                    }

                    double avg_F_macro = Math.Round(temp_F_macro / XuLyFile.k(), XuLyFile.docRound());
                    double avg_F_micro = Math.Round(temp_F_micro / XuLyFile.k(), XuLyFile.docRound());

                    final.WriteLine("Trung binh F-macro = " + avg_F_macro);
                    final.WriteLine("Trung binh F-micro = " + avg_F_macro);
                }
                Console.WriteLine("Hoan tat!!!");
            }
        }
    }
}
