﻿/*
 *   Copyright (c) 2016 CAST
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
using CastReporting.Reporting.Languages;
using CastReporting.BLL.Computing;
using CastReporting.Domain;

namespace CastReporting.Reporting.Block.Table
{
    [Block("RULES_LIST")]
    public class RulesList : TableBlock
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportData"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            string srBusinessCriterias = (options != null && options.ContainsKey("PAR")) ? options["PAR"] : null;
            int count = 0;
            if (options == null || !options.ContainsKey("COUNT") || !Int32.TryParse(options["COUNT"], out count))
            {
                count = reportData.Parameter.NbResultDefault;
            }
            
            if (!string.IsNullOrWhiteSpace(srBusinessCriterias))
            {
                // Parse business criterias Ids
                List<int> businessCriteriasIds = new List<int>();
                string[] parentMetrics = srBusinessCriterias.Split('|');
                foreach (var metric in parentMetrics.Distinct())
                {
                    int metricId = 0;
                    if (int.TryParse(metric, out metricId))
                    {
                        businessCriteriasIds.Add(metricId);
                    }
                }
                                              
                //Build result
                List<string> rowData = new List<string>();
                rowData.AddRange(new string[] { Labels.Criticality, Labels.Weight, Labels.Grade, Labels.TechnicalCriterion, Labels.RuleName, Labels.ViolCount, Labels.TotalOk });

                var results = RulesViolationUtility.GetNbViolationByRule(reportData.CurrentSnapshot, reportData.RuleExplorer, businessCriteriasIds, count);
                int nbRows = 0;
                foreach (var item in results)
                {
                    
                    rowData.Add(item.Rule.Critical ? "µ" : string.Empty);
                    rowData.Add(item.Rule.CompoundedWeight.ToString());              
                    rowData.Add(item.Grade.Value.ToString("N2"));
                    rowData.Add(item.TechnicalCriteraiName);
                    rowData.Add(item.Rule.Name);

                    rowData.Add(item.TotalFailed.HasValue ? item.TotalFailed.Value.ToString("N0") : Constants.No_Data);
                    rowData.Add(item.TotalChecks.HasValue ? item.TotalChecks.Value.ToString("N0") : Constants.No_Value);                       
                   
                    nbRows++;
                }

                TableDefinition resultTable = new TableDefinition
                {
                    HasRowHeaders = false,
                    HasColumnHeaders = true,
                    NbRows = nbRows + 1,
                    NbColumns = 7,
                    Data = rowData
                };

                return resultTable;
            }

            return null;
        }



      
    }
}
