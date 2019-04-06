using System.Runtime.Serialization;

namespace PopoverGPMDP.Structures {
    /// <summary>
    /// Contents are auto-filled by DataContractJsonSerializer
    /// </summary>
    [DataContract]
    public struct Playback {
        [DataMember] public bool playing;
        [DataMember] public Song song;
        [DataMember] public Time time;
        [DataMember] public string shuffle;
        [DataMember] public string repeat;

//        [DataMember]
//        public int volume;

        public bool IsNull() {
            return song.IsNull();
        }
    }
}