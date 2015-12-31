namespace Altea.Classes.Members
{
    using System;
    using System.Collections.Generic;

    using Altea.Common.Classes;

    using Newtonsoft.Json;

    [Serializable]
    public class User
    {
        #region Properties

        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "level", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public Level Level { get; set; }

        [JsonProperty(PropertyName = "proLevel", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public ProLevel ProLevel { get; set; }

        [JsonProperty(PropertyName = "lastActivity", Required = Required.Always)]
        public DateTime LastActivity { get; set; }

        [JsonProperty(PropertyName = "from", Required = Required.Always)]
        public Language From { get; set; }

        [JsonProperty(PropertyName = "to", Required = Required.Always)]
        public Language To { get; set; }

        [JsonProperty(PropertyName = "approved", Required = Required.Always)]
        public bool Approved { get; set; }

        [JsonProperty(PropertyName = "firstName", Required = Required.Always)]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "lastName", Required = Required.Always)]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "email", Required = Required.Always)]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "busy", Required = Required.Always)]
        public bool Busy { get; set; }

        #endregion

        #region Private Fields

        private readonly Func<string, Guid, Language, Language, IDictionary<string, string>> _getSettings;

        private readonly Guid _appId;

        private readonly Action<User> _cacheWhenModified;

        #endregion

        #region Settings

        private IDictionary<string, string> _settings = null;

        public string this[string option]
        {
            get
            {
                if (this._settings == null)
                {
                    this.UpdateAllSettings();
                }

                string value;
                this._settings.TryGetValue(option, out value);
                return value;
            }
        }

        #endregion

        #region Constructors

        public User()
        {
        }

        public User(Func<string, Guid, Language, Language, IDictionary<string, string>> getSettings, Guid appId)
            : this(getSettings, appId, null)
        {
        }

        public User(Func<string, Guid, Language, Language, IDictionary<string, string>> getSettings, Guid appId, Action<User> cacheWhenModified)
        {
            this._getSettings = getSettings;
            this._appId = appId;
            this._cacheWhenModified = cacheWhenModified;
        }

        #endregion

        public void UpdateAllSettings()
        {
            if (this._getSettings == null)
            {
                throw new InvalidOperationException();
            }

            this._settings = this._getSettings.Invoke(this.Name, this._appId, this.From, this.To);

            if (this._cacheWhenModified != null)
            {
                this._cacheWhenModified.Invoke(this);
            }
        }
    }
}
