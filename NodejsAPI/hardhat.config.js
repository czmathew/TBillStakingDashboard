/**
* @type import('hardhat/config').HardhatUserConfig
*/
require('dotenv').config();
require("@nomiclabs/hardhat-ethers");

const { PRIVATE_KEY } = process.env;
module.exports = {
  solidity: "0.8.0",
  defaultNetwork: "theta_mainnet",
  networks: {
    hardhat: {},
    theta_mainnet: {
      url: `https://eth-rpc-api.thetatoken.org/rpc`,
      accounts: [`${PRIVATE_KEY}`],
      chainId: 361,
      gasPrice: 4000000000000
    }
  }
}