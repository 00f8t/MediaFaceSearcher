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
using MahApps.Metro.Controls.Dialogs;
using MediaFaceSearcher.Converters;
using MediaFaceSearcher.DAL;
using MediaFaceSearcher.Model;
using MediaFaceSearcher.Model.Events;
using Unity.Policy;
using System.Diagnostics;

namespace MediaFaceSearcher.ViewModels
{
    class PeoplePageViewModel : BindableBase
    {
        private readonly IPersonDao _personDao;
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly IEventAggregator _eventAggregator;
        private List<Person> _allPersons = new();

        public PeoplePageViewModel(IPersonDao personDao, IDialogCoordinator diaoCoordinator, IEventAggregator eventAggregator)
        {
            CloseFolderCommand = new DelegateCommand(CloseFolder);
            DeletePersonCommand = new DelegateCommand(DeletePerson);
            RenamePersonCommand = new DelegateCommand(RenamePerson);
            DeleteFolderCommand = new DelegateCommand<Folder>(DeleteFolder);
            OpenPhotoCommand = new DelegateCommand<Photo>(OpenPhoto);
            MovePhotoCommand = new DelegateCommand<MovePhotoArgs>(MovePhoto);
            MakePhotoMainCommand = new DelegateCommand<Photo>(MakePhotoMain);
            DeletePhotoCommand = new DelegateCommand<Photo>(DeletePhoto);

            _personDao = personDao;
            _dialogCoordinator = diaoCoordinator;
            _eventAggregator = eventAggregator;


            ValidatePhotos();
            _filteredPersons = new ObservableCollection<Person>(_allPersons);

            _eventAggregator.GetEvent<PersonListChangedEvent>().Subscribe(ValidatePhotos);
        }


        private ObservableCollection<Person> _filteredPersons;
        public ObservableCollection<Person> FilteredPersons
        {
            get => _filteredPersons;
            set => SetProperty(ref _filteredPersons, value);
        }

        private void ValidatePhotos()
        {
            _allPersons = _personDao.Read();
            if (_allPersons.Any())
            {

                List<Person> toDeletePersons = new List<Person>();
                foreach (var person in _allPersons)
                {
                    List<Photo> toDeletePhotos = new List<Photo>();
                    foreach (var photo in person.Photos)
                    {
                        if (!File.Exists(photo.FilePath)) toDeletePhotos.Add(photo);
                        if (photo.DateAdded == DateTime.MinValue) photo.DateAdded = DateTime.Now;
                    }

                    person.Photos.RemoveAll(p => toDeletePhotos.Contains(p));

                    if (person.Photos.Count == 0)
                    {
                        toDeletePersons.Add(person);
                        continue;
                    }

                    if (person.MainPhoto == null)
                    {
                        var lastPhoto = person.Photos.LastOrDefault();
                        person.MainPhoto = new(lastPhoto.FilePath, lastPhoto.FaceBox);
                    }
                }

                _allPersons.RemoveAll(p => toDeletePersons.Contains(p));
            }

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

        private Person? _selectedPerson;
        public Person? SelectedPerson
        {
            get => _selectedPerson;
            set => SetProperty(ref _selectedPerson, value, PersonChanged);
        }

        private void PersonChanged()
        {
            if (_selectedPerson != null)
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
            else
            {
                Folders = new ObservableCollection<Folder>();
            }
        }

        public DelegateCommand CloseFolderCommand { get; }
        private void CloseFolder()
        {
            SelectedFolder = null;
        }


        public DelegateCommand DeletePersonCommand { get; }
        private async void DeletePerson()
        {
            if (SelectedPerson == null) return;
            var result = await _dialogCoordinator.ShowMessageAsync(
                this,
                "Підтвердження видалення",
                "Ви впенені що бажаєте видалити людину? Цю дію неможливо скасувати!",
                MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings()
                {
                    AffirmativeButtonText = "Так",
                    NegativeButtonText = "Ні",
                });
            if (result == MessageDialogResult.Affirmative)
            {
                _allPersons.Remove(SelectedPerson);
                _personDao.Update(_allPersons);
                SelectedPerson = null;
            }
        }


        public DelegateCommand RenamePersonCommand { get; }
        private async void RenamePerson()
        {
            if (_selectedPerson == null)
                return;

            var settings = new MetroDialogSettings
            {
                DefaultText = _selectedPerson.Name,
                AffirmativeButtonText = "ОК",
                NegativeButtonText = "Скасувати",
                AnimateShow = true
            };

            string result = await _dialogCoordinator.ShowInputAsync(this, "Перейменування", "Введіть нове ім’я:", settings);

            if (!string.IsNullOrWhiteSpace(result) && result != _selectedPerson.Name)
            {
                _selectedPerson.Name = result;
                _personDao.Update(_allPersons);
            }
        }


        public DelegateCommand<Folder> DeleteFolderCommand { get; }
        private async void DeleteFolder(Folder folder)
        {
            if (_selectedPerson == null) return;
            var result = await _dialogCoordinator.ShowMessageAsync(
                this,
                "Підтвердження видалення",
                "Ви впенені що бажаєте видалити папку? Цю дію неможливо скасувати!",
                MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings()
                {
                    AffirmativeButtonText = "Так",
                    NegativeButtonText = "Ні",
                });
            if (result == MessageDialogResult.Affirmative)
            {
                _selectedPerson.Photos.RemoveAll(p => p.Emotion.ToString() == folder.Name);

                if(!_selectedPerson.Photos.Any()) 
                    _allPersons.Remove(_selectedPerson);

                _personDao.Update(_allPersons);
            }
        }


        public DelegateCommand<Photo> OpenPhotoCommand { get; }
        private void OpenPhoto(Photo photo)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = photo.FilePath,
                UseShellExecute = true // Required to open with default associated app
            });
        }


        public DelegateCommand<MovePhotoArgs> MovePhotoCommand { get; }
        private void MovePhoto(MovePhotoArgs args)
        {
            _selectedPerson?.Photos.Remove(args.Photo);
            args.Photo.Emotion = Enum.Parse<Emotion>(args.TargetFolderName);
            _selectedPerson?.Photos.Add(args.Photo);
            _personDao.Update(_allPersons);
        }

        public DelegateCommand<Photo> MakePhotoMainCommand { get; }
        private void MakePhotoMain(Photo photo)
        {
            _selectedPerson.MainPhoto = new MainPhoto(photo.FilePath, photo.FaceBox);
            _personDao.Update(_allPersons);
        }


        public DelegateCommand<Photo> DeletePhotoCommand { get; }
        private async void DeletePhoto(Photo photo)
        {
            if (_selectedPerson == null) return;
            var result = await _dialogCoordinator.ShowMessageAsync(
                this,
                "Підтвердження видалення",
                "Ви впенені що бажаєте видалити фото? Цю дію неможливо скасувати!",
                MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings()
                {
                    AffirmativeButtonText = "Так",
                    NegativeButtonText = "Ні",
                });
            if (result == MessageDialogResult.Affirmative)
            {
                _selectedPerson.Photos.Remove(photo);
                //if(File.Exists(photo.FilePath)) File.Delete(photo.FilePath);
            }
        }
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
