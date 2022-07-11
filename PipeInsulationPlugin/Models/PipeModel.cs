using Autodesk.Revit.DB;

namespace PipeInsulationPlugin
{
    public class PipeModel : ElementModel
    {
        public PipeModel(Element element, ElementId id, string name, string sizeString, 
            string systemClassification, string systemType, string insulationType, 
            double insulationThickness, string comments)
            : base(element, id, name, sizeString, systemClassification, 
                  systemType, insulationType, insulationThickness, comments)
        {
        }
    }

}
