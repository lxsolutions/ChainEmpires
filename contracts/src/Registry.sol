

// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "@openzeppelin/contracts/access/AccessControl.sol";

/**
 * @title Registry
 * @dev Central registry for contract addresses and role management
 */
contract Registry is AccessControl {
    // Role definitions
    bytes32 public constant TOURNAMENT_ADMIN_ROLE = keccak256("TOURNAMENT_ADMIN_ROLE");
    bytes32 public constant RESULT_RELAYER_ROLE = keccak256("RESULT_RELAYER_ROLE");
    bytes32 public constant PAYOUT_MANAGER_ROLE = keccak256("PAYOUT_MANAGER_ROLE");

    // Contract address mappings
    mapping(bytes32 => address) public contractAddresses;

    // Events
    event ContractAddressSet(bytes32 indexed contractName, address indexed contractAddress);

    /**
     * @dev Constructor
     * @param admin Address to grant default admin role
     */
    constructor(address admin) {
        _grantRole(DEFAULT_ADMIN_ROLE, admin);
        _grantRole(TOURNAMENT_ADMIN_ROLE, admin);
        _grantRole(RESULT_RELAYER_ROLE, admin);
        _grantRole(PAYOUT_MANAGER_ROLE, admin);
    }

    /**
     * @dev Set contract address
     * @param contractName Name identifier for the contract
     * @param contractAddress Address of the contract
     */
    function setContractAddress(bytes32 contractName, address contractAddress) external onlyRole(DEFAULT_ADMIN_ROLE) {
        contractAddresses[contractName] = contractAddress;
        emit ContractAddressSet(contractName, contractAddress);
    }

    /**
     * @dev Get contract address
     * @param contractName Name identifier for the contract
     * @return Address of the contract
     */
    function getContractAddress(bytes32 contractName) external view returns (address) {
        return contractAddresses[contractName];
    }

    /**
     * @dev Grant role to multiple accounts
     * @param role Role to grant
     * @param accounts Array of accounts to grant role to
     */
    function grantRoleBatch(bytes32 role, address[] calldata accounts) external onlyRole(getRoleAdmin(role)) {
        for (uint256 i = 0; i < accounts.length; i++) {
            _grantRole(role, accounts[i]);
            emit RoleGranted(role, accounts[i], msg.sender);
        }
    }

    /**
     * @dev Revoke role from multiple accounts
     * @param role Role to revoke
     * @param accounts Array of accounts to revoke role from
     */
    function revokeRoleBatch(bytes32 role, address[] calldata accounts) external onlyRole(getRoleAdmin(role)) {
        for (uint256 i = 0; i < accounts.length; i++) {
            _revokeRole(role, accounts[i]);
            emit RoleRevoked(role, accounts[i], msg.sender);
        }
    }

    // Predefined contract name constants
    bytes32 public constant BASE_NFT = keccak256("BASE_NFT");
    bytes32 public constant TOURNAMENT_FACTORY = keccak256("TOURNAMENT_FACTORY");
    bytes32 public constant TOURNAMENT_ESCROW = keccak256("TOURNAMENT_ESCROW");
}

