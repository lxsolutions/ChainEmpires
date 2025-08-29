
// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "@openzeppelin/contracts/token/ERC721/ERC721.sol";
import "@openzeppelin/contracts/access/Ownable.sol";

/**
 * @title BaseNFT
 * @dev ERC-721 token representing a player's persistent "chips" in the tournament system
 * Each Base NFT is tied to a specific tournament and has transfer restrictions while active
 */
contract BaseNFT is ERC721, Ownable {
    // Token ID counter
    uint256 private _tokenIdCounter;

    // Tournament ID for active tournament (0 means not in tournament)
    mapping(uint256 => uint256) public activeTournamentId;

    // Metadata base URI
    string private _baseTokenURI;

    // Events
    event BaseMinted(address indexed player, uint256 indexed tokenId, uint256 tournamentId, string metadataURI);
    event TournamentStatusUpdated(uint256 indexed tokenId, uint256 tournamentId, bool isActive);

    /**
     * @dev Constructor
     * @param name_ Token name
     * @param symbol_ Token symbol
     * @param baseTokenURI_ Base URI for token metadata
     */
    constructor(
        string memory name_,
        string memory symbol_,
        string memory baseTokenURI_
    ) ERC721(name_, symbol_) Ownable(msg.sender) {
        _baseTokenURI = baseTokenURI_;
    }

    /**
     * @dev Mint a new Base NFT for a player
     * @param player Address of the player receiving the NFT
     * @param tournamentId ID of the tournament this Base is associated with
     * @param metadataURI Custom metadata URI for this token
     * @return tokenId The minted token ID
     */
    function mintTo(
        address player,
        uint256 tournamentId,
        string memory metadataURI
    ) external onlyOwner returns (uint256) {
        _tokenIdCounter++;
        uint256 tokenId = _tokenIdCounter;
        
        _mint(player, tokenId);
        activeTournamentId[tokenId] = tournamentId;
        
        emit BaseMinted(player, tokenId, tournamentId, metadataURI);
        return tokenId;
    }

    /**
     * @dev Update tournament status for a token
     * @param tokenId Token ID to update
     * @param tournamentId Tournament ID (0 to deactivate)
     */
    function setTournamentStatus(uint256 tokenId, uint256 tournamentId) external onlyOwner {
        require(_exists(tokenId), "BaseNFT: token does not exist");
        activeTournamentId[tokenId] = tournamentId;
        
        emit TournamentStatusUpdated(tokenId, tournamentId, tournamentId != 0);
    }

    /**
     * @dev Override transfer functions to prevent transfers during active tournaments
     */
    function _update(
        address to,
        uint256 tokenId,
        address auth
    ) internal virtual override returns (address) {
        address from = _ownerOf(tokenId);
        
        // Allow minting and burning, but prevent transfers while in active tournament
        if (from != address(0) && to != address(0)) {
            require(activeTournamentId[tokenId] == 0, "BaseNFT: cannot transfer during active tournament");
        }
        
        return super._update(to, tokenId, auth);
    }

    /**
     * @dev Get the base URI for token metadata
     */
    function _baseURI() internal view virtual override returns (string memory) {
        return _baseTokenURI;
    }

    /**
     * @dev Check if a token exists
     */
    function _exists(uint256 tokenId) internal view returns (bool) {
        return _ownerOf(tokenId) != address(0);
    }

    /**
     * @dev Get current token ID counter
     */
    function getCurrentTokenId() external view returns (uint256) {
        return _tokenIdCounter;
    }

    /**
     * @dev Get tournament status for a token
     */
    function getTournamentStatus(uint256 tokenId) external view returns (uint256) {
        return activeTournamentId[tokenId];
    }
}
