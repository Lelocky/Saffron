using AutoMapper;
using Spice.DiscordClient.Models;
using Spice.Saffron.ViewModels;

namespace Spice.Saffron.Configuration
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ChannelMessages, ChannelMessagesViewModel>();
            CreateMap<ChannelMessages.Author, ChannelMessagesViewModel.Author>();
            CreateMap<ChannelMessages.Attachment, ChannelMessagesViewModel.Attachment>();
            CreateMap<ChannelMessages.Field, ChannelMessagesViewModel.Field>();
            CreateMap<ChannelMessages.Provider, ChannelMessagesViewModel.Provider>();
            CreateMap<ChannelMessages.Thumbnail, ChannelMessagesViewModel.Thumbnail>();
            CreateMap<ChannelMessages.Video, ChannelMessagesViewModel.Video>();
            CreateMap<ChannelMessages.Embed, ChannelMessagesViewModel.Embed>();
            CreateMap<ChannelMessages.Mention, ChannelMessagesViewModel.Mention>();
            CreateMap<ChannelMessages.Emoji, ChannelMessagesViewModel.Emoji>();
            CreateMap<ChannelMessages.Reaction, ChannelMessagesViewModel.Reaction>();
            CreateMap<ChannelMessages.ChannelMessage, ChannelMessagesViewModel.ChannelMessage>();
            CreateMap<ChannelMessages.Embed, ChannelMessagesViewModel.Embed>();
        }
    }
}
