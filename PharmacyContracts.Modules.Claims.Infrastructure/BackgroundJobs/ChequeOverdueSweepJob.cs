using Microsoft.Extensions.Logging;
using PharmacyContracts.Modules.Claims.Application.Interfaces;
using PharmacyContracts.Modules.Claims.Domain.Enums;

namespace PharmacyContracts.Modules.Claims.Infrastructure.BackgroundJobs
{
    public class ChequeOverdueSweepJob
    {
        private readonly IChequeRepository _chequeRepository;
        private readonly ILogger<ChequeOverdueSweepJob> _logger;

        public ChequeOverdueSweepJob(IChequeRepository chequeRepository, ILogger<ChequeOverdueSweepJob> logger)
        {
            _chequeRepository = chequeRepository;
            _logger = logger;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var today = DateTime.UtcNow.Date;
            var overdueCandidates = await _chequeRepository.GetOverdueCandidatesAsync(today, cancellationToken);

            if (overdueCandidates.Count == 0)
                return;

            foreach (var cheque in overdueCandidates)
                cheque.Status = ChequeStatus.Overdue;

            _chequeRepository.UpdateRange(overdueCandidates);
            await _chequeRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Marked {Count} cheques as overdue.", overdueCandidates.Count);
        }
    }
}