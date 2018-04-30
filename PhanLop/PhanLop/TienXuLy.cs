using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace PhanLop
{
    class TienXuLy
    {
        TienXuLy()
        {

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

        public static string BoStopWords(string str)
        {
            string stwline = null;
            StringBuilder templine = new StringBuilder();

            List<string> StopWords = new List<string>();

            bool flag;

            //Doc cac stop words tu file stop
            using (StreamReader filestop = new StreamReader("stop.txt"))
            {
                while ((stwline = filestop.ReadLine()) != null)
                {
                    StopWords.Add(stwline);
                }
                filestop.Close();
            }

            //Xu ly co ban cho cac dong stop words
            for (int j = 0; j < StopWords.Count; j++)
            {
                StopWords[j] = StopWords[j].ToLower();
                StopWords[j] = XoaKyTu(StopWords[j]);
                StopWords[j] = XoaTrang(StopWords[j]);
            }

            string[] temp = str.Split(' ');
            for (int i = 0; i < temp.Length; i++)
            {
                flag = true;
                foreach (string st in StopWords)
                {
                    if (st == temp[i])
                        flag = false;
                }

                if (flag == true)
                {
                    if (i == (temp.Length - 1))
                        templine.Append(temp[i]);
                    else
                        templine.Append(temp[i] + " ");
                }
            }
            return XoaTrang(templine.ToString());
        }

        public static string PorterStemming(string str)
        {
            EnglishPorter2Stemmer stemword = new EnglishPorter2Stemmer();
            StringBuilder templine = new StringBuilder();

            List<string> StopWords = new List<string>();

            string[] temp = str.Split(' ');
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = stemword.Stem(temp[i]).Value;
                if (i == (temp.Length - 1))
                    templine.Append(temp[i]);
                else
                    templine.Append(temp[i] + " ");
            }
            return templine.ToString();
        }

        public static void tienXuLy(string input, string output)
        {
            EnglishPorter2Stemmer stemword = new EnglishPorter2Stemmer();

            string line = null;

            List<string> Texts = new List<string>();

            //Doc cac van ban tu file input
            using (StreamReader filein = new StreamReader(input))
            {
                while ((line = filein.ReadLine()) != null)
                {
                    Texts.Add(line);
                }
                filein.Close();
            }

            //Console.WriteLine("===============Cac dong trong file input===============");
            //foreach (string t in Texts)
            //{
            //    Console.WriteLine(t);
            //}

            //Console.WriteLine("===============Cac dong trong file output===============");
            using (StreamWriter fileout = new StreamWriter(output))
            {
                for (int i = 0; i < Texts.Count(); i++)
                {
                    Texts[i] = Texts[i].ToLower();
                    Texts[i] = XoaKyTu(Texts[i]);
                    Texts[i] = XoaTrang(Texts[i]);
                    Texts[i] = BoStopWords(Texts[i]);
                    Texts[i] = PorterStemming(Texts[i]);

                    if (i == (Texts.Count - 1))
                    {
                        // Console.WriteLine(Texts[i]);
                        fileout.Write(Texts[i]);
                    }
                    else
                    {
                        //Console.WriteLine(Texts[i]);
                        fileout.WriteLine(Texts[i]);
                    }
                }
                fileout.Close();
            }
        }
    }
}
