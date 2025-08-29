
// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "@openzeppelin/contracts/access/AccessControl.sol";
import "./Registry.sol";

/**
 * @title TournamentFactory
 * @dev Factory contract for creating tournament instances
 */
contract TournamentFactory is AccessControl {
    // Tournament parameters structure
    struct TournamentParams {
        address buyInToken;      // ERC-20 token address for buy-in (address(0) for native token)
        uint256 buyInAmount;     // Buy-in amount in wei
        uint256 tableSize;       // Number of players per table
        uint256 advanceCount;    // Number of players advancing from each table
        uint256 rakeBps;         // Rake in basis points (100 = 1%)
        uint256[] prizeSplits;   // Prize distribution in basis points
        uint256 startTime;       // Tournament start timestamp
        uint256 duration;        // Tournament duration in seconds
    }

    // Tournament instance
    struct Tournament {
        uint256 id;
        TournamentParams params;
        address creator;
        uint256 createdAt;
        bool isActive;
    }

    // Registry contract
    Registry public registry;

    // Tournament counter and mapping
    uint256 public tournamentCounter;
    mapping(uint256 => Tournament) public tournaments;

    // Events
    event TournamentCreated(
        uint256 indexed tournamentId,
        address indexed creator,
        TournamentParams params
    );
    event TournamentStatusChanged(uint256 indexed tournamentId, bool isActive);

    /**
     * @dev Constructor
     * @param registry_ Address of the Registry contract
     */
    constructor(address registry_) {
        registry = Registry(registry_);
        _grantRole(DEFAULT_ADMIN_ROLE, msg.sender);
    }

    /**
     * @dev Create a new tournament
     * @param params Tournament parameters
     * @return tournamentId ID of the created tournament
     */
    function createTournament(
        TournamentParams calldata params
    ) external onlyRole(registry.TOURNAMENT_ADMIN_ROLE()) returns (uint256) {
        require(params.tableSize >= 2 && params.tableSize <= 16, "Invalid table size");
        require(params.advanceCount >= 1 && params.advanceCount <= params.tableSize, "Invalid advance count");
        require(params.rakeBps <= 1000, "Rake too high"); // Max 10% rake
        require(params.prizeSplits.length > 0, "Prize splits required");
        require(_isValidPrizeSplits(params.prizeSplits), "Invalid prize splits");

        tournamentCounter++;
        uint256 tournamentId = tournamentCounter;

        tournaments[tournamentId] = Tournament({
            id: tournamentId,
            params: params,
            creator: msg.sender,
            createdAt: block.timestamp,
            isActive: true
        });

        emit TournamentCreated(tournamentId, msg.sender, params);
        return tournamentId;
    }

    /**
     * @dev Set tournament active status
     * @param tournamentId ID of the tournament
     * @param isActive Whether the tournament is active
     */
    function setTournamentStatus(
        uint256 tournamentId,
        bool isActive
    ) external onlyRole(registry.TOURNAMENT_ADMIN_ROLE()) {
        require(tournamentId > 0 && tournamentId <= tournamentCounter, "Invalid tournament ID");
        tournaments[tournamentId].isActive = isActive;
        emit TournamentStatusChanged(tournamentId, isActive);
    }

    /**
     * @dev Get tournament parameters
     * @param tournamentId ID of the tournament
     * @return params Tournament parameters
     */
    function getTournamentParams(uint256 tournamentId) external view returns (TournamentParams memory) {
        require(tournamentId > 0 && tournamentId <= tournamentCounter, "Invalid tournament ID");
        return tournaments[tournamentId].params;
    }

    /**
     * @dev Check if prize splits are valid (sum to 10000 bps = 100%)
     * @param prizeSplits Array of prize splits in basis points
     * @return bool Whether prize splits are valid
     */
    function _isValidPrizeSplits(uint256[] memory prizeSplits) internal pure returns (bool) {
        uint256 total;
        for (uint256 i = 0; i < prizeSplits.length; i++) {
            total += prizeSplits[i];
        }
        return total == 10000; // 100% in basis points
    }

    /**
     * @dev Get total number of tournaments
     * @return count Total tournament count
     */
    function getTournamentCount() external view returns (uint256) {
        return tournamentCounter;
    }

    /**
     * @dev Check if tournament exists and is active
     * @param tournamentId ID of the tournament
     * @return bool Whether tournament exists and is active
     */
    function isTournamentActive(uint256 tournamentId) external view returns (bool) {
        return tournamentId > 0 && tournamentId <= tournamentCounter && tournaments[tournamentId].isActive;
    }
}
