using LogInTask.Models;

namespace LogInTask.Services
{
    public interface IElectricMeterService
    {
       public MeterQueryResponse QueryMeterAsync(MeterQueryRequest request);
       public MeterQueryResponse ProcessPaymentAsync(MeterQueryRequest request);
    }
}
