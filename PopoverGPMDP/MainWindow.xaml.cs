using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using PopoverGPMDP.Structures;
// ReSharper disable InheritdocConsiderUsage

namespace PopoverGPMDP {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        private bool _poppingOver;
        private bool _extendPopover;

        private delegate void StringCallback(string str);

        private delegate void BooleanCallback(bool b);

        private delegate void UpdateSongCallback(Song song);

        public MainWindow() {
            InitializeComponent();

            var fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Google Play Music Desktop Player\\json_store\\playback.json");

            var fileWatcher = new JsonWatcher<Playback, PlaybackUpdateEvent>(fileName) {
                CheckUpdate = (c, p) => new PlaybackUpdateEvent(c, p),
                OnUpdate = OnPlaybackUpdate,
                OnLoad = p => {
                    if (p.IsNull() || p.song.IsNull())
                        return;
                    
                    Dispatcher.Invoke(new UpdateSongCallback(UpdateSong), p.song);
                    Dispatcher.InvokeAsync(Popover);
                }
            };

            Loaded += (s, e) => {
                fileWatcher.Start();
                Visibility = Visibility.Hidden;
            };
            Deactivated += WindowDeactivated;
            
            var thread = new Thread(MouseOverThreadLoop);
            thread.Start();
        }

        private void MouseOverThreadLoop(object obj) {
            while (true) {
                Thread.Sleep(10);
                
                if (!_poppingOver) {
                    continue;
                }

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
            // ReSharper disable once FunctionNeverReturns
        }

        private static void WindowDeactivated(object sender, EventArgs e) {
            var window = (Window) sender;
            window.Topmost = true;
        }

        private void OnPlaybackUpdate(PlaybackUpdateEvent e) {
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
                Dispatcher.InvokeAsync(Popover);
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
            
            for (var x = -(int)Height; x <= 0; x++) {
                Top = x > 0 ? 0 : x;
                if (x % 7 == 0)
                    await Task.Delay(1);
            }

            do {
                _extendPopover = false;
                await Task.Delay(4000);
            } while(_extendPopover);

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