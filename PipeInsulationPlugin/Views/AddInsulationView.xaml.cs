﻿using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Microsoft.Win32;
using SKRevitPluginPipeInsulation.Views.UserControls;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SKRevitPluginPipeInsulation.Views
{
    /// <summary>
    /// Interaction logic for AddInsulationView.xaml
    /// </summary>
    public partial class AddInsulationView : Window
    {
        List<PipeModel> allPipes;
        List<ElementModel> allFilteredPipes = new List<ElementModel>();
        List<ElementModel> SingleFilterPipesList = new List<ElementModel>();
        List<PipeFittingModel> allPipeFittings;
        List<InsulationModel> insulationWithParameters;
        List<ElementModel> allPipesAndFittings;
        Document doc;
        List<Element> insulationTypesList;
        List<FilterUserControl> allFilterUserControls = new List<FilterUserControl>();

        public AddInsulationView(List<PipeModel> pipes, List<PipeFittingModel> pipeFittings, List<InsulationModel> insulations, Document document)
        {
            InitializeComponent();
            allPipes = pipes;
            allPipeFittings = pipeFittings;
            insulationWithParameters = insulations;
            doc = document;

            allPipesAndFittings = allPipes.Cast<ElementModel>().Concat(allPipeFittings.Cast<ElementModel>()).ToList();

            insulationTypesList = HelperFunctionalClass.GetInsulationTypes(doc);

            Filter1UC.InsulationTypeCombobox.ItemsSource = SKRevitPluginPipeInsulation.HelperFunctionalClass.GetInsulationTypeNames(doc);

            allFilterUserControls.Add(Filter1UC);
        }

        private List<ElementModel> FilterSingleUC(TextBox SystemTypeTB, TextBox sizeFromTB, TextBox sizeToTB, TextBox CommentsTB, CheckBox CommentsCheckBox)
        {
            SingleFilterPipesList.Clear();

            double sizeFrom = HelperFunctionalClass.GetPipeSizeFromTextBox(sizeFromTB, sizeToTB).Item1;
            double sizeTo = HelperFunctionalClass.GetPipeSizeFromTextBox(sizeFromTB, sizeToTB).Item2;

            foreach (var pipe in allPipesAndFittings)
            {
                if (CommentsTB.Text != "" && CommentsCheckBox.IsChecked == true)
                {
                    if (pipe.Comments != null)
                    {
                        if (pipe.SystemType.ToString().ToLower().Contains(SystemTypeTB.Text.ToString().ToLower())
                            && pipe.PipeNominalSize >= sizeFrom
                            && pipe.PipeNominalSize <= sizeTo
                            && pipe.Comments.ToString().ToLower().Contains(CommentsTB.Text.ToString().ToLower()))
                        {
                            SingleFilterPipesList.Add(pipe);
                        }
                    }
                }

                else if(CommentsTB.Text == "" && CommentsCheckBox.IsChecked == true)
                {
                    if (pipe.SystemType.ToString().ToLower().Contains(SystemTypeTB.Text.ToString().ToLower())
                        && pipe.PipeNominalSize >= sizeFrom
                        && pipe.PipeNominalSize <= sizeTo
                        && pipe.Comments == null)
                    {
                        SingleFilterPipesList.Add(pipe);
                    }
                }

                else if (CommentsCheckBox.IsChecked == false)
                {
                    if (pipe.SystemType.ToString().ToLower().Contains(SystemTypeTB.Text.ToString().ToLower())
                        && pipe.PipeNominalSize >= sizeFrom
                        && pipe.PipeNominalSize <= sizeTo)
                    {
                        SingleFilterPipesList.Add(pipe);
                    }
                }
            }

            return SingleFilterPipesList;
        }

        private void AddFilterUC_Click(object sender, RoutedEventArgs e)
        {
            AddNewFilterUserControl();
        }

        private void AddNewFilterUserControl()
        {
            string filterUCName = "Filter" + (BatchAddingStackPanel.Children.Count + 1).ToString() + "UC";

            FilterUserControl newFilter = new FilterUserControl();
            newFilter.FilterNameLabel.Text = "FILTER " + (BatchAddingStackPanel.Children.Count + 1).ToString();

            BatchAddingStackPanel.Children.Add(newFilter);
            allFilterUserControls.Add(newFilter);

            newFilter.InsulationTypeCombobox.ItemsSource = SKRevitPluginPipeInsulation.HelperFunctionalClass.GetInsulationTypeNames(doc);

            newFilter.Name = filterUCName;

            foreach (FilterUserControl userControl in allFilterUserControls)
            {
                if (userControl.Name == "newFilter")
                {
                    userControl.Name = "filterUCName";
                }
            }
        }

        private void BatchAddInsulationButton_Click(object sender, RoutedEventArgs e)
        {
            int totalInsulatedElements = 0;

            foreach (FilterUserControl userControl in allFilterUserControls)
            {
                bool isValidThicknessValue = double.TryParse(userControl.InsulationThicknessTextBox.Text.ToString(), out double insulationThickness);

                List<ElementModel> SingleFilterPipes = new List<ElementModel>();
                SingleFilterPipes = FilterSingleUC(userControl.SystemTypeTextBox, userControl.SizeFromTextBox, userControl.SizeToTextBox, userControl.CommentsTextBox, userControl.CommentsCheckBox);

                if (isValidThicknessValue)
                {
                    HelperFunctionalClass.DeleteInsulationForFilteredPipes(doc, SingleFilterPipes);

                    HelperFunctionalClass.CreatePipeInsulation(doc, SingleFilterPipes, userControl.InsulationTypeCombobox, insulationThickness);

                    totalInsulatedElements += SingleFilterPipes.Count;
                }
                else if (SingleFilterPipes.Count == 0)
                {
                    MessageBox.Show("Please press add filter'");
                }
                else if (userControl.InsulationThicknessTextBox.Text == "" || isValidThicknessValue == false)
                {
                    MessageBox.Show("Please add valid insulation thickness value");
                }
            }

            MessageBox.Show($"Insulation added successfully for {totalInsulatedElements} elements");
        }

        private void LoadFilters_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Comma Separated Values File|*.csv";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.ShowDialog();
            string filePath = openFileDialog.FileName;

            var linesFromCSV = File.ReadAllLines(filePath).Select(a => a.Split(','));

            AddValuesToFilterUC(linesFromCSV.ElementAt(0), Filter1UC);
            List<string[]> linesListFromSecond = linesFromCSV.ToList();
            linesListFromSecond.RemoveAt(0);

            foreach (var line in linesListFromSecond)
            {
                AddNewFilterUserControl();
                AddValuesToFilterUC(line, allFilterUserControls.Last());
            }
        }

        private static void AddValuesToFilterUC(string[] line, FilterUserControl filterUserControl)
        {
            filterUserControl.SystemTypeTextBox.Text = line[0];
            filterUserControl.SizeFromTextBox.Text = line[1];
            filterUserControl.SizeToTextBox.Text = line[2];
            filterUserControl.InsulationTypeCombobox.SelectedItem = line[3];
            filterUserControl.InsulationThicknessTextBox.Text = line[4];
            filterUserControl.CommentsTextBox.Text = line[5];
            filterUserControl.CommentsCheckBox.IsChecked = bool.Parse(line[6]);
        }

        private void SaveFilters_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Comma Separated Values File|*.csv";
            saveFileDialog.Title = "Save an CSV File";
            saveFileDialog.ShowDialog();

            string filePath = Path.GetFullPath(saveFileDialog.FileName);
            List<string> allFiltersData = new List<string>();

            foreach (FilterUserControl userControl in allFilterUserControls)
            {
                string filterData = $"{userControl.SystemTypeTextBox.Text},{userControl.SizeFromTextBox.Text},{userControl.SizeToTextBox.Text}" +
                    $",{userControl.InsulationTypeCombobox.Text},{userControl.InsulationThicknessTextBox.Text},{userControl.CommentsTextBox.Text}," +
                    $"{userControl.CommentsCheckBox.IsChecked.ToString()}";
                allFiltersData.Add(filterData);
            }

            File.WriteAllLines(filePath, allFiltersData);
            MessageBox.Show("Filters saved");
        }
    }
}
