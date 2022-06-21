using Autodesk.Revit.DB;

namespace PipeInsulationPlugin
{
    public class PipeModel
    {
        public Element PipeElement { get; set; }
        public ElementId Id { get; set; }
        public string PipeName { get; set; }
        public string PipeNominalSizeString { get; set; }
        public double PipeNominalSize { get; set; }
        public string SystemClassification { get; set; }
        public string InsulationType { get; set; }
        public double InsulationThickness { get; set; }

        public PipeModel(Element element, ElementId id, string name, string sizeString, string systemClassification, string insulationType, double insulationThickness)
        {
            PipeElement = element;
            Id = id;
            PipeName = name;
            PipeNominalSizeString = sizeString;
            string sizeTemp = PipeNominalSizeString.Substring(0, PipeNominalSizeString.Length - 3);
            PipeNominalSize = double.Parse(sizeTemp);
            SystemClassification = systemClassification;
            InsulationType = insulationType;
            InsulationThickness = insulationThickness;
        }
    }

}
