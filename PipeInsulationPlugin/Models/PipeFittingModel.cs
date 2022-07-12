using Autodesk.Revit.DB;

namespace SKRevitPluginPipeInsulation
{
    public class PipeFittingModel : ElementModel
    {
        public PipeFittingModel(Element element, ElementId id, string name, string sizeString,
            string systemClassification, string systemType, string insulationType,
            double insulationThickness, string comments)
            : base(element, id, name, sizeString, systemClassification,
                  systemType, insulationType, insulationThickness, comments)
        {
        }
    }
}
