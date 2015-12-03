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
using CastReporting.Domain;


namespace CastReporting.Reporting.Block.Table
{
    /// <summary>
    /// TopNonCriticalViolations Class
    /// </summary>
    [Block("TOP_NON_CRITICAL_VIOLATIONS")]
    class TopNonCriticalViolations : TableBlock
    {
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            int nbRows = 0;
            int nbLimitTop = 0;
            List<string> rowData = new List<string>();
            
            rowData.AddRange(new string[] { "Rule Names", "Count" });
                                   
            Int32? metricId = (options != null && options.ContainsKey("BC-ID")) ? Convert.ToInt32(options["BC-ID"]) : (Int32?)null;
            if (metricId == null)
                metricId = (options != null && options.ContainsKey("PAR")) ? Convert.ToInt32(options["PAR"]) : (Int32?)null;
            if (options  == null || !options.ContainsKey("COUNT") || !Int32.TryParse(options["COUNT"], out nbLimitTop))
            {
                nbLimitTop = reportData.Parameter.NbResultDefault;
            }

            if (reportData != null && reportData.CurrentSnapshot!=null && metricId.HasValue)
            {

                var NonCriticalRulesViolation = RulesViolationUtility.GetRuleViolations(reportData.CurrentSnapshot, 
                                                                                        Constants.RulesViolation.NonCriticalRulesViolation,
                                                                                       (Constants.BusinessCriteria)metricId,
                                                                                       true, 
                                                                                       nbLimitTop);


                if (NonCriticalRulesViolation != null && NonCriticalRulesViolation.Count() > 0)
                {                    
                    foreach (var elt in NonCriticalRulesViolation)
                    {
                        rowData.AddRange(new string[] { elt.Rule.Name, elt.TotalFailed.Value.ToString("N0") });
                    }
                }
                else
                {
                    rowData.AddRange(new string[] { "No enable item.", string.Empty });
                }
                nbRows = NonCriticalRulesViolation.Count();
            }

            TableDefinition resultTable = new TableDefinition
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
