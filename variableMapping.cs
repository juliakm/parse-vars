using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace parse_vars
{
    class MapVariables
    {

        public static void Mapper(string[] args)
        {
            // We support these moniker versions:
            List<string> monikers = new List<string>
            {
                "azure-devops",
                "azure-devops-2020",
                "azure-devops-2019",
                "tfs-2018",
                "tfs-2017",
                "tfs-2015",
                "tfs-2013"
            };

            // We have variables, each one has a name, and each supported monikered version some text
            // We can represent this with a Dictionary that maps variable name to the collection of
            // moniker to text mapping, using a Dictionary of a moniker to text dictionary, keyed by variable name

            Dictionary<string, Dictionary<string, string>> variables = new Dictionary<string, Dictionary<string, string>>();
            // The mapping is like this:
            // variable name -> azure-devops -> text
            //                  azure-devops-2020 -> text
            //                  azure-devops-2019 -> text



            // This is our "outer" loop, we will iterate the moniker list, and hit the page for that version
            foreach (string moniker in monikers)
            {
                // For each moniker version, hit the page and read in the variables.
                string url = $"https://docs.microsoft.com/en-us/azure/devops/pipelines/build/variables?view={moniker}";
                Console.WriteLine("Retrieving variables for {0}\r\n{1}", moniker, url);


                // We will simulate a single variable here, Agent.BuildDirectory
                string name = "Agent.BuildDirectory";
                string text = $"This is the body text for Agent.BuildDirectory for {moniker}.";

                // If we have encountered this variable for the first time
                // it won't have an entry in the dictionary, so add it
                if (!variables.ContainsKey(name))
                {
                    // Add it
                    variables.Add(name, new Dictionary<string, string>());
                }

                // Retrieve the moniker->text dictionary for this variable so we can add the details
                Dictionary<string, string> details = variables[name];

                // Add in the details for this moniker. It doesn't need to be a string, it
                // could be a class with different properties etc. but in this sample it is a string
                details.Add(moniker, text);

              // In this example we are simulating that this variable is only
              // for azure-devops, azure-devops-2020, azure-devops-2019
                if (string.Compare(moniker, "azure-devops-2019", true) == 0)
                {
                    // Break out of this foreach loop after we "read" the azure-devops-2019
                    // version of this variable
                    break;
                }
            }



            // OK now we are at the end, for each variable in the dictionary, write out its content
            // in the appropriate moniker blocks
            // If we want to do it alphabetically, for example we are also going to build
            // a table out of it or auto-gen the toc, we can do it by creating a List<string>
            // of the dictionary's keys, sorting it, and then traversing the List<string>
            // and use the values to access the dictionary using our desired order.
            // (there are more complex ways but here is the easiest aka the one I know how to do :) )
            List<string> sortedVariables = variables.Keys.ToList<string>();
            sortedVariables.Sort();

            // Traverse the sorted list and use these keys to access the dictionary

            foreach (string varName in sortedVariables)
            {
                // varName is the first variable, and varDetails is the dictionary mapping
                // its monikers to that version of the variable

                Dictionary<string, string> varDetails = variables[varName];

                // Build the topic level moniker string (I've never used this syntax but it is in
                // the internal docs :) )

                StringBuilder monikerVersionsSB = new StringBuilder();
                foreach (string moniker in monikers)
                {
                    // If this variable contains an entry for this moniker, add it to the moniker string
                    if (varDetails.ContainsKey(moniker))
                    {
                        // If this isn't the first moniker, then append " || "
                        if (monikerVersionsSB.Length > 0)
                        {
                            monikerVersionsSB.Append(" || ");
                        }

                        monikerVersionsSB.Append(moniker);
                    }
                }



                // Write out some metadata including the moniker string
                Console.WriteLine("---");
                Console.WriteLine("title: {0}", varName);
                Console.WriteLine("monikerRange: '{0}'", monikerVersionsSB.ToString());
                Console.WriteLine("---\r\n");


                // Write out some page content
                Console.WriteLine("# {0}\r\n", varName);



                // Let's check each of our monikers to see if that version is present
                // We will put the newest version first, so to do that we'll iterate our
                // moniker list to check and see if it's present in the dictionary. We could
                // just iterate the keys of this variable's moniker->text dictionary but the
                // order isn't guaranteed. If we didn't care about the order we could do this:
               /*
                // Note that coincidentally this version is in order but that is not guaranteed
                foreach(KeyValuePair<string, string> item in varDetails)
                {
                    Console.WriteLine("::: moniker range=\"{0}\"", item.Key);
                    Console.WriteLine();
                    Console.WriteLine(item.Value);
                    Console.WriteLine();
                    Console.WriteLine("::: moniker-end");
                    Console.WriteLine();
                }

                */


                // Since in this example we want the newest stuff first, we do it like this
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

                    }

                }

            }



        }

        internal static void Mapper(string moniker)
        {
            throw new NotImplementedException();
        }
    }

}

 