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

            List<PipeModel> pipesWithParameters = HelperFunctionalClass.GetAllParametersToLists(doc).Item1;
            List<InsulationModel> insulationWithParameters = HelperFunctionalClass.GetAllParametersToLists(doc).Item2;
            List<PipeModel> filteredPipes = new List<PipeModel>();
            List<Element> insulationTypes = HelperFunctionalClass.GetInsulationTypes(doc);

            //List<string> insulationTypesNames = HelperFunctionalClass.GetInsulationTypeNames(doc);



            UserWindowPipes userWindowPipes = new UserWindowPipes(doc, pipesWithParameters, insulationWithParameters, filteredPipes);

            userWindowPipes.ShowDialog();

            return Result.Succeeded;
        }
    }
}
