using Application.Commons;
using Application.Commons.DTOs;
using Application.Commons.Tools;
using Domain.Entities.DASS21;
using MassTransit;
using MediatR;

namespace Application.Commands.CalcDass21Quizz
{
    public class CalcDass21QuizzCommandHandler : IRequestHandler<CalcDass21QuizzCommand, BaseResponse<Dass21Result>>
    {
        private readonly IMongoRepository<Dass21Result> _mongoRepository;

        public CalcDass21QuizzCommandHandler(IMongoRepository<Dass21Result> mongoRepository)
        {
            _mongoRepository = mongoRepository ?? throw new ArgumentNullException(nameof(mongoRepository));
        }

        public async Task<BaseResponse<Dass21Result>> Handle(CalcDass21QuizzCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<Dass21Result>
            {
                Id = NewId.NextSequentialGuid(),
                Timestamp = DateTime.UtcNow
            };

            try
            {
                // Kiểm tra tính hợp lệ của dữ liệu đầu vào
                if (!ValidateScore(request.Dass21QuizzResultRequest))
                {
                    response.Success = false;
                    response.StatusCode = 400;
                    response.Message = "Dữ liệu không hợp lệ. Mỗi mảng phải có 7 phần tử và các giá trị nằm trong khoảng từ 0 đến 4.";
                    return response;
                }

                // Tính điểm Stress
                int stressScore = request.Dass21QuizzResultRequest.Score?.Stress?.Sum() ?? 0;

                // Tính điểm Anxiety
                int anxietyScore = request.Dass21QuizzResultRequest.Score?.Anxiety?.Sum() ?? 0;

                // Tính điểm Depression
                int depressionScore = request.Dass21QuizzResultRequest.Score?.Depression?.Sum() ?? 0;

                // Đánh giá mức độ mô tả dựa trên điểm số
                string stressDescription = GetStressDescription(stressScore);
                string anxietyDescription = GetAnxietyDescription(anxietyScore);
                string depressionDescription = GetDepressionDescription(depressionScore);

                // Thêm lời nhận xét chung
                string overallComment = GenerateOverallComment(stressScore, anxietyScore, depressionScore);

                var userId = Authentication.GetUserIdFromHttpContext(request.HttpContext);
                var userGuid = Guid.Parse(userId);

                // Kiểm tra xem kết quả đã tồn tại hay chưa
                var existingResult = await _mongoRepository.GetByPropertyAsync(x => x.UserId == userGuid);

                if (existingResult != null)
                {
                    // Cập nhật bản ghi nếu đã tồn tại
                    existingResult.StressScore = stressScore;
                    existingResult.AnxietyScore = anxietyScore;
                    existingResult.DepressionScore = depressionScore;
                    existingResult.SressDescription = stressDescription;
                    existingResult.AnxietyDescription = anxietyDescription;
                    existingResult.DepressionDescription = depressionDescription;
                    existingResult.OverallComment = overallComment;
                    existingResult.DateTaken = request.Dass21QuizzResultRequest.DateTaken;

                    // Cập nhật bản ghi vào MongoDB
                    await _mongoRepository.Update(existingResult.Id, existingResult);

                    // Thiết lập phản hồi thành công
                    response.Data = existingResult;
                    response.Message = "DASS-21 kết quả đã được cập nhật thành công.";
                }
                else
                {
                    // Tạo đối tượng kết quả mới nếu không tồn tại
                    var newResult = new Dass21Result
                    {
                        Id = NewId.NextSequentialGuid(),
                        UserId = userGuid,
                        DateTaken = request.Dass21QuizzResultRequest.DateTaken,
                        StressScore = stressScore,
                        AnxietyScore = anxietyScore,
                        DepressionScore = depressionScore,
                        SressDescription = stressDescription,
                        AnxietyDescription = anxietyDescription,
                        DepressionDescription = depressionDescription,
                        OverallComment = overallComment
                    };

                    // Lưu kết quả mới vào MongoDB
                    await _mongoRepository.Create(newResult);

                    // Thiết lập phản hồi thành công
                    response.Data = newResult;
                    response.Message = "DASS-21 kết quả đã được tính toán và lưu trữ thành công.";
                }

                response.Success = true;
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.StatusCode = 500;

                // Xử lý lỗi và thêm thông tin vào phản hồi
                response.Errors?.Add(ex.Message);
                response.Message = "Có lỗi xảy ra khi tính toán kết quả DASS-21.";
            }

            return response;
        }

        // Phương thức kiểm tra tính hợp lệ của điểm số
        private bool ValidateScore(Dass21QuizzResultRequest request)
        {
            // Kiểm tra mảng stress
            if (request.Score.Stress == null || request.Score.Stress.Length != 7 || !request.Score.Stress.All(x => x >= 0 && x <= 4))
            {
                return false;
            }

            // Kiểm tra mảng anxiety
            if (request.Score.Anxiety == null || request.Score.Anxiety.Length != 7 || !request.Score.Anxiety.All(x => x >= 0 && x <= 4))
            {
                return false;
            }

            // Kiểm tra mảng depression
            if (request.Score.Depression == null || request.Score.Depression.Length != 7 || !request.Score.Depression.All(x => x >= 0 && x <= 4))
            {
                return false;
            }

            // Nếu tất cả các điều kiện đều đúng
            return true;
        }

        // Phương thức để đánh giá mô tả mức độ stress
        private string GetStressDescription(int score)
        {
            if (score <= 14) return "Bình thường";
            if (score <= 18) return "Nhẹ";
            if (score <= 25) return "Trung bình";
            if (score <= 33) return "Nặng";
            return "Rất nặng";
        }

        // Phương thức để đánh giá mô tả mức độ anxiety
        private string GetAnxietyDescription(int score)
        {
            if (score <= 7) return "Bình thường";
            if (score <= 9) return "Nhẹ";
            if (score <= 14) return "Trung bình";
            if (score <= 19) return "Nặng";
            return "Rất nặng";
        }

        // Phương thức để đánh giá mô tả mức độ depression
        private string GetDepressionDescription(int score)
        {
            if (score <= 9) return "Bình thường";
            if (score <= 13) return "Nhẹ";
            if (score <= 20) return "Trung bình";
            if (score <= 27) return "Nặng";
            return "Rất nặng";
        }

        // Phương thức để tạo lời nhận xét chung dựa trên điểm tổng
        private string GenerateOverallComment(int stressScore, int anxietyScore, int depressionScore)
        {
            int totalScore = stressScore + anxietyScore + depressionScore;

            if (totalScore <= 20)
            {
                return "Tình trạng tâm lý của bạn có vẻ ổn định. Bạn không có dấu hiệu đáng lo ngại về stress, lo âu hoặc trầm cảm.";
            }
            else if (totalScore <= 40)
            {
                return "Bạn có thể đang gặp một số căng thẳng nhẹ và lo âu. Hãy cố gắng thư giãn và tìm cách giảm bớt áp lực.";
            }
            else if (totalScore <= 60)
            {
                return "Bạn đang trải qua mức độ căng thẳng trung bình và có thể gặp phải các vấn đề liên quan đến lo âu và trầm cảm. Hãy cân nhắc tham khảo ý kiến chuyên gia nếu tình trạng kéo dài.";
            }
            else
            {
                return "Tình trạng tâm lý của bạn đang ở mức đáng lo ngại. Bạn nên gặp gỡ chuyên gia tâm lý để được tư vấn và hỗ trợ.";
            }
        }
    }
}
