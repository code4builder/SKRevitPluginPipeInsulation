using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using PipeInsulationPlugin.Views;

namespace PipeInsulationPlugin
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class StartClassPlugin : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            FilteredElementCollector allElementsCollectorPipes = new FilteredElementCollector(doc);

            List<Element> allFilteredPipes = allElementsCollectorPipes.OfCategory(BuiltInCategory.OST_PipeCurves).WhereElementIsNotElementType().ToList();

            List<PipeModel> pipesWithParameters = new List<PipeModel>();

            GetParametersClass.GetPipeParameters(allFilteredPipes, pipesWithParameters);


            FilteredElementCollector allElementsCollectorInsulationTypes = new FilteredElementCollector(doc);

            List<Element> allInsulationTypes = allElementsCollectorInsulationTypes.OfCategory(BuiltInCategory.OST_PipeInsulations).WhereElementIsElementType().ToList();

            List<InsulationModel> insulationTypesWithParameters = new List<InsulationModel>();

            GetParametersClass.GetInsulationParameters(allInsulationTypes, insulationTypesWithParameters);


            FilteredElementCollector allElementsCollectorInsulation = new FilteredElementCollector(doc);

            List<Element> allPipeInsulation = allElementsCollectorInsulation.OfCategory(BuiltInCategory.OST_PipeInsulations).WhereElementIsNotElementType().ToList();

            List<InsulationModel> insulationWithParameters = new List<InsulationModel>();

            

            UserWindowPipes userWindowPipes = new UserWindowPipes(doc, pipesWithParameters, insulationWithParameters);

            userWindowPipes.ShowDialog();

            return Result.Succeeded;
        }
    }
}
