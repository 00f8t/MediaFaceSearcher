using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MediaFaceSearcher.Model;

namespace MediaFaceSearcher.ViewModels
{
    class AddingPersonViewModel : BindableBase
    {
        private List<PotentialPerson> _persons;

        public AddingPersonViewModel(List<PotentialPerson> persons, List<Person> allPersons)
        {
            NextPhotoCommand = new DelegateCommand(NextPhoto, CanGoNext);
            PreviousPhotoCommand = new DelegateCommand(PreviousPhoto, CanGoPrevious);
            DeletePhotoCommand = new DelegateCommand(DeletePhoto);

            _persons = persons;

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

            if (SelectedPerson != null) CanChangeName = false;

            AllPersons.AddRange(allPersons);
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


        private List<Person> _allPersons = new() { new Person() { Name = "<Додати нову>" } };

        public List<Person> AllPersons
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
            CanChangeName = _selectedPerson.Name == "<Додати нову>";
            if (!CanChangeName)
            {
                Name = SelectedPerson.Name;
            }
        }


        public DelegateCommand NextPhotoCommand { get; }

        private void NextPhoto()
        {
            OnPageChanged();
            int currentIndex = _persons.IndexOf(CurrentPerson);
            if (currentIndex < _persons.Count - 1)
            {
                if (CurrentPerson.ClosestPerson != _allPersons.First())
                {
                    CurrentPerson.ClosestPerson.Photos.Add(new Photo()
                    {
                        Emotion = Emotion,
                        FilePath = CurrentPerson.FilePath,
                        Embedding = CurrentPerson.Embedding,
                        FaceBox = CurrentPerson.FaceDetectorResult.Box,
                        Keypoints = CurrentPerson.FaceDetectorResult.Landmarks
                    });
                }

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
                
            }
        }

        public DelegateCommand PreviousPhotoCommand { get; }

        private void PreviousPhoto()
        {
            OnPageChanged();
            int currentIndex = _persons.IndexOf(CurrentPerson);
            if (currentIndex > 0)
            {
                CurrentPerson = _persons[currentIndex - 1];
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
            int currentIndex = _persons.IndexOf(CurrentPerson);
            if (currentIndex >= 0)
            {
                _persons.RemoveAt(currentIndex);
                if (_persons.Count > 0)
                {
                    OnPageChanged();
                    CurrentPerson = _persons[Math.Min(currentIndex, _persons.Count - 1)];
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
                    MessageBox.Show("No more photos to display. Please add new photos or exit.");
                }
            }
        }
    }
}
