using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;
using MediaFaceSearcher.DAL;
using MediaFaceSearcher.Model;
using MediaFaceSearcher.Views;
using SixLabors.ImageSharp;
using Point = SixLabors.ImageSharp.Point;

namespace MediaFaceSearcher.ViewModels
{
    class AddingPersonViewModel : BindableBase
    {
        private AddingPersonView _view;
        private List<PotentialPerson> _persons = new();
        private IPersonDao _personDao;
        private IDialogCoordinator _dialogCoordinator;
        public AddingPersonViewModel(IPersonDao personDao, IDialogCoordinator dialogCoordinator)
        {
            NextPhotoCommand = new DelegateCommand(NextPhoto, CanGoNext);
            PreviousPhotoCommand = new DelegateCommand(PreviousPhoto, CanGoPrevious);
            DeletePhotoCommand = new DelegateCommand(DeletePhoto);

            _personDao = personDao;
            _dialogCoordinator = dialogCoordinator;
        }

        public void Initialize(List<PotentialPerson> persons, List<Person> allPersons, AddingPersonView view)
        {
            _view = view;
            _persons = persons;
            AllPersons.AddRange(allPersons);
            if (_persons is { Count: > 0 })
            {
                CurrentPerson = _persons[0];
                SelectedPerson = CurrentPerson.ClosestPerson;
            }
            if (SelectedPerson != null)
            {
                Name = SelectedPerson.Name;
            }
            else
            {
                SelectedPerson = _allPersons.FirstOrDefault();
                Name = string.Empty;
            }
            Emotion = CurrentPerson.Emotion;
            DeletePhotoCommand.RaiseCanExecuteChanged();
        }

        private PotentialPerson _currentPerson;
        public PotentialPerson CurrentPerson
        {
            get => _currentPerson;
            set => SetProperty(ref _currentPerson, value);
        }


        private Person _selectedPerson;
        public Person SelectedPerson
        {
            get => _selectedPerson;
            set => SetProperty(ref _selectedPerson, value, OnSelectedPersonChanged);
        }


        private bool _canChangeName = true;
        public bool CanChangeName
        {
            get => _canChangeName;
            set => SetProperty(ref _canChangeName, value);
        }


        private ObservableCollection<Person> _allPersons = new() { new Person() { Name = "<Додати нову>" } };
        public ObservableCollection<Person> AllPersons
        {
            get => _allPersons;
            set => SetProperty(ref _allPersons, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value, NextPhotoCommand.RaiseCanExecuteChanged);
        }

        private Emotion _emotion;
        public Emotion Emotion
        {
            get => _emotion;
            set => SetProperty(ref _emotion, value);
        }


        private void OnSelectedPersonChanged()
        {
            if(_selectedPerson == null) return;
            CanChangeName = _selectedPerson == _allPersons.First();
            if (!CanChangeName)
            {
                Name = _selectedPerson.Name;
            }
        }


        public DelegateCommand NextPhotoCommand { get; }
        private async void NextPhoto()
        {
            CurrentPerson.Name = Name;
            CurrentPerson.Emotion = Emotion;
            CurrentPerson.ClosestPerson = SelectedPerson;

            OnPageChanged();
            int currentIndex = _persons.IndexOf(CurrentPerson);
            if (currentIndex < _persons.Count - 1)
            {
                CurrentPerson = _persons[currentIndex + 1];
                if (CurrentPerson.ClosestPerson != null)
                {
                    SelectedPerson = CurrentPerson.ClosestPerson;
                    Name = SelectedPerson.Name;
                }
                else
                {
                    SelectedPerson = _allPersons.First();
                    Name = string.Empty;
                }

                Emotion = CurrentPerson.Emotion;
            }
            else
            {
                var result = await _dialogCoordinator.ShowMessageAsync(
                    this,
                    "Завершення додавання",
                    "Всі фотографії додані. Натисніть 'Готово' для збереження.",
                    MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings()
                    {
                        AffirmativeButtonText = "Готово",
                        NegativeButtonText = "Скасувати",
                    });

                if (result == MessageDialogResult.Affirmative)
                {
                    SavePersons();
                    _view.DialogResult = true;
                    _view.Close();
                }
            }
        }

        private void SavePersons()
        {
            foreach (var person in _persons)
            {
                var photo = new Photo()
                {
                    Embedding = person.Embedding,
                    Emotion = person.Emotion,
                    FilePath = person.FilePath,
                    FaceBox = person.FaceBox,
                    DateAdded = DateTime.Now
                };

                if (SelectedPerson != _allPersons.First())
                {
                    _allPersons[_allPersons.IndexOf(person.ClosestPerson)].Photos.Add(photo);
                }
                else
                {
                    _allPersons.Add(new Person()
                    {
                        Name = person.Name,
                        Photos = new List<Photo> { photo }
                    });

                }
            }

            _allPersons.RemoveAt(0);
            _personDao.Update(_allPersons.ToList());
        }

        public DelegateCommand PreviousPhotoCommand { get; }
        private void PreviousPhoto()
        {
            OnPageChanged();
            int currentIndex = _persons.IndexOf(CurrentPerson);
            if (currentIndex > 0)
            {
                CurrentPerson = _persons[currentIndex - 1];
                if (CurrentPerson.ClosestPerson != null && CurrentPerson.ClosestPerson != _allPersons.First()) 
                {
                    SelectedPerson = CurrentPerson.ClosestPerson;
                    Name = SelectedPerson.Name;
                }
                else
                {
                    SelectedPerson = _allPersons.First();
                    Name = CurrentPerson.Name;
                }

                Emotion = CurrentPerson.Emotion;
            }
        }

        private void OnPageChanged()
        {
            PreviousPhotoCommand.RaiseCanExecuteChanged();
        }

        public bool CanGoPrevious() => _persons.IndexOf(CurrentPerson) > 0;
        public bool CanGoNext() => !string.IsNullOrEmpty(Name);

        public DelegateCommand DeletePhotoCommand { get; }
        private void DeletePhoto()
        {
            if (_persons.Count == 1)
            {
                _view.DialogResult = false;
                _view.Close();
                return;
            }

            int currentIndex = _persons.IndexOf(CurrentPerson);
            if (currentIndex >= 0)
            {
                DeletePhotoCommand.RaiseCanExecuteChanged();
                _persons.RemoveAt(currentIndex);
                if (_persons.Count > 0)
                {
                    OnPageChanged();
                    CurrentPerson = _persons[Math.Min(currentIndex, _persons.Count - 1)];
                    if (CurrentPerson.ClosestPerson != null && CurrentPerson.ClosestPerson != _allPersons.First())
                    {
                        SelectedPerson = CurrentPerson.ClosestPerson;
                        Name = SelectedPerson.Name;
                    }
                    else
                    {
                        SelectedPerson = _allPersons.First();
                        Name = CurrentPerson.Name;
                    }

                    Emotion = CurrentPerson.Emotion;
                }
            }
        }
    }
}
