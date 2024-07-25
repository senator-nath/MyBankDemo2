using Microsoft.Extensions.Logging;
using MyBankApp.Application.Configuration;
using MyBankApp.Application.Contracts.IServices;
using MyBankApp.Application.Contracts.Persistence;
using MyBankApp.Domain.Dto.RequestDto;
using MyBankApp.Domain.Dto.ResponseDto;
using MyBankApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBankApp.Persistence.Services
{
    public class LGAService : ILGAService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<LGAService> _logger;

        public LGAService(IUnitOfWork unitOfWork, ILogger<LGAService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<LGAResponseDto>> GetLGAsByStateIdAsync(int id)
        {
            List<LGAResponseDto> listResponse = new();
            var result = await _unitOfWork.lgaRepository.GetAllByColumnAsync(x => x.StateId == id);

            if (result == null)
            {
                return listResponse;
            }

            foreach (var item in result)
            {

                var response = new LGAResponseDto
                {

                    Id = item.Id,
                    Name = item.Name,
                };

                listResponse.Add(response);

            }

            return listResponse;
        }
    }
}
