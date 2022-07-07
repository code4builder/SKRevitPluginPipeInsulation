using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Microsoft.Win32;
using PipeInsulationPlugin.Views.UserControls;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PipeInsulationPlugin.Views
{
    /// <summary>
    /// Interaction logic for AddInsulationView.xaml
    /// </summary>
    public partial class AddInsulationView : Window
    {
        List<PipeModel> allPipes;
        List<PipeModel> allFilteredPipes;
        List<PipeModel> SingleFilterPipesList = new List<PipeModel>();
        List<InsulationModel> insulationWithParameters;
        Document doc;
        List<Element> insulationTypesList;
        List<FilterUserControl> allFilterUserControls = new List<FilterUserControl>();

        public AddInsulationView(List<PipeModel> pipes, List<PipeModel> filteredPipes, List<InsulationModel> insulations, Document document)
        {
            InitializeComponent();
            allPipes = pipes;
            allFilteredPipes = filteredPipes;
            insulationWithParameters = insulations;
            doc = document;

            insulationTypesList = HelperFunctionalClass.GetInsulationTypes(doc);

            Filter1UC.InsulationTypeCombobox.ItemsSource = PipeInsulationPlugin.HelperFunctionalClass.GetInsulationTypeNames(doc);

            allFilterUserControls.Add(Filter1UC);
        }

        private List<PipeModel> FilterSingleUC(TextBox SystemTypeTB, TextBox sizeFromTB, TextBox sizeToTB, TextBox CommentsTB)
        {
            SingleFilterPipesList.Clear();

            double sizeFrom = HelperFunctionalClass.GetPipeSizeFromTextBox(sizeFromTB, sizeToTB).Item1;
            double sizeTo = HelperFunctionalClass.GetPipeSizeFromTextBox(sizeFromTB, sizeToTB).Item2;

            foreach (var pipe in allPipes)
            {
                if (CommentsTB.Text != "")
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
                else if (pipe.SystemType.ToString().ToLower().Contains(SystemTypeTB.Text.ToString().ToLower())
                        && pipe.PipeNominalSize >= sizeFrom
                        && pipe.PipeNominalSize <= sizeTo)
                {
                    SingleFilterPipesList.Add(pipe);
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
            newFilter.FilterNameLabel.Content = "FILTER " + (BatchAddingStackPanel.Children.Count + 1).ToString();

            BatchAddingStackPanel.Children.Add(newFilter);
            allFilterUserControls.Add(newFilter);

            newFilter.InsulationTypeCombobox.ItemsSource = PipeInsulationPlugin.HelperFunctionalClass.GetInsulationTypeNames(doc);

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
            allFilteredPipes.Clear();

            foreach (FilterUserControl userControl in allFilterUserControls)
            {
                allFilteredPipes.AddRange(FilterSingleUC(userControl.SystemTypeTextBox, userControl.SizeFromTextBox, userControl.SizeToTextBox, userControl.CommentsTextBox));
            }

            HelperFunctionalClass.DeleteInsulationForFilteredPipes(doc, allFilteredPipes);

            foreach (FilterUserControl userControl in allFilterUserControls)
            {
                using (Transaction t = new Transaction(doc))
                {
                    t.Start("BatchAddInsulation");

                    double insulationThickness = double.Parse(userControl.InsulationThicknessTextBox.Text.ToString());
                    double insulationThicknessConversion = UnitUtils.ConvertToInternalUnits(insulationThickness, DisplayUnitType.DUT_METERS) / 1000;

                    ElementId insulationTypeIdforSingleFilter = HelperFunctionalClass.GetInsulationTypeId(userControl.InsulationTypeCombobox, insulationTypesList);

                    List<PipeModel> SingleFilterPipes = new List<PipeModel>();
                    SingleFilterPipes = FilterSingleUC(userControl.SystemTypeTextBox, userControl.SizeFromTextBox, userControl.SizeToTextBox, userControl.CommentsTextBox);

                    foreach (var element in SingleFilterPipes)
                    {
                        _ = PipeInsulation.Create(doc, element.Id, insulationTypeIdforSingleFilter, insulationThicknessConversion);
                    }
                    t.Commit();
                }
            }

            MessageBox.Show("Insulation added successfully");

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
            List<string[]> linesListFromSecond =  linesFromCSV.ToList();
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
                    $",{userControl.InsulationTypeCombobox.Text},{userControl.InsulationThicknessTextBox.Text},{userControl.CommentsTextBox.Text}";
                allFiltersData.Add(filterData);
            }

            File.WriteAllLines(filePath, allFiltersData);
            MessageBox.Show("Filters saved");
        }
    }
}
