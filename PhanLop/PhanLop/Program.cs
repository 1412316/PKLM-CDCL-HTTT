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
            //Nhập lựa chọn
            Console.WriteLine("====Nhap so 0 đe phan lop theo Euclidean Similarity====");
            Console.WriteLine("====Nhap so 1 đe phan lop theo Cosine Similarity====");
            Console.WriteLine("Nhap vao lua chon: ");
            string choice = Console.ReadLine();
            int choicenum = Int32.Parse(choice);

            String preprocessedFilePath = "C:\\Users\\nhkim\\Documents\\Visual Studio 2017\\Projects\\PhanLop\\PhanLop\\bin\\Debug\\preprocessedClasses\\";
            String[] filePaths = Directory.GetFiles("C:\\Users\\nhkim\\Documents\\Visual Studio 2017\\Projects\\PhanLop\\PhanLop\\bin\\Debug\\classes", "*.txt");
            String splitPath = "C:\\Users\\nhkim\\Documents\\Visual Studio 2017\\Projects\\PhanLop\\PhanLop\\bin\\Debug\\splitFiles\\"; // Đường dẫn chứa các file train và text

            // Chia file
            DocumentDaPhanLop.chiaFile(filePaths, splitPath);

            //=========================================Phần xử lý các document=========================================
            // File resultfinal.txt sẽ lưu lại các lần tính otan1 và trung bình F-macro, F-micro
            using (StreamWriter final = new StreamWriter(splitPath + "resultfinal.txt"))
            {
                double temp_F_macro = 0;
                double temp_F_micro = 0;

                //Dùng phương pháp cross-validation lấy 1 file làm Test, 9 file làm Train
                for (int t = 1; t <= 10; t++)
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

                    double f_macro = ClassInfo.F_Macro(listClass);
                    double f_micro = ClassInfo.F_Micro(listClass, DocumentDaPhanLop.demSoDocument(splitPath + "file" + t + ".txt"));

                    temp_F_macro += f_macro;
                    temp_F_micro += f_micro;

                    final.WriteLine("F-macro[" + t + "] = " + f_macro);
                    final.WriteLine("F-micro[" + t + "] = " + f_micro);
                    final.WriteLine();
                }

                double avg_F_macro = Math.Round(temp_F_macro / 10, XuLyFile.docRound());
                double avg_F_micro = Math.Round(temp_F_micro / 10, XuLyFile.docRound());

                final.WriteLine("Trung binh F-macro = " + avg_F_macro);
                final.WriteLine("Trung binh F-micro = " + avg_F_macro);
            }
            Console.WriteLine("Hoan tat!!!");
        }
    }
}
