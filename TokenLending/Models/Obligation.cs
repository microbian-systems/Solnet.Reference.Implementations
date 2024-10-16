﻿using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Solnet.Programs.TokenLending.Models
{
    /// <summary>
    /// Represents the collateral of an obligation.
    /// </summary>
    public class ObligationCollateral
    {
        /// <summary>
        /// The layout of the <see cref="ObligationCollateral"/> structure.
        /// </summary>
        public static class Layout
        {
            /// <summary>
            /// The length of the <see cref="ObligationCollateral"/> structure.
            /// </summary>
            public const int Length = 56;

            /// <summary>
            /// The offset at which the deposit reserve value begins.
            /// </summary>
            public const int DepositReserveOffset = 0;

            /// <summary>
            /// The offset at which the cumulative borrow rate value begins.
            /// </summary>
            public const int DepositedAmountOffset = 32;

            /// <summary>
            /// The offset at which the borrow amount value begins.
            /// </summary>
            public const int MarketValueOffset = 40;
        }

        /// <summary>
        /// Reserve collateral is deposited to
        /// </summary>
        public PublicKey DepositReserve;

        /// <summary>
        /// Amount of collateral deposited
        /// </summary>
        public ulong DepositedAmount;

        /// <summary>
        /// Collateral market value in quote currency
        /// </summary>
        public BigInteger MarketValue;

        /// <summary>
        /// Initialize the <see cref="ObligationCollateral"/> with the given data.
        /// </summary>
        /// <param name="data">The byte array.</param>
        public ObligationCollateral(ReadOnlySpan<byte> data)
        {
            if (data.Length != Layout.Length)
                throw new ArgumentException($"{nameof(data)} has wrong size. Expected {Layout.Length} bytes, actual {data.Length} bytes.");

            DepositReserve = data.GetPubKey(Layout.DepositReserveOffset);
            DepositedAmount = data.GetU64(Layout.DepositedAmountOffset);
            MarketValue = data.GetBigInt(Layout.MarketValueOffset, 16, true);
        }

        /// <summary>
        /// Deserialize the given byte array into the <see cref="ObligationCollateral"/> structure.
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <returns>The <see cref="ObligationCollateral"/> instance.</returns>
        public static ObligationCollateral Deserialize(byte[] data) => new(data.AsSpan());
    }

    /// <summary>
    /// Represents the the liquidity of an obligation.
    /// </summary>
    public class ObligationLiquidity
    {
        /// <summary>
        /// The layout of the <see cref="ObligationLiquidity"/> structure.
        /// </summary>
        public static class Layout
        {
            /// <summary>
            /// The length of the <see cref="ObligationLiquidity"/> structure.
            /// </summary>
            public const int Length = 80;

            /// <summary>
            /// The offset at which the borrow reserve value begins.
            /// </summary>
            public const int BorrowReserveOffset = 0;

            /// <summary>
            /// The offset at which the cumulative borrow rate value begins.
            /// </summary>
            public const int CumulativeBorrowRateOffset = 32;

            /// <summary>
            /// The offset at which the borrow amount value begins.
            /// </summary>
            public const int BorrowAmountOffset = 48;

            /// <summary>
            /// The offset at which the market value begins.
            /// </summary>
            public const int MarketValueOffset = 64;
        }

        /// <summary>
        /// Reserve liquidity is borrowed from
        /// </summary>
        public PublicKey BorrowReserve;

        /// <summary>
        /// Borrow rate used for calculating interest
        /// </summary>
        public BigInteger CumulativeBorrowRateWads;

        /// <summary>
        /// Amount of liquidity borrowed plus interest
        /// </summary>
        public BigInteger BorrowedAmountWads;

        /// <summary>
        /// Liquidity market value in quote currency
        /// </summary>
        public BigInteger MarketValue;

        /// <summary>
        /// Initialize the <see cref="ObligationLiquidity"/> with the given data.
        /// </summary>
        /// <param name="data">The byte array.</param>
        public ObligationLiquidity(ReadOnlySpan<byte> data)
        {
            if (data.Length != Layout.Length)
                throw new ArgumentException($"{nameof(data)} has wrong size. Expected {Layout.Length} bytes, actual {data.Length} bytes.");

            BorrowReserve = data.GetPubKey(Layout.BorrowReserveOffset);
            CumulativeBorrowRateWads = data.GetBigInt(Layout.CumulativeBorrowRateOffset, 16, true);
            BorrowedAmountWads = data.GetBigInt(Layout.BorrowAmountOffset, 16, true);
            MarketValue = data.GetBigInt(Layout.MarketValueOffset, 16, true);
        }

        /// <summary>
        /// Deserialize the given byte array into the <see cref="ObligationLiquidity"/> structure.
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <returns>The <see cref="ObligationLiquidity"/> instance.</returns>
        public static ObligationLiquidity Deserialize(byte[] data) => new(data.AsSpan());
    }

    /// <summary>
    /// Represents an obligation.
    /// </summary>
    public class Obligation
    {
        /// <summary>
        /// The layout of the <see cref="Obligation"/> structure.
        /// </summary>
        public static class Layout
        {
            /// <summary>
            /// The length of the <see cref="Obligation"/> structure.
            /// </summary>
            public const int Length = 916;

            /// <summary>
            /// The offset at which the version value begins.
            /// </summary>
            public const int VersionOffset = 0;

            /// <summary>
            /// The offset at which the last update value begins.
            /// </summary>
            public const int LastUpdateOffset = 1;

            /// <summary>
            /// The offset at which the lending market value begins.
            /// </summary>
            public const int LendingMarketOffset = 10;

            /// <summary>
            /// The offset at which the liquidity value begins.
            /// </summary>
            public const int OwnerOffset = 42;

            /// <summary>
            /// The offset at which the deposits vector begins.
            /// </summary>
            public const int DepositsOffset = 74;

            /// <summary>
            /// The offset for the deposited value, only valid after reading the deposits and borrows vectors.
            /// </summary>
            public const int DepositedValueOffset = 0;

            /// <summary>
            /// The offset for the deposited value, only valid after reading the deposits and borrows vectors.
            /// </summary>
            public const int BorrowedValueOffset = 16;

            /// <summary>
            /// The offset for the deposited value, only valid after reading the deposits and borrows vectors.
            /// </summary>
            public const int AllowedBorrowValueOffset = 32;

            /// <summary>
            /// The offset for the deposited value, only valid after reading the deposits and borrows vectors.
            /// </summary>
            public const int UnhealthyBorrowValueOffset = 48;
        }

        /// <summary>
        /// Version of the struct
        /// </summary>
        public byte Version;

        /// <summary>
        /// Last slot when supply and rates updated
        /// </summary>
        public LastUpdate LastUpdate;

        /// <summary>
        /// Lending market public key
        /// </summary>
        public PublicKey LendingMarket;

        /// <summary>
        /// Owner authority which can borrow liquidity
        /// </summary>
        public PublicKey Owner;

        /// <summary>
        /// Deposited collateral for the obligation, unique by deposit reserve address
        /// </summary>
        public List<ObligationCollateral> Deposits;

        /// <summary>
        /// Borrowed liquidity for the obligation, unique by borrow reserve address
        /// </summary>
        public List<ObligationLiquidity> Borrows;

        /// <summary>
        /// Market value of deposits
        /// </summary>
        public BigInteger DepositedValue;

        /// <summary>
        /// Market value of borrows
        /// </summary>
        public BigInteger BorrowedValue;

        /// <summary>
        /// The maximum borrow value at the weighted average loan to value ratio
        /// </summary>
        public BigInteger AllowedBorrowValue;

        /// <summary>
        /// The dangerous borrow value at the weighted average liquidation threshold
        /// </summary>
        public BigInteger UnhealthyBorrowValue;

        /// <summary>
        /// Initialize the <see cref="Obligation"/> with the given data.
        /// </summary>
        /// <param name="data">The byte array.</param>
        public Obligation(ReadOnlySpan<byte> data)
        {
            if (data.Length != Layout.Length)
                throw new ArgumentException($"{nameof(data)} has wrong size. Expected {Layout.Length} bytes, actual {data.Length} bytes.");

            List<ObligationCollateral> deposits = new();
            int numDeposits = (int)data.GetU32(Layout.DepositsOffset);
            ReadOnlySpan<byte> depositsBytes =
                data.Slice(Layout.DepositsOffset + sizeof(uint), ObligationCollateral.Layout.Length * numDeposits);

            for (int i = 0; i < numDeposits; i++)
            {
                ObligationCollateral obligationCollateral = new (depositsBytes.GetSpan(
                    i * ObligationCollateral.Layout.Length,
                    ObligationCollateral.Layout.Length));
                deposits.Add(obligationCollateral);
            }

            int borrowsOffset = Layout.DepositsOffset + sizeof(uint) + ObligationCollateral.Layout.Length * numDeposits;
            List<ObligationLiquidity> borrows = new();
            int numBorrows = (int)data.GetU32(borrowsOffset);
            ReadOnlySpan<byte> borrowsBytes =
                data.Slice(borrowsOffset + sizeof(uint), ObligationLiquidity.Layout.Length * numBorrows);
            int postBorrowsOffset = borrowsOffset + sizeof(uint) + ObligationLiquidity.Layout.Length * numBorrows;

            for (int i = 0; i < numBorrows; i++)
            {
                ObligationLiquidity obligationLiquidity = new (borrowsBytes.GetSpan(
                    i * ObligationLiquidity.Layout.Length,
                    ObligationLiquidity.Layout.Length));
                borrows.Add(obligationLiquidity);
            }

            Version = data.GetU8(Layout.VersionOffset);
            LastUpdate = new(data.Slice(Layout.LastUpdateOffset, LastUpdate.Layout.Length));
            LendingMarket = data.GetPubKey(Layout.LendingMarketOffset);
            Owner = data.GetPubKey(Layout.OwnerOffset);
            Deposits = deposits;
            Borrows = borrows;
            DepositedValue = data.GetBigInt(postBorrowsOffset + Layout.DepositedValueOffset, 16, true);
            BorrowedValue = data.GetBigInt(postBorrowsOffset + Layout.BorrowedValueOffset, 16, true);
            AllowedBorrowValue = data.GetBigInt(postBorrowsOffset + Layout.AllowedBorrowValueOffset, 16, true);
            UnhealthyBorrowValue = data.GetBigInt(postBorrowsOffset + Layout.UnhealthyBorrowValueOffset, 16, true);
        }

        /// <summary>
        /// Deserialize the given byte array into the <see cref="Obligation"/> structure.
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <returns>The <see cref="Obligation"/> instance.</returns>
        public static Obligation Deserialize(byte[] data) => new(data.AsSpan());
    }
}
