﻿using System;
using System.Text;
using Nethereum.Hex.HexConvertors.Extensions;
using System.Threading.Tasks;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RPC.TransactionManagers;
using Nethereum.Signer;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using Nethereum.Hex.HexTypes;

public class Program
{
    private static async Task Main(string[] args)
    {
        //First let's create an account with our private key for the account address 
        var privateKey = "0x7580e7fb49df1c861f0050fae31c2224c6aba908e116b8da44ee8cd927b990b0";
        var account = new Account(privateKey);

        Console.WriteLine("Our account: " + account.Address);
        //Now let's create an instance of Web3 using our account pointing to our nethereum testchain
        var web3 = new Web3(account, "http://testchain.nethereum.com:8545");

        var toAddress = "0x1bb31d596c34bd81e1f0be1edf3840a7b43dd9cd";
        var transactionManager = web3.TransactionManager;

        //Gas price and nonce is calculated by the transaction manager so no need to set it
        var txnInput = new TransactionInput()
        {
            From = account.Address,
            To = toAddress,
            Data = "0x7b227465737444617465223a2232372f30352f323032302031333a32353a35322e38373120504d227d"
        };

        Console.WriteLine(transactionManager.DefaultGas);
        //lets estimate and set the gas as we have some data
        var gasEstimate = await transactionManager.EstimateGasAsync(txnInput);
        Console.WriteLine(gasEstimate.Value);
        txnInput.Gas = gasEstimate;

        var txnHash1 = await transactionManager.SendTransactionAsync(txnInput);

        Console.WriteLine(txnHash1);

        var txnInput2 = new TransactionInput()
        {
            From = account.Address,
            To = toAddress,
            Data = "0x7b227465737444617465223a2232372f30352f323032302031333a32353a35322e38373120504d227d",
            Gas = gasEstimate,
            //Lets use the same nonce to replace the transaction
            Nonce = txnInput.Nonce
        };

        //Lets increment the gas price to replace the transaction
        var gasPrice = await web3.Eth.GasPrice.SendRequestAsync();
        //incrementing the gas price
        txnInput2.GasPrice = new HexBigInteger(gasPrice.Value + Web3.Convert.ToWei(1, UnitConversion.EthUnit.Gwei));

        var txnHash2 = await transactionManager.SendTransactionAsync(txnInput2);

        Console.WriteLine(txnHash2);

    }
}