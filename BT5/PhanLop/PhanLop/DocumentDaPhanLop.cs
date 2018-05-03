using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PhanLop
{
    // Class này dùng để chứa các document có gắn class sẵn
    class DocumentDaPhanLop
    {
        public string content { get; set; } // content là 1 document
        public string className { get; set; }

        public DocumentDaPhanLop(string content, string classname)
        {
            this.content = content;
            this.className = classname;
        }

        // Tạo list các document được gắn sẵn class trong file text
        public static List<DocumentDaPhanLop> layCacDocumentDaPhanLop(String input)
        {
            // List chứa tất cả document đọc từ file text 
            List<DocumentDaPhanLop> listofalldocument = new List<DocumentDaPhanLop>();
            // Chứa 1 document trong quá trình đọc vào
            string line = null;

            using (StreamReader sr = new StreamReader(input))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    string[] temp = line.Split(' '); // Cắt từng từ bỏ vào mảng string
                    StringBuilder build = new StringBuilder();
                    for (int j = 0; j < temp.Length; j++)
                    {
                        // Tạo document từ các phần tử của mảng
                        // Phần tử cuối mảng sẽ là class của document nên ko thêm vào StringBuilder
                        if (j != temp.Length - 1)
                        {
                            if (j == temp.Length - 2)
                                build.Append(temp[j]);
                            else
                                build.Append(temp[j] + " ");
                        }
                    }
                    // Thêm 1 document vào list bằng constructor của nó
                    listofalldocument.Add(new DocumentDaPhanLop(build.ToString().Trim(), temp[temp.Length - 1]));
                }
                return listofalldocument;
            }
        }

        // Tạo ra một file text gồm document đã xóa các class
        public static void removeClassDocument(String input, String output)
        {
            List<string> listofalldocument = new List<string>();
            string line = null;

            using (StreamReader sr = new StreamReader(input))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    listofalldocument.Add(line);
                }
            }

            using (StreamWriter sw = new StreamWriter(output))
            {
                for (int i = 0; i < listofalldocument.Count; i++)
                {
                    string[] temp = listofalldocument[i].Split(' ');
                    StringBuilder build = new StringBuilder();
                    for (int j = 0; j < temp.Length; j++)
                    {
                        if (j != temp.Length - 1)
                        {
                            if (j == temp.Length - 2)
                                build.Append(temp[j]);
                            else
                                build.Append(temp[j] + " ");
                        }
                    }
                    sw.WriteLine(build.ToString().Trim());
                }
            }
        }

        // Đếm số document trong file text
        public static int demSoDocument(String input)
        {
            List<DocumentDaPhanLop> listDocument = DocumentDaPhanLop.layCacDocumentDaPhanLop(input);
            return listDocument.Count;
        }

        // Hàm tạo lại các file class.txt từ 9 file train
        public static void taoTrainClasstxt(int chiso, String splitPath, String[] className)
        {
            List<DocumentDaPhanLop> listalldocument = new List<DocumentDaPhanLop>();
            int k = XuLyFile.k();

            for (int i = 1; i <= k; i++)
            {
                if (i != chiso)
                {
                    List<DocumentDaPhanLop> listtemp = DocumentDaPhanLop.layCacDocumentDaPhanLop(splitPath + "file" + i + ".txt");
                    for (int j = 0; j < listtemp.Count; j++)
                        listalldocument.Add(listtemp[j]);
                }
            }

            for (int i = 0; i < className.Length; i++)
            {
                className[i] = className[i].Split('\\').Last().Split('.').First();
                using (StreamWriter sw = new StreamWriter(splitPath + className[i] + ".txt"))
                {
                    for (int j = 0; j < listalldocument.Count; j++)
                    {
                        if (listalldocument[j].className == className[i])
                            sw.WriteLine(listalldocument[j].content);
                    }
                }
                Console.WriteLine("Tao file class " + className[i] + ".txt thanh cong");
            }
        }


        // Hàm dùng để ghi các phần tử tại 1 khoảng nhất định (begin, end) vào file 
        public static void SplitMember(int begin, int end, List<string> listalldoc, String file)
        {
            using (StreamWriter sw = new StreamWriter(file))
            {
                for (int i = begin; i < end; i++)
                {
                    if (i != end - 1)
                        sw.WriteLine(listalldoc[i]);
                    else
                        sw.Write(listalldoc[i]);
                }

            }
        }

        // Hàm để chia 1 file text lớn thành các file text nhỏ
        public static void SplitText(String sourcefile, String splitPath, int sofile)
        {
            List<string> listalldoc = new List<string>();
            string line = null;
            using (StreamReader sr = new StreamReader(sourcefile))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    listalldoc.Add(line);
                }
                sr.Close();
            }

            float count = listalldoc.Count;
            float k = (float)sofile;
            double result = Math.Round((double)(count / k), 0);
            //Console.WriteLine(result);
            int numdocoffile = (int)result;

            for (int i = 0; i < k; i++)
            {
                if (i != k - 1)
                    SplitMember(i * numdocoffile, (i + 1) * numdocoffile, listalldoc, splitPath + "file" + (i + 1) + ".txt");
                if (i == k - 1)
                    SplitMember(i * numdocoffile, listalldoc.Count, listalldoc, splitPath + "file" + (i + 1) + ".txt");
                //Console.WriteLine("Hoan tat file" + (i + 1) + ".txt");
            }
        }

        // Hàm chia file 
        public static void chiaFile(String[] filePaths, String splitPath)
        {
            List<DocumentDaPhanLop> listofalldocument = new List<DocumentDaPhanLop>();
            string line = null;

            // Tạo list các document của các class (Document chưa tienXuLy)
            for (int i = 0; i < filePaths.Length; i++)
            {
                using (StreamReader sr = new StreamReader(filePaths[i]))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        listofalldocument.Add(new DocumentDaPhanLop(line, filePaths[i].Split('\\').Last().Split('.').First()));
                    }
                }
            }

            // SHuffle các phần tữ của list
            listofalldocument.Shuffle();

            using (StreamWriter sw = new StreamWriter(splitPath + "alldocument.txt"))
            {
                for (int i = 0; i < listofalldocument.Count; i++)
                {
                    if (i != listofalldocument.Count - 1)
                        sw.WriteLine(listofalldocument[i].content + "  " + listofalldocument[i].className);
                    else
                        sw.Write(listofalldocument[i].content + "  " + listofalldocument[i].className);
                }
                //Console.WriteLine("Tao file alldocument.txt thanh cong");
            }

            // Chia file alldocument.txt chứa tất cả các document thành 10 file text nhỏ
            SplitText(splitPath + "alldocument.txt", splitPath, XuLyFile.k());

        }
    }
}
