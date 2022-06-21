using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;

namespace PipeInsulationPlugin
{
    public static class GetParametersClass
    {
        public static void GetPipeParameters(List<Element> allPipes, List<PipeModel> pipesWithParameters)
        {
            foreach (Element element in allPipes)
            {
                string diameterString = element.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsValueString().ToString();
                ElementId id = element.Id;
                string systemClassification = element.get_Parameter(BuiltInParameter.RBS_SYSTEM_CLASSIFICATION_PARAM).AsString();
                string insulationType = element.get_Parameter(BuiltInParameter.RBS_REFERENCE_INSULATION_TYPE).AsString();
                string insulationThicknessString = element.get_Parameter(BuiltInParameter.RBS_REFERENCE_INSULATION_THICKNESS).AsValueString();
                double insulationThickness = double.Parse(insulationThicknessString.Substring(0, insulationThicknessString.Length - 3));
                pipesWithParameters.Add(new PipeModel(element, id, element.Name, diameterString, systemClassification, insulationType, insulationThickness));
            }
        }
        public static void GetInsulationParameters(List<Element> allInsulationTypes, List<InsulationModel> insulationsWithParameters)
        {
            foreach (Element element in allInsulationTypes)
            {
                ElementId id = element.Id;
                insulationsWithParameters.Add(new InsulationModel(element, id, element.Name));
            }
        }
    }
}
