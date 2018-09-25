using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SWIFT.HumanifyMessage
{
    class Utils
    {
        static SWIFTInformation swiftInfo = new SWIFTInformation();

        public string returnFirstStringMatch(string regexPattern, string inputString) {
            
            try
            {
                Regex regex = new Regex(regexPattern);
                string resultingMatch = regex.Match(inputString.ToString()).ToString();
                return resultingMatch;
            }
           catch
            {
                return "";
            }
            
        }

        public string buildSwiftSubFieldOutput(string swiftFieldInput, string machineFieldIdentified)
        /// <summary>
        /// Loops through the swift subfield dictionary, creating human readable formatting 
        /// and breaking up the unreadable format
        /// </summary>
        {
            string swiftModifiedFieldOutput = "\n";
            // try to match subfields, don't crash if subfields not found
            try
            {
                int messageSubFieldCount = swiftInfo.swiftMessageSubFields[machineFieldIdentified].Count();
                if (messageSubFieldCount > 0)
                {
                    foreach (var swiftSubFields in swiftInfo.swiftMessageSubFields[machineFieldIdentified])
                    {
                        // loop through subfields dictionary
                        // primary dict key = field main name ex :60F:
                        // primary dict value = secondary dict of subfields
                        // secondary dict key = secondary field english name
                        // secondary dict value = secondary dict regex 
                        string swiftSubFieldHumanName = swiftSubFields.Key;
                        string swiftSubFieldRegexValue = swiftSubFields.Value;

                        string subfieldValue = returnFirstStringMatch(swiftSubFieldRegexValue, swiftFieldInput);
                        swiftModifiedFieldOutput = swiftModifiedFieldOutput + "\t" + swiftSubFieldHumanName + ":" + subfieldValue + "\n";
                    }
                }
            }
            catch
            {
            }
            return swiftModifiedFieldOutput;

        }
    }
}
