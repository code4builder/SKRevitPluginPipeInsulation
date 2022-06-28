using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;

namespace PipeInsulationPlugin
{
    public static class HelperFunctionalClass
    {
        public static Tuple<List<PipeModel>, List<InsulationModel>> GetAllParametersToLists(Document doc)
        {
            FilteredElementCollector allElementsCollectorPipes = new FilteredElementCollector(doc);

            List<Element> allFilteredPipes = allElementsCollectorPipes.OfCategory(BuiltInCategory.OST_PipeCurves).WhereElementIsNotElementType().ToList();

            List<PipeModel> pipesWithParameters = new List<PipeModel>();

            GetPipeParameters(allFilteredPipes, pipesWithParameters);


            FilteredElementCollector allElementsCollectorInsulationTypes = new FilteredElementCollector(doc);

            List<Element> allInsulationTypes = allElementsCollectorInsulationTypes.OfCategory(BuiltInCategory.OST_PipeInsulations).WhereElementIsElementType().ToList();

            List<InsulationModel> insulationTypesWithParameters = new List<InsulationModel>();

            GetInsulationParameters(allInsulationTypes, insulationTypesWithParameters);


            FilteredElementCollector allElementsCollectorInsulation = new FilteredElementCollector(doc);

            List<Element> allPipeInsulation = allElementsCollectorInsulation.OfCategory(BuiltInCategory.OST_PipeInsulations).WhereElementIsNotElementType().ToList();

            List<InsulationModel> insulationWithParameters = new List<InsulationModel>();

            return Tuple.Create(pipesWithParameters, insulationWithParameters);

        }

        public static void GetPipeParameters(List<Element> allPipes, List<PipeModel> pipesWithParameters)
        {
            foreach (Element element in allPipes)
            {
                string diameterString = element.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsValueString().ToString();
                ElementId id = element.Id;
                string systemClassification = element.get_Parameter(BuiltInParameter.RBS_SYSTEM_CLASSIFICATION_PARAM).AsString();
                string systemType = element.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsValueString();
                string insulationType = element.get_Parameter(BuiltInParameter.RBS_REFERENCE_INSULATION_TYPE).AsString();
                string insulationThicknessString = element.get_Parameter(BuiltInParameter.RBS_REFERENCE_INSULATION_THICKNESS).AsValueString();
                double insulationThickness = double.Parse(insulationThicknessString.Substring(0, insulationThicknessString.Length - 3));
                string comments = element.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsString();
                pipesWithParameters.Add(new PipeModel(element, id, element.Name, diameterString, systemClassification, systemType, insulationType, insulationThickness, comments));
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
