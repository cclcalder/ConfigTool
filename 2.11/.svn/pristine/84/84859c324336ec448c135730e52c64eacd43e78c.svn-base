using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Model.Utilities
{
    public static class Web
    {
        /// <summary>
        /// Sends a get request to the specified url for the specified browser control.
        /// Args (if any) are concatenated to the url.
        /// The user's session id is automatically concatenated to the url as well.
        /// </summary>
        public static void Get(WebBrowser browser, string url, Dictionary<string, string> args = null)
        {
            if (args != null && args.Any())
                url += "&" + JoinArguments(args);

            browser.Navigate(url);
        }

        /// <summary>
        /// Creates url basing on baseUrl, controller and action.
        /// Sends a get request to the created url for the specified browser control.
        /// Args (if any) are concatenated to the url.
        /// The user's session id is automatically concatenated to the url as well.
        /// </summary>
        public static void Get(WebBrowser browser, string baseUrl, string controller = null, string action = null, Dictionary<string, string> args = null)
        {
            Get(browser, CreateUrl(baseUrl, controller, action), args);
        }

        /// <summary>
        /// Sends a post request to the specified url for the specified browser control.
        /// Args (if any) are contained in post data.
        /// The user's session id is automatically concatenated to the url.
        /// </summary>
        public static void Post(WebBrowser browser, string url, Dictionary<string, string> args = null)
        {
            ASCIIEncoding encode = new ASCIIEncoding();

            var joinedArguments = JoinArguments(args);

            // Even if an action doesn't expect any arguments we must send something in post. Without doing that we won't invoke the action.
            // That's why we're sending "default", even if it doesn't mean anything.
            if (string.IsNullOrEmpty(joinedArguments))
                joinedArguments = "default";

            byte[] post = encode.GetBytes(joinedArguments);

            browser.Navigate(url, string.Empty, post, "Content-Type: application/x-www-form-urlencoded");
        }

        /// <summary>
        /// Creates url basing on baseUrl, controller and action.
        /// Sends a post request to the created url for the specified browser control.
        /// Args (if any) are contained in post data.
        /// The user's session id is automatically concatenated to the url.
        /// </summary>
        public static void Post(WebBrowser browser, string baseUrl, string controller = null, string action = null, Dictionary<string, string> args = null)
        {
            Post(browser, CreateUrl(baseUrl, controller, action), args);
        }

        private static string SessionIdArg
        {
            get { return "sessionID=" + User.CurrentUser.Session; }
        }

        private static string CreateUrl(string baseUrl, string controller = null, string action = null)
        {
            var url = baseUrl;

            if (!string.IsNullOrEmpty(controller))
                url += "/" + controller;

            if (!string.IsNullOrEmpty(action))
                url += "/" + action;

            url += "?" + SessionIdArg;

            return url;
        }

        private static string JoinArguments(Dictionary<string, string> args)
        {
            var arguments = string.Empty;

            if (args != null)
            {
                foreach (var arg in args)
                {
                    arguments += arg.Key + "=" + arg.Value;
                    arguments += "&";
                }

                // Remove the last "&"
                arguments = arguments.Remove(arguments.Length - 1, 1);
            }

            return arguments;
        }
    }

    public delegate void WebNavigatedEventHandler(string controller, string action, Dictionary<string, string> args);
}