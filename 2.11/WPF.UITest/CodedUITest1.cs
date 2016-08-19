using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Microsoft.VisualStudio.TestTools.UITesting.WpfControls;
using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;


namespace WPF.UITest
{
    /// <summary>
    /// Summary description for CodedUITest1
    /// </summary>
    [CodedUITest]
    public class CodedUITest1
    {
        private static BrowserWindow browser;

        private static void LogInAsDefaultUser()
        {
            const string currentVersionLink = "http://localhost/wpf.xbap";

            //const string userLogin = "tu";
            //const string userPassword = "1234";

            browser = BrowserWindow.Launch(currentVersionLink);
            browser.CloseOnPlaybackCleanup = false;

            //WpfEdit loginTextBox = new WpfEdit(browser);
            //loginTextBox.SearchProperties[WpfControl.PropertyNames.AutomationId] = "txtUserName";
            //Keyboard.SendKeys(loginTextBox, userLogin);

            //WpfEdit passwordTextBox = new WpfEdit(browser);
            //passwordTextBox.SearchProperties[WpfControl.PropertyNames.AutomationId] = "txtPassword";
            //Keyboard.SendKeys(passwordTextBox, userPassword);

            //WpfButton loginButton = new WpfButton(browser);
            //loginButton.SearchProperties[WpfControl.PropertyNames.AutomationId] = "btnLogin";
            //Mouse.Click(loginButton);
        }

        [TestMethod]
        public void CreatePromotion()
        {
        }

        //[TestMethod]
        //public void CreatingPromo()
        //{

        //    //Playback.Initialize();

        //    try
        //    {
        //        MyCodedUITests.SpeedUpTests();
        //        LogInAsDefaultUser();
        //    }
        //    finally
        //    {
        //        Playback.Cleanup();
        //    }

        //    Playback.Wait(10000);

        //    WpfButton newPromoButton = new WpfButton(browser);
        //    newPromoButton.SearchProperties[WpfControl.PropertyNames.AutomationId] = "btnAddPromotion";
        //    Mouse.Click(newPromoButton);
        //}

        #region Additional test attributes

        // You can use the following additional attributes as you write your tests:

        //Use ClassInitialize to run code once before all tests
        [ClassInitialize]
        public static void MyClassInitialize(TestContext context)
        {
            Playback.Initialize();
            MyCodedUITests.SpeedUpTests();

            try
            {
                LogInAsDefaultUser();
            }
            finally
            {
                Playback.Cleanup();
            }
        }

        //Use ClassCleanup to run code once after all tests have run
        [ClassCleanup]
        public static void MyClassCleanup()
        {
            browser.Close();
        }

        //Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
        }

        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
        }

        #endregion

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
        private TestContext testContextInstance;

        //public UIMap UIMap
        //{
        //    get
        //    {
        //        if ((map == null))
        //        {
        //            map = new UIMap();
        //        }

        //        return map;
        //    }
        //}

        //private UIMap map;
    }

    internal class MyCodedUITests
    {
        /// <summary> Test startup. </summary>
        public static void SpeedUpTests()
        {
            // Configure the playback engine
            Playback.PlaybackSettings.SearchTimeout = 1000;
            Playback.PlaybackSettings.ShouldSearchFailFast = true;
            Playback.PlaybackSettings.WaitForReadyLevel = WaitForReadyLevel.UIThreadOnly;
            Playback.PlaybackSettings.WaitForReadyTimeout = 5000;
            Playback.PlaybackSettings.SmartMatchOptions = SmartMatchOptions.TopLevelWindow;
            Playback.PlaybackSettings.MatchExactHierarchy = false;

            //Playback.PlaybackSettings.WaitForReadyLevel = WaitForReadyLevel.Disabled;
            //Playback.PlaybackSettings.MaximumRetryCount = 10;
            //Playback.PlaybackSettings.ShouldSearchFailFast = false;
            //Playback.PlaybackSettings.DelayBetweenActions = 500;
            //Playback.PlaybackSettings.SearchTimeout = 1000;

            // Add the error handler
            Playback.PlaybackError -= Playback_PlaybackError; // Remove the handler if it's already added
            Playback.PlaybackError += Playback_PlaybackError; // Ta dah...
        }

        /// <summary> PlaybackError event handler. </summary>
        private static void Playback_PlaybackError(object sender, PlaybackErrorEventArgs e)
        {
            // Wait a second
            System.Threading.Thread.Sleep(100);

            // Retry the failed test operation
            e.Result = PlaybackErrorOptions.Retry;
        }
    }
}
