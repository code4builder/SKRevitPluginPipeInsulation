using Autodesk.Revit.DB;

namespace SKRevitPluginPipeInsulation
{
    public class InsulationModel
    {
        public Element InsulationElement { get; set; }
        public ElementId Id { get; set; }
        public string InsulationName { get; set; }

        public InsulationModel(Element element, ElementId id, string name)
        {
            InsulationElement = element;
            Id = id;
            InsulationName = name;
        }


    }
}
