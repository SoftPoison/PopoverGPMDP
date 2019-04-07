using System.Runtime.Serialization;

namespace PopoverGPMDP.Structures {
    /// <summary>
    /// Contents are auto-filled by DataContractJsonSerializer
    /// </summary>
    [DataContract]
    public struct Playback {
        [DataMember] public readonly bool playing;
        [DataMember] public Song song;
        [DataMember] public Time time;
        [DataMember] public readonly string shuffle;
        [DataMember] public readonly string repeat;

//        [DataMember]
//        public int volume;

        public bool IsNull() {
            return song.IsNull();
        }
    }
}