namespace PopoverGPMDP.Structures {
    public struct PlaybackUpdateEvent : IUpdateEvent {
        private readonly bool _updated;

        public readonly bool PlayStateUpdated;
        public readonly bool SongUpdated;
        public readonly bool TimeUpdated;
        public readonly bool ShuffleUpdated;
        public readonly bool RepeatUpdated;

        public readonly bool ImportantUpdate;

        public readonly Playback Current;
        public readonly Playback Previous;

        public PlaybackUpdateEvent(Playback current, Playback previous) {
            Current = current;
            Previous = previous;

            if (current.IsNull()) {
                _updated = false;
                PlayStateUpdated = false;
                SongUpdated = false;
                TimeUpdated = false;
                ShuffleUpdated = false;
                RepeatUpdated = false;
                ImportantUpdate = false;
                return;
            }
            
            PlayStateUpdated = current.playing != previous.playing;
            SongUpdated = !current.song.Equals(previous.song);
            TimeUpdated = !current.time.Equals(previous.time);
            ShuffleUpdated = !current.shuffle.Equals(previous.shuffle);
            RepeatUpdated = !current.repeat.Equals(previous.repeat);

            _updated = PlayStateUpdated || SongUpdated || TimeUpdated || ShuffleUpdated || RepeatUpdated;
            ImportantUpdate = PlayStateUpdated || SongUpdated;
        }

        public bool Updated() {
            return _updated;
        }
    }
}