using System.Runtime.Serialization;

namespace PopoverGPMDP.Structures {
    /// <summary>
    /// Contents are auto-filled by DataContractJsonSerializer
    /// </summary>
    [DataContract]
    public struct Song {
        [DataMember] public readonly string title;
        [DataMember] public readonly string artist;
        [DataMember] public readonly string album;
        [DataMember] public readonly string albumArt;

        public bool IsNull() {
            return title == null;
        }

        public override string ToString() {
            return title + " - " + artist;
        }

        public bool Equals(Song other) {
            return string.Equals(title, other.title) && string.Equals(artist, other.artist) && string.Equals(album, other.album);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Song song && Equals(song);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = title != null ? title.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (artist != null ? artist.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (album != null ? album.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}