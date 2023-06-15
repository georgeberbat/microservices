using System;
using System.Threading.Tasks;
using AutoMapper;
using Profile.ServiceInterfaces.Repositories;
using Profile.ServiceInterfaces.Services;
using ProfileDomain;

namespace Profile.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<User> GetUser(Guid id)
        {
            var userDb = await _userRepository.GetUser(id);
            var user = _mapper.Map<User>(userDb);
            return user;
        }
    }
}