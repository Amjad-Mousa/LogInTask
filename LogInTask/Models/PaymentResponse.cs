namespace LogInTask.Models
{
    public class PaymentResponse
    {
        bool IsSuccessful { get; set; } 
        public string? MeterNo { get; set; }    
        public string? CustomerName { get; set; }
        public string? AccountNo { get; set; }  
        public string? Token { get; set; }
        public string? ReferenceNo { get; set; }
        public string? AccountUsed { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal UnitsAdded { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.Now;

    }

}
