using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Exceedra.Common.Utilities
{
    public class PasswordValidator
    {
        private readonly List<PasswordPolicy> _policies;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="userName">
        /// Required for some password policies (CannotContainUserName). 
        /// If you don't use any of them you don't need to specify a value for this argument.
        /// </param>
        public PasswordValidator(XElement xml, string userName = null)
        {
            _policies = new List<PasswordPolicy>();

            foreach (var xPasswordPolicy in xml.Elements("PasswordPolicy"))
            {
                var xPolicy = xPasswordPolicy.Element("Policy");
                var xValue = xPasswordPolicy.Element("Value");
                var xMessage = xPasswordPolicy.Element("PolicyDescription");

                if (xPolicy == null || xValue == null || xMessage == null) continue;

                switch (xPolicy.Value.ToLower())
                {
                    case "minimumpasswordlength":
                        _policies.Add(
                            new MinLengthPasswordPolicy
                            {
                                Value = int.Parse(xValue.Value),
                                Description = xMessage.Value
                            });
                        break;

                    case "maximumpasswordlength":
                        _policies.Add(
                            new MaxLengthPasswordPolicy
                            {
                                Value = int.Parse(xValue.Value),
                                Description = xMessage.Value
                            });
                        break;

                    case "cannotcontainusername":
                        _policies.Add(
                            new ForbiddenPhrasePasswordPolicy
                            {
                                Value = int.Parse(xValue.Value),
                                Description = xMessage.Value,
                                Phrase = userName
                            });
                        break;

                    case "mustcontain":
                        _policies.Add(
                            new RequiredCharsPasswordPolicy
                            {
                                Value = int.Parse(xValue.Value),
                                Description = xMessage.Value,
                                CharacterSet = xPasswordPolicy.MaybeElement("CharacterSet").MaybeValue()
                            });
                        break;
                }
            }
        }

        /// <summary>
        /// Returns true if all policies are valid.
        /// Otherwise returns false.
        /// </summary>
        public bool IsValid
        {
            get { return _policies.All(policy => policy.IsValid == true); }
        }

        public List<PasswordPolicy> Validate(string password)
        {
            if (password == null)
                throw new Exception("Tried to validate a password that is null.");

            foreach (var policy in _policies)
                policy.IsValid = policy.Validate(password);

            return _policies;
        }
    }

    public abstract class PasswordPolicy
    {
        public int Value { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// null - not checked yet
        /// </summary>
        public bool? IsValid { get; set; }

        public abstract bool Validate(string password);
    }

    public class MinLengthPasswordPolicy : PasswordPolicy
    {
        public override bool Validate(string password)
        {
            if (password.Length < Value)
                return false;

            return true;
        }
    }

    public class MaxLengthPasswordPolicy : PasswordPolicy
    {
        public override bool Validate(string password)
        {
            if (password.Length > Value)
                return false;

            return true;
        }
    }

    public class ForbiddenPhrasePasswordPolicy: PasswordPolicy
    {
        public string Phrase { get; set; }

        public override bool Validate(string password)
        {
            if (Phrase == null)
                return true;

            if (password.ToLower().Contains(Phrase.ToLower()))
                return false;

            return true;
        }
    }

    public class RequiredCharsPasswordPolicy: PasswordPolicy
    {
        public string CharacterSet { get; set; }

        public override bool Validate(string password)
        {
            if (CharacterSet == null)
                return true;

            int countLeft = Value;

            foreach (var c in CharacterSet.Distinct())
                countLeft -= password.Count(x => x == c);

            if (countLeft <= 0)
                return true;

            return false;
        }
    }
}
