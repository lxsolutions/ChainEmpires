

// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "@openzeppelin/contracts/access/AccessControl.sol";
import "@openzeppelin/contracts/token/ERC20/IERC20.sol";
import "@openzeppelin/contracts/token/ERC20/utils/SafeERC20.sol";
import "@openzeppelin/contracts/utils/ReentrancyGuard.sol";
import "./Registry.sol";
import "./BaseNFT.sol";

/**
 * @title TournamentEscrow
 * @dev Escrow contract for handling tournament buy-ins, results, and payouts
 */
contract TournamentEscrow is AccessControl, ReentrancyGuard {
    using SafeERC20 for IERC20;

    // Registry contract
    Registry public registry;

    // Tournament entry structure
    struct Entry {
        uint256 tournamentId;
        uint256 baseTokenId;
        address player;
        uint256 buyInAmount;
        address buyInToken;
        bool isActive;
        uint256 joinTime;
    }

    // Table result structure
    struct TableResult {
        uint256 tableId;
        uint256 winnerTokenId;
        bytes32 commitHash;
        bool isRecorded;
        uint256 recordedAt;
    }

    // Payout structure
    struct Payout {
        uint256 tournamentId;
        uint256 entryId;
        uint256 amount;
        address token;
        bool distributed;
        bytes32 transactionHash;
    }

    // Mappings
    mapping(uint256 => Entry) public entries;
    mapping(uint256 => TableResult) public tableResults;
    mapping(uint256 => Payout) public payouts;
    mapping(uint256 => uint256[]) public tournamentEntries;
    mapping(uint256 => uint256) public tournamentPrizePools;

    // Counters
    uint256 public entryCounter;
    uint256 public payoutCounter;

    // Events
    event EntryCreated(
        uint256 indexed entryId,
        uint256 indexed tournamentId,
        uint256 indexed baseTokenId,
        address player,
        uint256 amount,
        address token
    );
    event ResultRecorded(
        uint256 indexed tableId,
        uint256 indexed winnerTokenId,
        bytes32 commitHash,
        address indexed relayer
    );
    event PayoutDistributed(
        uint256 indexed payoutId,
        uint256 indexed entryId,
        address indexed player,
        uint256 amount,
        address token,
        bytes32 transactionHash
    );
    event FundsDeposited(
        uint256 indexed tournamentId,
        uint256 amount,
        address token,
        address indexed from
    );
    event FundsWithdrawn(
        uint256 indexed tournamentId,
        uint256 amount,
        address token,
        address indexed to
    );

    /**
     * @dev Constructor
     * @param registry_ Address of the Registry contract
     */
    constructor(address registry_) {
        registry = Registry(registry_);
        _grantRole(DEFAULT_ADMIN_ROLE, msg.sender);
    }

    /**
     * @dev Receive function to accept native currency
     */
    receive() external payable {}

    /**
     * @dev Create tournament entry (buy-in)
     * @param tournamentId ID of the tournament
     * @param baseTokenId Base NFT token ID
     * @param player Address of the player
     * @param amount Buy-in amount
     * @param token Buy-in token address (address(0) for native)
     */
    function createEntry(
        uint256 tournamentId,
        uint256 baseTokenId,
        address player,
        uint256 amount,
        address token
    ) external payable nonReentrant {
        require(amount > 0, "Invalid amount");

        // Handle payment
        if (token == address(0)) {
            require(msg.value == amount, "Incorrect native amount");
        } else {
            require(msg.value == 0, "Native token not expected");
            IERC20(token).safeTransferFrom(msg.sender, address(this), amount);
        }

        entryCounter++;
        uint256 entryId = entryCounter;

        entries[entryId] = Entry({
            tournamentId: tournamentId,
            baseTokenId: baseTokenId,
            player: player,
            buyInAmount: amount,
            buyInToken: token,
            isActive: true,
            joinTime: block.timestamp
        });

        tournamentEntries[tournamentId].push(entryId);
        tournamentPrizePools[tournamentId] += amount;

        emit EntryCreated(entryId, tournamentId, baseTokenId, player, amount, token);
        emit FundsDeposited(tournamentId, amount, token, player);
    }

    /**
     * @dev Record table result
     * @param tableId ID of the table
     * @param winnerTokenId Winning Base NFT token ID
     * @param commitHash Hash commitment of the battle log
     */
    function recordResult(
        uint256 tableId,
        uint256 winnerTokenId,
        bytes32 commitHash
    ) external onlyRole(registry.RESULT_RELAYER_ROLE()) {
        require(!tableResults[tableId].isRecorded, "Result already recorded");

        tableResults[tableId] = TableResult({
            tableId: tableId,
            winnerTokenId: winnerTokenId,
            commitHash: commitHash,
            isRecorded: true,
            recordedAt: block.timestamp
        });

        emit ResultRecorded(tableId, winnerTokenId, commitHash, msg.sender);
    }

    /**
     * @dev Distribute payout to winner
     * @param entryId ID of the winning entry
     * @param amount Payout amount
     * @param position Final position (for tracking)
     */
    function distributePayout(
        uint256 entryId,
        uint256 amount,
        uint256 position
    ) external onlyRole(registry.PAYOUT_MANAGER_ROLE()) nonReentrant returns (uint256) {
        Entry storage entry = entries[entryId];
        require(entry.isActive, "Entry not active");
        require(amount > 0, "Invalid payout amount");
        require(amount <= tournamentPrizePools[entry.tournamentId], "Insufficient prize pool");

        payoutCounter++;
        uint256 payoutId = payoutCounter;

        payouts[payoutId] = Payout({
            tournamentId: entry.tournamentId,
            entryId: entryId,
            amount: amount,
            token: entry.buyInToken,
            distributed: true,
            transactionHash: bytes32(0)
        });

        tournamentPrizePools[entry.tournamentId] -= amount;

        // Transfer funds
        if (entry.buyInToken == address(0)) {
            (bool success, ) = entry.player.call{value: amount}("");
            require(success, "Native transfer failed");
            payouts[payoutId].transactionHash = keccak256(abi.encodePacked(block.number, payoutId));
        } else {
            IERC20(entry.buyInToken).safeTransfer(entry.player, amount);
            payouts[payoutId].transactionHash = keccak256(abi.encodePacked(block.number, payoutId));
        }

        emit PayoutDistributed(
            payoutId,
            entryId,
            entry.player,
            amount,
            entry.buyInToken,
            payouts[payoutId].transactionHash
        );

        return payoutId;
    }

    /**
     * @dev Get tournament prize pool
     * @param tournamentId ID of the tournament
     * @return prizePool Total prize pool amount
     */
    function getTournamentPrizePool(uint256 tournamentId) external view returns (uint256) {
        return tournamentPrizePools[tournamentId];
    }

    /**
     * @dev Get tournament entries
     * @param tournamentId ID of the tournament
     * @return entryIds Array of entry IDs
     */
    function getTournamentEntries(uint256 tournamentId) external view returns (uint256[] memory) {
        return tournamentEntries[tournamentId];
    }

    /**
     * @dev Emergency withdrawal (admin only)
     * @param token Token address to withdraw (address(0) for native)
     * @param amount Amount to withdraw
     * @param to Recipient address
     */
    function emergencyWithdraw(
        address token,
        uint256 amount,
        address to
    ) external onlyRole(DEFAULT_ADMIN_ROLE) nonReentrant {
        if (token == address(0)) {
            (bool success, ) = to.call{value: amount}("");
            require(success, "Native transfer failed");
        } else {
            IERC20(token).safeTransfer(to, amount);
        }
    }
}

