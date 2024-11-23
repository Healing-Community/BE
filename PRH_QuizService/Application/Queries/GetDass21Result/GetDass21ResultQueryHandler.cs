using Application.Commons;
using Application.Commons.Tools;
using Domain.Entities.DASS21;
using MediatR;

namespace Application.Queries.GetDass21Result
{
    public class GetDass21ResultQueryHandler(IMongoRepository<Dass21Result> mongoRepository) : IRequestHandler<GetDass21ResultQuery, BaseResponse<Dass21Result>>
    {
        public async Task<BaseResponse<Dass21Result>> Handle(GetDass21ResultQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Lấy UserId từ HttpContext và kiểm tra giá trị
                var userId = Authentication.GetUserIdFromHttpContext(request.HttpContext);

                // Tìm kết quả DASS-21 của người dùng theo UserId
                var result = await mongoRepository.GetByPropertyAsync(x => x.UserId == userId);
                if (result == null)
                {
                    return BaseResponse<Dass21Result>.NotFound();
                }

                // Thiết lập phản hồi thành công
                return BaseResponse<Dass21Result>.SuccessReturn(result);
            }
            catch(Exception ex)
            {
                return BaseResponse<Dass21Result>.InternalServerError(ex.Message);
            }
        }
    }
}
