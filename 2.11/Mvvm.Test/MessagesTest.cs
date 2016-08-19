using System.Threading;
 
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reactive.Linq;
using Exceedra.Common.Mvvm;

namespace Mvvm.Test
{
    
    
    /// <summary>
    ///This is a test class for MessagesTest and is intended
    ///to contain all MessagesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MessagesTest
    {
        /// <summary>
        ///A test for Publish
        ///</summary>
        [TestMethod()]
        public void PutErrorMessageGetsErrorMessageTest()
        {
            bool got = false;
            const string message = "Foo";
            Messages.Instance.All.OfType<ErrorMessage>().Subscribe(s => got = s.Text == message);
            Messages.Instance.PutError(message);
            SpinWait.SpinUntil(() => got, 100);
            Assert.IsTrue(got);
        }

        /// <summary>
        ///A test for SetInstance
        ///</summary>
        [TestMethod()]
        public void SetInstanceTest()
        {
            IMessages instance = new MessagesStub();
            Messages.SetInstance(instance);
            Assert.AreSame(instance, Messages.Instance);
        }
    }

    class MessagesStub : IMessages
    {
        public IObservable<MessageBase> All
        {
            get { throw new NotImplementedException(); }
        }

        public void Put(MessageBase message)
        {
            throw new NotImplementedException();
        }

        public void PutError(string message)
        {
            throw new NotImplementedException();
        }

        public void PutInfo(string message)
        {
            throw new NotImplementedException();
        }

        public void PutWarning(string message)
        {
            throw new NotImplementedException();
        }
    }
}
