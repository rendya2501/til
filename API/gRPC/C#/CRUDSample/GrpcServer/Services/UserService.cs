using Grpc.Core;
using GrpcServer.Data.Repositories;

namespace GrpcCRUDSample;

public class UserService : User.UserBase
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public override async Task<GetUserResponse> GetUser(GetUserRequest request, ServerCallContext context)
    {
        var user = await _userRepository.GetUserByIdAsync(request.Id);

        if (user == null)
        {
            _logger.LogInformation($"User with ID {request.Id} not found.");
            throw new RpcException(new Status(StatusCode.NotFound, $"User with ID {request.Id} not found."));
        }
        var response = new GetUserResponse
        {
            User = new AllUserModel
            {
                Id = user.Id,
                Name = user.Name,
                UniqueId = user.UniqueID
            }
        };

        return response;
    }

    public override async Task<GetAllUsersResponse> GetAllUsers(Empty request, ServerCallContext context)
    {
        var users = await _userRepository.GetAllUsersAsync();
        var responseUsers = users.Select(user => new UserModel { Id = user.Id, Name = user.Name }).ToList();

        return new GetAllUsersResponse { Users = { responseUsers } };
    }

    public override async Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        var user = new GrpcServer.Models.User { Name = request.Name };
        int userId = await _userRepository.CreateUserAsync(user);

        return new CreateUserResponse { Id = userId };
    }

    public override async Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request, ServerCallContext context)
    {
        var user = new GrpcServer.Models.User { Id = request.User.Id, Name = request.User.Name };
        bool success = await _userRepository.UpdateUserAsync(user);

        return new UpdateUserResponse { Success = success };
    }

    public override async Task<DeleteUserResponse> DeleteUser(DeleteUserRequest request, ServerCallContext context)
    {
        bool success = await _userRepository.DeleteUserAsync(request.Id);

        return new DeleteUserResponse { Success = success };
    }
}