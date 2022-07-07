using System;
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
                "PipeInsulationPlugin",
                "PipeInsulationPlugin",
                thisAssemblyPath,
                "PipeInsulationPlugin.StartClassPlugin");

            PushButton pb1 = ribbonPanel.AddItem(b1Data) as PushButton;
            pb1.ToolTip = "Add Pipe Insulation";
            BitmapImage pb1Image = new BitmapImage(new Uri("pack://application:,,,/PipeInsulationPlugin;component/Resources/pipe_insulation_new.png"));
            pb1.LargeImage = pb1Image;
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
    }
}

//using Autodesk.Revit.UI;
//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;

//namespace PipeInsulationPlugin
//{
//    public class App : IExternalApplication
//    {

//        public Result OnStartup(UIControlledApplication a)
//        {
//            string ribbonTab = "SK Plugins";
//            string ribbonPanel = "My Plugin";

//            try
//            {
//                a.CreateRibbonTab(ribbonTab);
//            }
//            catch (System.Exception)
//            {
//                throw;
//            }
//            RibbonPanel panel = null;
//            List<RibbonPanel> panels = a.GetRibbonPanels(ribbonTab);
//            foreach (RibbonPanel pnl in panels)
//            {
//                if (panel.Name == ribbonPanel)
//                {
//                    panel = pnl;
//                    break;
//                }
//            }

//            if (panel == null)
//            {
//                panel = a.CreateRibbonPanel(ribbonTab, ribbonPanel);
//            }

//            Image img = Properties.Resources.pipe_insulation_60;
//            ImageSource imgSrc = GetImageSource(img);

//            PushButtonData btnData = new PushButtonData("Add Pipe Insulation", "Add Pipe Insulation",
//                Assembly.GetExecutingAssembly().Location, "PipeInsulationPlugin.StartClassPlugin")
//            {
//                Image = imgSrc,
//                LargeImage = imgSrc
//            };

//            PushButton button = panel.AddItem(btnData) as PushButton;
//            button.Enabled = true;

//            return Result.Succeeded;
//        }
//        public Result OnShutdown(UIControlledApplication a)
//        {
//            return Result.Succeeded;
//        }

//        private BitmapSource GetImageSource(Image img)
//        {
//            BitmapImage bmp = new BitmapImage();

//            using (MemoryStream ms = new MemoryStream())
//            {
//                img.Save(ms, ImageFormat.Png);
//                ms.Position = 0;
//                bmp.BeginInit();
//                bmp.EndInit();

//                bmp.CacheOption = BitmapCacheOption.OnLoad;
//                bmp.UriSource = null;
//                bmp.StreamSource = ms;

//                return bmp;
//            }
//        }
//    }
//}
