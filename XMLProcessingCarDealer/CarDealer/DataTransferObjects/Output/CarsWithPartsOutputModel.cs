﻿using System.Xml.Serialization;

namespace CarDealer.DataTransferObjects.Output
{
    [XmlType("car")]
    public class CarsWithPartsOutputModel
    {
        [XmlAttribute("make")]
        public string Make { get; set; }

        [XmlAttribute("model")]
        public string Model { get; set; }

        [XmlAttribute("travelled-distance")]
        public long TravelledDistance { get; set; }

        [XmlArray("parts")]
        public PartsFromCarOutputModel[] CarParts { get; set; }
    }
}
