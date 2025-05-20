using System.Collections.ObjectModel;
using FaceAiSharp;
using FaceAiSharp.Extensions;
using MediaFaceSearcher.DAL;
using MediaFaceSearcher.Model;
using MediaFaceSearcher.NeuralNetwork;
using Microsoft.Win32;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MediaFaceSearcher.ViewModels
{
    public class MainPageViewModel : BindableBase
    {
        private readonly IPersonDao _personDao;

        private readonly FaceDetector _faceDetector;
        private readonly FaceRecognizer _faceRecognizer;

        private Canvas? FaceCanvas;
        private List<Person> _allPersons;
        public MainPageViewModel(IPersonDao personDao)
        {
            OpenMediaFileCommand = new DelegateCommand<Button>(OpenMediaFile);
            CloseMediaCommand = new DelegateCommand(CloseMedia);
            CanvasLoadedCommand = new DelegateCommand<Canvas>(CanvasLoaded);

            _personDao = personDao;

            _faceDetector = new FaceDetector();
            _faceDetector = new FaceDetector();
            _faceRecognizer = new FaceRecognizer();
            _allPersons = _personDao.Read();
        }


        #region Binding
        private ObservableCollection<PotentialPerson> _recentPersons = new();
        public ObservableCollection<PotentialPerson> RecentPersons
        {
            get => _recentPersons;
            set => SetProperty(ref _recentPersons, value);
        }

        private BitmapSource _mediaSource;
        public BitmapSource MediaSource
        {
            get => _mediaSource;
            set => SetProperty(ref _mediaSource, value, MediaSourceChanged);
        }

        #endregion

        #region Commands
        public DelegateCommand<Button> OpenMediaFileCommand { get; }
        private void OpenMediaFile(Button button)
        {
            var dialog = CreateOpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                using var originalBitmap = new Bitmap(dialog.FileName);
                int targetWidth = (int)button.ActualWidth;
                int targetHeight = (int)button.ActualHeight;

                using var paddedBitmap = CreatePaddedBitmap(originalBitmap, targetWidth, targetHeight, out int scaledWidth, out int scaledHeight);

                MediaSource = ConvertBitmapToBitmapSource(paddedBitmap);

                var faces = _faceDetector.Detect(dialog.FileName);
                DrawFacesOnCanvas(faces, originalBitmap.Width, originalBitmap.Height, scaledWidth, scaledHeight);
                AddNewPersons(faces, originalBitmap, dialog.FileName);
            }
        }


        public DelegateCommand CloseMediaCommand { get; }
        private void CloseMedia()
        {
            MediaSource = null;
            //_mediaPlayer.Stop();
        }

        public DelegateCommand<Canvas> CanvasLoadedCommand { get; }
        private void CanvasLoaded(Canvas canvas)
        {
            FaceCanvas = canvas;
        }
        #endregion

        #region Methods
        private OpenFileDialog CreateOpenFileDialog()
        {
            return new OpenFileDialog
            {
                Filter = "Media Files|*.jpg;*.jpeg;*.png" // *.mp4;*.mkv;*.avi;*.mov;
            };
        }

        private Bitmap CreatePaddedBitmap(Bitmap originalBitmap, int targetWidth, int targetHeight, out int scaledWidth, out int scaledHeight)
        {
            // Calculate scale to fit while preserving aspect ratio
            float scale = Math.Min((float)targetWidth / originalBitmap.Width, (float)targetHeight / originalBitmap.Height);
            scaledWidth = (int)(originalBitmap.Width * scale);
            scaledHeight = (int)(originalBitmap.Height * scale);

            // Center the image
            int offsetX = (targetWidth - scaledWidth) / 2;
            int offsetY = (targetHeight - scaledHeight) / 2;

            var paddedBitmap = new Bitmap(targetWidth, targetHeight);
            using (Graphics g = Graphics.FromImage(paddedBitmap))
            {
                g.Clear(System.Drawing.Color.Black);
                g.DrawImage(originalBitmap, offsetX, offsetY, scaledWidth, scaledHeight);
            }
            return paddedBitmap;
        }

        private BitmapSource ConvertBitmapToBitmapSource(Bitmap bitmap)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        private void DrawFacesOnCanvas(IReadOnlyCollection<FaceDetectorResult> faces, int originalWidth, int originalHeight, int scaledWidth, int scaledHeight)
        {
            if (FaceCanvas == null) return;
            FaceCanvas.Children.Clear();
            FaceCanvas.Width = scaledWidth;
            FaceCanvas.Height = scaledHeight;

            var scaleX = (double)scaledWidth / originalWidth;
            var scaleY = (double)scaledHeight / originalHeight;

            foreach (var face in faces)
            {
                // Draw face rectangle
                var rect = new System.Windows.Shapes.Rectangle
                {
                    Width = face.Box.Width * scaleX,
                    Height = face.Box.Height * scaleY,
                    Stroke = System.Windows.Media.Brushes.Green,
                    StrokeThickness = 3
                };
                Canvas.SetLeft(rect, face.Box.X * scaleX);
                Canvas.SetTop(rect, face.Box.Y * scaleX);
                FaceCanvas.Children.Add(rect);

                // Draw keypoints as small dots
                if (face.Landmarks != null)
                {
                    foreach (var point in face.Landmarks)
                    {
                        var ellipse = new System.Windows.Shapes.Ellipse
                        {
                            Width = 4,
                            Height = 4,
                            Fill = System.Windows.Media.Brushes.Lime,
                            Opacity = 0.5
                        };
                        Canvas.SetLeft(ellipse, (point.X * scaleX) - 2);
                        Canvas.SetTop(ellipse, (point.Y * scaleY) - 2);
                        FaceCanvas.Children.Add(ellipse);
                    }
                }
            }
        }

        private void AddNewPersons(IReadOnlyCollection<FaceDetectorResult> faces, Bitmap originalBitmap, string filePath)
        {
            foreach (var face in faces)
            {
                if (TryFindExistingPerson(face, filePath, out Person closestPerson, out float[] embedding, out float confidence))
                {
                    RecentPersons.Add(new PotentialPerson
                    {
                        ClosestPerson = closestPerson,
                        CroppedImage = ConvertBitmapToBitmapSource(originalBitmap.Clone(new RectangleF(face.Box.X, face.Box.Y, face.Box.Width, face.Box.Height), PixelFormat.Format24bppRgb)),
                        Confidence = confidence,
                        FaceDetectorResult = face,
                        FilePath = filePath,
                        Embedding = embedding
                    });
                }
                else
                {
                    RecentPersons.Add(new PotentialPerson()
                    {
                        CroppedImage = ConvertBitmapToBitmapSource(originalBitmap.Clone(new RectangleF(face.Box.X, face.Box.Y, face.Box.Width, face.Box.Height), PixelFormat.Format24bppRgb)),
                        FilePath = filePath,
                        Embedding = embedding,
                        FaceDetectorResult = face,
                    });
                }
            }
        }

        private bool TryFindExistingPerson(FaceDetectorResult face, string filePath, out Person closestPerson, out float[] embedding, out float confidence)
        {
            closestPerson = null;
            embedding = _faceRecognizer.Detect(filePath, face.Landmarks);

            float maxConfidence = 0.5f;
            Person bestPerson = null;
            confidence = 0;

            foreach (var person in _allPersons)
            {
                foreach (var photo in person.Photos)
                {
                    confidence = embedding.Dot(photo.Embedding);
                    if (confidence > maxConfidence)
                    {
                        maxConfidence = confidence;
                        bestPerson = person;
                    }
                }
            }

            if (bestPerson != null)
            {
                closestPerson = bestPerson;
                return true;
            }

            return false;
        }

        private void MediaSourceChanged()
        {
            RecentPersons.Clear();
        }
        #endregion
    }
}
