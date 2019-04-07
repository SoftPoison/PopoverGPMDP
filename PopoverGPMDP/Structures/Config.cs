using System.Runtime.Serialization;

namespace PopoverGPMDP.Structures {
    /// <summary>
    /// Contents are auto-filled by DataContractJsonSerializer
    /// </summary>
    [DataContract]
    public struct Config {   
        [DataMember(Name = "syncColorsWithGPMDP", IsRequired = true)]
        public bool Sync;

        [DataMember(Name = "backgroundColor", IsRequired = true)]
        public string BackgroundColor;
        
        [DataMember(Name = "textColor", IsRequired = true)]
        public string TextColor;

        [DataMember(Name = "highlightColor", IsRequired = true)]
        public string HighlightColor;

        [DataMember(Name = "xPosition", IsRequired = true)]
        public double XPos;
        
        [DataMember(Name = "yPosition", IsRequired = true)]
        public double YPos;
        
        [DataMember(Name = "width", IsRequired = true)]
        public double Width;
        
        [DataMember(Name = "height", IsRequired = true)]
        public double Height;

        public static readonly Config DefaultConfig = new Config {
            Sync = true,
            BackgroundColor = "#fafafa",
            TextColor = "#000000",
            HighlightColor = "#ff5732",
            XPos = 0,
            YPos = 0,
            Width = 400,
            Height = 100
        };
    }
}