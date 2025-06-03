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
using System.Windows.Shapes;
using MahApps.Metro.Controls.Dialogs;
using MediaFaceSearcher.DAL;
using MediaFaceSearcher.Model;
using MediaFaceSearcher.ViewModels;

namespace MediaFaceSearcher.Views
{
    /// <summary>
    /// Interaction logic for AddingPersonView.xaml
    /// </summary>
    public partial class AddingPersonView
    {
        public AddingPersonView(List<PotentialPerson> RecentPersons, List<Person> allPersons)
        {
            InitializeComponent();
            (DataContext as AddingPersonViewModel).Initialize(RecentPersons, allPersons, this);
        }
    }

}
