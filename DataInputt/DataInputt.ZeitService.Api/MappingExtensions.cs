namespace DataInputt.ZeitService.Api
{
    public static class MappingExtensions
    {
        public static User ToUser(this UserDto userDto)
        {
            return new User
            {
                Name = userDto.Name,
                Passwort = userDto.Passwort
            };
        }

        public static UserDto ToUserDto(this User user)
        {
            return new UserDto
            {
                Name = user.Name,
                Passwort = user.Passwort
            };
        }

        public static TimeDto ToTimeDto(this Time time)
        {
            return new TimeDto
            {
                Start = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(time.Start.ToUniversalTime()),
                End = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(time.End.ToUniversalTime()),
                Project = time.Project,
                Uid = time.uId,
                Id = time.Id
            };
        }

        public static Time ToTime(this TimeDto timeDto)
        {
            return new Time
            {
                Start = timeDto.Start.ToDateTime(),
                End = timeDto.End.ToDateTime(),
                Project = timeDto.Project,
                uId = timeDto.Uid,
                Id = timeDto.Id
            };
        }
    }
}
