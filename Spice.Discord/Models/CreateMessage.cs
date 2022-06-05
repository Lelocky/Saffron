using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spice.DiscordClient.Models
{
    public class CreateMessage
    {
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("tts")]
        public bool Tts { get; set; }

        [JsonProperty("embeds")]
        public List<Embed> Embeds { get; set; }

        public class Embed
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("color")]
            public int Color { get; set; }

            [JsonProperty("fields")]
            public List<Field> Fields { get; set; }

            [JsonProperty("timestamp")]
            public DateTime Timestamp { get; set; }

            [JsonProperty("thumbnail")]
            public Thumbnail Thumbnail { get; set; }

            [JsonProperty("footer")]
            public Footer Footer { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }
        }

        public class Field
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("value")]
            public string Value { get; set; }
        }

        public class Footer
        {
            [JsonProperty("text")]
            public string Text { get; set; }

            [JsonProperty("icon_url")]
            public string IconUrl { get; set; }
        }

        public class Thumbnail
        {
            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("height")]
            public int Height { get; set; }

            [JsonProperty("width")]
            public int Width { get; set; }
        }


    }
}
