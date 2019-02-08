namespace PopoverGPMDP.Structures {
    /// <summary>
    /// Contents are auto-filled by DataContractJsonSerializer
    /// </summary>
    public struct Playback {
        public bool playing;
        public Song song;
        public Time time;
        public string shuffle;
        public string repeat;
//        public int volume;

        public bool IsNull() {
            return song.IsNull();
        }
    }
}