namespace PopoverGPMDP.Structures {
    public class PlaybackUpdateEvent : IUpdateEvent {
        private readonly bool _updated;

        public bool PlayStateUpdated { get; }
        public bool SongUpdated { get; }
        public bool TimeUpdated { get; }
        public bool ShuffleUpdated { get; }
        public bool RepeatUpdated { get; }
        
        public bool ImportantUpdate { get; }
        
        public Playback Current { get; }
        public Playback Previous { get; }

        public PlaybackUpdateEvent(Playback current, Playback previous) {
            Current = current;
            Previous = previous;

            if (current.IsNull() || previous.IsNull() || current.IsNull()) return;
            
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