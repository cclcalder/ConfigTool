using System;
using System.Linq;
using System.Xml.Linq;
using Exceedra.Common.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WPF.Test.PasswordValidatorTests
{
    [TestClass]
    public class PasswordValidatorTests
    {
        #region Edge cases

        [ExpectedException(typeof(Exception))]
        [TestMethod]
        public void None_ProvidedPasswordIsNull_ThrowsException()
        {
            // Arrange
            var passwordValidator = new PasswordValidator(
                XElement.Parse(@"
                <Results>
                </Results>
                "));

            // Act
            passwordValidator.Validate(null);

            // Assert
            Assert.Fail();
        }

        #endregion

        #region MinimumPasswordLength

        [TestMethod]
        public void MinimumPasswordLengthOf8_ProvidedShorterPassword_ReturnsPolicyAsInvalid()
        {
            // Arrange
            var password = "abc";

            var passwordValidator = new PasswordValidator(
                XElement.Parse(@"
                <Results>
                  <PasswordPolicy>
                    <Policy>MinimumPasswordLength</Policy>
                    <PolicyDescription>Password must be of minimum 8 characters long</PolicyDescription>
                    <Value>8</Value>
                  </PasswordPolicy>
                </Results>
                "));

            // Act
            var result = passwordValidator.Validate(password);

            // Assert
            Assert.AreEqual(false, result.First().IsValid);
        }

        [TestMethod]
        public void MinimumPasswordLengthOf8_ProvidedPasswordOfLength8_ReturnsPolicyAsValid()
        {
            // Arrange
            var password = "12345678";

            var passwordValidator = new PasswordValidator(
                XElement.Parse(@"
                <Results>
                  <PasswordPolicy>
                    <Policy>MinimumPasswordLength</Policy>
                    <PolicyDescription>MinimumPasswordLength failed</PolicyDescription>
                    <Value>8</Value>
                  </PasswordPolicy>
                </Results>
                "));

            // Act
            var result = passwordValidator.Validate(password);

            // Assert
            Assert.AreEqual(true, result.First().IsValid);
        }

        [TestMethod]
        public void MinimumPasswordLengthOf8_ProvidedLongerPassword_ReturnsPolicyAsValid()
        {
            // Arrange
            var password = "hjdfsgjfghvcxhdfhsg";

            var passwordValidator = new PasswordValidator(
                XElement.Parse(@"
                <Results>
                  <PasswordPolicy>
                    <Policy>MinimumPasswordLength</Policy>
                    <PolicyDescription>MinimumPasswordLength failed</PolicyDescription>
                    <Value>8</Value>
                  </PasswordPolicy>
                </Results>
                "));

            // Act
            var result = passwordValidator.Validate(password);

            // Assert
            Assert.AreEqual(true, result.First().IsValid);
        }

        #endregion

        #region MaximumPasswordLength

        [TestMethod]
        public void MaximumPasswordLengthOf50_ProvidedShorterPassword_ReturnsPolicyAsValid()
        {
            // Arrange
            var password = "abc";

            var passwordValidator = new PasswordValidator(
                XElement.Parse(@"
                <Results>
                  <PasswordPolicy>
                    <Policy>MaximumPasswordLength</Policy>
                    <PolicyDescription>MaximumPasswordLength failed</PolicyDescription>
                    <Value>50</Value>
                  </PasswordPolicy>
                </Results>
                "));

            // Act
            var result = passwordValidator.Validate(password);

            // Assert
            Assert.AreEqual(true, result.First().IsValid);
        }

        [TestMethod]
        public void MaximumPasswordLengthOf50_ProvidedPasswordOfLength50_ReturnsPolicyAsValid()
        {
            // Arrange
            var password = "abcdefghijabcdefghijabcdefghijabcdefghijabcdefghij";

            var passwordValidator = new PasswordValidator(
                XElement.Parse(@"
                <Results>
                  <PasswordPolicy>
                    <Policy>MaximumPasswordLength</Policy>
                    <PolicyDescription>MaximumPasswordLength failed</PolicyDescription>
                    <Value>50</Value>
                  </PasswordPolicy>
                </Results>
                "));

            // Act
            var result = passwordValidator.Validate(password);

            // Assert
            Assert.AreEqual(true, result.First().IsValid);
        }

        [TestMethod]
        public void MaximumPasswordLengthOf50_ProvidedLongerPassword_ReturnsPolicyAsInvalid()
        {
            // Arrange
            var password = "abcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghij";

            var passwordValidator = new PasswordValidator(
                XElement.Parse(@"
                <Results>
                  <PasswordPolicy>
                    <Policy>MaximumPasswordLength</Policy>
                    <PolicyDescription>MaximumPasswordLength failed</PolicyDescription>
                    <Value>50</Value>
                  </PasswordPolicy>
                </Results>
                "));

            // Act
            var result = passwordValidator.Validate(password);

            // Assert
            Assert.AreEqual(false, result.First().IsValid);
        }

        #endregion

        #region CannotContainString

        [TestMethod]
        public void CannotContainUserName_ProvidedPasswordWithUserName_ReturnsPolicyAsInvalid()
        {
            // Arrange
            var password = "35436TestUser$^#235";

            var passwordValidator = new PasswordValidator(
                XElement.Parse(@"
                <Results>
                  <PasswordPolicy>
                    <Policy>CannotContainUsername</Policy>
                    <PolicyDescription>CannotContainUsername failed</PolicyDescription>
                    <Value>1</Value>
                  </PasswordPolicy>
                </Results>
                "),
                "TestUser");

            // Act
            var result = passwordValidator.Validate(password);

            // Assert
            Assert.AreEqual(false, result.First().IsValid);
        }

        [TestMethod]
        public void CannotContainUserName_ProvidedPasswordWithUserNameInDifferentCasing_ReturnsPolicyAsInvalid()
        {
            // Arrange
            var password = "35436TEstUseR$^#235";

            var passwordValidator = new PasswordValidator(
                XElement.Parse(@"
                <Results>
                  <PasswordPolicy>
                    <Policy>CannotContainUsername</Policy>
                    <PolicyDescription>CannotContainUsername failed</PolicyDescription>
                    <Value>1</Value>
                  </PasswordPolicy>
                </Results>
                "),
                "TestUser");

            // Act
            var result = passwordValidator.Validate(password);

            // Assert
            Assert.AreEqual(false, result.First().IsValid);
        }

        #endregion

        #region MustContainCharacters

        [TestMethod]
        public void MustContainFourUpperCaseLetters_ProvidedPasswordWithoutFourUpperCaseLetters_ReturnsPolicyAsInvalid()
        {
            // Arrange
            var password = "passwordwithjustThreeUpperCaseletters";

            var passwordValidator = new PasswordValidator(
                XElement.Parse(@"
                <Results>
                  <PasswordPolicy>
                    <Policy>MustContain</Policy>
                    <PolicyDescription>MustContain failed</PolicyDescription>
                    <Value>4</Value>
                    <CharacterSet>ABCDEFGHIJKLMNOPQRSTUVWXYZ</CharacterSet>
                  </PasswordPolicy>
                </Results>
                "));

            // Act
            var result = passwordValidator.Validate(password);

            // Assert
            Assert.AreEqual(false, result.First().IsValid);
        }

        [TestMethod]
        public void MustContainFourUpperCaseLetters_ProvidedPasswordWithFourUpperCaseLetters_ReturnsPolicyAsValid()
        {
            // Arrange
            var password = "iamapasswordcontainingFourUpperCaseLetters";

            var passwordValidator = new PasswordValidator(
                XElement.Parse(@"
                <Results>
                  <PasswordPolicy>
                    <Policy>MustContain</Policy>
                    <PolicyDescription>MustContain failed</PolicyDescription>
                    <Value>4</Value>
                    <CharacterSet>ABCDEFGHIJKLMNOPQRSTUVWXYZ</CharacterSet>
                  </PasswordPolicy>
                </Results>
                "));

            // Act
            var result = passwordValidator.Validate(password);

            // Assert
            Assert.AreEqual(true, result.First().IsValid);
        }

        [TestMethod]
        public void MustContainThreeLowerCaseLetters_ProvidedPasswordWithOnlyUpperCaseLetters_ReturnsPolicyAsInvalid()
        {
            // Arrange
            var password = "CHEERSFORANYONEWHOISCHECKINGTHESETESTSTHATSAWESOME";

            var passwordValidator = new PasswordValidator(
                XElement.Parse(@"
                <Results>
                  <PasswordPolicy>
                    <Policy>MustContain</Policy>
                    <PolicyDescription>MustContain failed</PolicyDescription>
                    <Value>3</Value>
                    <CharacterSet>abcdefghijklmnopqrstuvwxyz</CharacterSet>
                  </PasswordPolicy>
                </Results>
                "));

            // Act
            var result = passwordValidator.Validate(password);

            // Assert
            Assert.AreEqual(false, result.First().IsValid);
        }

        [TestMethod]
        public void MustContainThreeLowerCaseLetters_ProvidedPasswordWithMoreThanThreeLowerCaseLetters_ReturnsPolicyAsValid()
        {
            // Arrange
            var password = "whydidyouvotetoexitidontwanttoleave";

            var passwordValidator = new PasswordValidator(
                XElement.Parse(@"
                <Results>
                  <PasswordPolicy>
                    <Policy>MustContain</Policy>
                    <PolicyDescription>MustContain failed</PolicyDescription>
                    <Value>3</Value>
                    <CharacterSet>abcdefghijklmnopqrstuvwxyz</CharacterSet>
                  </PasswordPolicy>
                </Results>
                "));

            // Act
            var result = passwordValidator.Validate(password);

            // Assert
            Assert.AreEqual(true, result.First().IsValid);
        }

        [TestMethod]
        public void MustContainTwoNumbers_ProvidedPasswordWithoutNumbers_ReturnsPolicyAsInvalid()
        {
            // Arrange
            var password = "AnotherOneBitesTheDust";

            var passwordValidator = new PasswordValidator(
                XElement.Parse(@"
                <Results>
                  <PasswordPolicy>
                    <Policy>MustContain</Policy>
                    <PolicyDescription>MustContain failed</PolicyDescription>
                    <Value>2</Value>
                    <CharacterSet>0123456789</CharacterSet>
                  </PasswordPolicy>
                </Results>
                "));

            // Act
            var result = passwordValidator.Validate(password);

            // Assert
            Assert.AreEqual(false, result.First().IsValid);
        }

        [TestMethod]
        public void MustContainTwoNumbers_ProvidedPasswordWithTwoNumbers_ReturnsPolicyAsValid()
        {
            // Arrange
            var password = "England 1-2 Iceland";

            var passwordValidator = new PasswordValidator(
                XElement.Parse(@"
                <Results>
                  <PasswordPolicy>
                    <Policy>MustContain</Policy>
                    <PolicyDescription>MustContain failed</PolicyDescription>
                    <Value>2</Value>
                    <CharacterSet>0123456789</CharacterSet>
                  </PasswordPolicy>
                </Results>
                "));

            // Act
            var result = passwordValidator.Validate(password);

            // Assert
            Assert.AreEqual(true, result.First().IsValid);
        }

        [TestMethod]
        public void MustContainOneSpecialCharacter_ProvidedPasswordWithoutSpecialCharacters_ReturnsPolicyAsInvalid()
        {
            // Arrange
            var password = "What if Stephen Hawking was the real slim shady but we would never know because he couldnt stand up";

            var specialCharactersSet = "~!@#$%^&amp;*_-+=`|\\(){}:;\"&lt;&gt;,.?/";

            var passwordValidator = new PasswordValidator(
                XElement.Parse(@"
                <Results>
                  <PasswordPolicy>
                    <Policy>MustContain</Policy>
                    <PolicyDescription>MustContain failed</PolicyDescription>
                    <Value>1</Value>
                    <CharacterSet>" + specialCharactersSet + @"</CharacterSet>
                  </PasswordPolicy>
                </Results>
                "));

            // Act
            var result = passwordValidator.Validate(password);

            // Assert
            Assert.AreEqual(false, result.First().IsValid);
        }

        [TestMethod]
        public void MustContainOneSpecialCharacter_ProvidedPasswordWithEnoughSpecialCharacters_ReturnsPolicyAsValid()
        {
            // Arrange
            var password = "What if Stephen Hawking was the real slim shady but we would never know because he couldn't stand up?";

            var specialCharactersSet = "~!@#$%^&amp;*_-+=`|\\(){}:;\"&lt;&gt;,.?/";

            var passwordValidator = new PasswordValidator(
                XElement.Parse(@"
                <Results>
                  <PasswordPolicy>
                    <Policy>MustContain</Policy>
                    <PolicyDescription>MustContain failed</PolicyDescription>
                    <Value>1</Value>
                    <CharacterSet>" + specialCharactersSet + @"</CharacterSet>
                  </PasswordPolicy>
                </Results>
                "));

            // Act
            var result = passwordValidator.Validate(password);

            // Assert
            Assert.AreEqual(true, result.First().IsValid);
        }

        [TestMethod]
        public void MustContainSpecialCharacter_ProvidedSpecialCharactersWithTheSameCharacterRepeatedMultipleTimes_CountsTheSameCharacterOnlyOnce()
        {
            // Arrange
            var password = "abrakadabra";

            var passwordValidator = new PasswordValidator(
                XElement.Parse(@"
                <Results>
                  <PasswordPolicy>
                    <Policy>MustContain</Policy>
                    <PolicyDescription>MustContain failed</PolicyDescription>
                    <Value>15</Value>
                    <CharacterSet>aaa</CharacterSet>
                  </PasswordPolicy>
                </Results>
                "));

            // Act
            var result = passwordValidator.Validate(password);

            // Assert
            Assert.AreEqual(false, result.First().IsValid);
        }

        #endregion

        #region Rulesets

        private PasswordValidator _firstRuleset = new PasswordValidator(
                XElement.Parse(@"
                <Results>
                    <PasswordPolicy>
                      <Policy>MinimumPasswordLength</Policy>
                      <PolicyDescription>MinimumPasswordLength failed</PolicyDescription>
                      <Value>8</Value>
                    </PasswordPolicy>
                    <PasswordPolicy>
                      <Policy>MaximumPasswordLength</Policy>
                      <PolicyDescription>MaximumPasswordLength failed</PolicyDescription>
                      <Value>50</Value>
                    </PasswordPolicy>
                    <PasswordPolicy>
                      <Policy>CannotContainUsername</Policy>
                      <PolicyDescription>CannotContainUsername failed</PolicyDescription>
                      <Value>1</Value>
                    </PasswordPolicy>
                    <PasswordPolicy>
                      <Policy>MustContain</Policy>
                      <PolicyDescription>MustContain failed</PolicyDescription>
                      <Value>4</Value>
                      <CharacterSet>ABCDEFGHIJKLMNOPQRSTUVWXYZ</CharacterSet>
                    </PasswordPolicy>
                    <PasswordPolicy>
                      <Policy>MustContain</Policy>
                      <PolicyDescription>MustContain failed</PolicyDescription>
                      <Value>3</Value>
                      <CharacterSet>abcdefghijklmnopqrstuvwxyz</CharacterSet>
                    </PasswordPolicy>
                    <PasswordPolicy>
                      <Policy>MustContain</Policy>
                      <PolicyDescription>MustContain failed</PolicyDescription>
                      <Value>2</Value>
                      <CharacterSet>0123456789</CharacterSet>
                    </PasswordPolicy>
                    <PasswordPolicy>
                      <Policy>MustContain</Policy>
                      <PolicyDescription>MustContain failed</PolicyDescription>
                      <Value>1</Value>
                      <CharacterSet>" + "~!@#$%^&amp;*_-+=`|\\(){}:;\"&lt;&gt;,.?/" + @"</CharacterSet>
                  </PasswordPolicy>
                </Results>
                "),
                "TestUser");

        [TestMethod]
        public void FirstRulesetTest()
        {
            // good one!
            Assert.IsTrue(_firstRuleset.Validate("ABCDefg01~").All(policy => policy.IsValid == true));

            // randomly generated correct passwords
            Assert.IsTrue(_firstRuleset.Validate("VATHe4aS3e?_").All(policy => policy.IsValid == true));
            Assert.IsTrue(_firstRuleset.Validate("Wepe5R6*E@uB").All(policy => policy.IsValid == true));
            Assert.IsTrue(_firstRuleset.Validate("W_AcAcrE8a-0").All(policy => policy.IsValid == true));
            Assert.IsTrue(_firstRuleset.Validate("CuXes2K8sW-t").All(policy => policy.IsValid == true));

            // not enough chars
            Assert.IsFalse(_firstRuleset.Validate("ADeg1~").All(policy => policy.IsValid == true));

            // too many chars
            Assert.IsFalse(_firstRuleset.Validate("ABCDefg01~ABCDefg01~ABCDefg01~ABCDefg01~ABCDefg01~ABCDefg01~ABCDefg01~ABCDefg01~").All(policy => policy.IsValid == true));

            // forbidden chars
            Assert.IsFalse(_firstRuleset.Validate("ABCDeTestUserfg01~").All(policy => policy.IsValid == true));

            // not enough upper cases
            Assert.IsFalse(_firstRuleset.Validate("ABCefg01~").All(policy => policy.IsValid == true));

            // not enough lower cases
            Assert.IsFalse(_firstRuleset.Validate("ABCDef01~").All(policy => policy.IsValid == true));

            // not enough numbers
            Assert.IsFalse(_firstRuleset.Validate("ABCDefg0~").All(policy => policy.IsValid == true));

            // not enough special chars
            Assert.IsFalse(_firstRuleset.Validate("ABCDefg01").All(policy => policy.IsValid == true));

            // randomly generated incorrect passwords
            Assert.IsFalse(_firstRuleset.Validate("-acU3amuXest").All(policy => policy.IsValid == true));
            Assert.IsFalse(_firstRuleset.Validate("T2+3PAtGGaBR").All(policy => policy.IsValid == true));
            Assert.IsFalse(_firstRuleset.Validate("s4eMACheBu!e").All(policy => policy.IsValid == true));
            Assert.IsFalse(_firstRuleset.Validate("mU7Hak2sepUX").All(policy => policy.IsValid == true));
        }

        #endregion
    }
}