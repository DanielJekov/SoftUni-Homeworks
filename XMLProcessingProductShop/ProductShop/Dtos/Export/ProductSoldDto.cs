using System.Collections.Generic;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    [XmlType("SoldProducts")]
    public class ProductSoldDto
    {
        [XmlElement("count")]
        public int CountOfProductsSoldByUser { get; set; }

        [XmlArray("products")]
        public NestedProductsSold[] ProductsSoldByUser { get; set; }
        
    }
}
