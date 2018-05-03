using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhanLop
{
    class SimilarityMeasure
    {
        public double value { get; set; }
        public int index { get; set; }

        public SimilarityMeasure(double value, int index)
        {
            this.value = value;
            this.index = index;
        }
    }
}
