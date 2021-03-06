//
// DO NOT REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
//
// @Authors:
//       peters
//
// Copyright 2004-2013 by OM International
//
// This file is part of OpenPetra.org.
//
// OpenPetra.org is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// OpenPetra.org is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with OpenPetra.org.  If not, see <http://www.gnu.org/licenses/>.
//
using System;
using System.Data;
using System.Globalization;

using Ict.Common;
using Ict.Common.DB;
using Ict.Common.Verification;
using Ict.Petra.Shared.MFinance.Account.Data;
using Ict.Petra.Shared.MFinance.GL.Data;
using Ict.Petra.Server.App.Core.Security;
using Ict.Petra.Server.MFinance.Account.Data.Access;

namespace Ict.Petra.Server.MFinance.Setup.WebConnectors
{
    /// <summary>
    /// setup the Corporate Exchange Rates
    /// </summary>
    public partial class TCorporateExchangeRatesSetupWebConnector
    {
        /// <summary>
        /// Saves the corporate exchange setup TDS.
        /// </summary>
        /// <param name="AInspectDS">Containing all ACorporateExchangeRate records to be saved.</param>
        /// <param name="ATransactionsChanged">Number of transaction that were updated with the new exchange rate</param>
        /// <param name="AVerificationResult"></param>
        /// <returns></returns>
        [RequireModulePermission("FINANCE-1")]
        public static TSubmitChangesResult SaveCorporateExchangeSetupTDS(ref CorporateExchangeSetupTDS AInspectDS,
            out int ATransactionsChanged, out TVerificationResultCollection AVerificationResult)
        {
            AVerificationResult = new TVerificationResultCollection();
            ATransactionsChanged = -1;
            int TransactionsChanged = -1;

            AInspectDS = AInspectDS.GetChangesTyped(true);

            if (AInspectDS == null)
            {
                AVerificationResult.Add(new TVerificationResult(
                        Catalog.GetString("Save Corportate Exchange Rates"),
                        Catalog.GetString("No changes - nothing to do"),
                        TResultSeverity.Resv_Info));
                return TSubmitChangesResult.scrNothingToBeSaved;
            }

            TDBTransaction Transaction = null;
            bool SubmissionOK = false;
            CorporateExchangeSetupTDS InspectDS = AInspectDS;

            DBAccess.GDBAccessObj.GetNewOrExistingAutoTransaction(IsolationLevel.Serializable,
                ref Transaction, ref SubmissionOK,
                delegate
                {
                    foreach (ACorporateExchangeRateRow Row in InspectDS.ACorporateExchangeRate.Rows)
                    {
                        if ((Row.RowState == DataRowState.Modified) || (Row.RowState == DataRowState.Added))
                        {
                            // should only be -1 if no exchange rates were modified or created
                            if (TransactionsChanged == -1)
                            {
                                TransactionsChanged = 0;
                            }

                            // update international amounts for all gl transaction using modified or new exchange rate
                            string Query = "UPDATE a_transaction SET a_amount_in_intl_currency_n = " +
                                           "(a_amount_in_base_currency_n / " + Row.RateOfExchange.ToString(CultureInfo.InvariantCulture) + ")" +
                                           " FROM a_ledger" +
                                           " WHERE EXTRACT(MONTH FROM a_transaction.a_transaction_date_d) = " + Row.DateEffectiveFrom.Month +
                                           " AND EXTRACT(YEAR FROM a_transaction.a_transaction_date_d) = " + Row.DateEffectiveFrom.Year +
                                           " AND a_ledger.a_ledger_number_i = a_transaction.a_ledger_number_i" +
                                           " AND a_ledger.a_base_currency_c = '" + Row.FromCurrencyCode + "'" +
                                           " AND a_ledger.a_intl_currency_c = '" + Row.ToCurrencyCode + "'";

                            TransactionsChanged += DBAccess.GDBAccessObj.ExecuteNonQuery(Query, Transaction);
                        }
                    }

                    // save changes to exchange rates
                    ACorporateExchangeRateAccess.SubmitChanges(InspectDS.ACorporateExchangeRate, Transaction);

                    SubmissionOK = true;
                });

            TSubmitChangesResult SubmissionResult;

            if (SubmissionOK)
            {
                SubmissionResult = TSubmitChangesResult.scrOK;
            }
            else
            {
                SubmissionResult = TSubmitChangesResult.scrError;
            }

            AInspectDS = InspectDS;
            ATransactionsChanged = TransactionsChanged;

            return SubmissionResult;
        }

        /// <summary>
        /// Returns true if a Corporate Exchange Rate can be deleted.
        /// Cannot be deleted if it is effective for a period in the current year which has at least one batch.
        /// </summary>
        /// <param name="ADateEffectiveFrom">Corporate Exchange Rate's Date Effective From</param>
        /// <param name="AIntlCurrency"></param>
        /// <param name="ATransactionCurrency"></param>
        /// <returns></returns>
        [RequireModulePermission("FINANCE-1")]
        public static bool CanDeleteCorporateExchangeRate(DateTime ADateEffectiveFrom, string AIntlCurrency, string ATransactionCurrency)
        {
            bool ReturnValue = true;
            TDBTransaction ReadTransaction = null;

            DBAccess.GDBAccessObj.GetNewOrExistingAutoReadTransaction(IsolationLevel.ReadCommitted,
                TEnforceIsolationLevel.eilMinimum,
                ref ReadTransaction,
                delegate
                {
                    // get accounting period for when the exchange rate is effective (if it exists)
                    string Query = "SELECT * FROM a_accounting_period" +
                                   " WHERE a_accounting_period.a_period_end_date_d >= '" + ADateEffectiveFrom + "'" +
                                   " AND a_accounting_period.a_period_start_date_d <= '" + ADateEffectiveFrom + "'";

                    AAccountingPeriodTable AccountingPeriodTable = new AAccountingPeriodTable();
                    DBAccess.GDBAccessObj.SelectDT(AccountingPeriodTable, Query, ReadTransaction);

                    // no accounting period if effective in a year other that the current year
                    if ((AccountingPeriodTable == null) || (AccountingPeriodTable.Rows.Count == 0))
                    {
                        return;
                    }

                    AAccountingPeriodRow AccountingPeriodRow = AccountingPeriodTable[0];

                    // search for batches for the found accounting period
                    string Query2 = "SELECT CASE WHEN EXISTS (" +
                                    "SELECT * FROM a_batch, a_journal, a_ledger" +
                                    " WHERE a_batch.a_date_effective_d <= '" + AccountingPeriodRow.PeriodEndDate + "'" +
                                    " AND a_batch.a_date_effective_d >= '" + AccountingPeriodRow.PeriodStartDate + "'" +
                                    " AND a_journal.a_ledger_number_i = a_batch.a_ledger_number_i" +
                                    " AND a_journal.a_batch_number_i = a_batch.a_batch_number_i" +
                                    " AND a_ledger.a_ledger_number_i = a_batch.a_ledger_number_i" +
                                    " AND ((a_journal.a_transaction_currency_c = '" + ATransactionCurrency + "'" +
                                    " AND a_ledger.a_intl_currency_c = '" + AIntlCurrency + "')" +
                                    " OR (a_journal.a_transaction_currency_c = '" + AIntlCurrency + "'" +
                                    " AND a_ledger.a_intl_currency_c = '" + ATransactionCurrency + "'))" +
                                    ") THEN 'TRUE'" +
                                    " ELSE 'FALSE' END";

                    DataTable DT = DBAccess.GDBAccessObj.SelectDT(Query2, "temp", ReadTransaction);

                    // a batch has been found
                    if ((DT != null) && (DT.Rows.Count > 0) && (DT.Rows[0][0].ToString() == "TRUE"))
                    {
                        ReturnValue = false;
                        return;
                    }
                });

            return ReturnValue;
        }
    }
}