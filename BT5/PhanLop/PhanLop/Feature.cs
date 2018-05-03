using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhanLop
{
    class Feature
    {
        public String feature { get; set; }
        public double weight { get; set; }

        public void tangWeight()
        {
            this.weight++;
        }

        public Feature()
        {
            this.feature = "";
            this.weight = 0;
        }

        public Feature(String feature, double weight)
        {
            this.feature = feature;
            this.weight = weight;
        }
    }
}
