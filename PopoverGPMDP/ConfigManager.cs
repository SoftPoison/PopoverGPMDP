using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using PopoverGPMDP.Structures;

namespace PopoverGPMDP {
    public class ConfigManager {
        private readonly Regex _numberMatcher = new Regex(@"\d+");
        
        private readonly string _configFile;
        private readonly DataContractJsonSerializer _serializer = new DataContractJsonSerializer(typeof(Config));

        private JsonWatcher<GpmdpSettings, GpmdpSettingsEvent> _fileWatcher;
        private Config _config;
        public Config CurrentConfig => _config;

        public delegate void DoOnConfigUpdated();

        private readonly DoOnConfigUpdated _configUpdated;

        public ConfigManager(string configFile, DoOnConfigUpdated configUpdated) {
            _configFile = configFile;
            _configUpdated = configUpdated;

            var gpmdpSettingsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Google Play Music Desktop Player", "json_store", ".settings.json");
            _fileWatcher = new JsonWatcher<GpmdpSettings, GpmdpSettingsEvent>(gpmdpSettingsFile, 1000) {
                CheckUpdate = (c, p) => new GpmdpSettingsEvent(c, p),
                OnUpdate = OnGpmdpSettingsUpdate,
                OnLoad = s => {
//                    if (!s.enableJSON_API)
//                        return;

                    OnGpmdpSettingsUpdate(new GpmdpSettingsEvent(s, new GpmdpSettings()));
                }
            };
        }

        /// <summary>
        /// Attempts to load the config file into memory
        /// </summary>
        public void Load() {
            try {
                using (var stream = File.OpenRead(_configFile)) {
                    _config = (Config) _serializer.ReadObject(stream);
                }
            }
            catch (Exception) { // file is either corrupt or doesn't exist (or some other error)
                try { // try and back the file up
                    File.Move(_configFile, _configFile + ".errored");
                }
                catch (Exception) {
                    // ignored
                }

                _config = Config.DefaultConfig;

                Save();
            }
            
            if (_config.Sync)
                _fileWatcher.Start();
            
            _configUpdated();
        }

        /// <summary>
        /// Attempts to save the config to a file
        /// </summary>
        public void Save() {
            try {
                _serializer.WriteObject(File.Create(_configFile), _config);
            }
            catch (Exception) {
                // ignored
            }
        }

        private void OnGpmdpSettingsUpdate(GpmdpSettingsEvent e) {
            if (!_config.Sync) {
                return;
            }

            if (e.JsonApiUpdated && !e.Settings.enableJSON_API) {
                // API was disabled, so do something about it
                _fileWatcher.Stop();
            }

            // load default theme if custom themes are disabled
            if (!e.Settings.theme) {
                _config.BackgroundColor = "#fafafa";
                _config.TextColor = "#000000";
                _config.HighlightColor = "#ff5732";
            }
            else {
                if (e.ThemeColorUpdated) {
                    // update highlight colour
                    var themeColor = e.Settings.themeColor;
                    
                    // default theme colours are in a different format so convert them first
                    if (!themeColor.StartsWith("#")) {
                        var matches = _numberMatcher.Matches(themeColor);
                        themeColor = "#";
                        foreach (var match in matches)
                            themeColor += int.Parse(match.ToString()).ToString("X");
                    }

                    _config.HighlightColor = themeColor;
                }

                if (e.ThemeTypeUpdated) {
                    // set light or dark mode
                    if (e.Settings.themeType == "FULL") { // dark mode
                        _config.BackgroundColor = "#1a1b1d";
                        _config.TextColor = "#ffffff";
                    }
                    else {
                        _config.BackgroundColor = "#fafafa";
                        _config.TextColor = "#000000";
                    }
                }
            }

            _configUpdated();
//            Save();
        }
    }
}