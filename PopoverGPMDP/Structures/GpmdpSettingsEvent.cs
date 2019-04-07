namespace PopoverGPMDP.Structures {
    public struct GpmdpSettingsEvent : IUpdateEvent {
        private readonly bool _updated;

        public readonly bool JsonApiUpdated;
        public readonly bool ThemeUpdated;
        public readonly bool ThemeColorUpdated;
        public readonly bool ThemeTypeUpdated;
        
        public readonly GpmdpSettings Settings;

        public GpmdpSettingsEvent(GpmdpSettings current, GpmdpSettings previous) {
            Settings = current;

            JsonApiUpdated = current.enableJSON_API != previous.enableJSON_API;
            ThemeUpdated = current.theme != previous.theme;
            ThemeColorUpdated = current.themeColor != previous.themeColor;
            ThemeTypeUpdated = current.themeType != previous.themeType;
            
            _updated = !current.Equals(previous);
        }
        
        public bool Updated() {
            return _updated;
        }
    }
}