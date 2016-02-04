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
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CastReporting.Domain
{
    /// <summary>
    /// </summary>
    [DataContract(Name = "TechnologyResult")]
    public class TechnologyResult
    {

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "technology")]
        public string Technology { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "result")]
        public ResultDetail DetailResult { get; set; }


    }
}
