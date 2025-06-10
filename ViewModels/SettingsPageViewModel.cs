using MediaFaceSearcher.DAL;
using MediaFaceSearcher.Model;
using Microsoft.Win32;

namespace MediaFaceSearcher.ViewModels
{
    class SettingsPageViewModel : BindableBase
    {
        private readonly ISettingsDao _settingsDao;

        public SettingsPageViewModel(ISettingsDao settingsDao)
        {
            _settingsDao = settingsDao;

            OpenPhotoFolderCommand = new DelegateCommand(OpenPhotoFolder);
            SaveSettingsCommand = new DelegateCommand(SaveSettings);

            LoadSettings();
        }

        private void LoadSettings()
        {
            var settings = _settingsDao.Read();
            DetectionConfidence = settings.DetectionConfidence;
            RecognitionConfidence = settings.RecognitionConfidence;
            MinFaceWidth = settings.MinFaceWidth;
            MinFaceHeight = settings.MinFaceHeight;
            SavePhotoCopy = settings.SavePhotoCopy;
            SavePhotoCopyPath = settings.SavePhotoCopyPath;
        }


        private float _detectionConfidence;
        public float DetectionConfidence
        {
            get => _detectionConfidence;
            set => SetProperty(ref _detectionConfidence, value);
        }

        private float _recognitionConfidence;

        public float RecognitionConfidence
        {
            get => _recognitionConfidence;
            set => SetProperty(ref _recognitionConfidence, value);
        }

        private int _minFaceWidth;
        public int MinFaceWidth
        {
            get => _minFaceWidth;
            set => SetProperty(ref _minFaceWidth, value);
        }

        private int _minFaceHeight;
        public int MinFaceHeight
        {
            get => _minFaceHeight;
            set => SetProperty(ref _minFaceHeight, value);
        }

        private bool _savePhotoCopy;

        public bool SavePhotoCopy
        {
            get => _savePhotoCopy;
            set => SetProperty(ref _savePhotoCopy, value);
        }

        private string _savePhotoCopyPath;
        public string SavePhotoCopyPath
        {
            get => _savePhotoCopyPath;
            set => SetProperty(ref _savePhotoCopyPath, value);
        }


        public DelegateCommand OpenPhotoFolderCommand { get; }
        private void OpenPhotoFolder()
        {
            var dialog = new OpenFolderDialog();
            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                SavePhotoCopyPath = dialog.FolderName;
            }
        }



        public DelegateCommand SaveSettingsCommand { get; }
        private void SaveSettings()
        {
            var settings = new Settings
            {
                DetectionConfidence = DetectionConfidence,
                RecognitionConfidence = RecognitionConfidence,
                MinFaceWidth = MinFaceWidth,
                MinFaceHeight = MinFaceHeight,
                SavePhotoCopy = SavePhotoCopy,
                SavePhotoCopyPath = SavePhotoCopyPath
            };

            _settingsDao.Save(settings);
        }
    }
}
