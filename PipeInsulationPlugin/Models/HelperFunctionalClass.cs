﻿using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace PipeInsulationPlugin
{
    public class HelperFunctionalClass
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

        public static List<Element> GetInsulationTypes(Document doc)
        {
            FilteredElementCollector allElementsCollectorInsulationTypes = new FilteredElementCollector(doc);

            List<Element> allInsulationTypes = allElementsCollectorInsulationTypes.OfCategory(BuiltInCategory.OST_PipeInsulations).WhereElementIsElementType().ToList();

            return allInsulationTypes;
        }

        public static List<string> GetInsulationTypeNames(Document doc)
        {
            List<Element> allInsulationTypes = GetInsulationTypes(doc);

            List<string> insulationTypeNames = new List<string>();
            foreach (Element element in allInsulationTypes)
            {
                string insulationTypeName = element.get_Parameter(BuiltInParameter.SYMBOL_NAME_PARAM).AsString();
                insulationTypeNames.Add(insulationTypeName);
            }
            return insulationTypeNames;
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

        public static Tuple<double, double> GetPipeSizeFromTextBox(TextBox sizeFromTB, TextBox sizeToTB)
        {
            if (double.TryParse(sizeFromTB.Text, out double sizeFrom) == false && sizeFromTB.Text != "")
            {
                sizeFromTB.Text = "Insert valid value";
            }
            else if (sizeFromTB.Text == "")
            {
                sizeFrom = 0;
            }

            if (double.TryParse(sizeToTB.Text, out double sizeTo) == false && sizeToTB.Text != "")
            {
                sizeToTB.Text = "Insert valid value";
            }
            else if (sizeToTB.Text == "")
            {
                sizeTo = 999999;
            }

            return Tuple.Create(sizeFrom, sizeTo);
        }

        public static ElementId GetInsulationTypeId(ComboBox insulationTypeCombobox, List<Element> insulationTypes)
        {
            Element selectedInsulationType = insulationTypes.ElementAt(insulationTypeCombobox.SelectedIndex);

            return selectedInsulationType.Id;
        }

        public static void AddToCombo(Array array, ComboBox comboBox)
        {
            foreach (var item in array)
            {
                comboBox.Items.Add(item);
            }
        }

        public static string ConvertComboboxItemToProperty(string selectedItem)
        {
            string propertyForSorting = "";
            if (selectedItem == "Pipe Type")
                propertyForSorting = "PipeName";
            else if (selectedItem == "Size")
                propertyForSorting = "PipeNominalSize";
            else if (selectedItem == "System Classification")
                propertyForSorting = "SystemClassification";
            else if (selectedItem == "System Type")
                propertyForSorting = "SystemType";
            else if (selectedItem == "Insulation Type")
                propertyForSorting = "InsulationType";
            else if (selectedItem == "Comments")
                propertyForSorting = "Comments";
            else if (selectedItem == "ID")
                propertyForSorting = "Id";
            else propertyForSorting = "InsulationThickness";
            return propertyForSorting;
        }

        public static void DeleteInsulationForFilteredPipes(Document doc, List<PipeModel> FilteredPipes)
        {
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Delete Insulation");

                FilteredElementCollector allElementsCollectorForInsulation = new FilteredElementCollector(doc);

                List<Element> allPipeInsulationElements = allElementsCollectorForInsulation.OfCategory(BuiltInCategory.OST_PipeInsulations).WhereElementIsNotElementType().ToList();

                List<PipeInsulation> allPipeInsulation = new List<PipeInsulation>();

                foreach (var item in allPipeInsulationElements)
                {
                    allPipeInsulation.Add(item as PipeInsulation);
                }

                List<ElementId> InsulationIdToBeDeleted = (from PipeInsulation in allPipeInsulation
                                                           join PipeModel in FilteredPipes on PipeInsulation.HostElementId equals PipeModel.Id
                                                           select PipeInsulation.Id).ToList();

                foreach (var item in InsulationIdToBeDeleted)
                {
                    doc.Delete(item);
                }

                t.Commit();
            }
        }
    }
}
