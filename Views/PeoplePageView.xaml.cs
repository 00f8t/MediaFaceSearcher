using MediaFaceSearcher.Model;
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

namespace MediaFaceSearcher.Views
{
    /// <summary>
    /// Interaction logic for PeoplePageView.xaml
    /// </summary>
    public partial class PeoplePageView : UserControl
    {
        public PeoplePageView()
        {
            InitializeComponent();
        }

        private void Grid_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            if (grid?.DataContext is Folder folder)
            {
                var listViewItem = ItemsControl.ContainerFromElement(Groups, grid) as ListViewItem;
                if (listViewItem != null)
                {
                    e.Handled = true; // Важливо: щоб не викликався SelectedFolder on click
                    //listViewItem.IsSelected = true;
                }
            }
        }
    }
}
