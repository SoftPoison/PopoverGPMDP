using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading;

namespace PopoverGPMDP {
    public class JsonWatcher<TJsonStruct, TEvent> where TEvent : IUpdateEvent {
        private readonly string _filename;
        private readonly int _updateTimeout;
        private readonly DataContractJsonSerializer _serializer = new DataContractJsonSerializer(typeof(TJsonStruct));
        private readonly Thread _thread;
        private bool _watching;
        private bool _unloaded = true;
        private TJsonStruct _json;
        private TJsonStruct _lastJson;

        public delegate TEvent UpdateChecker(TJsonStruct current, TJsonStruct previous);

        private UpdateChecker _checkUpdate;

        public UpdateChecker CheckUpdate {
            set => _checkUpdate = value;
        }

        public delegate void DoOnUpdate(TEvent e);

        private DoOnUpdate _onUpdate;

        public DoOnUpdate OnUpdate {
            set => _onUpdate = value;
        }

        public delegate void DoOnLoad(TJsonStruct json);

        private DoOnLoad _onLoad;

        public DoOnLoad OnLoad {
            set => _onLoad = value;
        }

        /// <summary>
        /// Watches a JSON formatted file for updates based on a user defined update method and structure type
        /// </summary>
        /// <param name="filename">The file to watch</param>
        /// <param name="updateTimeout">The number of milliseconds between updates</param>
        public JsonWatcher(string filename, int updateTimeout) {
            _filename = filename;
            _updateTimeout = updateTimeout;
            
            _thread = new Thread(MainLoop);
            
        }

        private void MainLoop(object obj) {
            while (_watching) {
                _lastJson = _json;
                _json = Read();

                if (_json != null && _lastJson != null) {
                    if (_unloaded) {
                        _unloaded = false;
                        _onLoad(_json);
                    }
                    else {
                        var e = _checkUpdate(_json, _lastJson);
                        if (e.Updated())
                            _onUpdate(e);
                    }
                }

                Thread.Sleep(_updateTimeout);
            }
        }

        private TJsonStruct Read() {
            try {
                using (var stream = File.OpenRead(_filename)) {
                    return (TJsonStruct) _serializer.ReadObject(stream);
                }
            }
            catch (Exception) {
                return _json;
            }
        }

        /// <summary>
        /// Starts the watcher
        /// </summary>
        public void Start() {
            if (_watching) return;
            
            _watching = true;
            _thread.Start();
        }

        /// <summary>
        /// Stops the watcher
        /// Watcher can be restarted using <see cref="Start"/>
        /// </summary>
        public void Stop() {
            _watching = false;
        }
    }

    public interface IUpdateEvent {
        bool Updated();
    }
}