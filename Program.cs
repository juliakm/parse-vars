using System;
using System.Collections.Generic;
using System.IO;
using HtmlAgilityPack;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;


namespace parse_vars
{

    class Program
    {
        static void Main(string[] args)
        {
            // List of possible monikers
            // We support these moniker versions:
            List<string> monikers = new List<string>
            {
                "azure-devops",
                "azure-devops-2020",
                "azure-devops-2019",
                "tfs-2018",
                "tfs-2017",
                "tfs-2015"
               // "tfs-2013"
            };

            // Set up Dictionary
            // We have variables, each one has a name, and each supported monikered version some text
            // We can represent this with a Dictionary that maps variable name to the collection of
            // moniker to text mapping, using a Dictionary of a moniker to text dictionary, keyed by variable name

            Dictionary<string, Dictionary<string, string>> variables = new Dictionary<string, Dictionary<string, string>>();
            // The mapping is like this:
            // variable name -> azure-devops -> text
            //                  azure-devops-2020 -> text
            //                  azure-devops-2019 -> text
            // each variable can point to multiple monikers

            //define VarData
            Dictionary<string,string> VarData = new Dictionary<string, string>();

             // This is our "outer" loop, we will iterate the moniker list, and hit the page for that version
            foreach (string moniker in monikers)
            {

                // For each moniker version, hit the page and read in the variables
                // iterates through all of the values in the monikers List, regardless of whether there is a match
                string url = $"https://docs.microsoft.com/en-us/azure/devops/pipelines/build/variables?view={moniker}";

                // Prints out the URL pattern for each moniker in the List
    //            Console.WriteLine("Retrieving variables for {0}\r\n{1}", moniker, url);

                // Builds htmlDoc, object that holds a page and its values
                HtmlWeb web = new();
                var htmlDoc = web.Load(url);
                //var rows = htmlDoc.DocumentNode.SelectNodes("//*[@id='main']//tr"); //search for right moniker data-moniker

                //Console.WriteLine(moniker);
                var regex = $"//*[@id='main']//div[@data-moniker=\"{moniker}\"]//tr";
                //Console.WriteLine(regex);

                var rows = htmlDoc.DocumentNode.SelectNodes(regex);
              
                foreach (var row in rows)
                {
                    var nodes = row.SelectNodes("td"); //grabbing each <td> within a row
                    if (nodes != null) // if the <td> is not empty
                    {

                        // We will simulate a single variable here, Agent.BuildDirectory
                        //string name = "Agent.BuildDirectory";
                        //return text = $"This is the body text for Agent.BuildDirectory for {moniker}.";
                        var name = nodes[0].InnerText;
                        var text = nodes[1].InnerText;


                        //Console.WriteLine("{0}: {1}", moniker, name);

                        // if this is the first time, add to the dictionary
                        if (!variables.ContainsKey(name))
                        {
                            // Add it
                            variables.Add(name, new Dictionary<string, string>());

                        }

                        // if we know that the variable is there, then stuff in the moniker and this version of the text
                        Dictionary<string, string> details = variables[name];
                        details.Add(moniker, text); //add the moniker and text to details
                    }

                }

            }
            

            // Code should live outside of this
            //for each variable in the dictionary, write out its content
            // in the appropriate moniker blocks
            List<string> sortedVariables = variables.Keys.ToList<string>();
            sortedVariables.Sort();

            // Traverse the sorted list and use these keys to access the dictionary
            // Go though every variable (this includes all the variables by moniker, even the ones that don't exist for the moniker)
            foreach(string varName in sortedVariables)
            {
                // varName is the first variable, and varDetails is the dictionary mapping
                // its monikers to that version of the variable
                Dictionary<string, string> varDetails = variables[varName];
                StringBuilder monikerVersionsSB = new StringBuilder();
            
                // loop through the moniker instances 
                foreach (string monikerInstance in monikers)
                {
                    // If this variable contains an entry for this moniker, add it to the moniker string
                    if (varDetails.ContainsKey(monikerInstance)) // TODO: Fix association here
                    {
                        //Console.WriteLine(varDetails[monikerInstance]);
                        // If this isn't the first moniker, then append " || " to the version string 
                        if(monikerVersionsSB.Length > 0)
                        {
                            monikerVersionsSB.Append(" || ");
                        } 
                        // after you've (maybe) appended, add the moniker
                        monikerVersionsSB.Append(monikerInstance); 
                    }
                }
                // Write out some metadata including the moniker string
                Console.WriteLine("---");
                Console.WriteLine("title: {0}", varName);
                Console.WriteLine("monikerRange: '{0}'", monikerVersionsSB.ToString());
                Console.WriteLine("---\r\n");

                // // Write out some page content
                Console.WriteLine("# {0}\r\n", varName);

                Console.WriteLine("---\r\n");


        }


        }
      }
    }

