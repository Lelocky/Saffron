using Newtonsoft.Json;

namespace Spice.Saffron.ViewModels
{
    public class ChannelMessagesViewModel
    {
        public IEnumerable<ChannelMessage> Messages { get; set; }
        public class Author
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("username")]
            public string Username { get; set; }

            [JsonProperty("avatar")]
            public string Avatar { get; set; }

            [JsonProperty("avatar_decoration")]
            public object AvatarDecoration { get; set; }

            [JsonProperty("discriminator")]
            public string Discriminator { get; set; }

            [JsonProperty("public_flags")]
            public int PublicFlags { get; set; }
        }

        public class Attachment
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("filename")]
            public string Filename { get; set; }

            [JsonProperty("size")]
            public int Size { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("proxy_url")]
            public string ProxyUrl { get; set; }

            [JsonProperty("width")]
            public int Width { get; set; }

            [JsonProperty("height")]
            public int Height { get; set; }

            [JsonProperty("content_type")]
            public string ContentType { get; set; }
        }

        public class Field
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("value")]
            public string Value { get; set; }

            [JsonProperty("inline")]
            public bool Inline { get; set; }
        }

        public class Provider
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }
        }

        public class Thumbnail
        {
            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("proxy_url")]
            public string ProxyUrl { get; set; }

            [JsonProperty("width")]
            public int Width { get; set; }

            [JsonProperty("height")]
            public int Height { get; set; }
        }

        public class Video
        {
            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("width")]
            public int Width { get; set; }

            [JsonProperty("height")]
            public int Height { get; set; }

            [JsonProperty("proxy_url")]
            public string ProxyUrl { get; set; }
        }

        public class Embed
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("fields")]
            public List<Field> Fields { get; set; }

            [JsonProperty("provider")]
            public Provider Provider { get; set; }

            [JsonProperty("thumbnail")]
            public Thumbnail Thumbnail { get; set; }

            [JsonProperty("video")]
            public Video Video { get; set; }

            [JsonProperty("color")]
            public int? Color { get; set; }
        }

        public class Mention
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("username")]
            public string Username { get; set; }

            [JsonProperty("avatar")]
            public string Avatar { get; set; }

            [JsonProperty("avatar_decoration")]
            public object AvatarDecoration { get; set; }

            [JsonProperty("discriminator")]
            public string Discriminator { get; set; }

            [JsonProperty("public_flags")]
            public int PublicFlags { get; set; }
        }

        public class Emoji
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("animated")]
            public bool? Animated { get; set; }
        }

        public class Reaction
        {
            [JsonProperty("emoji")]
            public Emoji Emoji { get; set; }

            [JsonProperty("count")]
            public int Count { get; set; }

            [JsonProperty("me")]
            public bool Me { get; set; }
        }

        public class ChannelMessage
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("type")]
            public int Type { get; set; }

            [JsonProperty("content")]
            public string Content { get; set; }

            [JsonProperty("channel_id")]
            public string ChannelId { get; set; }

            [JsonProperty("author")]
            public Author Author { get; set; }

            [JsonProperty("attachments")]
            public List<Attachment> Attachments { get; set; }

            [JsonProperty("embeds")]
            public List<Embed> Embeds { get; set; }

            [JsonProperty("mentions")]
            public List<Mention> Mentions { get; set; }

            [JsonProperty("mention_roles")]
            public List<string> MentionRoles { get; set; }

            [JsonProperty("pinned")]
            public bool Pinned { get; set; }

            [JsonProperty("mention_everyone")]
            public bool MentionEveryone { get; set; }

            [JsonProperty("tts")]
            public bool Tts { get; set; }

            [JsonProperty("timestamp")]
            public DateTime Timestamp { get; set; }

            [JsonProperty("edited_timestamp")]
            public DateTime? EditedTimestamp { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("components")]
            public List<object> Components { get; set; }

            [JsonProperty("reactions")]
            public List<Reaction> Reactions { get; set; }
        }
    }
}
