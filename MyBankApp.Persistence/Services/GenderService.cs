using Microsoft.Extensions.Logging;
using MyBankApp.Application.Configuration;
using MyBankApp.Application.Contracts.IServices;
using MyBankApp.Application.Contracts.Persistence;
using MyBankApp.Domain.Dto.RequestDto;
using MyBankApp.Domain.Dto.ResponseDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyBankApp.Persistence.Services
{
    public class GenderService : IGenderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GenderService> _logger;

        public GenderService(IUnitOfWork unitOfWork, ILogger<GenderService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }


        public async Task<IEnumerable<GenderResponseDto>> GetAllPostsAsync()
        {
            try
            {
                if (_unitOfWork != null)
                {
                    var genders = await _unitOfWork.gender.GetAllAsync();

                    return genders.Select(g => new GenderResponseDto
                    {
                        Id = g.Id,
                        Description = g.Description
                    });
                }
                else
                {
                    _logger.LogError("Unit of work is null");
                    return Enumerable.Empty<GenderResponseDto>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all genders");
                return Enumerable.Empty<GenderResponseDto>();
            }
        }

    }
}
