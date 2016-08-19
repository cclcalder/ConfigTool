using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStack.White.InputDevices;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TreeItems;
using TestStack.White.UIItems.WindowStripControls;
using TestStack.White.UIItems.WPFUIItems;
using TestStack.White.WebBrowser;
using TestStack.White.WindowsAPI;
using Button = TestStack.White.UIItems.Button;
using CheckBox = TestStack.White.UIItems.CheckBox;
using ComboBox = TestStack.White.UIItems.ListBoxItems.ComboBox;
using ListView = TestStack.White.UIItems.ListView;
using TextBox = TestStack.White.UIItems.TextBox;

namespace WPF.Test.White
{
    [TestClass]
    public class ExampleWhiteUnitTest
    {
        #region initializers and cleanups

        private static InternetExplorerWindow _browser;

        [ClassInitialize]
        public static void ClassSetup(TestContext a)
        {
            LogInAsAdmin();
        }

        [TestInitialize]
        public void TestInit()
        {
        }

        [TestCleanup]
        public void TestCleanUp()
        {
            GoToVeryTop();
        }

        [ClassCleanup]
        public static void ClassCleanUp()
        {
            LogOutAndCloseBrowser();
        }

        #endregion

        #region tests

        [TestMethod]
        public void CreatePromotion_MostBasicExample()
        {
            // Create new promotion
            ClickButtonName("Promotions");
            ClickButtonName("New Promotion");

            // The Customers screen
            EnterText("customerNameTextBox", "White promotion");
            SelectTreeControlNode("Customer", "ALL", "United Kingdom", "Grocery", "Tesco (20043)");
            ClickButtonId("btnNext");

            // The Dates screen
            ClickButtonId("btnNext");

            // The Products screen
            CheckTreeNode("radTreeView2", "ALL", "Mixers", "Soda");
            ClickButtonId("btnNext");

            // The Attributes screen
            SelectOption("MECHANIC", "BOGOF");
            ClickButtonId("btnNext");

            // The Volumes screen
            ClickButtonId("btnNext");

            // The Financials screen
            ClickButtonId("btnNext");

            // The Review screen
            ClickButtonId("btnFinish");
        }

        [TestMethod]
        public void LoadFromCsv_ValidFileProvided_SelectsSubCustomers()
        {
            // Create new promotion
            ClickButtonName("Promotions");
            ClickButtonName("New Promotion");

            // The Customers screen
            SelectTreeControlNode("Customer", "ALL", "United Kingdom", "Grocery", "Tesco (20043)");
            ClickButtonName("Load from csv");

            // The select file dialog
            OpenFile("Open", "Desktop", "demoSubCustomers.csv");

            // ASSERT
            var tescoExpressNodeCheckBox = GetTreeControlNodeCheckBox("Sub Customers", "Tesco (20043)", "Express (EXPRESS)");
            Assert.IsTrue(tescoExpressNodeCheckBox.Checked);

            var tescoMetroNodeCheckBox = GetTreeControlNodeCheckBox("Sub Customers", "Tesco (20043)", "Metro (METRO)");
            Assert.IsFalse(tescoMetroNodeCheckBox.Checked);
        }

        #endregion

        #region methods

        // Start
        private static void LogInAsAdmin()
        {
            _browser = InternetExplorer.Launch(@"https://internal.exceedra-sp.com:10111/deployapp/2.10/wpf.xbap", "ESP - Login - Internet Explorer");

            EnterText("txtUserName", "ea");
            EnterText("txtPassword", "1234");

            ClickButtonId("btnLogin");
        }
        private static void GoToVeryTop()
        {

        }
        private static void LogOutAndCloseBrowser()
        {

        }

        // Dialogs
        private static void OpenFile(string dialogBoxName, string fileLocation, string fileName)
        {
            var openDialog = _browser.ModalWindows().FirstOrDefault(modalWindow => modalWindow.Name == dialogBoxName);
            while (openDialog == null)
            {
                Thread.Sleep(100);
                openDialog = _browser.ModalWindows().FirstOrDefault(modalWindow => modalWindow.Name == dialogBoxName);
            }

            var openDialogToolbar = openDialog.Get<ToolStrip>(SearchCriteria.ByAutomationId("1001"));
            openDialogToolbar.Click();
            openDialogToolbar.Enter(fileLocation);
            Keyboard.Instance.PressSpecialKey(KeyboardInput.SpecialKeys.RETURN);

            var fileNameTextBox = openDialog.Get<TextBox>(SearchCriteria.ByAutomationId("1148"));
            fileNameTextBox.Enter(fileName);

            var openButton = openDialog.Get<Button>(SearchCriteria.ByAutomationId("1"));
            openButton.Click();
        }

        // Button
        private static void ClickButtonId(string buttonId)
        {
            Button button = _browser.Get<Button>(SearchCriteria.ByAutomationId(buttonId));
            button.Click();
        }
        private static void ClickButtonName(string buttonName)
        {
            Button button = _browser.Get<Button>(SearchCriteria.ByText(buttonName));
            button.Click();
        }

        // TextBox
        private static void EnterText(string textBoxId, string textToEnter)
        {
            var textBox = _browser.Get<TextBox>(SearchCriteria.ByAutomationId(textBoxId));
            textBox.SetValue(textToEnter);
        }

        // ComboBox
        private static void SelectOption(string comboBoxId, string option)
        {
            var comboBox = _browser.Get<ComboBox>(SearchCriteria.ByText(comboBoxId));
            comboBox.Select(option);
        }

        // TreeControl
        private static Tree GetTreeControl(string groupBoxName)
        {
            var groupBox = _browser.Get(SearchCriteria.ByClassName("GroupBox").AndByText(groupBoxName));
            var tree = groupBox.Get<Tree>(SearchCriteria.ByAutomationId("MainRadTreeViewNew"));
            return tree;
        }
        private static TreeNode GetTreeControlNode(string groupBoxName, params string[] nodePath)
        {
            var tree = GetTreeControl(groupBoxName);
            var node = tree.Node(nodePath);

            return node;
        }
        private static CheckBox GetTreeControlNodeCheckBox(string groupBoxName, params string[] nodePath)
        {
            var node = GetTreeControlNode(groupBoxName, nodePath);
            var checkBox = node.Get<CheckBox>(SearchCriteria.ByAutomationId("CheckBoxElement"));

            return checkBox;
        }
        private static void SelectTreeControlNode(string groupBoxName, params string[] nodePath)
        {
            var tree = GetTreeControl(groupBoxName);
            var node = tree.Node(nodePath);
            node.Select();
        }
        private static void CheckTreeControlNode(string groupBoxName, params string[] nodePath)
        {
            var tree = GetTreeControl(groupBoxName);
            var node = tree.Node(nodePath);

            if (node != null)
            {
                var checkBox = node.Get<CheckBox>(SearchCriteria.ByAutomationId("CheckBoxElement"));
                checkBox.Click();
            }
        }

        // Tree
        private static Tree GetTree(string treeId)
        {
            var tree = _browser.Get<Tree>(SearchCriteria.ByAutomationId(treeId));
            return tree;
        }
        private static TreeNode GetTreeNode(string treeId, params string[] nodePath)
        {
            var tree = GetTree(treeId);
            var node = tree.Node(nodePath);

            return node;
        }
        private static CheckBox GetTreeNodeCheckBox(string treeId, params string[] nodePath)
        {
            var node = GetTreeNode(treeId, nodePath);
            var checkBox = node.Get<CheckBox>(SearchCriteria.ByAutomationId("CheckBoxElement"));

            return checkBox;
        }
        private static void SelectTreeNode(string treeId, params string[] nodePath)
        {
            var tree = GetTree(treeId);
            var node = tree.Node(nodePath);
            node.Select();
        }
        private static void CheckTreeNode(string treeId, params string[] nodePath)
        {
            var tree = GetTree(treeId);
            var node = tree.Node(nodePath);

            if (node != null)
            {
                var checkBox = node.Get<CheckBox>(SearchCriteria.ByAutomationId("CheckBoxElement"));
                checkBox.Click();
            }
        }

        // Dynamic grid
        private static void EnterText(string dynamicGridId, string idColumnName, string rowIdValue, string targetColumn, string newValue)
        {
            var mainDataGrid = _browser.Get(SearchCriteria.ByAutomationId(dynamicGridId)).Get<ListView>(SearchCriteria.ByAutomationId("dynGrid"));

            var columnToUpdate = mainDataGrid.Header.Columns.FirstOrDefault(header => header.Name == targetColumn);
            var columnToUpdateIndex = mainDataGrid.Header.Columns.IndexOf(columnToUpdate);

            var rowToUpdate = mainDataGrid.Row(idColumnName, rowIdValue);

            var cellToUpdate = rowToUpdate.GetMultiple(SearchCriteria.ByClassName("DataGridCell"))[columnToUpdateIndex];

            var textBox = cellToUpdate.Get<TextBox>(SearchCriteria.All);
            textBox.SetValue(newValue);
        }
        private static void EnterText(string dynamicGridId, int rowIndex, int columnIndex, string newValue)
        {
            var mainDataGrid = _browser.Get(SearchCriteria.ByAutomationId(dynamicGridId)).Get<ListView>(SearchCriteria.ByAutomationId("dynGrid"));

            var rowToUpdate = mainDataGrid.Rows[rowIndex];
            var cellToUpdate = rowToUpdate.GetMultiple(SearchCriteria.ByClassName("DataGridCell"))[columnIndex];

            var textBox = cellToUpdate.Get<TextBox>(SearchCriteria.All);
            textBox.SetValue(newValue);
        }

        #endregion
    }
}
