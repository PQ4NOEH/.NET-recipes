namespace Altea.Classes.Lists
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class ListGroupType
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        private string[] groupsString;
        private List<ListGroupContent> groups;
        
        [JsonIgnore]
        [JsonProperty(PropertyName = "groups", Required = Required.Always)]
        public string[] GroupsString {
            get
            {
                return this.groupsString;
            }
            set
            {
                this.groupsString = value;

                this.groups = new List<ListGroupContent>();

                for (int i = 0; i < value.Length; i++)
                {
                    string[] groupValues = value[i].Split(',');
                    ListGroupContent content = new ListGroupContent
                        {
                            Position = i,
                            DataSplit = Int32.Parse(groupValues[0].TrimEnd('%')),
                            PercentageSplit = groupValues[0][groupValues[0].Length - 1] == '%',
                            SplitCount = Int32.Parse(groupValues[1])
                        };

                    this.groups.Add(content);
                }
            }
        }

        [JsonProperty(PropertyName = "groups", Required = Required.Always)]
        public IEnumerable<ListGroupContent> Groups {
            get
            {
                return this.groups;
            }
        }
    }
}
