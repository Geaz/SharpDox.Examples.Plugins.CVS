﻿using SharpDox.Model;
using SharpDox.Model.Repository;
using SharpDox.Sdk.Exporter;
using System;
using System.IO;
using System.Linq;

namespace CSVExporter
{
    public class CSVExport : IExporter
    {
        /// <summary>
        /// sharpDox listens on this event to get any warnings.
        /// All messages will be shown in the build window.
        /// </summary>
        public event Action<string> OnRequirementsWarning;

        /// <summary>
        /// sharpDox listens on this event to get any status
        /// messages. All messages will be shown in the progressbar.
        /// </summary>
        public event Action<string> OnStepMessage;
        
        /// <summary>
        /// sharpDox listens on this event to get any status update.
        /// This event will move forward the secondary progressbar.
        /// </summary>
        public event Action<int> OnStepProgress;

        private readonly CSVConfig _csvConfig;

        public CSVExport(CSVConfig csvConfig)
        {
            // Get our new configuration
            _csvConfig = csvConfig;
        }

        // This function just returns true. No requirements to check for.
        public bool CheckRequirements()
        {
            return true;
        }

        // The export function gets the parsed solution and the output path.
        public void Export(SDProject sdProject, string outputPath)
        {
            var csv = string.Empty;
            var types = sdProject.Repositories.Values.Single().GetAllTypes().OfType<SDType>().Where(o => !o.IsProjectStranger);
            
            foreach (var type in types)
            {
                ExecuteOnStepMessage("Creating entry for " + type.Fullname);
                csv += string.Format("{1}{0}{2}{0}{3}", _csvConfig.Divider, type.Fullname, type.Name, type.Namespace) + System.Environment.NewLine;
            }

            File.WriteAllText(Path.Combine(outputPath, "methods.csv"), csv);
        }

        private void ExecuteOnStepMessage(string message)
        {
            var handler = OnStepMessage;
            if (handler != null)
            {
                handler(message);
            }
        }

        private void ExecuteOnStepProgress(int progress)
        {
            var handler = OnStepProgress;
            if (handler != null)
            {
                handler(progress);
            }
        }

        // The name of the exporter will be used to 
        // create a subdirectory in the output path.
        public string ExporterName { get { return "CSV"; } }
    }
}
