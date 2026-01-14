using Corno.Data.Admin;

namespace Corno.Education.Areas.Services.Interfaces;

public interface IOtpService 
{
    string SendLoginOtp(LoginViewModel loginViewModel);
    bool ValidateOtp(LoginViewModel loginViewModel);
}