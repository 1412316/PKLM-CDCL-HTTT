using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhanLop
{
    class FeatureIDF
    {
        public String feature { get; set; }
        public double idf { get; set; }

        public FeatureIDF()
        {
            this.feature = "";
            this.idf = 0;
        }

        public FeatureIDF(String feature, double idf)
        {
            this.feature = feature;
            this.idf = idf;
        }
    }
}
