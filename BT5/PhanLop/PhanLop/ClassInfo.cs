using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhanLop
{
    class ClassInfo
    {
        public String ClassName { get; set; }
        public int NumberRightofClass { get; set; } //So luong document duoc phan dung vao class i
        public int NumberWrongofClass { get; set; } //So luong document bi phan sai vao class i
        public int NumberWrongoutClass { get; set; } //So luong document class i bi phan sai vao class khac
        public double Precision { get; set; } //Do chinh xac cua class i
        public double Recall { get; set; } //Do bao phu cua class i
        public double Fscore { get; set; } //Do do tong hop cua class i

        public ClassInfo()
        {
            this.ClassName = "";
            this.NumberRightofClass = 0;
            this.NumberWrongofClass = 0;
            this.NumberWrongoutClass = 0;
            this.Precision = 0;
            this.Recall = 0;
            this.Fscore = 0;
        }

        public ClassInfo(String ClassName, int NumberRightofClass, int NumberWrongofClass, int NumberWrongoutClass, double Precision, double Recall, double Fscore)
        {
            this.ClassName = ClassName;
            this.NumberRightofClass = NumberRightofClass;
            this.NumberWrongofClass = NumberWrongofClass;
            this.NumberWrongoutClass = NumberWrongoutClass;
            this.Precision = Precision;
            this.Recall = Recall;
            this.Fscore = Fscore;
        }

        //Kiểm tra xem class đó có tồn tại trong list các class chưa
        static int IsExist(List<ClassInfo> List, String str)
        {
            int result = 0;
            for (int i = 0; i < List.Count; i++)
            {
                if (List[i].ClassName == str)
                {
                    result = 1;
                }
            }
            return result;
        }

        //Hàm tính và lưu các thông số về các class
        public static List<ClassInfo> ThongTinClass(String filetrue, String fileknn, String[] allclass)
        {
            List<DocumentDaPhanLop> listTrue = DocumentDaPhanLop.layCacDocumentDaPhanLop(filetrue);
            List<DocumentDaPhanLop> listPhanLop = DocumentDaPhanLop.layCacDocumentDaPhanLop(fileknn);
            List<ClassInfo> listClass = new List<ClassInfo>();

            //Lay cac class trong file txt dung 
            for (int i = 0; i < allclass.Length; i++)
            {
                String temp = allclass[i].Split('\\').Last().Split('.').First();
                listClass.Add(new ClassInfo(temp, 0, 0, 0, 0, 0, 0));
            }

            //Tinh toan cho tung class
            for (int k = 0; k < listClass.Count; k++)
            {
                //Xet class cua tung dong trong 2 van ban
                for (int i = 0; i < listTrue.Count; i++)
                {
                    if (listTrue[i].className == listPhanLop[i].className && listTrue[i].className == listClass[k].ClassName)
                        listClass[k].NumberRightofClass++;
                    if (listTrue[i].className != listPhanLop[i].className && listPhanLop[i].className == listClass[k].ClassName)
                        listClass[k].NumberWrongofClass++;
                    if (listTrue[i].className != listPhanLop[i].className && listTrue[i].className == listClass[k].ClassName)
                        listClass[k].NumberWrongoutClass++;
                }

                //Tính precisoin, recall, f-score
                if (listClass[k].NumberRightofClass != 0)
                {
                    listClass[k].Precision = Math.Round((double)listClass[k].NumberRightofClass / (listClass[k].NumberRightofClass + listClass[k].NumberWrongofClass), XuLyFile.docRound());
                    listClass[k].Recall = Math.Round((double)listClass[k].NumberRightofClass / (listClass[k].NumberRightofClass + listClass[k].NumberWrongoutClass), XuLyFile.docRound());
                    listClass[k].Fscore = Math.Round((double)(2 * listClass[k].Precision * listClass[k].Recall) / (listClass[k].Precision + listClass[k].Recall), XuLyFile.docRound());
                }
                else
                {
                    listClass[k].Precision = 0;
                    listClass[k].Recall = 0;
                    listClass[k].Fscore = 0;
                }

            }

            return listClass;
        }

        //Hàm tính F-macro
        public static double F_Macro(List<ClassInfo> List)
        {
            double result = 0;
            double p_macro = 0;
            double r_macro = 0;

            //Tính toán p_macro và r_macro
            for (int i = 0; i < List.Count; i++)
            {
                p_macro += Math.Round((double)List[i].Precision / List.Count, XuLyFile.docRound());
                r_macro += Math.Round((double)List[i].Recall / List.Count, XuLyFile.docRound());
            }

            result = Math.Round((double)(2 * p_macro * r_macro) / (p_macro + r_macro), XuLyFile.docRound());
            return result;
        }

        //Hàm tính F-micro
        public static double F_Micro(List<ClassInfo> List, int tongsovb)
        {
            double result = 0;
            double temp = 0;

            for (int i = 0; i < List.Count; i++)
            {
                temp += List[i].NumberRightofClass;
            }

            result = Math.Round((double)temp / tongsovb, XuLyFile.docRound());
            return result;
        }

    }
}
