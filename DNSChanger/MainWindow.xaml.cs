using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace DNSChanger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The link to the author's GitHub profile page
        /// </summary>
        private static string GITHUB_PROFILE_LINK = "https://github.com/luisbraganca";

        /// <summary>
        /// A raw text website with the DNS IP
        /// </summary>
        private static string DNS_LINK = "https://gist.githubusercontent.com/luisbraganca/1c756ab03c94ce49f60be89092f28c0b/raw/2f7bbf59ed80cd499c5873deedea655a206c09e9/opendns.txt";

        /// <summary>
        /// The name of the temporary .bat file with the batch script to change the DNS
        /// this file will be created, used, then deleted when no longer necessary
        /// </summary>
        private static string CHANGE_FILE_NAME = "change.bat";

        /// <summary>
        /// The name of the temporary .bat file with the batch script to reset the DNS
        /// this file will be created, used, then deleted when no longer necessary
        /// </summary>
        private static string RESET_FILE_NAME = "reset.bat";

        /// <summary>
        /// The name of the temporary .bat file with the batch script to list all the network
        /// interfaces into the .txt file (NETWORK_INTERFACES_LIST variable).
        /// this file will be created, used, then deleted when no longer necessary
        /// </summary>
        private static string LIST_ALL_NETWORK_INTERFACES_FILE_NAME = "list_all_interfaces.bat";

        /// <summary>
        /// The name of the temporary .txt file with the list of all the network interfaces
        /// this file will be created, used, then deleted when no longer necessary
        /// </summary>
        private static string NETWORK_INTERFACES_RESULTS_FILE_NAME = "net_interfaces_results.txt";

        /// <summary>
        /// The batch command to list all the network interfaces
        /// </summary>
        private static string CMD_LIST_ALL_NET_INFERFACES = "netsh interface show interface";

        /// <summary>
        /// The error to be shown when the user selected a browser that it isn't installed on the device
        /// </summary>
        private static string BROWSER_ERROR_MESSAGE = "Error: <BROWSER> doesn't seem to be installed on this device. You can always start it manually as the DNS was changed anyway.";

        /// <summary>
        /// A generic error message when an unknown exception is thrown.
        /// </summary>
        private static string UNKNOWN_ERROR = "Some unknown error ocurred, please visit our GitHub page and write an issue with the following message: ";

        /// <summary>
        /// Class property holding the loaded DNS on start
        /// </summary>
        private string dns;

        public MainWindow()
        {
            InitializeComponent();
            Start();
            changeButton.IsEnabled = true;
            resetButton.IsEnabled = true;
        }

        /// <summary>
        /// Loads the DNS
        /// </summary>
        private void Start()
        {
            Log("Recieving DNS...");
            string dnsIP = DownloadLinkData(DNS_LINK, "Failed", "Failed retrieving DNS, check your internet connection.");
            Log("Data recieved. Verifying DNS...");
            if (!IsValidDns(dnsIP))
            {
                Error("Invalid DNS", "Invalid DNS.");
            } else
            {
                Success("Recieved DNS is valid, ready to change.");
                dns = dnsIP;
            }
        }

        /// <summary>
        /// Downloads all the data from a website, it's recommended to use a website
        /// with raw text
        /// </summary>
        /// <param name="url">The url from which the data will be downloaded</param>
        /// <param name="errorMessage">The error message to be shown in case of an unreachable website
        /// either do conectivity failure or URL not reachable</param>
        /// <param name="errorDescription">A more detailed message from that same error</param>
        /// <returns>The loaded data from the website</returns>
        private string DownloadLinkData(string url, string errorMessage, string errorDescription)
        {
            WebClient wc = new WebClient();
            Byte[] response = null;
            try
            {
                response = wc.DownloadData(url);
            }
            catch (WebException)
            {
                Error(errorMessage);
            }
            return Encoding.ASCII.GetString(response);
        }

        /// <summary>
        /// Appends a message on the RichTextArea on the User Interface, along with the time
        /// </summary>
        /// <param name="message"></param>
        private void Log(string message)
        {
            statusTextBox.Background = Brushes.Black;
            statusTextBox.AppendText(GetTime() + message + "\n");
            statusTextBox.ScrollToEnd();
        }

        /// <summary>
        /// Appends a message on the RichTextArea on the User Interface, leaves it Red,
        /// shows the error on a new window, and exits the application
        /// </summary>
        /// <param name="message">The message to be shown</param>
        private void Error(string message)
        {
            SafeDelete(CHANGE_FILE_NAME);
            SafeDelete(LIST_ALL_NETWORK_INTERFACES_FILE_NAME);
            SafeDelete(NETWORK_INTERFACES_RESULTS_FILE_NAME);
            SafeDelete(RESET_FILE_NAME);
            Error(message, message);
        }

        /// <summary>
        /// Appends a message on the RichTextArea on the User Interface, leaves it Red,
        /// shows the error on a new window, and exits the application
        /// </summary>
        /// <param name="message">The message to be shown</param>
        /// <param name="description">A more detailed message from that same error</param>
        private void Error(string message, string description)
        {
            Log(message);
            statusTextBox.Background = Brushes.DarkRed;
            MessageBox.Show(description);
            Environment.Exit(0);
        }

        /// <summary>
        /// Appends a message on the RichTextArea on the User Interface and leaves it Green
        /// </summary>
        /// <param name="message">The message to be shown</param>
        private void Success(string message)
        {
            Log(message);
            statusTextBox.Background = Brushes.DarkGreen;
        }

        /// <summary>
        /// Pings an IP address
        /// </summary>
        /// <param name="address">The address</param>
        /// <returns>True if it's reachable, false otherwise</returns>
        private static bool PingAddress(string address)
        {
            try
            {
                return (new Ping().Send(address).Status == IPStatus.Success);
            }
            catch (PingException)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if a DNS is valid and reachable
        /// </summary>
        /// <param name="receivedDNS">string with the DNS</param>
        /// <returns>true if the DNS is valid and reachable, false otherwise</returns>
        private bool IsValidDns(string receivedDNS)
        {
            string[] octets = receivedDNS.Split('.');
            if (octets.Length != 4)
            {
                return false;
            }
            for(int i = 0; i < octets.Length; i++)
            {
                int integerOctet;
                try
                {
                    integerOctet = Int32.Parse(octets[i]);
                }
                catch (Exception)
                {
                    return false;
                }
                if (integerOctet < 0 || integerOctet > 255)
                {
                    return false;
                }
            }
            if (octets[0] + "." + octets[1] + "." + octets[2] + "." + octets[3] == receivedDNS)
            {
                return PingAddress(receivedDNS);
            }
            return false;
        }

        /// <summary>
        /// Gets a String with time on the [HH:mm:ss] format, used on Log
        /// </summary>
        /// <returns>A string with time</returns>
        private string GetTime()
        {
            return DateTime.Now.ToString("[HH:mm:ss] ");
        }

        /// <summary>
        /// Change button event
        /// TODO: Increase browser compatibility
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeButtonClick(object sender, EventArgs e)
        {
            changeButton.IsEnabled = false;
            try
            {
                Log("Changing DNS...");
                string[] netInterfaces = GetAllNetworkInterfaces();
                File.WriteAllLines(CHANGE_FILE_NAME, GetChangeDnsBatchScript(netInterfaces));
                File.SetAttributes(CHANGE_FILE_NAME, FileAttributes.Hidden);
                System.Diagnostics.Process.Start(CHANGE_FILE_NAME).WaitForExit();
                SafeDelete(CHANGE_FILE_NAME);
                Success("DNS changed succesfully.");

                if (startCheckBox.IsChecked == true)
                {
                    if (chromeRadioButton.IsChecked == true)
                    {
                        try
                        {   // Start Chrome in incognito mode
                            System.Diagnostics.Process.Start(@"chrome.exe", "--incognito http://google.com");
                        }
                        catch (Exception) { BrowserError(BROWSER_ERROR_MESSAGE.Replace("<BROWSER>", "Chrome")); }
                    }
                    else if (firefoxRadioButton.IsChecked == true)
                    {
                        try
                        {   // Start Firefox in incognito mode
                            System.Diagnostics.Process.Start(@"firefox.exe", "-private-window http://google.com");
                        }
                        catch (Exception) { BrowserError(BROWSER_ERROR_MESSAGE.Replace("<BROWSER>", "Firefox")); }
                    } 
                    //else if (operaRadioButton.IsChecked == true)
                    //{
                    //    try
                    //    {
                    //        // Some code here
                    //    }
                    //    catch (Exception) { BrowserError(BROWSER_ERROR_MESSAGE.Replace("<BROWSER>", "Opera")); }
                    //}
                }

            }
            catch (Exception exception)
            {
                Error(UNKNOWN_ERROR + exception.Message);
            }
            changeButton.IsEnabled = true;
        }

        /// <summary>
        /// What happens when a browser fails to start
        /// </summary>
        /// <param name="error">String with the error to be shown</param>
        private void BrowserError(string error)
        {
            MessageBox.Show(error);
            startCheckBox.IsEnabled = false;
        }

        /// <summary>
        /// Gets the batch script to change the DNS of all the recieved network interfaces
        /// TODO: Also set the alternative (secondary) DNS, not just the primary
        /// </summary>
        /// <param name="netInferfaces">All the network interfaces to change the DNS</param>
        /// <returns>The script, one line per index</returns>
        private string[] GetChangeDnsBatchScript(string[] netInferfaces)
        {
            IList<string> unblock;
            unblock = new List<string>();
            unblock.Add("@echo off");
            for (int i = 0; i < netInferfaces.Length; i++)
            {
                unblock.Add("netsh interface ipv4 set dns name=\"" + netInferfaces[i] + "\" static " + dns + " primary");
            }
            // unblock.Add("pause"); // mostly for debug purposes
            return unblock.ToArray();
        }

        /// <summary>
        /// Gets all the network interfaces on the device
        /// TODO: Try to simplify the method
        /// </summary>
        /// <returns>An array with a network interface per index</returns>
        private static string[] GetAllNetworkInterfaces()
        {
            IList<string> netInterfaces = new List<string>();
            // Create temp .bat file to run "netsh interface show interface > net_interfaces_results.txt" on cmd
            File.WriteAllLines(LIST_ALL_NETWORK_INTERFACES_FILE_NAME, new string[]{
                "@echo off",
                CMD_LIST_ALL_NET_INFERFACES + " > " + NETWORK_INTERFACES_RESULTS_FILE_NAME});
            File.SetAttributes(LIST_ALL_NETWORK_INTERFACES_FILE_NAME, FileAttributes.Hidden);
            System.Diagnostics.Process.Start(LIST_ALL_NETWORK_INTERFACES_FILE_NAME).WaitForExit();
            // Delete the temp file after it's executed
            SafeDelete(LIST_ALL_NETWORK_INTERFACES_FILE_NAME);
            // Load the list with all the network interfaces
            string[] output = File.ReadAllLines(NETWORK_INTERFACES_RESULTS_FILE_NAME);
            SafeDelete(NETWORK_INTERFACES_RESULTS_FILE_NAME); // We already have the strings on RAM so it's safe
            // to delete the file.
            // First 2 lines are: Blank line and Column Name so we can start from third
            for (int i = 3; i < output.Length; i++)
            {
                string[] lineParts = output[i].Split(' '); // To get all the elements separated between white spaces
                // However this array still saves a bunch of empty strings so we remove them on the line below
                lineParts = lineParts.Where(a => !String.IsNullOrEmpty(a)).ToArray();
                // Now we start building a string with the network interface name
                // you might find this is useless but remember that some network interface names
                // might have spaces in them so we need to join all the parts, just like before
                // the first 2 results aren't needed so we can start from the third place again
                string result = "";
                for (int j = 3; j < lineParts.Length; j++)
                {
                    result += lineParts[j] + " ";
                }
                result = result.Trim(); // to remove the last white space
                if (!String.IsNullOrEmpty(result)) // to avoid blank lines, specially the last one
                //that is written on the file
                {
                    netInterfaces.Add(result.Trim());
                }
            }
            return netInterfaces.ToArray();
        }

        /// <summary>
        /// Gets the batch script to reset the DNS of all the recieved network interfaces
        /// TODO: Also reset the alternative (secondary) DNS, not just the primary
        /// </summary>
        /// <param name="netInferfaces">All the network interfaces to reset the DNS</param>
        /// <returns>The script, one line per index</returns>
        private string[] GetResetDnsBatchScript(string[] netInterfaces)
        {
            IList<string> reset = new List<string>();
            reset.Add("@echo off");
            for (int i = 0; i < netInterfaces.Length; i++)
            {
                reset.Add("netsh interface ip set dns name=\"" + netInterfaces[i] + "\" source=dhcp");
            }
            // reset.Add("pause"); // mostly for debug purposes
            return reset.ToArray();
        }

        /// <summary>
        /// Reset button event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetButtonClick(object sender, EventArgs e)
        {
            resetButton.IsEnabled = false;
            try
            {
                Log("Resetting...");
                string[] netInterfaces = GetAllNetworkInterfaces();
                File.WriteAllLines(RESET_FILE_NAME, GetResetDnsBatchScript(netInterfaces));
                File.SetAttributes(RESET_FILE_NAME, FileAttributes.Hidden);
                System.Diagnostics.Process.Start(RESET_FILE_NAME).WaitForExit();
                SafeDelete(RESET_FILE_NAME);
                Success("All DNS were set to default.");
            }
            catch (Exception exception)
            {
                Error(UNKNOWN_ERROR + exception.Message);
            }
            resetButton.IsEnabled = true;
        }

        /// <summary>
        /// Safely deletes a file without crashing the application
        /// even if some exception is thrown
        /// </summary>
        /// <param name="fileName">Name of the file to delete</param>
        private static void SafeDelete(string fileName)
        {
            try
            {
                File.Delete(fileName);
            }
            catch (Exception) { };
        }

        /// <summary>
        /// Browser startup check box click event, basically
        /// enables the browser radio buttons if the checkbox is activated,
        /// or disables them otherwise
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartCheckBoxClick(object sender, RoutedEventArgs e)
        {
            chromeRadioButton.IsEnabled = startCheckBox.IsChecked == true? true : false;
            firefoxRadioButton.IsEnabled = startCheckBox.IsChecked == true ? true : false;
        }

        /// <summary>
        /// Author label click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AuthorLabelClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(GITHUB_PROFILE_LINK);
        }

        /// <summary>
        /// Help button event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HelpButtonClick(object sender, EventArgs e)
        {
            MessageBox.Show("It's highly advisable to:\n" +
                "- Use incognito/anonymous mode on the first use after you change your DNS;\n" +
                "- In case something's wrong, try:\n" +
                "    - Changing your wireless network interface's name to \"Wi-Fi\"\n" + 
                "    - Changing your wired network interface's name to \"Ethernet\"\n" +
                "      (Control Panel > Network and Internet > Network Connections),\n" +
                "    Some unusual characters MIGHT cause problems (like \"ã\" or \"ç\");\n" +
                "- If you're still facing some problems, don't hesitate to drop an issue message on GitHub:\n" +
                "    https://github.com/luisbraganca/dns-changer"
            );
        }
    }
}
