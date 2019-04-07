using System.Runtime.Serialization;

namespace PopoverGPMDP.Structures {
    /// <summary>
    /// Contents are auto-filled by DataContractJsonSerializer
    /// </summary>
    [DataContract]
    public struct GpmdpSettings {
        [DataMember] public readonly bool enableJSON_API;
        [DataMember] public readonly bool theme;
        [DataMember] public readonly string themeColor;
        [DataMember] public readonly string themeType; //FULL -> dark mode, HIGHLIGHT_ONLY -> light mode

        public bool Equals(GpmdpSettings other) {
            return enableJSON_API == other.enableJSON_API && theme == other.theme && string.Equals(themeColor, other.themeColor) && string.Equals(themeType, other.themeType);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            return obj is GpmdpSettings settings && Equals(settings);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = enableJSON_API.GetHashCode();
                hashCode = (hashCode * 397) ^ theme.GetHashCode();
                hashCode = (hashCode * 397) ^ (themeColor != null ? themeColor.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (themeType != null ? themeType.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}