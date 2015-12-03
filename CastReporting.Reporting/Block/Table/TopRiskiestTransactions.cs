﻿/*
 *   Copyright (c) 2015 CAST
 *
 * Licensed under a custom license, Version 1.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License, accessible in the main project
 * source code: Empowerment.
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.BLL.Computing;


namespace CastReporting.Reporting.Block.Table
{
    [Block("TOP_RISKIEST_TRANSACTIONS")]
    class TopRiskiestTransactions : TableBlock
    {
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            TableDefinition resultTable = null;
            int nbLimitTop = reportData.Parameter.NbResultDefault;

            // Default Options
            int businessCriteria = 0;
            if (options != null &&
                options.ContainsKey("SRC"))
            {
                var source = options["SRC"];
                switch (source)
                {
                    case "PERF": { businessCriteria = (int)CastReporting.Domain.Constants.BusinessCriteria.Performance; } break;
                    case "ROB": { businessCriteria = (int)CastReporting.Domain.Constants.BusinessCriteria.Robustness; } break;
                    case "SEC": { businessCriteria = (int)CastReporting.Domain.Constants.BusinessCriteria.Security; } break;
                }
            }
            if (options == null ||
                !options.ContainsKey("COUNT") ||
                !int.TryParse(options["COUNT"], out nbLimitTop))
            {
                nbLimitTop = 10;
            }

            var bc = reportData.CurrentSnapshot.BusinessCriteriaResults.Where(_ => _.Reference.Key == businessCriteria).FirstOrDefault();

            bc.Transactions = reportData.SnapshotExplorer.GetTransactions(reportData.CurrentSnapshot.Href, bc.Reference.Key.ToString(), nbLimitTop);

            List<string> rowData = new List<string>(new string[] { "Transaction Entry Point", "TRI" });
            int nbRows = 0;

            const string metricFormat = "N0";
            if (bc.Transactions !=null && bc.Transactions.Count() > 0)
            {
                foreach (var transaction in bc.Transactions)
                {
                    rowData.Add(transaction.Name);
                    rowData.Add(transaction.TransactionRiskIndex.ToString(metricFormat));
                    nbRows += 1;
                }
            }
            else
            {
                rowData.AddRange(new string[] { "No enable item.", string.Empty });
            }

            resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = nbRows + 1,
                NbColumns = 2,
                Data = rowData
            };

            return resultTable;
        }
    }
}
