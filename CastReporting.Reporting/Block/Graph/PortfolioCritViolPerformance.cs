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
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Languages;
using CastReporting.BLL.Computing;
using CastReporting.BLL.Computing.DTO;
using CastReporting.Domain;
using System.Globalization;
using System.Threading;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CastReporting.Reporting.Block.Graph
{
    [Block("PF_TREND_CRIT_VIOL")]
    class PortfolioCritViolPerformance : GraphBlock
    {
        private const string _MetricFormat = "N0";

        #region METHODS

        private static int GetQuarter(DateTime dt)
        {
            return (dt.Month / 4) + 1;
        }


        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            int metricId;
            #region Item BCID
            if (options == null ||
                !options.ContainsKey("BCID") ||
                !int.TryParse(options["BCID"], out metricId))
            {
                metricId = reportData.Parameter.NbResultDefault;
            }
            #endregion Item BCID

            var rowData = new List<String>();
            rowData.AddRange(new string[] {
				" ",
				Labels.ViolationsCritical + " - " + Labels.ViolationsRemoved ,
				Labels.ViolationsCritical + " - " + Labels.ViolationsAdded,
				Labels.ViolationsCritical + " - " + Labels.Total
			});

            DataTable dtDates = new DataTable();
            dtDates.Columns.Add("Quarter", typeof(int));
            dtDates.Columns.Add("Year", typeof(int));
            dtDates.Columns.Add("RemovedViol", typeof(double));
            dtDates.Columns.Add("AddedViol", typeof(double));
            dtDates.Columns.Add("TotalViol", typeof(double));
            dtDates.AcceptChanges();

            #region Fetch SnapshotsPF

            if (reportData != null && reportData.Applications != null && reportData.snapshots != null)
            {
                DateTime DateNow = DateTime.Now;
                //DateTime DateNow = Convert.ToDateTime("03 01 2014");
                Application[] AllApps = reportData.Applications;
                Snapshot[] AllSnapshots = reportData.snapshots;
                int generateQuater = 6;
                int currentYear = DateNow.Year;
                int currentQuater = GetQuarter(DateNow);
                for (int i = generateQuater; i > 0; i--)
                {
                    DataRow dr = dtDates.NewRow();
                    dr["Quarter"] = currentQuater;
                    dr["Year"] = currentYear;
                    dtDates.Rows.InsertAt(dr, 0);
                    //dtDates.Rows.Add(currentQuater, currentYear);
                    if (--currentQuater == 0)
                    {
                        currentQuater = 4;
                        currentYear--;
                    }
                }

                double? RemovedViol = 0;
                double? AddedViol = 0;
                double? TotalViol = 0;

                for (int i = 0; i < dtDates.Rows.Count; i++)
                {
                    RemovedViol = 0;
                    AddedViol = 0;
                    TotalViol = 0;

                    if (AllSnapshots.Count() > 0)
                    {
                        foreach (Snapshot snapshot in AllSnapshots.OrderBy(_ => _.Annotation.Date.DateSnapShot))
                        {
                            DateTime SnapshotDate = Convert.ToDateTime(snapshot.Annotation.Date.DateSnapShot.Value);
                            int intQuarter = Convert.ToInt32(dtDates.Rows[i]["Quarter"]);
                            int intYear = Convert.ToInt32(dtDates.Rows[i]["Year"]);

                            int intSnapshotQuarter = GetQuarter(SnapshotDate);
                            int intSnapshotYear = SnapshotDate.Year;

                            if (intQuarter == intSnapshotQuarter && intYear == intSnapshotYear)
                            {
                                var results = RulesViolationUtility.GetStatViolation(snapshot);
                                foreach (var resultModule in results)
                                {
                                    if (resultModule[(Constants.BusinessCriteria)metricId] != null)
                                    {
                                        int CriticalViolThisModulePerformanceTotal = (resultModule != null && resultModule[(Constants.BusinessCriteria)metricId].Total.HasValue) ?
                              resultModule[(Constants.BusinessCriteria)metricId].Total.Value : 0;

                                        int CriticalViolThisModulePerformanceAdded = (resultModule != null && resultModule[(Constants.BusinessCriteria)metricId].Added.HasValue) ?
                              resultModule[(Constants.BusinessCriteria)metricId].Added.Value : 0;

                                        int CriticalViolThisModulePerformanceRemoved = (resultModule != null && resultModule[(Constants.BusinessCriteria)metricId].Removed.HasValue) ?
                              resultModule[(Constants.BusinessCriteria)metricId].Removed.Value : 0;

                                        RemovedViol = RemovedViol + CriticalViolThisModulePerformanceRemoved;
                                        AddedViol = AddedViol + CriticalViolThisModulePerformanceAdded;
                                        TotalViol = TotalViol + CriticalViolThisModulePerformanceTotal;
                                    }

                                }
                            }

                        }
                    }

                    if (RemovedViol > 0)
                    {
                        RemovedViol = RemovedViol * -1;
                    }

                    dtDates.Rows[i]["RemovedViol"] = RemovedViol;
                    dtDates.Rows[i]["AddedViol"] = AddedViol;
                    dtDates.Rows[i]["TotalViol"] = TotalViol;
                }

                for (int i = 0; i < dtDates.Rows.Count; i++)
                {
                    string strQuarter = dtDates.Rows[i]["Year"].ToString() + " Q" + dtDates.Rows[i]["Quarter"].ToString();
                    rowData.AddRange(new string[] {
                                                    strQuarter,
                                                    dtDates.Rows[i]["RemovedViol"].ToString(),
                                                    dtDates.Rows[i]["AddedViol"].ToString(),
                                                    dtDates.Rows[i]["TotalViol"].ToString(),
                                                });
                }

            }
            #endregion Fetch SnapshotsPF






            TableDefinition resultTable = new TableDefinition
            {
                HasRowHeaders = true,
                HasColumnHeaders = false,
                NbRows = dtDates.Rows.Count + 1,
                NbColumns = 4,
                Data = rowData,
                GraphOptions = null
            };
            return resultTable;
        }

        #endregion METHODS
    }
}
