using System;

namespace SlugHub
{
    public class Slug
    {
        public Slug(string value, string groupingKey)
        {
            Value = value;

            if (!string.IsNullOrEmpty(groupingKey))
                GroupingKey = groupingKey;

            Created = DateTime.Now;
        }

        public string Value { get; set; }

        public string GroupingKey { get; set; }

        public DateTime Created { get; set; }
    }
}