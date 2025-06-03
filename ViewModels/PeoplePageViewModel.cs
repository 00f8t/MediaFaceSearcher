using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using MediaFaceSearcher.DAL;
using MediaFaceSearcher.Model;

namespace MediaFaceSearcher.ViewModels
{
    class PeoplePageViewModel : BindableBase
    {
        private readonly IPersonDao _personDao;
        public PeoplePageViewModel(IPersonDao personDao)
        {
            _personDao = personDao;

            AllPersons = new ObservableCollection<Person>(_personDao.Read());
            ValidatePhotos();
        }

        private ObservableCollection<Person> _allPersons = new();
        public ObservableCollection<Person> AllPersons
        {
            get => _allPersons;
            set => SetProperty(ref _allPersons, value);
        }

        private void ValidatePhotos()
        {
            foreach (var person in AllPersons)
            {
                var lastPhoto = person.Photos.LastOrDefault();
                var path = lastPhoto.FilePath;
                var bitmap = new Bitmap(path);
                var croppedImage = bitmap.Clone(lastPhoto.FaceBox, PixelFormat.Format24bppRgb).ToBItmapSource();
                person.MainPhoto = croppedImage;
            }
        }

    }
}
