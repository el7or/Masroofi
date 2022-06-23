using Puzzle.Masroofi.Core.Enums;
using System;

namespace Puzzle.Masroofi.Core.ViewModels.Auth
{
    public class UserIdentity
    {
        public UserIdentity(Guid? id, Language? language, ChannelType? channel)
        {
            Id = id;
            Language = language;
            Channel = channel;
        }

        public Guid? Id { get; private set; }
        public Language? Language { get; private set; }
        public ChannelType? Channel { get; private set; }
    }
}
