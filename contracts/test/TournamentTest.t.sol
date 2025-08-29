


// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "forge-std/Test.sol";
import "../src/Registry.sol";
import "../src/BaseNFT.sol";
import "../src/TournamentFactory.sol";
import "../src/TournamentEscrow.sol";

contract TournamentTest is Test {
    Registry public registry;
    BaseNFT public baseNFT;
    TournamentFactory public factory;
    TournamentEscrow public escrow;

    address public admin = address(0x1);
    address public player1 = address(0x2);
    address public player2 = address(0x3);
    address public relayer = address(0x4);

    uint256 public constant BUY_IN_AMOUNT = 1 ether;
    address public constant NATIVE_TOKEN = address(0);

    function setUp() public {
        vm.startPrank(admin);

        // Deploy contracts
        registry = new Registry(admin);
        baseNFT = new BaseNFT("Test Base", "TBASE", "https://test.com/");
        factory = new TournamentFactory(address(registry));
        escrow = new TournamentEscrow(address(registry));

        // Set contract addresses in registry
        registry.setContractAddress(registry.BASE_NFT(), address(baseNFT));
        registry.setContractAddress(registry.TOURNAMENT_FACTORY(), address(factory));
        registry.setContractAddress(registry.TOURNAMENT_ESCROW(), address(escrow));

        // Grant roles
        address[] memory relayers = new address[](1);
        relayers[0] = relayer;
        registry.grantRoleBatch(registry.RESULT_RELAYER_ROLE(), relayers);
        registry.grantRoleBatch(registry.PAYOUT_MANAGER_ROLE(), relayers);
        
        // Grant admin role to admin address
        address[] memory admins = new address[](1);
        admins[0] = admin;
        registry.grantRoleBatch(registry.TOURNAMENT_ADMIN_ROLE(), admins);

        vm.stopPrank();
    }

    function test_Deployment() public {
        assertEq(registry.getContractAddress(registry.BASE_NFT()), address(baseNFT));
        assertEq(registry.getContractAddress(registry.TOURNAMENT_FACTORY()), address(factory));
        assertEq(registry.getContractAddress(registry.TOURNAMENT_ESCROW()), address(escrow));
        assertTrue(registry.hasRole(registry.RESULT_RELAYER_ROLE(), relayer));
    }

    function test_CreateTournament() public {
        vm.startPrank(admin);

        uint256[] memory prizeSplits = new uint256[](3);
        prizeSplits[0] = 5000; // 50%
        prizeSplits[1] = 3000; // 30%
        prizeSplits[2] = 2000; // 20%

        TournamentFactory.TournamentParams memory params = TournamentFactory.TournamentParams({
            buyInToken: NATIVE_TOKEN,
            buyInAmount: BUY_IN_AMOUNT,
            tableSize: 4,
            advanceCount: 2,
            rakeBps: 200, // 2%
            prizeSplits: prizeSplits,
            startTime: block.timestamp + 1 hours,
            duration: 6 hours
        });

        uint256 tournamentId = factory.createTournament(params);
        assertEq(tournamentId, 1);
        assertTrue(factory.isTournamentActive(tournamentId));

        vm.stopPrank();
    }

    function test_MintBaseNFT() public {
        vm.startPrank(admin);

        uint256 tokenId = baseNFT.mintTo(player1, 1, "https://test.com/1");
        assertEq(tokenId, 1);
        assertEq(baseNFT.ownerOf(tokenId), player1);
        assertEq(baseNFT.getTournamentStatus(tokenId), 1);

        vm.stopPrank();
    }

    function test_BuyInAndCreateEntry() public {
        // Setup tournament
        vm.startPrank(admin);
        uint256[] memory prizeSplits = new uint256[](2);
        prizeSplits[0] = 7000;
        prizeSplits[1] = 3000;

        TournamentFactory.TournamentParams memory params = TournamentFactory.TournamentParams({
            buyInToken: NATIVE_TOKEN,
            buyInAmount: BUY_IN_AMOUNT,
            tableSize: 2,
            advanceCount: 1,
            rakeBps: 100,
            prizeSplits: prizeSplits,
            startTime: block.timestamp + 1 hours,
            duration: 2 hours
        });

        uint256 tournamentId = factory.createTournament(params);
        uint256 tokenId = baseNFT.mintTo(player1, tournamentId, "https://test.com/1");
        vm.stopPrank();

        // Player buys in
        vm.deal(player1, BUY_IN_AMOUNT);
        vm.startPrank(player1);
        escrow.createEntry{value: BUY_IN_AMOUNT}(
            tournamentId,
            tokenId,
            player1,
            BUY_IN_AMOUNT,
            NATIVE_TOKEN
        );

        assertEq(escrow.getTournamentPrizePool(tournamentId), BUY_IN_AMOUNT);
        vm.stopPrank();
    }

    function test_RecordResult() public {
        // Setup tournament and entries
        vm.startPrank(admin);
        uint256[] memory prizeSplits = new uint256[](2);
        prizeSplits[0] = 7000;
        prizeSplits[1] = 3000;

        TournamentFactory.TournamentParams memory params = TournamentFactory.TournamentParams({
            buyInToken: NATIVE_TOKEN,
            buyInAmount: BUY_IN_AMOUNT,
            tableSize: 2,
            advanceCount: 1,
            rakeBps: 100,
            prizeSplits: prizeSplits,
            startTime: block.timestamp + 1 hours,
            duration: 2 hours
        });

        uint256 tournamentId = factory.createTournament(params);
        uint256 tokenId = baseNFT.mintTo(player1, tournamentId, "https://test.com/1");
        vm.stopPrank();

        vm.deal(player1, BUY_IN_AMOUNT);
        vm.startPrank(player1);
        escrow.createEntry{value: BUY_IN_AMOUNT}(
            tournamentId,
            tokenId,
            player1,
            BUY_IN_AMOUNT,
            NATIVE_TOKEN
        );
        vm.stopPrank();

        // Record result
        vm.prank(relayer);
        bytes32 resultHash = keccak256(abi.encodePacked("battle_log_123"));
        escrow.recordResult(1, tokenId, resultHash);

        (uint256 tableId, uint256 winnerTokenId, bytes32 resultCommitHash, bool isRecorded, uint256 recordedAt) = escrow.tableResults(1);
        assertTrue(isRecorded);
        assertEq(winnerTokenId, tokenId);
        assertEq(resultCommitHash, resultHash);
    }

    function test_DistributePayout() public {
        // Setup tournament and entry
        vm.startPrank(admin);
        uint256[] memory prizeSplits = new uint256[](1);
        prizeSplits[0] = 10000;

        TournamentFactory.TournamentParams memory params = TournamentFactory.TournamentParams({
            buyInToken: NATIVE_TOKEN,
            buyInAmount: BUY_IN_AMOUNT,
            tableSize: 1,
            advanceCount: 1,
            rakeBps: 100,
            prizeSplits: prizeSplits,
            startTime: block.timestamp + 1 hours,
            duration: 2 hours
        });

        uint256 tournamentId = factory.createTournament(params);
        uint256 tokenId = baseNFT.mintTo(player1, tournamentId, "https://test.com/1");
        vm.stopPrank();

        uint256 initialBalance = player1.balance;
        vm.deal(player1, BUY_IN_AMOUNT);
        vm.startPrank(player1);
        escrow.createEntry{value: BUY_IN_AMOUNT}(
            tournamentId,
            tokenId,
            player1,
            BUY_IN_AMOUNT,
            NATIVE_TOKEN
        );
        vm.stopPrank();

        // Record result and distribute payout
        vm.prank(relayer);
        escrow.recordResult(1, tokenId, keccak256(abi.encodePacked("log")));

        vm.prank(relayer);
        uint256 payoutValue = BUY_IN_AMOUNT * 9900 / 10000; // After 1% rake
        uint256 payoutId = escrow.distributePayout(1, payoutValue, 1);

        assertEq(player1.balance, initialBalance + payoutValue);
        (uint256 payoutTournamentId, uint256 payoutEntryId, uint256 payoutAmount, address payoutToken, bool distributed, bytes32 transactionHash) = escrow.payouts(payoutId);
        assertTrue(distributed);
    }

    function test_TransferRestriction() public {
        vm.startPrank(admin);
        uint256 tokenId = baseNFT.mintTo(player1, 1, "https://test.com/1");
        vm.stopPrank();

        // Should not be able to transfer while in tournament
        vm.prank(player1);
        vm.expectRevert("BaseNFT: cannot transfer during active tournament");
        baseNFT.transferFrom(player1, player2, tokenId);

        // Deactivate tournament and should be able to transfer
        vm.prank(admin);
        baseNFT.setTournamentStatus(tokenId, 0);

        vm.prank(player1);
        baseNFT.transferFrom(player1, player2, tokenId);
        assertEq(baseNFT.ownerOf(tokenId), player2);
    }
}


