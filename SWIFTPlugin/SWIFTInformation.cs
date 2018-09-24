using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Kbg.NppPluginNET
{
    class SWIFTInformation
    {
        public Dictionary<string, string> swiftMessageFields = new Dictionary<string, string>
        {
            {":20", @"(:20:)\w\d{1,16}"},
            {":25", @":25:\w\d{1,35}"},
            {":28C", @":28C:([0-9]){1,5}\/*([0-9]){0,5}"},
            {":60A", @"(:60[A]:)[CD]([0-9]){6}([A-Z]){3}[0-9]*,[0-9]{0,2}"},
            {":60F", @"(:60[F]:)[CD]([0-9]){6}([A-Z]){3}[0-9]*,[0-9]{0,2}"},
            {":60M", @"(:60[M]:)[CD]([0-9]){6}([A-Z]){3}[0-9]*,[0-9]{0,2}"},
            {":61", @"(:61:)\w*\d*"},
            {":62A", @"(:62[A]:)[CD]([0-9]){6}([A-Z]){3}[0-9]*,[0-9]{0,2}"},
            {":62F", @"(:62[F]:)[CD]([0-9]){6}([A-Z]){3}[0-9]*,[0-9]{0,2}"},
            {":62M", @"(:62[M]:)[CD]([0-9]){6}([A-Z]){3}[0-9]*,[0-9]{0,2}"},
            {":64", "(:64:)[CD]([0-9]){6}([A-Z]){3}[0-9]*,[0-9]{0,2}"},
        };

        public Dictionary<string, string> swiftMessageNames = new Dictionary<string, string>
        {
            {":20", "transaction_reference_number"},
            {":25", "account_identification_statement"},
            {":28C", "statement_sequence_number"},
            {":60A", "opening_balance"},
            {":60F", "opening_balance"},
            {":60M", "opening_balance"},
            {":61", "statement_line"},
            {":62A", "closing_balance"},
            {":62F", "closing_balance"},
            {":62M", "closing_balance"},
            {":64", "closing_available_balance"},
        };

        public Dictionary<string, Dictionary<string, string>> swiftMessageSubFields = new Dictionary<string, Dictionary<string, string>>
        {
            {
                ":60F",
                new Dictionary<string, string>
                {
                    {"debitOrCredit", "(?<=60[FMA]:)[CD](?=[0-9])"},
                    {"date","(?<=[CD])[0-9]{6}(?=[A-Z])" },
                    {"currency", "(?<=[0-9])[A-Z]{3}(?=[0-9])" },
                    {"amount", "(?<=[A-Z]{0,3})[0-9]*,[0-9]{0,2}" },
                }
            }
        };

    }
}
