using Application.Commons.Tools;
using Application.Commons;
using Domain.Entities.MBTI21;
using MediatR;
using NUlid;
using Application.Commons.DTOs;
using Domain.Entities.DASS21;

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
                List<string> factors = GenerateFactors(extroversionScore, sensingScore, thinkingScore, judgingScore, request);
                List<string> shortTermEffects = GenerateShortTermEffects(extroversionScore, sensingScore, thinkingScore, judgingScore);
                List<string> longTermEffects = GenerateLongTermEffects(extroversionScore, sensingScore, thinkingScore, judgingScore);

                // Lấy User ID
                var userId = Authentication.GetUserIdFromHttpContext(request.HttpContext);

                var existingResult = await _mongoRepository.GetByPropertyAsync(x => x.UserId == userId);
                if (existingResult != null)
                {
                    existingResult.ExtroversionScore = extroversionScore;
                    existingResult.SensingScore = extroversionScore;
                    existingResult.ThinkingScore = thinkingScore;
                    existingResult.JudgingScore = judgingScore;
                    existingResult.ExtroversionDescription = extroversionDescription;
                    existingResult.SensingDescription = sensingDescription;
                    existingResult.ThinkingDescription = thinkingDescription;
                    existingResult.JudgingDescription = judgingDescription;
                    existingResult.OverallComment = overallComment;
                    existingResult.Factors = factors;
                    existingResult.ShortTermEffects = shortTermEffects;
                    existingResult.LongTermEffects = longTermEffects;
                    existingResult.DateTaken = DateTime.UtcNow + TimeSpan.FromHours(7);
                    // Cập nhật vào MongoDB
                    await _mongoRepository.Update(existingResult.Id, existingResult);
                    return BaseResponse<Mbti21Result>.SuccessReturn(existingResult);
                }

                 // Tạo kết quả
                var result = new Mbti21Result
                {
                    Id = Ulid.NewUlid().ToString(),
                    UserId = userId,
                    DateTaken = DateTime.UtcNow + TimeSpan.FromHours(7),
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
        private string GenerateOverallComment(int extroversionScore, int sensingScore, int thinkingScore, int judgingScore)
        {
            int totalScore = extroversionScore + sensingScore + thinkingScore + judgingScore;

            return totalScore switch
            {
                <= 28 =>
                "Tính cách của bạn thiên về sự yên tĩnh, linh hoạt và cảm xúc. " +
                "Nguyên nhân: Có thể bạn là người hướng nội, thích làm việc một cách tự do và dựa nhiều vào cảm giác và cảm xúc trong các quyết định của mình. Bạn thường tập trung vào các chi tiết thực tế và không bị ràng buộc bởi kế hoạch cứng nhắc. " +
                "Lời khuyên: Hãy tận dụng những điểm mạnh này để làm việc trong môi trường phù hợp. Nếu bạn cảm thấy cần phát triển khả năng giao tiếp hoặc lập kế hoạch rõ ràng hơn, hãy thử đặt mục tiêu nhỏ để cải thiện từng bước. Đồng thời, đừng quên dành thời gian để chăm sóc sức khỏe tinh thần và thể chất của mình.",

                <= 44 =>
                "Bạn có xu hướng cân bằng giữa sự hướng ngoại và hướng nội, giữa lý trí và cảm xúc. " +
                "Nguyên nhân: Bạn là người linh hoạt, dễ thích nghi với các môi trường khác nhau và có thể sử dụng cả trực giác lẫn phân tích thực tế để đưa ra quyết định. Điều này giúp bạn dễ dàng hòa nhập và làm việc hiệu quả trong các tình huống khác nhau. " +
                "Lời khuyên: Hãy tiếp tục tận dụng sự cân bằng này để phát triển sự nghiệp và các mối quan hệ. Nếu cảm thấy có một khía cạnh nào đó bạn muốn cải thiện (như kỹ năng giao tiếp hoặc tư duy logic), hãy tập trung vào việc rèn luyện trong từng lĩnh vực cụ thể.",

                <= 60 =>
                "Tính cách của bạn nổi bật với sự chủ động, kế hoạch rõ ràng và khả năng suy luận tốt. " +
                "Nguyên nhân: Bạn là người thích kết nối xã hội, lập kế hoạch chi tiết và đưa ra quyết định dựa trên phân tích logic. Điều này cho thấy bạn có khả năng lãnh đạo tốt và dễ dàng đạt được mục tiêu trong môi trường có tổ chức. " +
                "Lời khuyên: Hãy sử dụng thế mạnh này để đạt được thành công trong công việc và cuộc sống. Tuy nhiên, đừng quên duy trì sự linh hoạt và dành thời gian để thư giãn. Đôi khi, việc lắng nghe cảm xúc cá nhân và chấp nhận những thay đổi không dự đoán trước sẽ giúp bạn trở nên hoàn thiện hơn.",

                _ =>
                "Bạn là người hướng ngoại nổi bật với sự quyết đoán, tầm nhìn xa và tổ chức mạnh mẽ. " +
                "Nguyên nhân: Điểm số cao cho thấy bạn thích tương tác xã hội, sử dụng trực giác để đưa ra các quyết định sáng tạo và làm việc với kế hoạch rõ ràng. Bạn có thể là người dẫn đầu trong nhóm hoặc tổ chức, với khả năng truyền cảm hứng cho người khác. " +
                "Lời khuyên: Đừng quên giữ sự cân bằng giữa công việc và cuộc sống cá nhân. Đôi khi, việc lắng nghe và tìm hiểu quan điểm của người khác sẽ giúp bạn xây dựng mối quan hệ bền vững hơn. Hãy đảm bảo rằng bạn không quá đặt nặng vào mục tiêu mà bỏ qua những khoảnh khắc ý nghĩa trong cuộc sống."
            };
        }

        private List<string> GenerateFactors(int extroversion, int sensing, int thinking, int judging, SubmitMBTI21ResultCommand request)
        {
            var factors = new List<string>();

            if (extroversion > 7)
            {
                factors.Add("Môi trường làm việc nhóm hoặc giao tiếp xã hội có ảnh hưởng lớn đến tâm lý và hiệu suất.");
                factors.Add("Sự cởi mở với người khác giúp bạn dễ dàng xây dựng quan hệ.");
                factors.Add("Thích các hoạt động sôi động hoặc đòi hỏi sự tương tác liên tục.");
            }

            if (sensing > 7)
            {
                factors.Add("Sự cân bằng giữa việc tập trung vào chi tiết và bức tranh tổng thể ảnh hưởng đến cách giải quyết công việc.");
                factors.Add("Thói quen làm việc thực tế giúp bạn đưa ra quyết định nhanh chóng.");
                factors.Add("Khả năng phân tích các thông tin cụ thể và áp dụng chúng vào thực tế.");
            }

            if (thinking > 7)
            {
                factors.Add("Xu hướng ra quyết định dựa trên lý trí thay vì cảm xúc.");
                factors.Add("Khả năng phân tích dữ liệu và áp dụng chúng vào các tình huống phức tạp.");
                factors.Add("Cách bạn đánh giá tình huống thường dựa trên tính logic và hiệu quả.");
            }

            if (judging > 7)
            {
                factors.Add("Thích các kế hoạch rõ ràng, lịch trình cụ thể và các quy trình được chuẩn hóa.");
                factors.Add("Cảm thấy thoải mái khi mọi thứ được tổ chức và kiểm soát tốt.");
                factors.Add("Tập trung vào việc hoàn thành mục tiêu thay vì điều chỉnh quá nhiều theo tình huống.");
            }

            return factors.Distinct().ToList(); // Loại bỏ trùng lặp
        }

        private List<string> GenerateShortTermEffects(int extroversion, int sensing, int thinking, int judging)
        {
            var shortTermEffects = new List<string>();

            if (extroversion > 7)
            {
                shortTermEffects.Add("Cảm thấy thoải mái khi tương tác với đồng nghiệp và các nhóm lớn.");
                shortTermEffects.Add("Tăng cường năng lượng khi tham gia các hoạt động xã hội hoặc thảo luận nhóm.");
                shortTermEffects.Add("Dễ dàng xây dựng mối quan hệ thân thiện trong thời gian ngắn.");
            }

            if (sensing > 7)
            {
                shortTermEffects.Add("Thích ứng nhanh với các yêu cầu cụ thể và chi tiết trong công việc.");
                shortTermEffects.Add("Có khả năng tập trung vào các vấn đề thực tiễn và giải quyết vấn đề ngay lập tức.");
                shortTermEffects.Add("Chú ý đến các chi tiết nhỏ, giúp cải thiện độ chính xác trong nhiệm vụ.");
            }

            if (thinking > 7)
            {
                shortTermEffects.Add("Xử lý tốt các vấn đề đòi hỏi tư duy logic và phân tích.");
                shortTermEffects.Add("Đưa ra quyết định dựa trên lý trí, giúp giảm thiểu cảm xúc không cần thiết.");
                shortTermEffects.Add("Tăng khả năng giải quyết các tình huống khó khăn một cách khách quan.");
            }

            if (judging > 7)
            {
                shortTermEffects.Add("Hoàn thành công việc đúng hạn nhờ kỹ năng tổ chức và lập kế hoạch tốt.");
                shortTermEffects.Add("Luôn cảm thấy thoải mái khi tuân theo các quy trình hoặc lịch trình rõ ràng.");
                shortTermEffects.Add("Duy trì sự tập trung và hiệu quả trong các nhiệm vụ ngắn hạn.");
            }

            return shortTermEffects;
        }

        private List<string> GenerateLongTermEffects(int extroversion, int sensing, int thinking, int judging)
        {
            var longTermEffects = new List<string>();

            if (extroversion > 7)
            {
                longTermEffects.Add("Xây dựng và duy trì các mối quan hệ xã hội bền vững.");
                longTermEffects.Add("Phát triển kỹ năng giao tiếp chuyên sâu, hỗ trợ thăng tiến trong công việc.");
                longTermEffects.Add("Gia tăng sự tự tin và khả năng thuyết phục người khác trong các tình huống quan trọng.");
            }

            if (sensing > 7)
            {
                longTermEffects.Add("Nâng cao khả năng thực hiện các dự án dài hạn nhờ sự tập trung vào thực tế.");
                longTermEffects.Add("Phát triển thói quen làm việc chi tiết và chính xác, giảm thiểu lỗi trong công việc.");
                longTermEffects.Add("Gia tăng sự ổn định và đáng tin cậy trong công việc và cuộc sống cá nhân.");
            }

            if (thinking > 7)
            {
                longTermEffects.Add("Phát triển tư duy chiến lược và khả năng giải quyết vấn đề hiệu quả.");
                longTermEffects.Add("Cải thiện kỹ năng lãnh đạo thông qua các quyết định dựa trên phân tích.");
                longTermEffects.Add("Xây dựng danh tiếng là một người làm việc logic và đáng tin cậy.");
            }

            if (judging > 7)
            {
                longTermEffects.Add("Đạt được các mục tiêu dài hạn nhờ lập kế hoạch chi tiết và thực hiện nghiêm túc.");
                longTermEffects.Add("Phát triển tính kỷ luật cao, hỗ trợ duy trì hiệu quả làm việc lâu dài.");
                longTermEffects.Add("Tạo ra các hệ thống và quy trình giúp tăng năng suất và giảm căng thẳng.");
            }

            return longTermEffects;
        }

    }
}
