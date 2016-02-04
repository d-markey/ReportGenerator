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
using System.IO;

namespace Cast.Util
{
    /// <summary>
    /// 
    /// </summary>
    public static class PathUtil
    {

        #region Private METHODS
        
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fi"></param>
        public static string CreateTempCopy(string tempFolder, string templatePath)
        {
            if (!string.IsNullOrEmpty(tempFolder) && !Directory.Exists(tempFolder)) 
                Directory.CreateDirectory(tempFolder);


            
            string tempName = string.Format
                ("~{0}_{1}{2}"
                , Path.GetFileNameWithoutExtension(templatePath)
                , DateTime.Now.ToString("MMdd_HHmmss")
                , Path.GetExtension(templatePath)
                );

            string tempFile = Path.Combine(tempFolder, tempName);
            File.Copy(templatePath, tempFile);

            return tempFile;
        }

        public static string CreateTempCopyFlexi(string tempFolder, string templatePath)
        {
            if (!string.IsNullOrEmpty(tempFolder) && !Directory.Exists(tempFolder))
                Directory.CreateDirectory(tempFolder);



            string tempName = string.Format
                ("~{0}_{1}{2}"
                , Path.GetFileNameWithoutExtension(templatePath)
                , DateTime.Now.ToString("yyMMdd_HHmmss")
                , Path.GetExtension(templatePath)
                );

            string tempFile = Path.Combine(tempFolder, tempName);
            File.Copy(templatePath, tempFile);

            return tempFile;

        }

       
    }
}
