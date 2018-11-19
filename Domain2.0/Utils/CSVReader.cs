using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace BitPlate.Domain.Utils
{
    
    public static class CsvReader 
    {

        /// <summary>
        /// Parses a csv line
        /// </summary>
        /// <param name="line">Line</param>
        public static string[] ParseLine(string line)
        {
            List<string> fields = new List<string>();
            StringBuilder columnBuilder = new StringBuilder();
            bool inColumn = false;
            bool inQuotes = false;
            
            // Iterate through every character in the line
            for (int i = 0; i < line.Length; i++)
            {
                char character = line[i];

                // If we are not currently inside a column
                if (!inColumn)
                {
                    // If the current character is a double quote then the column value is contained within
                    // double quotes, otherwise append the next character
                    if (character == '"')
                        inQuotes = true;
                    else if (character != ';')
                    {
                        columnBuilder.Append(character);
                        inColumn = true;
                    }
                    else
                    {
                        fields.Add(columnBuilder.ToString());
                        columnBuilder.Remove(0, columnBuilder.Length);
                        if ((i + 1) == line.Length)
                        {
                            inColumn = true;
                        }
                    }
                    
                    continue;
                }

                // If we are in between double quotes
                if (inQuotes)
                {
                    // If the current character is a double quote and the next character is a comma or we are at the end of the line
                    // we are now no longer within the column.
                    // Otherwise increment the loop counter as we are looking at an escaped double quote e.g. "" within a column
                    if (character == '"' && ((line.Length > (i + 1) && line[i + 1] == ';') || ((i + 1) == line.Length)))
                    {
                        inQuotes = false;
                        inColumn = false;
                        i++;
                    }
                    else if (character == '"' && line.Length > (i + 1) && line[i + 1] == '"')
                        i++;
                }
                else if (character == ';')
                    inColumn = false;

                // If we are no longer in the column clear the builder and add the columns to the list
                if (!inColumn)
                {
                    fields.Add(columnBuilder.ToString());
                    columnBuilder.Remove(0, columnBuilder.Length);
                }
                else // append the current column
                    columnBuilder.Append(character);
            }

            // If we are still inside a column add a new one
            if (inColumn)
                fields.Add(columnBuilder.ToString());
            return fields.ToArray();
        }
        
        

    }
}
