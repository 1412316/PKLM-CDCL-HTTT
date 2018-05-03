using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PhanLop
{
    class XuLyFile
    {
        XuLyFile()
        {
           
        }

        public static int docRound()
        {
            using (StreamReader sr = new StreamReader("round.txt"))
            {
                return Int32.Parse(sr.ReadLine());
            }
        }

        public static int knn()
        {
            using (StreamReader sr = new StreamReader("knn.txt"))
            {
                return Int32.Parse(sr.ReadLine());
            }
        }
    }
}
