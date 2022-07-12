using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace SKRevitPluginPipeInsulation.Views
{
    /// <summary>
    /// Interaction logic for UserWindowPipes.xaml
    /// </summary>
    public partial class UserWindowPipes : Window
    {
        List<PipeModel> pipesWithParams;
        List<ElementModel> allFilteredPipes;
        List<InsulationModel> insulationWithParameters;
        List<PipeFittingModel> pipesFittingsWithParameters;
        Document doc;

        public UserWindowPipes(Document document, List<PipeModel> pipes, 
            List<InsulationModel> insulations, List<PipeFittingModel> pipesFittings)
        {
            InitializeComponent();

            doc = document;
            pipesWithParams = pipes;
            PipesListView.ItemsSource = pipesWithParams;
            pipesFittingsWithParameters = pipesFittings;
            insulationWithParameters = insulations;

            string[] SortingComboBoxArray = { "Pipe Type", "Size", "System Classification", "System Type", "Insulation Type", "Insulation Thickness", "Comments" };

            HelperFunctionalClass.AddToCombo(SortingComboBoxArray, SortByPrimaryKeyComboBox);
            HelperFunctionalClass.AddToCombo(SortingComboBoxArray, SortBySecondaryKeyComboBox);

            PipesListView.Items.SortDescriptions.Add(new SortDescription("SystemType", ListSortDirection.Ascending));
            PipesListView.Items.SortDescriptions.Add(new SortDescription("PipeNominalSize", ListSortDirection.Ascending));
            SortBtn.Click += SortBtn_Click;

            string[] FilteringComboBoxArray = { "Pipe Type", "System Classification", "System Type", "Insulation Type", "Comment" };
            HelperFunctionalClass.AddToCombo(FilteringComboBoxArray, Filter1ComboBox);

            InsulationTypeComboBox.ItemsSource = HelperFunctionalClass.GetInsulationTypeNames(doc);
        }

        private void sortByColumnHeaderProperty(string primaryPropertyForSorting)
        {
            int charactersToRemoveForAccessToProperty = 46;
            primaryPropertyForSorting = primaryPropertyForSorting.Substring(charactersToRemoveForAccessToProperty);

            primaryPropertyForSorting = HelperFunctionalClass.ConvertComboboxItemToProperty(primaryPropertyForSorting);

            PipesListView.Items.SortDescriptions[0] = new SortDescription(primaryPropertyForSorting, ListSortDirection.Ascending);
        }

        private void lvColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            sortByColumnHeaderProperty(sender.ToString());
        }

        public void SortList()
        {
            var SortProperty = SortByPrimaryKeyComboBox.SelectedItem.ToString();
            string primaryPropertyForSorting = "SystemType";
            string secondaryPropertyForSorting = "PipeNominalSize";

            primaryPropertyForSorting = HelperFunctionalClass.ConvertComboboxItemToProperty(SortByPrimaryKeyComboBox.SelectedItem.ToString());
            secondaryPropertyForSorting = HelperFunctionalClass.ConvertComboboxItemToProperty(SortBySecondaryKeyComboBox.SelectedItem.ToString());

            PipesListView.Items.SortDescriptions[0] = new SortDescription(primaryPropertyForSorting, ListSortDirection.Ascending);
            PipesListView.Items.SortDescriptions[1] = new SortDescription(secondaryPropertyForSorting, ListSortDirection.Ascending);
        }

        private void SortBtn_Click(object sender, RoutedEventArgs e)
        {
            SortList();
        }

        public Predicate<object> GetFilter()
        {
            if (Filter1ComboBox.Text == "Pipe Type")
                return PipeTypeFilter;
            else if (Filter1ComboBox.Text == "System Classification")
                return SystemClassificationFilter;
            else if (Filter1ComboBox.Text == "System Type")
                return SystemTypeFilter;
            else if (Filter1ComboBox.Text == "Insulation Type")
                return InsulationTypeFilter;
            else
                return CommentsFilter;
        }

        private bool PipeTypeFilter(object obj)
        {
            var Filterobj = obj as PipeModel;
            return Filterobj.PipeName.ToLower().Contains(TextBoxForFilter.Text.ToString().ToLower());
        }

        private bool SystemClassificationFilter(object obj)
        {
            var Filterobj = obj as PipeModel;
            return Filterobj.SystemClassification.ToLower().Contains(TextBoxForFilter.Text.ToString().ToLower());
        }

        private bool SystemTypeFilter(object obj)
        {
            var Filterobj = obj as PipeModel;
            return Filterobj.SystemType.ToLower().Contains(TextBoxForFilter.Text.ToString().ToLower());
        }
        private bool InsulationTypeFilter(object obj)
        {
            var Filterobj = obj as PipeModel;
            return Filterobj.InsulationType.ToLower().Contains(TextBoxForFilter.Text.ToString().ToLower());
        }
        private bool CommentsFilter(object obj)
        {
            var Filterobj = obj as PipeModel;
            return Filterobj.Comments.ToLower().Contains(TextBoxForFilter.Text.ToString().ToLower());
        }

        private void FilterApplyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxForFilter.Text == null)
            {
                PipesListView.Items.Filter = null;
            }
            else
            {
                PipesListView.Items.Filter = GetFilter();

                double sizeFrom = HelperFunctionalClass.GetPipeSizeFromTextBox(SizeFromTextBox, SizeToTextBox).Item1;
                double sizeTo = HelperFunctionalClass.GetPipeSizeFromTextBox(SizeFromTextBox, SizeToTextBox).Item2;

                List<PipeModel> filteredPipesByComboBox = new List<PipeModel>();

                foreach (var item in PipesListView.Items)
                {
                    filteredPipesByComboBox.Add(item as PipeModel);
                }

                List<ElementModel> filteredPipesBySize = new List<ElementModel>();

                foreach (var pipe in filteredPipesByComboBox)
                {
                    if (pipe.PipeNominalSize >= sizeFrom && pipe.PipeNominalSize <= sizeTo)
                    {
                        filteredPipesBySize.Add(pipe);
                    }
                }
                PipesListView.ItemsSource = filteredPipesBySize;
                allFilteredPipes = filteredPipesBySize;
            }
        }

        private void AddInsulation_Click(object sender, RoutedEventArgs e)
        {
            if (allFilteredPipes.Count != 0 && InsulationThicknessTextBox.Text != "")
            {
                InsulationModel.CreatePipeInsulation(doc, allFilteredPipes, InsulationTypeComboBox, InsulationThicknessTextBox);
            }
            else if (allFilteredPipes.Count == 0)
            {
                MessageBox.Show("Please add filter");
            }
            else if (InsulationThicknessTextBox.Text == "")
            {
                MessageBox.Show("Please add insulation thickness");
            }
        }

        private void BatchAddingInsulationBtn_Click(object sender, RoutedEventArgs e)
        {
            AddInsulationView addInsulationView = new AddInsulationView(pipesWithParams, pipesFittingsWithParameters, insulationWithParameters, doc);
            addInsulationView.Owner = this;
            addInsulationView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            addInsulationView.Show();
        }

        private void RefreshListView_Click(object sender, RoutedEventArgs e)
        {
            pipesWithParams = HelperFunctionalClass.GetAllParametersToLists(doc).Item1;
            PipesListView.ItemsSource = pipesWithParams;
        }

        private void YouTubeLinkButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Revit Plugin - Pipe Insulation \n \nVersion 1.0.0" +
                "\n \nDeveloped by Sergey Kuleshov \n \nEmail: code4builder@google.com");
        }
    }
}
