using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaFaceSearcher.Model;

namespace MediaFaceSearcher.ViewModels
{
    class AddingPersonViewModel : BindableBase
    {
        private List<PotentialPerson> _persons;

        public AddingPersonViewModel(List<PotentialPerson> persons, List<Person> allPersons)
        {
            _persons = persons;

            if (_persons is { Count: > 0 })
            {
                CurrentPerson = _persons[0];
                SelectedPerson = CurrentPerson.ClosestPerson;
            }

            Name = SelectedPerson.Name;
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


        private List<Person> _allPersons = new() {new Person() {Name = "<Додати нову>"}};
        public List<Person> AllPersons
        {
            get => _allPersons;
            set => SetProperty(ref _allPersons, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
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
    }
}
