using Application.Commons;
using Domain.Entities.MBTI21;
using MediatR;
using NUlid;

namespace Application.Commands.CreateMbti21Quiz
{
    public class CreateMBTI21CommandHandler(IMongoRepository<Mbti21> mongoRepository)
        : IRequestHandler<CreateMBTI21Command, BaseResponse<bool>>
    {
        public async Task<BaseResponse<bool>> Handle(CreateMBTI21Command request, CancellationToken cancellationToken)
        {
            try
            {
                var mbti21 = new Mbti21
                {
                    Id = Ulid.NewUlid().ToString(),
                    Categories = new List<MBTICategory>
                {
                    new MBTICategory
                    {
                        CategoryName = "Hướng Ngoại (E) / Hướng Nội (I)",
                        Questions = new List<MBTIQuestion>
                        {
                            new MBTIQuestion
                            {
                                QuestionText = "Tôi cảm thấy tràn đầy năng lượng khi ở cùng mọi người.",
                                Type = "radio",
                                Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                            },
                            new MBTIQuestion
                            {
                                QuestionText = "Tôi thích giao tiếp và trò chuyện với nhiều người.",
                                Type = "radio",
                                Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                            },
                            new MBTIQuestion
                            {
                                QuestionText = "Tôi cảm thấy mệt mỏi khi phải dành quá nhiều thời gian một mình.",
                                Type = "radio",
                                Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                            },
                            new MBTIQuestion
                            {
                                QuestionText = "Tôi thích tham gia các buổi tiệc hoặc sự kiện đông người.",
                                Type = "radio",
                                Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                            },
                            new MBTIQuestion
                            {
                                QuestionText = "Tôi cảm thấy mình rất dễ dàng bắt đầu cuộc trò chuyện với người lạ.",
                                Type = "radio",
                                Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                            },
                        }
                    },
                    new MBTICategory
                    {
                        CategoryName = "Giác Quan (S) / Trực Giác (N)",
                        Questions = new List<MBTIQuestion>
                        {
                            new MBTIQuestion
                            {
                                QuestionText = "Tôi tập trung vào các chi tiết hơn là bức tranh tổng thể.",
                                Type = "radio",
                                Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                            },
                            new MBTIQuestion
                            {
                                QuestionText = "Tôi dựa vào cảm nhận và trực giác để ra quyết định.",
                                Type = "radio",
                                Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                            },
                            new MBTIQuestion
                            {
                                QuestionText = "Tôi thích tưởng tượng về những ý tưởng sáng tạo.",
                                Type = "radio",
                                Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                            },
                            new MBTIQuestion
                            {
                                QuestionText = "Tôi thích làm việc với các số liệu và dữ liệu cụ thể.",
                                Type = "radio",
                                Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                            },
                            new MBTIQuestion
                            {
                                QuestionText = "Tôi quan tâm đến các khả năng tương lai hơn là thực tại.",
                                Type = "radio",
                                Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                            },
                        }
                    },
                    new MBTICategory
                    {
                        CategoryName = "Lý Trí (T) / Cảm Xúc (F)",
                        Questions = new List<MBTIQuestion>
                        {
                            new MBTIQuestion
                            {
                                QuestionText = "Tôi luôn cân nhắc các yếu tố logic và dữ liệu trước khi đưa ra quyết định.",
                                Type = "radio",
                                Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                            },
                            new MBTIQuestion
                            {
                                QuestionText = "Tôi ưu tiên cảm xúc và sự thấu cảm khi giải quyết các vấn đề.",
                                Type = "radio",
                                Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                            },
                            new MBTIQuestion
                            {
                                QuestionText = "Tôi thích phân tích vấn đề một cách chi tiết và logic.",
                                Type = "radio",
                                Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                            },
                            new MBTIQuestion
                            {
                                QuestionText = "Tôi bị ảnh hưởng nhiều bởi cảm xúc khi đưa ra quyết định.",
                                Type = "radio",
                                Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                            },
                            new MBTIQuestion
                            {
                                QuestionText = "Tôi luôn cố gắng giải quyết vấn đề một cách công bằng và không cảm xúc.",
                                Type = "radio",
                                Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                            }
                        }
                    },
                    new MBTICategory
                    {
                        CategoryName = "Nguyên Tắc (J) / Linh Hoạt (P)",
                        Questions = new List<MBTIQuestion>
                        {
                            new MBTIQuestion
                            {
                                QuestionText = "Tôi luôn có kế hoạch rõ ràng trước khi bắt đầu bất cứ việc gì.",
                                Type = "radio",
                                Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                            },
                            new MBTIQuestion
                            {
                                QuestionText = "Tôi thích sự tự do và linh hoạt thay vì tuân theo kế hoạch chặt chẽ.",
                                Type = "radio",
                                Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                            },
                            new MBTIQuestion
                            {
                                QuestionText = "Tôi thường cảm thấy hài lòng hơn khi mọi thứ đi đúng kế hoạch.",
                                Type = "radio",
                                Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                            },
                            new MBTIQuestion
                            {
                                QuestionText = "Tôi dễ dàng thích nghi khi kế hoạch bị thay đổi đột ngột.",
                                Type = "radio",
                                Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                            },
                            new MBTIQuestion
                            {
                                QuestionText = "Tôi cảm thấy thoải mái khi làm việc với môi trường có cấu trúc và trật tự.",
                                Type = "radio",
                                Options = new List<string> { "Không bao giờ", "Ít khi", "Thỉnh thoảng", "Thường xuyên" }
                            }
                        }
                    }
                }
                };
                await mongoRepository.Create(mbti21);
                return BaseResponse<bool>.SuccessReturn(true);
            }
            catch (Exception ex)
            {
                return BaseResponse<bool>.InternalServerError(ex.Message);
            }
        }
    }
}
