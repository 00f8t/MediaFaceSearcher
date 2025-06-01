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
using MediaFaceSearcher.Views;
using SixLabors.ImageSharp.PixelFormats;
using Size = System.Drawing.Size;

namespace MediaFaceSearcher.ViewModels
{
    public class MainPageViewModel : BindableBase
    {
        private readonly IPersonDao _personDao;

        private readonly FaceDetector _faceDetector;
        private readonly FaceRecognizer _faceRecognizer;
        private readonly EmotionDetector _emotionDetector;

        private Canvas? FaceCanvas;
        private List<Person> _allPersons;

        public MainPageViewModel(IPersonDao personDao)
        {
            OpenMediaFileCommand = new DelegateCommand<Button>(OpenMediaFile);
            CloseMediaCommand = new DelegateCommand(CloseMedia);
            CanvasLoadedCommand = new DelegateCommand<Canvas>(CanvasLoaded);
            SaveFacesCommand = new DelegateCommand(SaveFaces);

            _personDao = personDao;

            _faceDetector = new FaceDetector();
            _faceRecognizer = new FaceRecognizer();
            _emotionDetector = new EmotionDetector();

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


        public DelegateCommand SaveFacesCommand { get; }
        private void SaveFaces()
        {
            var saveWindow = new AddingPersonView
            {
                DataContext = new AddingPersonViewModel(RecentPersons.ToList(), _allPersons)
            };
            saveWindow.ShowDialog();
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

        private Bitmap CreatePaddedBitmap(Bitmap originalBitmap, int targetWidth, int targetHeight, out int scaledWidth,
            out int scaledHeight)
        {
            // Calculate scale to fit while preserving aspect ratio
            float scale = Math.Min((float)targetWidth / originalBitmap.Width,
                (float)targetHeight / originalBitmap.Height);
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
            //bitmap.Save(@"C:\Users\rosse\Downloads\foo.jpg");
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        private void DrawFacesOnCanvas(IReadOnlyCollection<FaceDetectorResult> faces, int originalWidth,
            int originalHeight, int scaledWidth, int scaledHeight)
        {
            if (FaceCanvas == null) return;
            FaceCanvas.Children.Clear();
            FaceCanvas.Width = scaledWidth;
            FaceCanvas.Height = scaledHeight;

            var scaleX = (double)scaledWidth / originalWidth;
            var scaleY = (double)scaledHeight / originalHeight;

            foreach (var face in faces)
            {
                var faceRect = GetClampedFaceRectangle(face, new Size(originalWidth, originalHeight));

                // Draw face rectangle
                var rect = new System.Windows.Shapes.Rectangle
                {
                    Width = faceRect.Width * scaleX,
                    Height = faceRect.Height * scaleY,
                    Stroke = System.Windows.Media.Brushes.Green,
                    StrokeThickness = 3
                };
                Canvas.SetLeft(rect, faceRect.X * scaleX);
                Canvas.SetTop(rect, faceRect.Y * scaleY);
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

        private void AddNewPersons(IReadOnlyCollection<FaceDetectorResult> faces, Bitmap originalBitmap,
            string filePath)
        {
            foreach (var face in faces)
            {
                var faceRect = GetClampedFaceRectangle(face, new Size(originalBitmap.Width, originalBitmap.Height));
                var croppedImage = ConvertBitmapToBitmapSource(originalBitmap.Clone(faceRect, PixelFormat.Format24bppRgb));


                if (TryFindExistingPerson(face, filePath, out Person closestPerson, out float[] embedding,
                        out float confidence, out Emotion emotion))
                {
                    RecentPersons.Add(new PotentialPerson
                    {
                        ClosestPerson = closestPerson,
                        CroppedImage = croppedImage,
                        Confidence = confidence,
                        FaceDetectorResult = face,
                        FilePath = filePath,
                        Embedding = embedding,
                        Emotion = emotion
                    });
                }
                else
                {
                    RecentPersons.Add(new PotentialPerson()
                    {
                        CroppedImage = croppedImage,
                        FilePath = filePath,
                        Embedding = embedding,
                        FaceDetectorResult = face,
                        Emotion = emotion
                    });
                }
            }

            //var person = new Person()
            //{
            //    Name = "Дмитро",
            //    Photos = RecentPersons.Select(p => new Photo
            //    {
            //        Embedding = p.Embedding,
            //        FilePath = p.FilePath,
            //    }).ToList()
            //};
            //_personDao.Update([person]);
        }

        private bool TryFindExistingPerson(FaceDetectorResult face, string filePath, out Person closestPerson,
            out float[] embedding, out float confidence, out Emotion emotion)
        {
            closestPerson = null;
            using var image = SixLabors.ImageSharp.Image.Load<Rgb24>(filePath);
            embedding = _faceRecognizer.Detect(image, face.Landmarks);
            //var aligned = _faceRecognizer.AlignFaceToBitmap(image, face.Landmarks);
            emotion = _emotionDetector.Detect(image.ToBitmap()).ToEmotion();


            float maxConfidence = 0.5f;
            Person bestPerson = null;
            confidence = 0;

            foreach (var person in _allPersons)
            {
                foreach (var photo in person.Photos)
                {
                    confidence = embedding.Dot(photo.Embedding);
                    if (!(confidence > maxConfidence)) continue;
                    
                    maxConfidence = confidence;
                    bestPerson = person;
                }
            }

            if (bestPerson == null) return false;

            closestPerson = bestPerson;
            return true;

        }

        private void MediaSourceChanged()
        {
            RecentPersons.Clear();
        }

        #endregion

        private RectangleF GetClampedFaceRectangle(FaceDetectorResult face, Size originalSize)
        {
            var faceX = face.Box.X;
            var faceY = face.Box.Y;
            var faceW = face.Box.Width;
            var faceH = face.Box.Height;

            // Clamp X and adjust width
            if (faceX < 0)
            {
                faceW += faceX; // reduce width by the amount x is negative
                faceX = 0;
            }

            if (faceX + faceW > originalSize.Width)
            {
                faceW = originalSize.Width - faceX;
            }

            // Clamp Y and adjust height
            if (faceY < 0)
            {
                faceH += faceY; // reduce height by the amount y is negative
                faceY = 0;
            }

            if (faceY + faceH > originalSize.Height)
            {
                faceH = originalSize.Height - faceY;
            }

            // Ensure width and height are not negative
            faceW = Math.Max(0, faceW);
            faceH = Math.Max(0, faceH);

            return new RectangleF(faceX, faceY, faceW, faceH);
        }
    }
}
