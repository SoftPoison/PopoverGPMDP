namespace PopoverGPMDP.Structures {
    public class Song {
        public string title;
        public string artist;
        public string album;
        public string albumArt;

        public string FullName() {
            return title + " - " + artist + " [ - " + album + " ]";
        }

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