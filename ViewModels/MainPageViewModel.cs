using Microsoft.Win32;
using System.Collections.ObjectModel;
using FlyleafLib.Controls.WPF;

namespace MediaFaceSearcher.ViewModels
{
    public class MainPageViewModel : BindableBase
    {
        private FlyleafME _mediaPlayer;
        public MainPageViewModel()
        {
            FooCommand = new DelegateCommand(Foo);
            OpenMediaFileCommand = new DelegateCommand(OpenMediaFile);
            PlayerLoadedCommand = new DelegateCommand<FlyleafME>(PlayerLoaded);
            CloseMediaCommand = new DelegateCommand(CloseMedia);

            FooItems.Add("C:\\Users\\rosse\\OneDrive\\Pictures\\Screenshots\\Screenshot 2025-03-16 225827.png");


            //Core.Initialize();

            //_libVLC = new LibVLC();
            //_mediaPlayer = new MediaPlayer(_libVLC);
        }


        #region Binding

        private string _mediaSource = string.Empty;
        public string MediaSource
        {
            get => _mediaSource;
            set => SetProperty(ref _mediaSource, value);
        }

        private ObservableCollection<string> _fooItems = new();
        public ObservableCollection<string> FooItems
        {
            get => _fooItems;
            set => SetProperty(ref _fooItems, value);
        }

        #endregion

        #region Commands
        public DelegateCommand OpenMediaFileCommand { get; }
        private void OpenMediaFile()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Media Files|*.mp4;*.mkv;*.avi;*.mov;*.jpg;*.jpeg;*.png"
            };
            if (dialog.ShowDialog() == true)
            {
                MediaSource = dialog.FileName;

                //var media = new Media(_libVLC, MediaSource, FromType.FromPath);
                //_mediaPlayer.Play(media);
            }
        }

        public DelegateCommand FooCommand { get; }
        private void Foo()
        {
            FooItems.Add(FooItems.Count.ToString());
        }

        public DelegateCommand CloseMediaCommand { get; }
        private void CloseMedia()
        {
            //_mediaPlayer.Stop();
        }


        public DelegateCommand<FlyleafME> PlayerLoadedCommand { get; set; }
        private void PlayerLoaded(FlyleafME player)
        {
            _mediaPlayer = player;
            //player.Play();
        }
        #endregion

        #region Methods

        #endregion
    }
}
