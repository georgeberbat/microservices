using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dex.Cap.Outbox.Interfaces;
using Profile.Dal.Domain;
using Profile.Dal.Repositories;
using Profile.Dal.Specifications;
using ProfileDomain;
using Shared.Dal;

namespace Profile.Services
{
    public class UserService : IUserService
    {
        private readonly IWriteUserRepository _writeUserRepository;
        private readonly IMapper _mapper;
        private readonly IUnityOfWork _dbContext;
        private readonly IOutboxService<IUnityOfWork> _outboxService;

        public UserService(IWriteUserRepository writeUserRepository, IMapper mapper, IUnityOfWork dbContext, IOutboxService<IUnityOfWork> outboxService)
        {
            _writeUserRepository = writeUserRepository;
            _mapper = mapper;
            _dbContext = dbContext;
            _outboxService = outboxService;
        }

        public async Task<ProfileDomain.User> GetUser(Guid id, CancellationToken cancellationToken)
        {
            return await _writeUserRepository.Read.GetByIdAsync(id, cancellationToken);
        }

        public async Task RegisterUser(IRegisterUserCommand request, CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            
            var userId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();

            await _outboxService.ExecuteOperationAsync(correlationId,
                new {DbContext = _dbContext, Mapper = _mapper, Repository = _writeUserRepository},
                async (token, ctx) =>
                {
                    var userDb = (await _writeUserRepository.Read
                            .FilterAsync(new ActiveUserByPhoneSpecification(request.PhoneNumber), token))
                        .FirstOrDefault();
                }, cancellationToken);
        }
    }
}