namespace LogInTask.Models
{
    public class MeterQueryResponse
    {
        public string? AccountNo { get; set; }  
        public string? AccountUsed { get; set; }
        public string? MeterNo { get; set; }        

        public string? QueryReference { get; set; } 

        public decimal Adjustments { get; set; }
        public decimal ChargeAmount { get; set; }   

        public bool IsSuccessful { get; set; }

        DateTime TimeStamp { get; set; } = DateTime.Now;

        public List<AdjustmentDetail> adjustmentDetails { get; set; } = new List<AdjustmentDetail>();   


    }
}
