using System.ComponentModel.DataAnnotations;

namespace Application.Commons.DTOs
{
    public class MBTIQuizzResultRequest
    {
        public int[] Extroversion { get; set; } = new int[5];
        public int[] Sensing { get; set; } = new int[5];
        public int[] Thinking { get; set; } = new int[5];
        public int[] Judging { get; set; } = new int[5];
    }
}
