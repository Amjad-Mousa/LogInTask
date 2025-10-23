using LogInTask.Models;

namespace LogInTask.Services
{
    public class MeterQueryRequestValidator
    {
        private readonly MeterQueryRequest? _mterQueryRequest;

        public MeterQueryRequestValidator(MeterQueryRequest? mterQueryRequest)
        {
            _mterQueryRequest = mterQueryRequest;
        }

        public void MeterNoValidator()
        {
            if (_mterQueryRequest == null || string.IsNullOrEmpty(_mterQueryRequest.MeterNo))
            {
                throw new ArgumentException("Meter number is required.");
            }

            if (_mterQueryRequest.MeterNo.Length != 11 && _mterQueryRequest.MeterNo.Length != 13)
            {
                throw new ArgumentException("Meter number must be exactly 11 or 13 digits.");
            }

            if (_mterQueryRequest.Amount < 20 || _mterQueryRequest.Amount > 500)
            {
                throw new ArgumentException("Amount must be between 20 and 500.");
            }
        }
    }
}
