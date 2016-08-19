using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Exceedra.Common.GroupingMenu;

namespace WPF.Test.GroupingMenuTests
{
    [TestClass]
    public class GroupingMenuTests
    {
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void MenuItems_AssigningMenuItemsWithTheSameId_ThrowsException()
        {
            // Arrange
            GroupingMenu target = new GroupingMenu();

            MenuItem firstMenuItem = new MenuItem { Id = "1", ParentId = null };
            MenuItem secondMenuItem = new MenuItem { Id = "1", ParentId = null };

            // Act
            target.MenuItems = new List<IMenuItem>
            {
                firstMenuItem,
                secondMenuItem
            };

            // Assert
            // Asserted by [ExpectedException(typeof(Exception))]
        }

        [TestMethod]
        public void GroupedMenuItems_AssigningMenuItemsWithAndWithoutParentId_ContainsOnlyMenuItemsWithoutParentId()
        {
            // Arrange
            GroupingMenu target = new GroupingMenu();

            MenuItem firstMenuItem = new MenuItem { Id = "1", ParentId = null };
            MenuItem secondMenuItem = new MenuItem { Id = "2", ParentId = "1" };

            // Act
            target.MenuItems = new List<IMenuItem>
            {
                firstMenuItem,
                secondMenuItem
            };

            // Assert
            Assert.IsTrue(target.GroupedMenuItems.Count == 1);

            Assert.IsTrue(target.GroupedMenuItems.Contains(firstMenuItem), "Does not contain the item with ParentId == null");
            Assert.IsFalse(target.GroupedMenuItems.Contains(secondMenuItem), "Does contain the item with ParentId != null");
        }

        [TestMethod]
        public void GroupedMenuItems_AssigningMenuItemWithEmptyStringParentId_ContainsThisMenuItem()
        {
            // Arrange
            GroupingMenu target = new GroupingMenu();

            MenuItem menuItem = new MenuItem { ParentId = string.Empty };

            // Act
            target.MenuItems = new List<IMenuItem> { menuItem };

            // Assert
            Assert.IsTrue(target.GroupedMenuItems.Contains(menuItem), "Does not contain the item with ParentId == string.Empty");
        }

        [TestMethod]
        public void GroupedMenuItems_AssigningMenuItemWithParentIdOfAnotherItem_IsChildOfThatItem()
        {
            // Arrange
            GroupingMenu target = new GroupingMenu();

            MenuItem firstMenuItem = new MenuItem { Id = "1", ParentId = null };
            MenuItem secondMenuItem = new MenuItem { Id = "2", ParentId = "1" };

            // Act
            target.MenuItems = new List<IMenuItem>
            {
                firstMenuItem,
                secondMenuItem
            };

            // Assert
            Assert.IsTrue(target.GroupedMenuItems[0].Children.Count == 1);
            
            Assert.IsTrue(target.GroupedMenuItems[0].Children.Contains(secondMenuItem), "The item with ParentId of another item is not it's child");   
        }

        [TestMethod]
        public void GroupedMenuItems_AssigningSimpleMenuItemsSet_IsMenuItemsHierarchyCorrect()
        {
            // Arrange
            GroupingMenu target = new GroupingMenu();

            MenuItem firstMenuItem = new MenuItem { Id = "1", ParentId = null };
            MenuItem secondMenuItem = new MenuItem { Id = "2", ParentId = "1" };
            MenuItem thirdMenuItem = new MenuItem { Id = "3", ParentId = "1" };
            MenuItem fourthMenuItem = new MenuItem { Id = "4", ParentId = string.Empty };
            MenuItem fifthMenuItem = new MenuItem { Id = "5", ParentId = "4" };
            MenuItem sixthMenuItem = new MenuItem { Id = "6", ParentId = "4" };
            MenuItem seventhMenuItem = new MenuItem { Id = "7", ParentId = "5" };
            MenuItem eightMenuItem = new MenuItem { Id = "8", ParentId = "6" };
            MenuItem ninthMenuItem = new MenuItem { Id = "9", ParentId = "6" };
            MenuItem tenthMenuItem = new MenuItem { Id = "10", ParentId = "9" };

            // Act
            target.MenuItems = new List<IMenuItem>
            {
                firstMenuItem,
                secondMenuItem,
                thirdMenuItem,
                fourthMenuItem,
                fifthMenuItem,
                sixthMenuItem,
                seventhMenuItem,
                eightMenuItem,
                ninthMenuItem,
                tenthMenuItem
            };

            // Assert

            // first
            //   second
            //   third
            // fourth
            //   fifth
            //     seventh
            //   sixth
            //     eigth
            //     ninth
            //       tenth

            Assert.IsTrue(target.GroupedMenuItems.Contains(firstMenuItem));
            Assert.IsTrue(target.GroupedMenuItems[0].Children.Contains(secondMenuItem));
            Assert.IsTrue(target.GroupedMenuItems[0].Children.Contains(thirdMenuItem));
            Assert.IsTrue(target.GroupedMenuItems.Contains(fourthMenuItem));
            Assert.IsTrue(target.GroupedMenuItems[1].Children.Contains(fifthMenuItem));
            Assert.IsTrue(target.GroupedMenuItems[1].Children.Contains(sixthMenuItem));
            Assert.IsTrue(target.GroupedMenuItems[1].Children[0].Children.Contains(seventhMenuItem));
            Assert.IsTrue(target.GroupedMenuItems[1].Children[1].Children.Contains(eightMenuItem));
            Assert.IsTrue(target.GroupedMenuItems[1].Children[1].Children.Contains(ninthMenuItem));
            Assert.IsTrue(target.GroupedMenuItems[1].Children[1].Children[1].Children.Contains(tenthMenuItem));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GroupedMenuItems_AssigningMenuItemWithParentIdOfNoOtherMenuItem_ThrowsException()
        {
            // Arrange
            GroupingMenu target = new GroupingMenu();

            MenuItem firstMenuItem = new MenuItem { Id = "1", ParentId = "2" };

            // Act
            target.MenuItems = new List<IMenuItem> { firstMenuItem };

            // Assert
            // ReSharper disable once UnusedVariable
            var justToFireException = target.GroupedMenuItems;
        }
    }
}
