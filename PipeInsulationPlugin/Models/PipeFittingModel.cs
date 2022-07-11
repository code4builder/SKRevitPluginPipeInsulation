using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace PipeInsulationPlugin
{
    public class PipeFittingModel : ElementModel
    {
        public PipeFittingModel(Element element, ElementId id, string name, string sizeString,
            string systemClassification, string systemType, string insulationType,
            double insulationThickness, string comments)
            : base(element, id, name, sizeString, systemClassification,
                  systemType, insulationType, insulationThickness, comments)
        {
        }
    }
}
