using System;
using System.Collections.Generic;
using System.Linq;
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
using Autodesk.Revit.DB.Plumbing;
using PipeInsulationPlugin;
using PipeInsulationPlugin.Views.UserControls;

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

        public AddInsulationView(List<PipeModel> pipes, List<PipeModel> filteredPipes, List<InsulationModel> insulations, Document document)
        {
            InitializeComponent();
            allPipes = pipes;
            allFilteredPipes = filteredPipes;
            insulationWithParameters = insulations;
            doc = document;

            insulationTypesList = PipeInsulationPlugin.HelperFunctionalClass.GetInsulationTypes(doc);

            Filter1UC.InsulationTypeCombobox.ItemsSource = PipeInsulationPlugin.HelperFunctionalClass.GetInsulationTypeNames(doc);
            Filter2UC.InsulationTypeCombobox.ItemsSource = PipeInsulationPlugin.HelperFunctionalClass.GetInsulationTypeNames(doc);



        }

        private List<PipeModel> FilterSingleUC(TextBox SystemTypeTB, TextBox sizeFromTB, TextBox sizeToTB)
        {
            SingleFilterPipesList.Clear();


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
                sizeFromTB.Text = "Insert valid value";
            }
            else if (sizeToTB.Text == "")
            {
                sizeTo = 999999;
            }

            //if (double.TryParse(sizeToTB.Text, out double sizeTo) == false && sizeToTB.Text != null)
            //{
            //    sizeToTB.Text = "Insert valid value";
            //}

            foreach (var pipe in allPipes)
            {
                if (pipe.SystemType.ToString().ToLower().Contains(SystemTypeTB.Text.ToString().ToLower())
                    && pipe.PipeNominalSize >= sizeFrom
                    && pipe.PipeNominalSize <= sizeTo)
                {
                    SingleFilterPipesList.Add(pipe);
                }
            }
            if (allFilteredPipes != null)
            {
                MessageBox.Show($"Single Filter: {SingleFilterPipesList.Count}");
            }
            else
            {
                MessageBox.Show("null");
            }
            return SingleFilterPipesList;
        }


        private ElementId GetInsulationTypeId(ComboBox insulationTypeCombobox, List<Element> insulationTypes)
        {
            Element selectedInsulationType = insulationTypes.ElementAt(insulationTypeCombobox.SelectedIndex);

            return selectedInsulationType.Id;
        }




        private void BatchAddInsulationButton_Click(object sender, RoutedEventArgs e)
        {
            allFilteredPipes.Clear();

            allFilteredPipes.AddRange(FilterSingleUC(Filter1UC.SystemTypeTextBox, Filter1UC.SizeFromTextBox, Filter1UC.SizeToTextBox));
            allFilteredPipes.AddRange(FilterSingleUC(Filter2UC.SystemTypeTextBox, Filter2UC.SizeFromTextBox, Filter2UC.SizeToTextBox));


            if (allFilteredPipes != null)
            {
                MessageBox.Show($"Total Filter: {allFilteredPipes.Count}");
            }
            else
            {
                MessageBox.Show("null");
            }

            HelperFunctionalClass.DeleteInsulationForFilteredPipes(doc, allFilteredPipes);




                using (Transaction t = new Transaction(doc))
            {

                double insulationThickness = double.Parse(Filter1UC.InsulationThicknessTextBox.Text.ToString());
                double insulationThicknessConversion = UnitUtils.ConvertToInternalUnits(insulationThickness, DisplayUnitType.DUT_METERS)/1000;

                t.Start("BatchAddInsulation");


                ElementId insulationTypeIdforFilter1 = GetInsulationTypeId(Filter1UC.InsulationTypeCombobox, insulationTypesList);

                foreach (var element in allFilteredPipes)
                {
                    _ = PipeInsulation.Create(doc, element.Id, insulationTypeIdforFilter1, insulationThicknessConversion);
                }

                t.Commit();
            }

            MessageBox.Show("Insulation added successfully");

        }
    }
}
