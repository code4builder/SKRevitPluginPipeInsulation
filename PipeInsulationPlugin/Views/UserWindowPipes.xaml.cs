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

            insulationWithParameters = insulations;

            //PipeInsulationListWindow pipeInsulationListWindow = new PipeInsulationListWindow(insulationWithParameters);

            //InsulationListView.ItemsSource = insulationWithParameters;

        }

        private void AddInsulation_Click(object sender, RoutedEventArgs e)
        {
            InsulationModel.CreatePipeInsulation(doc);
        }
    }
}
