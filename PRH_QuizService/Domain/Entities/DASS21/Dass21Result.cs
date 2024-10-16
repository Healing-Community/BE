using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.DASS21
{
    public class Dass21Result
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime DateTaken { get; set; }

        public int StressScore { get; set; }
        public int AnxietyScore { get; set; }
        public int DepressionScore { get; set; }

        public string? SressDescription { get; set; }
        public string? AnxietyDescription { get; set; }
        public string? DepressionDescription { get; set; }
        public string? OverallComment { get; set; }
    }
}
