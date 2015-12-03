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
    [Block("TQI_BY_MODULE")]
    class TQIbyModule : TableBlock
    {
        /// <summary>
        /// 
        /// </summary>
        private const string _MetricFormat = "N2";


        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportData"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            bool isDisplayShortHeader = (options != null && options.ContainsKey("HEADER") && "SHORT" == options["HEADER"]);

            List<string> rowData = new List<string>();
            int count = 0;
            rowData.AddRange(isDisplayShortHeader
                                ? new[] { " ", "Cur TQI", "Prev TQI", "Var" }
                                : new[] { " ", "Current TQI", "Previous TQI", "Variation" });

            var resultCurrentSnapshot = BusinessCriteriaUtility.GetBusinessCriteriaGradesModules(reportData.CurrentSnapshot);
            var resultPreviousSnapshot = BusinessCriteriaUtility.GetBusinessCriteriaGradesModules(reportData.PreviousSnapshot);
            
            
            if (resultCurrentSnapshot != null )
            {

                if (resultPreviousSnapshot == null) resultPreviousSnapshot = new List<BusinessCriteriaDTO>();

                var results =  from current in resultCurrentSnapshot
                               join prev in resultPreviousSnapshot on current.Name equals prev.Name
                               into g
                               from subset in g.DefaultIfEmpty()
                               select new
                               {
                                   Name = current.Name,
                                   TqiCurrent = current.TQI.HasValue ? current.TQI : (double?)null,
                                   TqiPrevious = subset != null ? subset.TQI : (double?)null,
                                   PercentVariation =  subset != null ? MathUtility.GetVariationPercent(current.TQI, subset.TQI):null
                               };


                foreach (var result in results.OrderBy(_ => _.Name))
                {
                    
                    rowData.AddRange(new[] {
                            result.Name,
                            result.TqiCurrent.HasValue  ? result.TqiCurrent.Value.ToString(_MetricFormat) : CastReporting.Domain.Constants.No_Value,     
                            result.TqiPrevious.HasValue ? result.TqiPrevious.Value.ToString(_MetricFormat) : CastReporting.Domain.Constants.No_Value,     
                            result.PercentVariation.HasValue ? TableBlock.FormatPercent(result.PercentVariation):CastReporting.Domain.Constants.No_Value,
                        });
                }
                count = results.Count();
            }

           

            var resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = count + 1,
                NbColumns = 4,
                Data = rowData
            };

            return resultTable;
        }
    }
}
