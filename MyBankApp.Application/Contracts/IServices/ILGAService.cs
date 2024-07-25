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
    public interface ILGAService
    {

        Task<IEnumerable<LGAResponseDto>> GetLGAsByStateIdAsync(int id);

    }
}
