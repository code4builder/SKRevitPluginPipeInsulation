using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using SKRevitPluginPipeInsulation.Views;
using System.Collections.Generic;

namespace SKRevitPluginPipeInsulation
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
            List<PipeFittingModel> pipesFittingsWithParameters = HelperFunctionalClass.GetAllParametersToLists(doc).Item3;
            List<Element> insulationTypes = HelperFunctionalClass.GetInsulationTypes(doc);
            UserWindowPipes userWindowPipes = new UserWindowPipes(doc, pipesWithParameters, insulationWithParameters, pipesFittingsWithParameters);

            userWindowPipes.ShowDialog();

            return Result.Succeeded;
        }
    }
}
