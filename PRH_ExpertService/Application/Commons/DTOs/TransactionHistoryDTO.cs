namespace Application.Commons.DTOs
{
    public class TransactionHistoryDTO
    {
        public string ExpertName { get; set; }
        public string ExpertEmail { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}