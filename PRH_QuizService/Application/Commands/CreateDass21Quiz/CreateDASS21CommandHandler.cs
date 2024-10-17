using Application.Commons;
using Domain.Entities.DASS21;
using MassTransit;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.CreateDass21Quiz
{
    public class CreateDASS21CommandHandler : IRequestHandler<CreateDASS21Command, BaseResponse<bool>>
    {
        private readonly IMongoRepository<Dass21> _mongoRepository;

        public CreateDASS21CommandHandler(IMongoRepository<Dass21> mongoRepository)
        {
            _mongoRepository = mongoRepository ?? throw new ArgumentNullException(nameof(mongoRepository));
        }

        public async Task<BaseResponse<bool>> Handle(CreateDASS21Command request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<bool>
            {
                Id = NewId.NextSequentialGuid(),
                Timestamp = DateTime.UtcNow
            };

            try
            {
                // Tạo dữ liệu DASS-21 thêm cứng (hardcoded)
                var dass21 = new Dass21
                {
                    Id = NewId.NextSequentialGuid(),
                    Dass21Categories = new List<Category>
                    {
                        new Category
                        {
                            CategoryName = "Depression",
                            Questions = new List<Question>
                            {
                                new Question
                                {
                                    QuestionText = "Tôi cảm thấy không còn hứng thú với bất cứ điều gì.",
                                    Type = "radio",
                                    Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                                },
                                new Question
                                {
                                    QuestionText = "Tôi thấy mình không thể trải nghiệm cảm xúc tích cực.",
                                    Type = "radio",
                                    Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                                },
                                new Question
                                {
                                    QuestionText = "Tôi cảm thấy không còn năng lượng hoặc mệt mỏi suốt ngày.",
                                    Type = "radio",
                                    Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                                },
                                new Question
                                {
                                    QuestionText = "Tôi thấy bản thân mình vô dụng hoặc không có giá trị.",
                                    Type = "radio",
                                    Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                                },
                                new Question
                                {
                                    QuestionText = "Tôi cảm thấy cuộc sống không có gì đáng để sống.",
                                    Type = "radio",
                                    Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                                },
                                new Question
                                {
                                    QuestionText = "Tôi khó mà bắt đầu bất cứ điều gì.",
                                    Type = "radio",
                                    Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                                },
                                new Question
                                {
                                    QuestionText = "Tôi cảm thấy buồn chán hoặc không vui.",
                                    Type = "radio",
                                    Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                                }
                            }
                        },
                        new Category
                        {
                            CategoryName = "Anxiety",
                            Questions = new List<Question>
                            {
                                new Question
                                {
                                    QuestionText = "Tôi cảm thấy miệng mình khô.",
                                    Type = "radio",
                                    Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                                },
                                new Question
                                {
                                    QuestionText = "Tôi có cảm giác lo sợ mà không có lý do rõ ràng.",
                                    Type = "radio",
                                    Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                                },
                                new Question
                                {
                                    QuestionText = "Tôi thấy khó mà thư giãn.",
                                    Type = "radio",
                                    Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                                },
                                new Question
                                {
                                    QuestionText = "Tôi cảm thấy run rẩy hoặc có cảm giác rối loạn.",
                                    Type = "radio",
                                    Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                                },
                                new Question
                                {
                                    QuestionText = "Tôi cảm thấy rất lo lắng sợ rằng điều tồi tệ sắp xảy ra.",
                                    Type = "radio",
                                    Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                                },
                                new Question
                                {
                                    QuestionText = "Tôi có cảm giác rợn người trong bụng.",
                                    Type = "radio",
                                    Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                                },
                                new Question
                                {
                                    QuestionText = "Tôi cảm thấy choáng ngợp hoặc choáng váng.",
                                    Type = "radio",
                                    Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                                }
                            }
                        },
                        new Category
                        {
                            CategoryName = "Stress",
                            Questions = new List<Question>
                            {
                                new Question
                                {
                                    QuestionText = "Tôi thấy khó giữ bình tĩnh.",
                                    Type = "radio",
                                    Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                                },
                                new Question
                                {
                                    QuestionText = "Tôi cảm thấy dễ bị khó chịu hoặc tức giận.",
                                    Type = "radio",
                                    Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                                },
                                new Question
                                {
                                    QuestionText = "Tôi cảm thấy căng thẳng hoặc áp lực.",
                                    Type = "radio",
                                    Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                                },
                                new Question
                                {
                                    QuestionText = "Tôi cảm thấy khó chịu về những chuyện vụn vặt.",
                                    Type = "radio",
                                    Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                                },
                                new Question
                                {
                                    QuestionText = "Tôi thấy khó tập trung vào bất cứ việc gì.",
                                    Type = "radio",
                                    Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                                },
                                new Question
                                {
                                    QuestionText = "Tôi thấy quá tải và không thể giải quyết được các công việc hàng ngày.",
                                    Type = "radio",
                                    Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                                },
                                new Question
                                {
                                    QuestionText = "Tôi cảm thấy rất bực bội.",
                                    Type = "radio",
                                    Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                                }
                            }
                        }
                    }
                };

                // Lưu dữ liệu vào MongoDB
                await _mongoRepository.Create(dass21);

                // Thiết lập phản hồi thành công
                response.Data = true;
                response.Message = "DASS-21 dữ liệu đã được lưu trữ thành công.";
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về thông báo thất bại
                response.Data = false;
                response.Message = $"Có lỗi xảy ra khi lưu trữ DASS-21 dữ liệu: {ex.Message}";
            }

            return response;
        }
    }
}
