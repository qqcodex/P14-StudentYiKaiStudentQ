using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRG2_ASSG
{
    internal class SpecialOffer
    {
        public string offerCode {  get; set; }
        public string offerDesc {  get; set; }
        public double discount { get; set; }

        public SpecialOffer() { }

        public SpecialOffer(string offerCode, string offerDesc, double discount)
        {
            this.offerCode = offerCode;
            this.offerDesc = offerDesc;
            this.discount = discount;
        }

        public override string ToString() 
        { 
            return $"{offerCode}: {offerDesc} ({discount}%)"; 
        }
    }
}
