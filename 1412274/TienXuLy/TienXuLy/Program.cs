using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.IO;

namespace TienXuLy
{
    class Program
    {
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
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '.' || str[i] == ',' || str[i] == '?' || str[i] == '!' || str[i] == '~' || str[i] == '@'
                    || str[i] == '#' || str[i] == '$' || str[i] == '%' || str[i] == '^' || str[i] == '&' || str[i] == '*'
                    || str[i] == '(' || str[i] == ')' || str[i] == '`' || str[i] == '-' || str[i] == '_' || str[i] == '='
                    || str[i] == '+' || str[i] == '|' || str[i] == '{' || str[i] == '}' || str[i] == '[' || str[i] == ']'
                    || str[i] == ':' || str[i] == ';' || str[i] == '"' || str[i] == '<' || str[i] == '>' || str[i] == '/')
                {
                    str = str.Replace((char)str[i], ' ');
                }
            }
            return str;
        }

        public static void Main()
        {
            //System.IO.File.WriteAllText(@"C:\Users\Kim\Desktop\output.txt", string.Empty);
            //System.IO.File.WriteAllText("output.txt", string.Empty);
            
            EnglishPorter2Stemmer stemword = new EnglishPorter2Stemmer();
            
            string line = null;

            List<string> Texts = new List<string>();
            List<string> StopWords = new List<string>();

            bool flag;

            //Doc cac van ban tu file input
            using (StreamReader filein = new StreamReader("input.txt"))
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

            using (StreamWriter fileout = new StreamWriter("output.txt"))
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
    }
}
