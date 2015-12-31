namespace Altea.Classes.Stax
{
    using System;

    using Newtonsoft.Json;

    public class Stack
    {
        [JsonProperty(PropertyName = "number", Required = Required.Always)]
        public int Number { get; set; }

        [JsonProperty(PropertyName = "success", Required = Required.Always)]
        public int Success { get; set; }

        [JsonProperty(PropertyName = "errors", Required = Required.Always)]
        public int Errors { get; set; }

        [JsonProperty(PropertyName = "mean", Required = Required.Always)]
        public TimeSpan Mean { get; set; }

        [JsonProperty(PropertyName = "data", Required = Required.Always)]
        public int Data { get; set; }
    }
}
