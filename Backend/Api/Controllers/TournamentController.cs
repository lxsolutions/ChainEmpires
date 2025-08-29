

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChainEmpires.Api.Models;
using ChainEmpires.Api.Models.Entities;
using ChainEmpires.Api.Models.DTOs;

namespace ChainEmpires.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TournamentController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TournamentController> _logger;

        public TournamentController(AppDbContext context, ILogger<TournamentController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<TournamentResponse>> CreateTournament([FromBody] CreateTournamentRequest request)
        {
            try
            {
                var tournament = new Tournament
                {
                    Name = request.Name,
                    BuyInAmount = request.BuyInAmount,
                    BuyInToken = request.BuyInToken,
                    TableSize = request.TableSize,
                    AdvanceCount = request.AdvanceCount,
                    RakeBps = request.RakeBps,
                    StartTime = request.StartTime,
                    Status = TournamentStatus.Draft
                };

                _context.Tournaments.Add(tournament);
                await _context.SaveChangesAsync();

                return Ok(MapToResponse(tournament));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tournament");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TournamentResponse>> GetTournament(Guid id)
        {
            var tournament = await _context.Tournaments
                .Include(t => t.Entries)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tournament == null)
            {
                return NotFound();
            }

            return Ok(MapToResponse(tournament));
        }

        [HttpGet]
        public async Task<ActionResult<List<TournamentResponse>>> GetTournaments()
        {
            var tournaments = await _context.Tournaments
                .Include(t => t.Entries)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return Ok(tournaments.Select(MapToResponse).ToList());
        }

        [HttpPost("{id}/buyin")]
        public async Task<ActionResult<BuyInResponse>> BuyIn(Guid id, [FromBody] BuyInRequest request)
        {
            try
            {
                var tournament = await _context.Tournaments.FindAsync(id);
                if (tournament == null || tournament.Status != TournamentStatus.Registration)
                {
                    return BadRequest("Tournament not available for registration");
                }

                // Check if player already registered
                var existingEntry = await _context.Entries
                    .FirstOrDefaultAsync(e => e.TournamentId == id && 
                                           e.PlayerAddress == request.PlayerAddress);
                
                if (existingEntry != null)
                {
                    return BadRequest("Player already registered for this tournament");
                }

                var entry = new Entry
                {
                    TournamentId = id,
                    PlayerAddress = request.PlayerAddress,
                    BaseNFTTokenId = request.BaseNFTTokenId,
                    RegisteredAt = DateTime.UtcNow
                };

                _context.Entries.Add(entry);
                await _context.SaveChangesAsync();

                // TODO: Integrate with real payment system
                var response = new BuyInResponse
                {
                    EntryId = entry.Id,
                    PaymentAddress = "0xTournamentEscrowAddress", // Placeholder
                    Amount = tournament.BuyInAmount,
                    Token = tournament.BuyInToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(15)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing buy-in");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/tables")]
        public async Task<ActionResult<TableAssignmentResponse>> GetPlayerTable(Guid id, [FromQuery] string playerAddress)
        {
            try
            {
                var entry = await _context.Entries
                    .Include(e => e.Table)
                    .ThenInclude(t => t!.Entries)
                    .FirstOrDefaultAsync(e => e.TournamentId == id && 
                                           e.PlayerAddress == playerAddress && 
                                           e.IsActive);

                if (entry == null || entry.TableId == null)
                {
                    return NotFound("No active table assignment found");
                }

                var table = entry.Table!;
                var opponents = table.Entries
                    .Where(e => e.Id != entry.Id)
                    .Select(e => new TableOpponent
                    {
                        BaseNFTTokenId = e.BaseNFTTokenId,
                        MMR = e.MMR,
                        CurrentRound = e.CurrentRound
                    })
                    .ToList();

                var response = new TableAssignmentResponse
                {
                    TableId = table.Id,
                    RoundNumber = table.RoundNumber,
                    Status = table.Status.ToString(),
                    Opponents = opponents,
                    StartTime = table.CreatedAt
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting table assignment");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("tables/{id}/result")]
        public async Task<ActionResult> SubmitResult(Guid id, [FromBody] SubmitResultRequest request)
        {
            try
            {
                var table = await _context.Tables
                    .Include(t => t.Entries)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (table == null || table.Status != TableStatus.InProgress)
                {
                    return BadRequest("Table not available for result submission");
                }

                // TODO: Validate result and determine winner
                // For now, we'll assume the first entry is the winner (placeholder)
                var winnerEntry = table.Entries.First();

                var result = new Result
                {
                    TableId = id,
                    WinnerEntryId = winnerEntry.Id,
                    LogHash = request.LogHash,
                    MerkleRoot = request.MerkleRoot,
                    RecordedAt = DateTime.UtcNow
                };

                table.Status = TableStatus.Completed;
                table.WinnerEntryId = winnerEntry.Id;
                table.UpdatedAt = DateTime.UtcNow;

                _context.Results.Add(result);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting result");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{id}/advance")]
        public async Task<ActionResult> AdvanceTournament(Guid id)
        {
            try
            {
                var tournament = await _context.Tournaments
                    .Include(t => t.Tables)
                    .ThenInclude(t => t.Entries)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (tournament == null)
                {
                    return NotFound();
                }

                // TODO: Implement proper tournament advancement logic
                // For now, this is a placeholder
                _logger.LogInformation("Tournament advancement triggered for {TournamentId}", id);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error advancing tournament");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{id}/payouts")]
        public async Task<ActionResult<List<PayoutResponse>>> CalculatePayouts(Guid id)
        {
            try
            {
                var tournament = await _context.Tournaments
                    .Include(t => t.Entries)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (tournament == null)
                {
                    return NotFound();
                }

                // TODO: Implement proper payout calculation
                // For now, return empty list
                return Ok(new List<PayoutResponse>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating payouts");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("health")]
        public ActionResult HealthCheck()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }

        private TournamentResponse MapToResponse(Tournament tournament)
        {
            return new TournamentResponse
            {
                Id = tournament.Id,
                Name = tournament.Name,
                BuyInAmount = tournament.BuyInAmount,
                BuyInToken = tournament.BuyInToken,
                TableSize = tournament.TableSize,
                AdvanceCount = tournament.AdvanceCount,
                RakeBps = tournament.RakeBps,
                Status = tournament.Status.ToString(),
                StartTime = tournament.StartTime,
                EndTime = tournament.EndTime,
                EntryCount = tournament.Entries.Count,
                TotalPrizePool = tournament.TotalPrizePool,
                CreatedAt = tournament.CreatedAt
            };
        }
    }
}

