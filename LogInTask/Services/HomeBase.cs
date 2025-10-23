using LogInTask.Models;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LogInTask.Services
{
    public class HomeBase : ComponentBase
    {
        [Inject] protected IElectricMeterService ElectricMeterService { get; set; } = default!;
        [Inject] protected IValidator<MeterQueryRequest> Validator { get; set; } = default!;
        [Inject] protected IJSRuntime JS { get; set; } = default!;

        protected MeterQueryRequest Request { get; set; } = new();
        protected MeterQueryResponse? LastQueryResults;
        protected PaymentResponse? LastPaymentResult;
        protected string? FormError;
        protected string? MeterNoError;
        protected string? AmountError;
        protected bool IsLoading;
        protected bool PaymentLoading;
        protected List<MeterQueryRequest> RecentQueries { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadRecentFromLocalStorage();
        }

        protected async Task OnSubmitClicked() => await OnSubmit();

        protected async Task OnSubmit()
        {
            LastPaymentResult = null;
            ClearErrors();
            var validation = await Validator.ValidateAsync(Request);
            if (!validation.IsValid)
            {
                foreach (var e in validation.Errors)
                {
                    if (e.PropertyName == nameof(Request.MeterNo))
                        MeterNoError = e.ErrorMessage;
                    else if (e.PropertyName == nameof(Request.Amount))
                        AmountError = e.ErrorMessage;
                    else
                        FormError = "يوجد خطأ في النموذج، يرجى التحقق من البيانات.";
                }
                return;
            }

            IsLoading = true;
            StateHasChanged();

            try
            {
                LastQueryResults = await ElectricMeterService.QueryMeterAsync(Request);
                if (LastQueryResults?.Success == true)
                    await AddToRecent(Request);
                else
                    FormError = "فشل الاستعلام، يرجى التحقق من رقم العداد.";
            }
            catch
            {
                FormError = "حدث خطأ أثناء الاستعلام، يرجى المحاولة لاحقًا.";
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected async Task OnProcessPaymentClicked()
        {
            if (LastQueryResults == null)
                return;

            bool confirm = await JS.InvokeAsync<bool>("confirm", $"هل تريد تأكيد دفع {Request.Amount} للعداد {Request.MeterNo}؟");
            if (!confirm)
                return;

            PaymentLoading = true;
            StateHasChanged();

            try
            {
                LastPaymentResult = await ElectricMeterService.ProcessPaymentAsync(Request);
                if (LastPaymentResult?.Success == true)
                    await ResetFormAfterPayment();
                else
                    FormError = "فشل الدفع، يرجى المحاولة مرة أخرى.";
            }
            catch
            {
                FormError = "حدث خطأ أثناء عملية الدفع، يرجى المحاولة لاحقًا.";
            }
            finally
            {
                PaymentLoading = false;
            }
        }

        protected void ClearErrors()
        {
            FormError = MeterNoError = AmountError = null;
        }

        protected async Task ResetFormAfterPayment()
        {
            var paymentResult = LastPaymentResult;
            Request = new();
            LastQueryResults = null;
            LastPaymentResult = paymentResult;
            await InvokeAsync(StateHasChanged);
        }

        protected void ResetForm()
        {
            Request = new();
            LastQueryResults = null;
            LastPaymentResult = null;
            ClearErrors();
        }

        protected void OnMeterInput(ChangeEventArgs e)
        {
            var raw = e.Value?.ToString() ?? string.Empty;
            var digits = new string(raw.Where(char.IsDigit).ToArray());
            if (digits.Length > 13)
                digits = digits.Substring(0, 13);
            Request.MeterNo = digits;
        }

        protected async Task AddToRecent(MeterQueryRequest req)
        {
            var copy = new MeterQueryRequest { MeterNo = req.MeterNo, Amount = req.Amount };
            RecentQueries.RemoveAll(r => r.MeterNo == copy.MeterNo && r.Amount == copy.Amount);
            RecentQueries.Insert(0, copy);
            if (RecentQueries.Count > 5)
                RecentQueries.RemoveAt(5);
            try
            {
                await JS.InvokeVoidAsync("localStorage.setItem", "recent_meter_queries",
                    System.Text.Json.JsonSerializer.Serialize(RecentQueries));
            }
            catch { }
        }

        protected async Task LoadRecentFromLocalStorage()
        {
            try
            {
                var json = await JS.InvokeAsync<string?>("localStorage.getItem", "recent_meter_queries");
                if (!string.IsNullOrEmpty(json))
                    RecentQueries = System.Text.Json.JsonSerializer.Deserialize<List<MeterQueryRequest>>(json) ?? new();
            }
            catch
            {
                RecentQueries = new();
            }
        }

        protected void LoadRecent(MeterQueryRequest q)
        {
            Request = new() { MeterNo = q.MeterNo, Amount = q.Amount };
            LastQueryResults = null;
            LastPaymentResult = null;
        }

        protected void ClosePaymentResult()
        {
            LastPaymentResult = null;
            StateHasChanged();
        }
    }
}