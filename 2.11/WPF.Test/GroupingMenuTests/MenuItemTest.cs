using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Exceedra.Common.GroupingMenu;

namespace WPF.Test.GroupingMenuTests
{
    [TestClass]
    public class MenuItemTest
    {
        [TestMethod]
        public void HasParent_SettingDifferentParentIds_BehavesCorrectly()
        {
            // Arrange
            MenuItem target = new MenuItem();
            MenuItem target2 = new MenuItem();
            MenuItem target3 = new MenuItem();

            // Act
            target.ParentId = null;
            target2.ParentId = string.Empty;
            target3.ParentId = "1";

            // Assert
            Assert.IsFalse(target.HasParent, "Has parent is true with ParentId == null");
            Assert.IsFalse(target2.HasParent, "Has parent is true with ParentId == \"\"");
            Assert.IsTrue(target3.HasParent, "Has parent is false with ParentId == 1");
        }

        [TestMethod]
        public void HasChildren_SettingDifferentChildrenValues_BehavesCorrectly()
        {
            // Arrange
            MenuItem target = new MenuItem();
            MenuItem target2 = new MenuItem();
            MenuItem target3 = new MenuItem();

            // Act
            target.Children = null;
            target2.Children = new List<IMenuItem>();
            target3.Children = new List<IMenuItem> { new MenuItem() };

            // Assert
            Assert.IsFalse(target.HasChildren, "Has children is true with Children == null");
            Assert.IsFalse(target2.HasChildren, "Has children is true with Children is an empty list");
            Assert.IsTrue(target3.HasChildren, "Has children is false with Children contains one element");
        }
    }
}
