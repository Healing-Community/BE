using System;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;

namespace Application.Commands_Queries.Commands.Users.BlockUser.BlockModerator;

public class BlockModeratorCommandHandler(IUserRepository userRepository) : IRequestHandler<BlockModeratorCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(BlockModeratorCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userRepository.GetByPropertyAsync(u => u.UserId == request.UserId && u.RoleId == 3);
            if (user == null)
            {
                return BaseResponse<string>.NotFound("Không tìm thấy người dùng");
            }
            user.Status = request.Status;
            await userRepository.UpdateAsync(user.UserId, user);
            if (request.Status == 0)
            {
                return BaseResponse<string>.SuccessReturn("Mở khóa tài khoản kiểm duyệt viên thành công");
            }
            return BaseResponse<string>.SuccessReturn("Khóa tài khoản kiểm duyệt viên thành công");
        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(e.Message);
            throw;
        }
    }
}
