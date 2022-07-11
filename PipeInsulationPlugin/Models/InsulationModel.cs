using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

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

        public static void CreatePipeInsulation(Document doc, List<ElementModel> filteredPipes, ComboBox InsulationTypeCombobox, TextBox InsulationThicknessTextBox)
        {
            HelperFunctionalClass.DeleteInsulationForFilteredPipes(doc, filteredPipes);

            double insulationThickness = double.Parse(InsulationThicknessTextBox.Text.ToString());
            double insulationThicknessConversion = UnitUtils.ConvertToInternalUnits(insulationThickness, DisplayUnitType.DUT_METERS) / 1000;

            List<Element> insulationTypesList = new List<Element>();
            insulationTypesList = HelperFunctionalClass.GetInsulationTypes(doc);

            ElementId insulationTypeIdforSingleFilter = HelperFunctionalClass.GetInsulationTypeId(InsulationTypeCombobox, insulationTypesList);

            using (Transaction t = new Transaction(doc))
            {
                t.Start("AddInsulation");

                foreach (var element in filteredPipes)
                {
                    _ = PipeInsulation.Create(doc, element.Id, insulationTypeIdforSingleFilter, insulationThicknessConversion);
                }
                MessageBox.Show("Insulation added successfully");
                t.Commit();
            }
        }
    }
}
