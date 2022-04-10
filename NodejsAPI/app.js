const express = require('express');
const app = express();
const port = process.env.PORT || 3030;

const http = require('http');
const url = require('url');

const { ethers } = require("hardhat");

const contractAbi = JSON.parse(`[
  {
      "inputs": [],
      "name": "name",
      "outputs": [
          {
              "internalType": "string",
              "name": "",
              "type": "string"
          }
      ],
      "stateMutability": "view",
      "type": "function"
  },
  {
      "inputs": [
          {
              "internalType": "uint256",
              "name": "",
              "type": "uint256"
          }
      ],
      "name": "tokenURI",
      "outputs": [
          {
              "internalType": "string",
              "name": "",
              "type": "string"
          }
      ],
      "stateMutability": "view",
      "type": "function"
  }
]`);

app.get('/', async (req, res) => {
  

  const [deployer] = await ethers.getSigners();

    const queryObject = url.parse(req.url, true).query;
    console.log('Contract: ' + queryObject.contract);
    // console.log(
    //     "Address:",
    //     await deployer.getAddress()
    // );

    if (queryObject == null || queryObject.contract == undefined || queryObject.contract == '' || queryObject.tokenId == undefined || queryObject.tokenId == '') {
        res.statusCode = 200;
        res.setHeader('Content-Type', 'text/plain');
        const result = {error:'Incorrect input data'}
        res.end(JSON.stringify(result));
    } else {

      const contract = new ethers.Contract(queryObject.contract, contractAbi, deployer);
    
      

      const name= await contract.name();
      // console.log("contract name: ", name);

      const uri = await contract.tokenURI(queryObject.tokenId);
      // console.log("token uri", uri);
      const result = {
          uri: uri
      }
      res.statusCode = 200;
      res.setHeader('Content-Type', 'text/plain');
      res.end(JSON.stringify(result));

    } 

});

/*
call this URL: http://127.0.0.1:3000/?contract=0x53ae8cdc2e60c81f4a1967dc381452a203dee836&tokenId=1537
*/

app.listen(port, () => console.log(`App listening on port ${port}!`))