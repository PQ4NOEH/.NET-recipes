namespace Altea.Classes.WiseTank
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class TankStream
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "position", Required = Required.Always)]
        public int Position { get; set; }

        [JsonProperty(PropertyName = "boxesWidth", Required = Required.Always)]
        public int BoxesWidth { get; set; }

        [JsonProperty(PropertyName = "boxes", Required = Required.Always)]
        public IEnumerable<TankBox> Boxes { get; set; }

        [JsonProperty(PropertyName = "refreshRate", Required = Required.Always)]
        public int RefreshRate { get; set; }
    }
}
