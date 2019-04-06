using System.Runtime.Serialization;

namespace PopoverGPMDP.Structures {
    /// <summary>
    /// Contents are auto-filled by DataContractJsonSerializer
    /// </summary>
    [DataContract]
    public struct Song {
        [DataMember] public string title;
        [DataMember] public string artist;
        [DataMember] public string album;
        [DataMember] public string albumArt;

        public bool IsNull() {
            return title == null;
        }

        public override string ToString() {
            return title + " - " + artist;
        }

        public override bool Equals(object obj) {
            if (!(obj is Song))
                return false;

            var song = (Song) obj;

            return title.Equals(song.title) && artist.Equals(song.artist) && album.Equals(song.album);
        }
    }
}