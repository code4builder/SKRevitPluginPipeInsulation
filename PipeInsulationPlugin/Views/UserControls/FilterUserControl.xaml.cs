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
using PipeInsulationPlugin.Views;

namespace PipeInsulationPlugin.Views.UserControls
{
    /// <summary>
    /// Interaction logic for FilterUserControl.xaml
    /// </summary>
    public partial class FilterUserControl : UserControl
    {
        List<PipeModel> allPipes = new List<PipeModel>();
        public FilterUserControl()
        {
            InitializeComponent();

            //allPipes = allPipesList;

            string[] FilteringComboBoxArray = { "Pipe Type", "System Classification", "System Type", "Insulation Type", "Comment"};

            foreach (var item in FilteringComboBoxArray)
            {
                Filter1ComboBox.Items.Add(item);
            }

            //pipesFilteredListView.Items.Filter = PipeTypeFilter;
        }

        //public Predicate<object> GetFilter()
        //{
        //    if (TextBoxForFilter.Text == "Pipe Type")
        //        return PipeTypeFilter;
        //    else if (TextBoxForFilter.Text == "System Classification")
        //        return SystemClassificationFilter;
        //    else if (TextBoxForFilter.Text == "System Type")
        //        return SystemTypeFilter;
        //    else if (TextBoxForFilter.Text == "Insulation Type")
        //        return InsulationTypeFilter;
        //    else
        //        return CommentsFilter;
        //}


        //private bool PipeTypeFilter(object obj)
        //{
        //    var Filterobj = obj as PipeModel;
        //    return Filterobj.PipeName.Contains(TextBoxForFilter.Text.ToString());
        //}

        //private bool SystemClassificationFilter(object obj)
        //{
        //    var Filterobj = obj as PipeModel;
        //    return Filterobj.SystemClassification.Contains(TextBoxForFilter.Text.ToString());
        //}

        //private bool SystemTypeFilter(object obj)
        //{
        //    var Filterobj = obj as PipeModel;
        //    return Filterobj.SystemType.Contains(TextBoxForFilter.Text.ToString());
        //}
        //private bool InsulationTypeFilter(object obj)
        //{
        //    var Filterobj = obj as PipeModel;
        //    return Filterobj.InsulationType.Contains(TextBoxForFilter.Text.ToString());
        //}
        //private bool CommentsFilter(object obj)
        //{
        //    var Filterobj = obj as PipeModel;
        //    return Filterobj.Comments.Contains(TextBoxForFilter.Text.ToString());
        //}

        //private void FilterApplyBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    if (TextBoxForFilter.Text == null)
        //    {
        //        TextBoxForFilter.Text = "Insert Something";
        //    }
        //    else
        //    {
        //        pipesFilteredListView.Items.Filter = GetFilter();
        //    }
        //}

        //private void Filter1ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    pipesFilteredListView.Items.Filter = GetFilter();
        //}
    }
}
