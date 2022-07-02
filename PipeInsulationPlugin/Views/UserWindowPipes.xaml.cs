using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autodesk.Revit.DB;
using PipeInsulationPlugin.Views.UserControls;
using PipeInsulationPlugin.Models;

namespace PipeInsulationPlugin.Views
{
    /// <summary>
    /// Interaction logic for UserWindowPipes.xaml
    /// </summary>
    public partial class UserWindowPipes : Window
    {
        List<PipeModel> pipesWithParams;
        List<PipeModel> allFilteredPipes;
        List<InsulationModel> insulationWithParameters;
        Document doc;

        public UserWindowPipes(Document document, List<PipeModel> pipes, List<InsulationModel> insulations, List<PipeModel> filteredPipes)
        {
            InitializeComponent();

            doc = document;
            pipesWithParams = pipes;
            PipesListView.ItemsSource = pipesWithParams;
            allFilteredPipes = filteredPipes;
            insulationWithParameters = insulations;

            string[] SortingComboBoxArray = { "Pipe Type", "Size", "System Classification", "System Type", "Insulation Type", "Insulation Thickness", "Comments" };

            HelperFunctionalClass.AddToCombo(SortingComboBoxArray, SortByPrimaryKeyComboBox);
            HelperFunctionalClass.AddToCombo(SortingComboBoxArray, SortBySecondaryKeyComboBox);

            PipesListView.Items.SortDescriptions.Add(new SortDescription("SystemType", ListSortDirection.Ascending));
            PipesListView.Items.SortDescriptions.Add(new SortDescription("PipeNominalSize", ListSortDirection.Ascending));
            SortBtn.Click += SortBtn_Click;

            string[] FilteringComboBoxArray = { "Pipe Type", "System Classification", "System Type", "Insulation Type", "Comment" };
            HelperFunctionalClass.AddToCombo(FilteringComboBoxArray, Filter1ComboBox);

            

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
            return Filterobj.PipeName.Contains(TextBoxForFilter.Text.ToString());
        }

        private bool SystemClassificationFilter(object obj)
        {
            var Filterobj = obj as PipeModel;
            return Filterobj.SystemClassification.Contains(TextBoxForFilter.Text.ToString());
        }

        private bool SystemTypeFilter(object obj)
        {
            var Filterobj = obj as PipeModel;
            return Filterobj.SystemType.Contains(TextBoxForFilter.Text.ToString());
        }
        private bool InsulationTypeFilter(object obj)
        {
            var Filterobj = obj as PipeModel;
            return Filterobj.InsulationType.Contains(TextBoxForFilter.Text.ToString());
        }
        private bool CommentsFilter(object obj)
        {
            var Filterobj = obj as PipeModel;
            return Filterobj.Comments.Contains(TextBoxForFilter.Text.ToString());
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
            }
        }







        private void AddInsulation_Click(object sender, RoutedEventArgs e)
        {
            InsulationModel.CreatePipeInsulation(doc);
        }

        private void BatchAddingInsulationBtn_Click(object sender, RoutedEventArgs e)
        {
            AddInsulationView addInsulationView = new AddInsulationView(pipesWithParams, allFilteredPipes, insulationWithParameters, doc);
            addInsulationView.Show();
        }
    }
}
