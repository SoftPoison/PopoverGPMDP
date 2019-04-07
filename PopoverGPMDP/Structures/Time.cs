using System.Runtime.Serialization;

namespace PopoverGPMDP.Structures {
    /// <summary>
    /// Contents are auto-filled by DataContractJsonSerializer
    /// </summary>
    [DataContract]
    public struct Time {
        [DataMember] public readonly int current;
        [DataMember] public readonly int total;

        public bool Equals(Time other) {
            return current == other.current && total == other.total;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Time time && Equals(time);
        }

        public override int GetHashCode() {
            unchecked {
                return (current * 397) ^ total;
            }
        }
    }
}