using MyBankApp.Domain.Dto.RequestDto;
using MyBankApp.Domain.Dto.ResponseDto;
using MyBankApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBankApp.Application.Contracts.IServices
{
    public interface IUserService
    {


        Task<UserResponseDetails> Register(UserRequestDto entity);
        Task<UserResponseDetails> Login(LoginRequestDto entity);
        Task<UserResponseDetails> ChangePassword(ChangePassWordRequestDto entity);

        Task<string> EmailConfirmation(EmailConfirmationRequestDto request);
        Task<UserResponseDetails> ResetPasswordRequest(ResetPasswordRequestDto entity);
        Task<UserResponseDetails> ResetPasswordWithToken(ResetPasswordRequestTokenDto entity);
    }
}
