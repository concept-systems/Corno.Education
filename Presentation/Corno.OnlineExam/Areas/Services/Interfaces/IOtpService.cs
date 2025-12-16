using Corno.Data.Admin;

namespace Corno.OnlineExam.Areas.Services.Interfaces;

public interface IOtpService 
{
    string SendLoginOtp(LoginViewModel loginViewModel);
    bool ValidateOtp(LoginViewModel loginViewModel);
}