using Application.Commons.Tools;
using Application.Commons;
using Domain.Entities.MBTI21;
using MediatR;
using NUlid;
using Application.Commons.DTOs;

namespace Application.Commands.CalcMbti21Quiz
{
    public class CalcMbti21QuizzCommandHandler : IRequestHandler<SubmitMBTI21ResultCommand, BaseResponse<Mbti21Result>>
    {
        private readonly IMongoRepository<Mbti21Result> _mongoRepository;

        public CalcMbti21QuizzCommandHandler(IMongoRepository<Mbti21Result> mongoRepository)
        {
            _mongoRepository = mongoRepository ?? throw new ArgumentNullException(nameof(mongoRepository));
        }

        public async Task<BaseResponse<Mbti21Result>> Handle(SubmitMBTI21ResultCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Kiểm tra tính hợp lệ của dữ liệu đầu vào
                if (!ValidateScores(request.MBTIQuizzResultRequest))
                {
                    return BaseResponse<Mbti21Result>.BadRequest("Dữ liệu không hợp lệ. Mỗi mảng điểm số phải chứa 5 phần tử với giá trị từ 0 đến 3.");
                }

                // Tính tổng điểm cho từng category
                int extroversionScore = request.MBTIQuizzResultRequest.Extroversion.Sum();
                int sensingScore = request.MBTIQuizzResultRequest.Sensing.Sum();
                int thinkingScore = request.MBTIQuizzResultRequest.Thinking.Sum();
                int judgingScore = request.MBTIQuizzResultRequest.Judging.Sum();

                // Sinh mô tả cho từng category
                string extroversionDescription = GetExtroversionDescription(extroversionScore);
                string sensingDescription = GetSensingDescription(sensingScore);
                string thinkingDescription = GetThinkingDescription(thinkingScore);
                string judgingDescription = GetJudgingDescription(judgingScore);

                // Sinh nhận xét tổng quát
                string overallComment = GenerateOverallComment(extroversionScore, sensingScore, thinkingScore, judgingScore);

                // Tính yếu tố ảnh hưởng, tác động ngắn hạn và dài hạn
                var factors = GenerateFactors(extroversionScore, sensingScore, thinkingScore, judgingScore);
                var shortTermEffects = GenerateShortTermEffects(extroversionScore, sensingScore, thinkingScore, judgingScore);
                var longTermEffects = GenerateLongTermEffects(extroversionScore, sensingScore, thinkingScore, judgingScore);

                // Lấy User ID
                var userId = Authentication.GetUserIdFromHttpContext(request.HttpContext);

                // Tạo kết quả
                var result = new Mbti21Result
                {
                    Id = Ulid.NewUlid().ToString(),
                    UserId = userId,
                    DateTaken = DateTime.UtcNow.AddHours(7),
                    ExtroversionScore = extroversionScore,
                    SensingScore = sensingScore,
                    ThinkingScore = thinkingScore,
                    JudgingScore = judgingScore,
                    ExtroversionDescription = extroversionDescription,
                    SensingDescription = sensingDescription,
                    ThinkingDescription = thinkingDescription,
                    JudgingDescription = judgingDescription,
                    OverallComment = overallComment,
                    Factors = factors,
                    ShortTermEffects = shortTermEffects,
                    LongTermEffects = longTermEffects
                };

                // Lưu kết quả vào MongoDB
                await _mongoRepository.Create(result);

                return BaseResponse<Mbti21Result>.SuccessReturn(result);
            }
            catch (Exception ex)
            {
                return BaseResponse<Mbti21Result>.InternalServerError($"Đã xảy ra lỗi: {ex.Message}");
            }
        }

        // Validate điểm số
        private bool ValidateScores(MBTIQuizzResultRequest request)
        {
            return ValidateArray(request.Extroversion) &&
                   ValidateArray(request.Sensing) &&
                   ValidateArray(request.Thinking) &&
                   ValidateArray(request.Judging);
        }

        private bool ValidateArray(int[] scores)
        {
            return scores != null && scores.Length == 5 && scores.All(x => x >= 0 && x <= 3);
        }

        // Sinh mô tả cho từng category
        private string GetExtroversionDescription(int score)
        {
            return score switch
            {
                <= 7 => "Hướng nội: Bạn thường thích dành thời gian cho bản thân và tránh các hoạt động xã hội quá đông đúc.",
                <= 12 => "Cân bằng: Bạn có thể cảm thấy thoải mái trong cả môi trường xã hội và khi ở một mình.",
                _ => "Hướng ngoại: Bạn thích giao tiếp, làm việc nhóm và thường xuyên tìm kiếm sự kết nối với người khác."
            };
        }

        private string GetSensingDescription(int score)
        {
            return score switch
            {
                <= 7 => "Chi tiết: Bạn tập trung vào các thông tin cụ thể và thực tế trong cuộc sống.",
                <= 12 => "Cân bằng: Bạn sử dụng cả giác quan thực tế và trực giác để ra quyết định.",
                _ => "Trực giác: Bạn thường tưởng tượng và suy nghĩ về các khả năng trong tương lai hơn là thực tế hiện tại."
            };
        }

        private string GetThinkingDescription(int score)
        {
            return score switch
            {
                <= 7 => "Cảm xúc: Bạn thường để cảm xúc chi phối các quyết định của mình.",
                <= 12 => "Cân bằng: Bạn sử dụng cả lý trí và cảm xúc để đưa ra quyết định.",
                _ => "Lý trí: Bạn ưu tiên dữ liệu và phân tích logic khi đưa ra quyết định."
            };
        }

        private string GetJudgingDescription(int score)
        {
            return score switch
            {
                <= 7 => "Linh hoạt: Bạn thích làm việc tự do và không thích bị ràng buộc bởi các kế hoạch cứng nhắc.",
                <= 12 => "Cân bằng: Bạn có thể linh hoạt hoặc làm việc có kế hoạch tùy tình huống.",
                _ => "Nguyên tắc: Bạn luôn có kế hoạch rõ ràng và thích hoàn thành công việc một cách trật tự."
            };
        }

        // Sinh nhận xét tổng quát
        private string GenerateOverallComment(int extroversion, int sensing, int thinking, int judging)
        {
            return "Bạn có sự kết hợp độc đáo giữa các khía cạnh tính cách, tạo nên một cá nhân đặc biệt với các ưu điểm riêng.";
        }

        private List<string> GenerateFactors(int extroversion, int sensing, int thinking, int judging)
        {
            return new List<string>
            {
                "Yếu tố môi trường ảnh hưởng đến cách bạn xử lý thông tin và ra quyết định.",
                "Sự cân bằng giữa trực giác và thực tế hỗ trợ khả năng lập kế hoạch hiệu quả.",
                "Khả năng giao tiếp xã hội nâng cao khi làm việc nhóm.",
                "Lựa chọn dựa trên lý trí hoặc cảm xúc có thể thay đổi theo tình huống."
            };
        }

        private List<string> GenerateShortTermEffects(int extroversion, int sensing, int thinking, int judging)
        {
            return new List<string>
            {
                "Cảm thấy thoải mái khi tương tác với đồng nghiệp.",
                "Thích ứng nhanh với các yêu cầu cụ thể trong công việc.",
                "Dễ dàng xử lý các vấn đề đòi hỏi phân tích logic.",
                "Hoàn thành công việc đúng hạn nhờ kỹ năng tổ chức tốt."
            };
        }

        private List<string> GenerateLongTermEffects(int extroversion, int sensing, int thinking, int judging)
        {
            return new List<string>
            {
                "Xây dựng các mối quan hệ xã hội bền vững.",
                "Phát triển tư duy chiến lược và khả năng giải quyết vấn đề.",
                "Cải thiện kỹ năng lãnh đạo và làm việc nhóm.",
                "Đạt được mục tiêu dài hạn nhờ sự kiên định và lập kế hoạch chi tiết."
            };
        }
    }
}
