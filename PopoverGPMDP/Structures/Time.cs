using System.Runtime.Serialization;

namespace PopoverGPMDP.Structures {
    /// <summary>
    /// Contents are auto-filled by DataContractJsonSerializer
    /// </summary>
    [DataContract]
    public struct Time {
        [DataMember] public int current;
        [DataMember] public int total;

        public override bool Equals(object obj) {
            if (!(obj is Time)) return false;

            var t = (Time) obj;

            return current == t.current && total == t.total;
        }
    }
}