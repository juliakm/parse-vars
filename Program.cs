using System;
using System.Collections.Generic;
using System.IO;
using HtmlAgilityPack;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using BFound.HtmlToMarkdown;

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

            // Clear out file we will work with later
               File.WriteAllText("WriteVars.txt", String.Empty);

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

                var regex = $"//*[@id='main']//div[@data-moniker=\"{moniker}\"]//tr";
                var rows = htmlDoc.DocumentNode.SelectNodes(regex);
              
                foreach (var row in rows)
                {
                    var nodes = row.SelectNodes("td"); //grabbing each <td> within a row
                    if (nodes != null) // if the <td> is not empty
                    {
                        //Grab each table data cell
                        var name = nodes[0].InnerText;
                        var textTemp = nodes[1].InnerHtml; //InnerHtml will preserve HTML. Need to do Markdown conversion 
                        // TODO: Ask Mike 

                        var markdown = MarkDownDocument.FromHtml(textTemp);
                        var text = markdown;



                        // Debugging to make sure there aren't multiple loops
                        //Console.WriteLine("{0}: {1}", moniker, name);

                        // if this is the first time, add to the dictionary
                        if (!variables.ContainsKey(name))
                        {
                            // Add it
                            variables.Add(name, new Dictionary<string, string>());

                        }

                        Dictionary<string, string> details = variables[name];
                        details.Add(moniker, text); //add the moniker and text to details
                    }

                }

            }

            //for each variable in the dictionary, write out its content
            // in the appropriate moniker blocks
            variables.Keys.ToList().Sort();

            // Traverse the sorted list and use these keys to access the dictionary
            // Go though every variable (this includes all the variables by moniker, even the ones that don't exist for the moniker)
            foreach(string varName in variables.Keys.ToList())
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

            // Create the file
           // StreamWriter w = new StreamWriter("WriteVars.txt", true);
            using (StreamWriter w = new StreamWriter("WriteVars.txt", true)) 
            {
                // Write to Console
                // Write out some metadata including the moniker string
                Console.WriteLine("---");
                Console.WriteLine("title: {0}", varName);
                Console.WriteLine("monikerRange: '{0}'", monikerVersionsSB.ToString());
                Console.WriteLine("---\r\n");

                // // Write out some page content
                Console.WriteLine("# {0}\r\n", varName);
                Console.WriteLine("---\r\n");


                // Write to file
                // Write out some metadata including the moniker string
                w.WriteLine("---");
                w.WriteLine("title: {0}", varName);
                w.WriteLine("monikerRange: '{0}'", monikerVersionsSB.ToString());
                w.WriteLine("---\r\n");

                // // Write out some page content
                w.WriteLine("# {0}\r\n", varName);
                w.WriteLine("---\r\n");

                foreach (string moniker in monikers)
                {
                    if (varDetails.ContainsKey(moniker))
                    {
                        // We have content for this moniker
                        Console.WriteLine("::: moniker range=\"{0}\"", moniker);
                        Console.WriteLine();
                        Console.WriteLine(varDetails[moniker]);
                        Console.WriteLine();
                        Console.WriteLine("::: moniker-end");
                        Console.WriteLine();

                        w.WriteLine("::: moniker range=\"{0}\"", moniker);
                        w.WriteLine();
                        w.WriteLine(varDetails[moniker]);
                        w.WriteLine();
                        w.WriteLine("::: moniker-end");
                        w.WriteLine();


                        //Write to file
                    }
                }
                }


            }


        }
      }
    }

