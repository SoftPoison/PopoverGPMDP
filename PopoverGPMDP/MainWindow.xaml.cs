using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using PopoverGPMDP.Structures;

// ReSharper disable InheritdocConsiderUsage

namespace PopoverGPMDP {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        private readonly ConfigManager _configManager;

        private bool _poppingOver;
        private bool _extendPopover;

        private delegate void StringCallback(string str);

        private delegate void BooleanCallback(bool b);

        private delegate void UpdateSongCallback(Song song);

        public MainWindow() {
            InitializeComponent();

            _configManager = new ConfigManager("config.json", OnConfigUpdated);

            var fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Google Play Music Desktop Player", "json_store", "playback.json");

            var fileWatcher = new JsonWatcher<Playback, PlaybackUpdateEvent>(fileName, 150) {
                CheckUpdate = (c, p) => new PlaybackUpdateEvent(c, p),
                OnUpdate = OnPlaybackUpdate,
                OnLoad = p => {
                    if (p.IsNull())
                        return;

                    Dispatcher.Invoke(new JsonWatcher<Playback, PlaybackUpdateEvent>.DoOnUpdate(OnPlaybackUpdate),
                        new PlaybackUpdateEvent(p, new Playback()));
                }
            };

            Loaded += (s, e) => {
                fileWatcher.Start();
                Visibility = Visibility.Hidden;
                _configManager.Load();
            };
            Deactivated += WindowDeactivated;
        }

        private void OnConfigUpdated() {
            Dispatcher.Invoke(() => {
                // decode the colours

                var backgroundColorInt = int.Parse(_configManager.CurrentConfig.BackgroundColor.Substring(1),
                    NumberStyles.HexNumber);
                var highlightColorInt = int.Parse(_configManager.CurrentConfig.HighlightColor.Substring(1),
                    NumberStyles.HexNumber);
                var textColorInt = int.Parse(_configManager.CurrentConfig.TextColor.Substring(1),
                    NumberStyles.HexNumber);

                var backgroundColor = Color.FromRgb(
                    (byte) (backgroundColorInt >> 16),
                    (byte) ((backgroundColorInt >> 8) & 0xff),
                    (byte) (backgroundColorInt & 0xff)
                );
                var highlightColor = Color.FromRgb(
                    (byte) (highlightColorInt >> 16),
                    (byte) ((highlightColorInt >> 8) & 0xff),
                    (byte) (highlightColorInt & 0xff)
                );
                var textColor = Color.FromRgb(
                    (byte) (textColorInt >> 16),
                    (byte) ((textColorInt >> 8) & 0xff),
                    (byte) (textColorInt & 0xff)
                );

                // tint the images by the specific colours

                Fade.Source = Utils.TintImage("fade.png", backgroundColorInt);
                PlayIcon.Source = Utils.TintImage("play.png", textColorInt);
                PauseIcon.Source = Utils.TintImage("pause.png", textColorInt);
                RepeatOffIcon.Source = Utils.TintImage("repeat.png", textColorInt);
                RepeatOnIcon.Source = Utils.TintImage("repeat.png", highlightColorInt);
                RepeatOneIcon.Source = Utils.TintImage("repeat_one.png", highlightColorInt);
                ShuffleOffIcon.Source = Utils.TintImage("shuffle.png", textColorInt);
                ShuffleOnIcon.Source = Utils.TintImage("shuffle.png", highlightColorInt);

                // colour the non-image elements
                
                Grid.Background = new SolidColorBrush(backgroundColor);
                ProgressBar.Fill = new SolidColorBrush(highlightColor);
                SongText.Foreground = new SolidColorBrush(textColor);
                ArtistAlbumText.Foreground = new SolidColorBrush(textColor);
                
                // position the window

                PopoverWindow.Left = _configManager.CurrentConfig.XPos;
                
                // currently does not work due to how animations are handled
                PopoverWindow.Top = _configManager.CurrentConfig.YPos;
            });
        }

        /// <summary>
        /// Uses win32 hooks to check whether the cursor is in the region where the window should be
        /// </summary>
        /// <param name="obj"></param>
        private void MouseOverThreadLoop(object obj) {
            while (_poppingOver) {
                Thread.Sleep(10);

                Dispatcher.Invoke(() => {
                    var pos = PointToScreen(W32Hooks.GetMousePosition());

                    if (pos.X >= 0 && pos.X <= 400 && pos.Y >= 0 && pos.Y <= 100) {
                        Visibility = Visibility.Hidden;
                    }
                    else {
                        Visibility = Visibility.Visible;
                    }
                });
            }
        }

        private static void WindowDeactivated(object sender, EventArgs e) {
            var window = (Window) sender;
            window.Topmost = true;
        }

        private void OnPlaybackUpdate(PlaybackUpdateEvent e) {
            //only update on the events we want

            if (e.TimeUpdated)
                Dispatcher.Invoke(() => ProgressBar.Width = e.Current.time.current * Width / e.Current.time.total);

            if (e.RepeatUpdated)
                Dispatcher.Invoke(new StringCallback(UpdateRepeat), e.Current.repeat);

            if (e.ShuffleUpdated)
                Dispatcher.Invoke(new BooleanCallback(UpdateShuffle), e.Current.shuffle.Equals("ALL_SHUFFLE"));

            if (e.PlayStateUpdated)
                Dispatcher.Invoke(new BooleanCallback(UpdatePlayPause), e.Current.playing);

            if (e.SongUpdated)
                Dispatcher.Invoke(new UpdateSongCallback(UpdateSong), e.Current.song);

            if (e.ImportantUpdate)
                Dispatcher.Invoke(Popover);
        }

        private void UpdateRepeat(string mode) {
            //NO_REPEAT ALL_REPEAT SINGLE_REPEAT
            switch (mode) {
                default:
                    RepeatOffIcon.Visibility = Visibility.Visible;
                    RepeatOnIcon.Visibility = Visibility.Collapsed;
                    RepeatOneIcon.Visibility = Visibility.Collapsed;
                    break;
                case "LIST_REPEAT":
                    RepeatOffIcon.Visibility = Visibility.Collapsed;
                    RepeatOnIcon.Visibility = Visibility.Visible;
                    RepeatOneIcon.Visibility = Visibility.Collapsed;
                    break;
                case "SINGLE_REPEAT":
                    RepeatOffIcon.Visibility = Visibility.Collapsed;
                    RepeatOnIcon.Visibility = Visibility.Collapsed;
                    RepeatOneIcon.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void UpdateShuffle(bool shuffle) {
            if (shuffle) {
                ShuffleOffIcon.Visibility = Visibility.Collapsed;
                ShuffleOnIcon.Visibility = Visibility.Visible;
            }
            else {
                ShuffleOffIcon.Visibility = Visibility.Visible;
                ShuffleOnIcon.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdatePlayPause(bool playing) {
            if (playing) {
                PlayIcon.Visibility = Visibility.Collapsed;
                PauseIcon.Visibility = Visibility.Visible;
            }
            else {
                PlayIcon.Visibility = Visibility.Visible;
                PauseIcon.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateSong(Song song) {
            SongText.Text = song.title;
            ArtistAlbumText.Text = song.artist + " - " + song.album;

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(song.albumArt, UriKind.Absolute);
            bitmap.EndInit();

            AlbumArt.Source = bitmap;
        }

        private async void Popover() {
            if (_poppingOver) {
                _extendPopover = true;
                return;
            }

            _poppingOver = true;
            Visibility = Visibility.Visible;

            new Thread(MouseOverThreadLoop).Start();

            //TODO: find a better way to do animations, perhaps using PopoverWindow
            for (var x = -(int) Height; x <= 0; x++) {
                Top = x > 0 ? 0 : x;
                if (x % 7 == 0)
                    await Task.Delay(1);
            }

            do {
                _extendPopover = false;
                await Task.Delay(4000);
            } while (_extendPopover);

            for (var x = 0; x >= -Height; x--) {
                Top = x;
                if (x % 7 == 0)
                    await Task.Delay(1);
            }

            Visibility = Visibility.Collapsed;
            _poppingOver = false;
        }
    }
}