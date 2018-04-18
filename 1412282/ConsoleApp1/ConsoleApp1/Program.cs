using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            //doc file test.txt
            string line=null;
            
            List<string> data = new List<string>();
            List<string> nclass_test = new List<string>();
            using (StreamReader filetest = new StreamReader("test.txt"))
            {
                while ((line = filetest.ReadLine()) != null)
                {
                    data.Add(line);
                }
                filetest.Close();
            }
            //lay name class bỏ vao list nclass_test
            for (int i = 0; i < data.Count; i++)
            {

                //Console.WriteLine(data[i]);
                nclass_test.Add(data[i].Split(' ').Last());

            }
           for (int i=0;i<nclass_test.Count;i++)
            {
                Console.WriteLine(nclass_test[i]);
            }
            //-----------------------
            //doc file test.txt
            string rline = null;

            List<string> rdata = new List<string>();
            List<string> nclass_result = new List<string>();
            using (StreamReader fileresult = new StreamReader("result.txt"))
            {
                while ((rline = fileresult.ReadLine()) != null)
                {
                   rdata.Add(rline);
                }
                fileresult.Close();
            }
            //lay name class bỏ vao list nclass_test
            for (int i = 0; i < rdata.Count; i++)
            {

                //Console.WriteLine(data[i]);

                //string s = rdata[i].Split(' ').Last();
                //nclass_result.Add(s.Split("class:").Last());
                nclass_result.Add(rdata[i].Split(' ').Last());

            }
            Console.WriteLine("---------------------------");
            //Console.WriteLine(nclass_result.Count);
            for (int i = 0; i < nclass_result.Count; i++)
            {
                Console.WriteLine(nclass_result[i]);
            }

            //-----------------------------tinh toan-------------------------------
            int count_lgm = 0;
            int count_spm = 0;
            for(int i=0;i<nclass_test.Count;i++)
            {
                if (nclass_test[i] == nclass_result[i] && (nclass_result[i] == "lgm"))
                    count_lgm++;
                else if (nclass_test[i] == nclass_result[i] && (nclass_result[i] == "spm"))
                    count_spm++;

            }
            int num_lgm = 0;
            int num_spm = 0;
            for(int i=0;i<nclass_test.Count;i++)
            {
                if (nclass_test[i] == "lgm")
                    num_lgm++;
                else num_spm++;
            }

            int rnum_lgm = 0;
            int rnum_spm = 0;
            for (int i = 0; i < nclass_result.Count; i++)
            {
                if (nclass_result[i] == "lgm")
                    rnum_lgm++;
                else rnum_spm++;
            }

            float p_lgm = (float)count_lgm / num_lgm;
            float r_lgm = (float)count_lgm / rnum_lgm;

            float p_spm = (float)count_spm / num_spm;
            float r_spm = (float)count_spm / rnum_spm;

            float r_macro = (r_lgm + r_spm)/2;
            float p_macro = (p_lgm + p_spm)/2;
            //Console.WriteLine(count_lgm);
            //Console.WriteLine(count_spm);
            using (StreamWriter sw = new StreamWriter("final.txt"))
            {
                sw.WriteLine("P(lgm)= {0}", System.Math.Round(p_lgm,2));
                sw.WriteLine("R(lgm)= {0}", System.Math.Round(r_lgm, 2));
                sw.WriteLine("F(lgm)= {0}", System.Math.Round((2 * p_lgm * r_lgm) / (p_lgm + r_lgm), 2));

                sw.WriteLine("P(spm)= {0}", System.Math.Round(p_spm, 2));
                sw.WriteLine("R(spm)= {0}", System.Math.Round(r_spm, 2));
                sw.WriteLine("F(spm)= {0}", System.Math.Round((2 * p_spm * r_spm) / (p_spm + r_spm), 2));

                sw.WriteLine("F(macro)= {0}", System.Math.Round((2 * r_macro * p_macro) / (r_macro + p_macro), 2));
                sw.WriteLine("F(micro)= {0}", System.Math.Round((float)(count_lgm + count_spm) / (nclass_result.Count), 2));
            }
            

                Console.ReadKey();
            
        }
    }
}
