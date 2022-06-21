using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;

namespace PipeInsulationPlugin
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

        public static void CreatePipeInsulation(Document doc)
        {
            using (Transaction t = new Transaction(doc))
            {
                double thickness = 0.020;
                double thicknessTemp = UnitUtils.ConvertToInternalUnits(thickness, DisplayUnitType.DUT_METERS);

                t.Start("AddInsulation");

                PipeInsulation pipeInsulation = PipeInsulation.Create(doc, new ElementId(177016), new ElementId(122535), thicknessTemp);

                t.Commit();
            }
        }
    }
}
