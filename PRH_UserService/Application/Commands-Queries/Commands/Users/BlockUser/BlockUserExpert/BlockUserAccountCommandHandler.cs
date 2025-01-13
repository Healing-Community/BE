using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;

namespace Application.Commands_Queries.Commands.Users.BlockUser;

public record BlockUserAccountCommandHandler(IUserRepository UserRepository) : IRequestHandler<BlockUserAccountCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(BlockUserAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await UserRepository.GetByPropertyAsync(u=>u.UserId == request.UserId && (u.RoleId == 2 || u.RoleId == 4));
        if (user == null)
        {
            return BaseResponse<string>.NotFound("Không tìm thấy người dùng");
        }
        user.Status = request.Status;
        await UserRepository.UpdateAsync(user.UserId, user);

        if (request.Status == 0)
        {
            return BaseResponse<string>.SuccessReturn("Mở khóa tài khoản thành công");
        }
        return BaseResponse<string>.SuccessReturn("Khóa tài khoản thành công");
    }
}
