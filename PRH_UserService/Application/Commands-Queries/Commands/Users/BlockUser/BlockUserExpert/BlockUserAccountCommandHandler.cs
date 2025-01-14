using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;

namespace Application.Commands_Queries.Commands.Users.BlockUser;

public class BlockUserAccountCommandHandler(IUserRepository userRepository) : IRequestHandler<BlockUserAccountCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(BlockUserAccountCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Fetch user by UserId and specific roles
            var user = await userRepository.GetByPropertyAsync(
                u => u.UserId == request.UserId
            );

            if (user == null)
            {
                return BaseResponse<string>.NotFound("Không tìm thấy người dùng");
            }

            if (user.RoleId == 2 || user.RoleId == 3)
            {
                return BaseResponse<string>.BadRequest("Không thể khóa tài khoản của Admin hoặc Moderator");
            }
            if (user.Status == request.Status)
            {
                return BaseResponse<string>.BadRequest("Tài khoản đã ở trạng thái này");
            }

            // Update user status
            user.Status = request.Status;
            await userRepository.UpdateAsync(user.UserId, user);

            var message = request.Status == 0
                ? "Khóa tài khoản thành công"
                : "Mở khóa tài khoản thành công";

            return BaseResponse<string>.SuccessReturn(message);
        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(e.Message);
        }
    }
}
