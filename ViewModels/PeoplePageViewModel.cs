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
using Unity.Policy;

namespace MediaFaceSearcher.ViewModels
{
    class PeoplePageViewModel : BindableBase
    {
        private readonly IPersonDao _personDao;
        private readonly List<Person> _allPersons = new();
        public PeoplePageViewModel(IPersonDao personDao)
        {
            CloseFolderCommand = new DelegateCommand(CloseFolder);

            _personDao = personDao;

            _allPersons = _personDao.Read();
            _filteredPersons = new ObservableCollection<Person>(_allPersons);

            ValidatePhotos();
            _personDao.PersonChanged += (_, _) => ValidatePhotos();
        }


        private ObservableCollection<Person> _filteredPersons;
        public ObservableCollection<Person> FilteredPersons
        {
            get => _filteredPersons;
            set => SetProperty(ref _filteredPersons, value);
        }

        private void ValidatePhotos()
        {
            if (_allPersons.Count == 0) return;

            List<Person> toDeletePersons = new List<Person>();
            foreach (var person in _allPersons)
            {
                var lastPhoto = person.Photos.LastOrDefault();
                var path = lastPhoto.FilePath;
                var bitmap = new Bitmap(path);
                var croppedImage = bitmap.Clone(lastPhoto.FaceBox, PixelFormat.Format24bppRgb).ToBItmapSource();
                person.MainPhoto = croppedImage;


                List<Photo> toDeletePhotos = new List<Photo>();
                foreach (var photo in person.Photos)
                {
                    if(!File.Exists(photo.FilePath)) toDeletePhotos.Add(photo);
                    if(photo.DateAdded == DateTime.MinValue) photo.DateAdded = DateTime.Now;
                }
                person.Photos.RemoveAll(p => toDeletePhotos.Contains(p));

                if(person.Photos.Count == 0)
                {
                    toDeletePersons.Add(person);
                }
            }
            _allPersons.RemoveAll(p => toDeletePersons.Contains(p));

            FilteredPersons = new ObservableCollection<Person>(_allPersons);
        }


        private Folder _selectedFolder;
        public Folder SelectedFolder
        {
            get => _selectedFolder;
            set => SetProperty(ref _selectedFolder, value);
        }

        private ObservableCollection<Folder> _folders;
        public ObservableCollection<Folder> Folders
        {
            get => _folders;
            set => SetProperty(ref _folders, value);
        }

        private Person _selectedPerson;
        public Person SelectedPerson
        {
            get => _selectedPerson;
            set => SetProperty(ref _selectedPerson, value, PersonChanged);
        }

        private void PersonChanged()
        {
            var grouped = _selectedPerson.Photos
                .GroupBy(p => p.Emotion)
                .Select(g => new Folder
                {
                    Name = g.Key.ToString(),
                    Photos = g.ToList()
                });

            Folders = new ObservableCollection<Folder>(grouped);
        }

        public DelegateCommand CloseFolderCommand { get; }
        private void CloseFolder()
        {
            SelectedFolder = null;
        }
    }

    public class Folder
    {
        public string Name { get; set; }
        public List<Photo> Photos { get; set; } = new List<Photo>();

        public override string ToString()
        {
            return Name;
        }
    }
}
