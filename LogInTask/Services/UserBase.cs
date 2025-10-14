using Microsoft.AspNetCore.Components;
using LogInTask.Services;

namespace LogInTask.Models
{
    public class UserBase : ComponentBase
    {
        [Inject]
        protected NavigationManager Navigation { get; set; } = default!;

        [Inject]    
        protected IAuthService AuthService { get; set; } = default!;

        protected LoginModel loginModel = new();
        protected OtpModel otpModel = new();
        protected bool showOtpForm = false;
        protected bool isLoading = false;
        protected string errorMessage = string.Empty;
        protected string successMessage = string.Empty;
        protected User? currentUser;

        protected async Task HandleLogin()
        {
            errorMessage = string.Empty;
            successMessage = string.Empty;
            isLoading = true;

            await Task.Delay(500);

            var (success, message) = await AuthService.LoginAsync(loginModel.Username, loginModel.Password);

            if (!success)
            {
                if (message == "OTP required")
                {
                    currentUser = AuthService.GetUserByUsername(loginModel.Username);
                    showOtpForm = true;
                    successMessage = $"Credentials verified! OTP has been sent. (Use code: {currentUser?.StaticOtp})";
                }
                else
                {
                    errorMessage = message;
                }
            }
            else
            {
                currentUser = AuthService.GetUserByUsername(loginModel.Username);
                successMessage = $"Welcome back, {currentUser?.Username}! Login successful.";
                await Task.Delay(1500);
                Navigation.NavigateTo("/home");
            }

            isLoading = false;
        }

        protected async Task HandleOtpVerification()
        {
            errorMessage = string.Empty;
            successMessage = string.Empty;
            isLoading = true;

            await Task.Delay(500);

            var (success, message) = await AuthService.LoginAsync(loginModel.Username, loginModel.Password, otpModel.OtpCode);

            if (success)
            {
                currentUser = AuthService.GetUserByUsername(loginModel.Username);
                successMessage = $"OTP verified! Welcome back, {currentUser.Username}!";
                await Task.Delay(1500);
                Navigation.NavigateTo("/home");
            }
            else
            {
                errorMessage = message ?? "Invalid OTP code. Please try again.";
            }

            isLoading = false;
        }

        protected void BackToLogin()
        {
            showOtpForm = false;
            errorMessage = string.Empty;
            successMessage = string.Empty;
            otpModel = new();
        }
    }
}