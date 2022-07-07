using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using PipeInsulationPlugin.Views;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PipeInsulationPlugin
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class StartClassPlugin : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            MessageBox.Show("Plugin Created");

            Assembly.LoadFrom(Path.Combine(@"C:\ProgramData\Autodesk\Revit\Addins\2020", "MaterialDesignThemes.Wpf.dll"));
            Assembly.LoadFrom(Path.Combine(@"C:\ProgramData\Autodesk\Revit\Addins\2020", "MaterialDesignColors.dll"));

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CommandLoad_AssemblyResolve);

            Document doc = commandData.Application.ActiveUIDocument.Document;

            List<PipeModel> pipesWithParameters = HelperFunctionalClass.GetAllParametersToLists(doc).Item1;
            List<InsulationModel> insulationWithParameters = HelperFunctionalClass.GetAllParametersToLists(doc).Item2;
            List<PipeModel> filteredPipes = new List<PipeModel>();
            List<Element> insulationTypes = HelperFunctionalClass.GetInsulationTypes(doc);
            UserWindowPipes userWindowPipes = new UserWindowPipes(doc, pipesWithParameters, insulationWithParameters, filteredPipes);
            userWindowPipes.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            userWindowPipes.ShowDialog();

            return Result.Succeeded;
        }

        private Assembly CommandLoad_AssemblyResolve(object sender, ResolveEventArgs args)
        {

            string assemblyFile = Path.Combine(@"C:\ProgramData\Autodesk\Revit\Addins\2020", "MaterialDesignThemes.Wpf.dll");

            return Assembly.LoadFrom(assemblyFile);

        }
    }
}
