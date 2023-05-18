
using HospitalApp.Models;

namespace HospitalApp.Services
{
    public interface IMailService
    {
        bool sendEmail(MailContent messagebody);
    }
}
