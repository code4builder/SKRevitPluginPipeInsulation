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

namespace PipeInsulationPlugin.Views
{
    /// <summary>
    /// Interaction logic for UserWindowPipes.xaml
    /// </summary>
    public partial class UserWindowPipes : Window
    {
        List<PipeModel> pipesWithParams;
        List<InsulationModel> insulationWithParameters;
        Document doc;

        public UserWindowPipes(Document document, List<PipeModel> pipes, List<InsulationModel> insulations)
        {
            InitializeComponent();

            doc = document;
            pipesWithParams = pipes;
            PipesListView.ItemsSource = pipesWithParams;

            string[] SortingComboBoxArray = { "Pipe Type", "Size", "System Classification", "System Type", "Insulation Type", "Insulation Thickness", "Comments" };

            AddToCombo(SortingComboBoxArray, SortByPrimaryKeyComboBox);
            AddToCombo(SortingComboBoxArray, SortBySecondaryKeyComboBox);

            PipesListView.Items.SortDescriptions.Add(new SortDescription("SystemType", ListSortDirection.Ascending));
            PipesListView.Items.SortDescriptions.Add(new SortDescription("PipeNominalSize", ListSortDirection.Ascending));
            SortBtn.Click += SortBtn_Click;

            FilterUserControl filter1 = new FilterUserControl();

            string propertyForFilteringString = ConvertComboboxItemToProperty(filter1.Filter1ComboBox.SelectedItem.ToString());
            //PipesListView.Items.Filter = PipeTypeFilter(filter1);

            TextBox texBoxForFiltering = filter1.TextBoxForFilter;

            Type type = typeof(PipeModel);
            PropertyInfo propertyForFiltering = type.GetProperty(propertyForFilteringString);
            ListView pipesFilteredListView = new ListView();
            pipesFilteredListView = PipesListView;

            //pipesFilteredListView.Items.Filter = PipeTypeFilter;


            insulationWithParameters = insulations;


            //PipeInsulationListWindow pipeInsulationListWindow = new PipeInsulationListWindow(insulationWithParameters);
            //InsulationListView.ItemsSource = insulationWithParameters;
        }

        //private bool PipeTypeFilter(object obj)
        //{
        //    var Filterobj = obj as PipeModel;
        //    return Filterobj.PipeName.Contains(texBoxForFiltering.Text, StringComparison.OrdinalIgnoreCase);
        //}

        //private bool PipeTypeFilter(object obj, FilterUserControl filter)
        //{
        //    var FilterObj = obj as PipeModel;
        //    return FilterObj.PipeName.Contains(TextBoxForFilter);
        //}

        //private PropertyInfo PropertyInfoByName(string name)
        //{
        //    Type type = this.GetType();
        //    PropertyInfo info = type.GetProperty(name);
        //    if (info == null)
        //        throw new Exception(String.Format(
        //          "A property called {0} can't be accessed for type {1}.",
        //          name, type.FullName));
        //    return info;
        //}

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
            else propertyForSorting = "InsulationThickness";
            return propertyForSorting;
        }

        public void SortList()
        {
            var SortProperty = SortByPrimaryKeyComboBox.SelectedItem.ToString();
            string primaryPropertyForSorting = "SystemType";
            string secondaryPropertyForSorting = "PipeNominalSize";

            primaryPropertyForSorting = ConvertComboboxItemToProperty(SortByPrimaryKeyComboBox.SelectedItem.ToString());
            secondaryPropertyForSorting = ConvertComboboxItemToProperty(SortBySecondaryKeyComboBox.SelectedItem.ToString());

            PipesListView.Items.SortDescriptions[0] = new SortDescription(primaryPropertyForSorting, ListSortDirection.Ascending);
            PipesListView.Items.SortDescriptions[1] = new SortDescription(secondaryPropertyForSorting, ListSortDirection.Ascending);
        }

        private void SortBtn_Click(object sender, RoutedEventArgs e)
        {
            SortList();
        }

        public static void AddToCombo(Array array, ComboBox comboBox)
        {
            foreach (var item in array)
            {
                comboBox.Items.Add(item);
            }
        }
        private void AddInsulation_Click(object sender, RoutedEventArgs e)
        {
            InsulationModel.CreatePipeInsulation(doc);
        }
    }
}
