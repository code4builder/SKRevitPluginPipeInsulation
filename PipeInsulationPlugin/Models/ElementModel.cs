using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeInsulationPlugin
{
    public class ElementModel
    {
        public Element PipeElement { get; set; }
        public ElementId Id { get; set; }
        public string PipeName { get; set; }
        public string PipeNominalSizeString { get; set; }
        public double PipeNominalSize { get; set; }
        public string SystemClassification { get; set; }
        public string SystemType { get; set; }
        public string InsulationType { get; set; }
        public double InsulationThickness { get; set; }
        public string Comments { get; set; }

        public ElementModel(Element element, ElementId id, string name, string sizeString, string systemClassification, string systemType, string insulationType, double insulationThickness, string comments)
        {
            PipeElement = element;
            Id = id;
            PipeName = name;
            PipeNominalSizeString = sizeString;
            string sizeTemp = PipeNominalSizeString.Substring(0, PipeNominalSizeString.Length - 3);
            PipeNominalSize = double.Parse(sizeTemp);
            SystemClassification = systemClassification;
            SystemType = systemType;
            InsulationType = insulationType;
            InsulationThickness = insulationThickness;
            Comments = comments;
        }
    }
}
