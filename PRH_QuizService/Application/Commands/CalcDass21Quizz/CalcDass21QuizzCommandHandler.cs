using Application.Commons;
using Application.Commons.DTOs;
using Application.Commons.Tools;
using Domain.Entities.DASS21;
using MediatR;
using NUlid;

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
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (!ValidateScore(request.Dass21QuizzResultRequest))
                {
                    return BaseResponse<Dass21Result>.BadRequest("Dữ liệu không hợp lệ. Mỗi mảng điểm số phải chứa 7 phần tử với giá trị từ 0 đến 3.");
                }

                // Tính điểm Stress, Anxiety, Depression
                int stressScore = request.Dass21QuizzResultRequest.Score?.Stress?.Sum() ?? 0;
                int anxietyScore = request.Dass21QuizzResultRequest.Score?.Anxiety?.Sum() ?? 0;
                int depressionScore = request.Dass21QuizzResultRequest.Score?.Depression?.Sum() ?? 0;

                // Sinh mô tả
                string stressDescription = GetStressDescription(stressScore);
                string anxietyDescription = GetAnxietyDescription(anxietyScore);
                string depressionDescription = GetDepressionDescription(depressionScore);

                // Sinh lời nhận xét tổng quát
                string overallComment = GenerateOverallComment(stressScore, anxietyScore, depressionScore);

                // Tính các yếu tố ảnh hưởng
                List<string> factors = CalculateFactors(stressScore, anxietyScore, depressionScore, request);

                // Tính tác động ngắn hạn và dài hạn
                List<string> shortTermEffects = CalculateShortTermEffects(stressScore, anxietyScore, depressionScore);
                List<string> longTermEffects = CalculateLongTermEffects(stressScore, anxietyScore, depressionScore);

                // Lấy User ID
                var userId = Authentication.GetUserIdFromHttpContext(request.HttpContext);

                // Kiểm tra xem kết quả đã tồn tại hay chưa
                var existingResult = await _mongoRepository.GetByPropertyAsync(x => x.UserId == userId);

                if (existingResult != null)
                {
                    // Cập nhật kết quả nếu đã tồn tại
                    existingResult.StressScore = stressScore;
                    existingResult.AnxietyScore = anxietyScore;
                    existingResult.DepressionScore = depressionScore;
                    existingResult.SressDescription = stressDescription;
                    existingResult.AnxietyDescription = anxietyDescription;
                    existingResult.DepressionDescription = depressionDescription;
                    existingResult.OverallComment = overallComment;
                    existingResult.Factors = factors;
                    existingResult.ShortTermEffects = shortTermEffects;
                    existingResult.LongTermEffects = longTermEffects;
                    existingResult.DateTaken = DateTime.UtcNow + TimeSpan.FromHours(7); // UTC+7

                    // Cập nhật vào MongoDB
                    await _mongoRepository.Update(existingResult.Id, existingResult);

                    return BaseResponse<Dass21Result>.SuccessReturn(existingResult);
                }

                // Tạo kết quả mới nếu chưa tồn tại
                var newResult = new Dass21Result
                {
                    Id = Ulid.NewUlid().ToString(),
                    UserId = userId,
                    DateTaken = DateTime.UtcNow + TimeSpan.FromHours(7), // UTC+7
                    StressScore = stressScore,
                    AnxietyScore = anxietyScore,
                    DepressionScore = depressionScore,
                    SressDescription = stressDescription,
                    AnxietyDescription = anxietyDescription,
                    DepressionDescription = depressionDescription,
                    OverallComment = overallComment,
                    Factors = factors,
                    ShortTermEffects = shortTermEffects,
                    LongTermEffects = longTermEffects
                };

                // Lưu vào MongoDB
                await _mongoRepository.Create(newResult);

                return BaseResponse<Dass21Result>.SuccessReturn(newResult);
            }
            catch (Exception ex)
            {
                return BaseResponse<Dass21Result>.InternalServerError($"Đã xảy ra lỗi: {ex.Message}");
            }
        }



        // Kiểm tra tính hợp lệ của điểm số
        private bool ValidateScore(Dass21QuizzResultRequest request)
        {
            return ValidateArray(request?.Score?.Stress ?? [0, 0, 0, 0, 0, 0, 0]) &&
                   ValidateArray(request?.Score?.Anxiety ?? [0, 0, 0, 0, 0, 0, 0]) &&
                   ValidateArray(request?.Score?.Depression ?? [0, 0, 0, 0, 0, 0, 0]);
        }

        // Hỗ trợ kiểm tra tính hợp lệ của từng mảng điểm số
        private bool ValidateArray(int[] scores)
        {
            return scores != null && scores.Length == 7 && scores.All(x => x >= 0 && x <= 3);
        }
        private List<string> CalculateShortTermEffects(int stressScore, int anxietyScore, int depressionScore)
        {
            var shortTermEffects = new List<string>();

            if (stressScore > 14)
            {
                shortTermEffects.Add("Mất ngủ hoặc giấc ngủ không sâu, dẫn đến cảm giác mệt mỏi và thiếu năng lượng.");
                shortTermEffects.Add("Khả năng tập trung giảm sút, ảnh hưởng đến hiệu quả làm việc và học tập.");
                shortTermEffects.Add("Gia tăng nhịp tim và cảm giác căng thẳng trong cơ thể, đặc biệt ở vùng vai gáy.");
            }

            if (anxietyScore > 7)
            {
                shortTermEffects.Add("Cảm giác bất an, lo lắng không rõ nguyên nhân.");
                shortTermEffects.Add("Dễ cáu gắt hoặc phản ứng quá mức với các tình huống nhỏ.");
                shortTermEffects.Add("Khó thở hoặc cảm giác tức ngực do phản ứng căng thẳng tạm thời của cơ thể.");
            }

            if (depressionScore > 9)
            {
                shortTermEffects.Add("Cảm giác buồn bã hoặc vô vọng kéo dài trong nhiều giờ hoặc nhiều ngày.");
                shortTermEffects.Add("Mất hứng thú trong các hoạt động yêu thích hàng ngày.");
                shortTermEffects.Add("Suy giảm khả năng ra quyết định hoặc giải quyết vấn đề.");
            }

            return shortTermEffects;
        }

        private List<string> CalculateLongTermEffects(int stressScore, int anxietyScore, int depressionScore)
        {
            var longTermEffects = new List<string>();

            if (stressScore > 14)
            {
                longTermEffects.Add("Tăng nguy cơ phát triển các bệnh tim mạch, bao gồm tăng huyết áp và nhồi máu cơ tim.");
                longTermEffects.Add("Suy giảm hệ miễn dịch, khiến cơ thể dễ bị nhiễm trùng và các bệnh mãn tính khác.");
                longTermEffects.Add("Gia tăng nguy cơ mắc các rối loạn giấc ngủ mãn tính, như mất ngủ kéo dài.");
                longTermEffects.Add("Suy giảm trí nhớ và khả năng nhận thức do ảnh hưởng của hormone căng thẳng dài hạn.");
            }

            if (anxietyScore > 7)
            {
                longTermEffects.Add("Phát triển rối loạn lo âu mãn tính, làm giảm chất lượng cuộc sống.");
                longTermEffects.Add("Gia tăng khả năng mắc các bệnh liên quan đến tiêu hóa, chẳng hạn như hội chứng ruột kích thích.");
                longTermEffects.Add("Tăng nguy cơ bị cô lập xã hội do lo ngại hoặc sợ hãi tiếp xúc với người khác.");
                longTermEffects.Add("Mất cân bằng hormone, ảnh hưởng đến sức khỏe toàn diện của cơ thể.");
            }

            if (depressionScore > 9)
            {
                longTermEffects.Add("Gia tăng nguy cơ trầm cảm mãn tính, có thể ảnh hưởng đến mọi khía cạnh của cuộc sống.");
                longTermEffects.Add("Tăng khả năng lạm dụng chất kích thích như rượu và thuốc để đối phó với cảm xúc.");
                longTermEffects.Add("Ảnh hưởng nghiêm trọng đến các mối quan hệ cá nhân và xã hội, dẫn đến cô lập và cô đơn.");
                longTermEffects.Add("Gia tăng nguy cơ các vấn đề sức khỏe tâm lý nghiêm trọng hơn, chẳng hạn như ý nghĩ tự tử.");
            }

            return longTermEffects;
        }


        // Tính tác nhân ngắn hạn
        private List<string> CalculateFactors(int stressScore, int anxietyScore, int depressionScore, CalcDass21QuizzCommand request)
        {
            var factors = new List<string>();

            // Yếu tố liên quan đến stress
            if (stressScore > 14)
            {
                factors.Add("Áp lực công việc hoặc học tập kéo dài, không có thời gian thư giãn.");
                factors.Add("Thiếu ngủ hoặc giấc ngủ không đủ chất lượng, ảnh hưởng đến khả năng phục hồi của cơ thể.");
                factors.Add("Mâu thuẫn trong gia đình hoặc các mối quan hệ thân thiết, gây căng thẳng kéo dài.");
                factors.Add("Quản lý thời gian không hiệu quả, dẫn đến cảm giác luôn bị quá tải.");
            }

            // Yếu tố liên quan đến anxiety
            if (anxietyScore > 7)
            {
                factors.Add("Lo lắng về tương lai không chắc chắn, chẳng hạn như tài chính, công việc, hoặc học tập.");
                factors.Add("Áp lực xã hội hoặc kỳ vọng từ người khác, gây cảm giác không đáp ứng được yêu cầu.");
                factors.Add("Các trải nghiệm tiêu cực hoặc chấn thương tâm lý trong quá khứ, gây cảm giác bất an kéo dài.");
                factors.Add("Mối quan hệ không ổn định, như xung đột với bạn bè, đồng nghiệp hoặc người thân.");
            }

            // Yếu tố liên quan đến depression
            if (depressionScore > 9)
            {
                factors.Add("Mất mát quan trọng, chẳng hạn như mất đi người thân, công việc, hoặc cơ hội lớn trong cuộc sống.");
                factors.Add("Thiếu sự hỗ trợ tinh thần từ gia đình hoặc bạn bè, khiến cảm giác cô đơn gia tăng.");
                factors.Add("Lối sống không lành mạnh, bao gồm việc thiếu hoạt động thể chất và dinh dưỡng không cân đối.");
                factors.Add("Cảm giác thất bại hoặc không đạt được kỳ vọng của bản thân, dẫn đến mất động lực.");
            }

            return factors.Distinct().ToList(); // Loại bỏ trùng lặp
        }


        // Lấy mô tả cho điểm Stress
        private string GetStressDescription(int score)
        {
            string description = score switch
            {
                <= 14 => "Bình thường: Bạn không có dấu hiệu stress đáng lo ngại.",
                <= 18 => "Nhẹ: Bạn có thể đang gặp phải một số áp lực nhỏ trong cuộc sống, nhưng không đáng lo ngại.",
                <= 25 => "Trung bình: Áp lực từ công việc hoặc cuộc sống có thể đang ảnh hưởng đến bạn. Hãy cân nhắc nghỉ ngơi hoặc tập luyện thể thao để giảm stress.",
                <= 33 => "Nặng: Bạn đang chịu nhiều áp lực. Nguyên nhân có thể là từ công việc, gia đình, hoặc các vấn đề cá nhân. Hãy tìm cách thư giãn và chia sẻ với người thân hoặc bạn bè.",
                _ => "Rất nặng: Stress của bạn đang ở mức báo động. Có thể có nhiều nguyên nhân như áp lực công việc lớn, các mối quan hệ không tốt, hoặc sức khỏe yếu. Hãy tìm sự hỗ trợ từ chuyên gia tâm lý."
            };
            return description;
        }

        // Lấy mô tả cho điểm Anxiety
        private string GetAnxietyDescription(int score)
        {
            string description = score switch
            {
                <= 7 => "Bình thường: Bạn không có dấu hiệu lo âu.",
                <= 9 => "Nhẹ: Bạn có thể đang lo lắng về một số vấn đề trong cuộc sống hàng ngày, nhưng không nghiêm trọng.",
                <= 14 => "Trung bình: Sự lo âu của bạn có thể xuất phát từ công việc, tài chính hoặc sức khỏe. Hãy thử thiền, yoga hoặc viết nhật ký để giảm bớt lo lắng.",
                <= 19 => "Nặng: Lo âu của bạn có thể đang ảnh hưởng đến cuộc sống hàng ngày. Nguyên nhân có thể là áp lực công việc, mối quan hệ hoặc nỗi sợ hãi nào đó. Hãy cân nhắc tham khảo ý kiến chuyên gia.",
                _ => "Rất nặng: Bạn đang trải qua mức độ lo âu cao. Điều này có thể do căng thẳng kéo dài, các sự kiện đau buồn hoặc bệnh lý tâm lý. Hãy tìm đến chuyên gia tâm lý để được hỗ trợ."
            };
            return description;
        }

        // Lấy mô tả cho điểm Depression
        private string GetDepressionDescription(int score)
        {
            string description = score switch
            {
                <= 9 => "Bình thường: Bạn không có dấu hiệu trầm cảm.",
                <= 13 => "Nhẹ: Bạn có thể cảm thấy buồn chán trong một thời gian ngắn, nhưng điều này thường tự cải thiện.",
                <= 20 => "Trung bình: Tâm trạng của bạn có thể bị ảnh hưởng bởi các vấn đề như công việc, mối quan hệ hoặc mất mát. Hãy tìm cách trò chuyện với người thân hoặc bạn bè để giải tỏa.",
                <= 27 => "Nặng: Bạn có thể cảm thấy mất hứng thú với cuộc sống và thường xuyên buồn bã. Nguyên nhân có thể liên quan đến sự cô đơn, áp lực cuộc sống hoặc vấn đề sức khỏe tâm lý. Hãy gặp chuyên gia tâm lý để được tư vấn.",
                _ => "Rất nặng: Trầm cảm của bạn có thể ảnh hưởng nghiêm trọng đến cuộc sống hàng ngày. Hãy nhanh chóng tìm sự hỗ trợ từ chuyên gia tâm lý hoặc bác sĩ."
            };
            return description;
        }

        // Sinh lời nhận xét tổng quát dựa trên tổng điểm
        private string GenerateOverallComment(int stressScore, int anxietyScore, int depressionScore)
        {
            int totalScore = stressScore + anxietyScore + depressionScore;

            return totalScore switch
            {
                <= 20 =>
                "Tình trạng tâm lý của bạn có vẻ ổn định. " +
                "Nguyên nhân: Bạn đang duy trì sự cân bằng tốt giữa các yếu tố trong cuộc sống như công việc, gia đình và sức khỏe cá nhân. Điều này cho thấy bạn có khả năng đối phó với áp lực một cách hiệu quả và giữ được sự bình an trong tâm trí. " +
                "Lời khuyên: Hãy tiếp tục duy trì lối sống lành mạnh thông qua chế độ ăn uống cân bằng, hoạt động thể chất thường xuyên và các hoạt động giải trí. Đừng quên tạo thời gian để kết nối với người thân và bạn bè, vì đây là một phần quan trọng để duy trì sức khỏe tinh thần tích cực. Ngoài ra, hãy chú ý đến giấc ngủ và không ngần ngại tìm sự hỗ trợ nếu cần thiết.",

                <= 40 =>
                "Bạn có thể đang gặp một số căng thẳng nhẹ và lo âu. " +
                "Nguyên nhân: Điều này có thể xuất phát từ những thay đổi gần đây trong cuộc sống như áp lực công việc, trách nhiệm gia đình tăng cao, hoặc những bất ổn tài chính. Cũng có thể bạn đang thiếu thời gian để chăm sóc bản thân hoặc đang đối mặt với kỳ vọng cao từ xã hội hoặc môi trường xung quanh. " +
                "Lời khuyên: Hãy thực hiện những hoạt động giúp giải tỏa căng thẳng như thiền định, yoga, hoặc tập thể dục. Sắp xếp lại thời gian để ưu tiên cho giấc ngủ và thư giãn. Thực hành các kỹ năng quản lý thời gian để giảm bớt áp lực từ công việc hoặc các nghĩa vụ khác. Nếu cảm thấy khó khăn kéo dài, đừng ngần ngại tham khảo ý kiến từ chuyên gia tâm lý hoặc bác sĩ.",

                <= 60 =>
                "Bạn đang trải qua mức độ căng thẳng trung bình và có thể gặp phải các vấn đề liên quan đến lo âu và trầm cảm. " +
                "Nguyên nhân: Những thách thức lớn trong cuộc sống, chẳng hạn như công việc đòi hỏi cao, mối quan hệ phức tạp hoặc những mất mát cá nhân gần đây, có thể đang tạo áp lực lên sức khỏe tinh thần của bạn. Sự kết hợp giữa thiếu sự hỗ trợ xã hội và cảm giác bị choáng ngợp bởi các trách nhiệm cũng có thể là nguyên nhân chính. " +
                "Lời khuyên: Hãy bắt đầu bằng cách tìm kiếm các hoạt động mang lại niềm vui và giảm căng thẳng, chẳng hạn như gặp gỡ bạn bè, tham gia các sở thích yêu thích hoặc dành thời gian ngoài trời. Cân nhắc viết nhật ký để nhận diện và quản lý cảm xúc. Đặc biệt, nếu bạn cảm thấy tình trạng này kéo dài hoặc ngày càng nghiêm trọng, hãy tham khảo ý kiến chuyên gia để được hỗ trợ và tìm hiểu thêm về các phương pháp trị liệu phù hợp như liệu pháp hành vi nhận thức (CBT) hoặc tư vấn tâm lý cá nhân.",

                _ =>
                "Tình trạng tâm lý của bạn đang ở mức đáng lo ngại. " +
                "Nguyên nhân: Áp lực tích lũy từ công việc hoặc cuộc sống cá nhân có thể đã vượt quá khả năng kiểm soát. Những trải nghiệm đau buồn, mâu thuẫn kéo dài trong các mối quan hệ, hoặc cảm giác bị cô lập có thể đang làm trầm trọng thêm tình trạng tâm lý của bạn. Bên cạnh đó, các vấn đề sức khỏe thể chất hoặc di truyền cũng có thể góp phần gây nên tình trạng này. " +
                "Lời khuyên: Đừng cố gắng vượt qua tình trạng này một mình. Hãy tìm kiếm sự hỗ trợ từ các chuyên gia tâm lý hoặc bác sĩ tâm thần ngay lập tức. Ngoài ra, hãy chia sẻ cảm xúc với những người bạn tin tưởng để giảm bớt cảm giác cô đơn. Đôi khi, việc thay đổi môi trường sống, tìm kiếm liệu pháp trị liệu chuyên sâu hoặc sử dụng các phương pháp như thiền chánh niệm có thể mang lại hiệu quả tích cực. Hãy nhớ rằng, sức khỏe tinh thần là một phần quan trọng không thể thiếu trong cuộc sống, và việc tìm kiếm hỗ trợ là bước đầu tiên để vượt qua những khó khăn này."
            };
        }

    }
}
