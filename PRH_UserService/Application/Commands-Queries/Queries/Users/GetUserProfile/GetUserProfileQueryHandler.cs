using Application.Commons;
using Application.Interfaces.Repository;
using AutoMapper;
using MediatR;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, BaseResponse<UserProfileDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserProfileQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<BaseResponse<UserProfileDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            return BaseResponse<UserProfileDto>.NotFound();
        }

        var userProfile = _mapper.Map<UserProfileDto>(user);
        return BaseResponse<UserProfileDto>.SuccessReturn(userProfile);
    }
}