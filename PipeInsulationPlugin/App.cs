﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Media.Imaging;

namespace PipeInsulationPlugin
{
    class App : IExternalApplication
    {
        // define a method that will create our tab and button
        static void AddRibbonPanel(UIControlledApplication application)
        {
            // Create a custom ribbon tab
            String tabName = "SKTools";
            application.CreateRibbonTab(tabName);

            // Add a new ribbon panel
            RibbonPanel ribbonPanel = application.CreateRibbonPanel(tabName, "Tools");

            // Get dll assembly path
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;

            // create push button for CurveTotalLength
            PushButtonData b1Data = new PushButtonData(
                "Pipe Insulation",
                "Pipe Insulation",
                thisAssemblyPath,
                "PipeInsulationPlugin.StartClassPlugin");

            PushButton pb1 = ribbonPanel.AddItem(b1Data) as PushButton;
            pb1.ToolTip = "Add Pipe Insulation";
            BitmapImage pb1Image = new BitmapImage(new Uri("pack://application:,,,/PipeInsulationPlugin;component/Resources/pipe_insulation_new.png"));
            pb1.LargeImage = pb1Image;

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            // do nothing
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            // call our method that will load up our toolbar
            AddRibbonPanel(application);
            return Result.Succeeded;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string assemblyName = args.RequestingAssembly.GetName().Name;
            if (assemblyName == "YourAssemblyName")
            {
                string resourceName = "YourAssemblyName.Assemblies." + new AssemblyName(args.Name).Name + ".dll";
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        byte[] assemblyData = new byte[stream.Length];
                        stream.Read(assemblyData, 0, assemblyData.Length);
                        return Assembly.Load(assemblyData);
                    }
                }
            }
            return null;
        }
    }
}
