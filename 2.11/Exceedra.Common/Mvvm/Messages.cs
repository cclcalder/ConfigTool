using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;

namespace Exceedra.Common.Mvvm
{
    public interface IMessages
    {
        IObservable<MessageBase> All { get; }
        void Put(MessageBase message);
        void PutError(string message);
        void PutInfo(string message);
        void PutWarning(string message);
    }

    public class Messages : IMessages
    {
        private readonly Subject<MessageBase> _stream = new Subject<MessageBase>();

        public IObservable<MessageBase> All
        {
            get
            {
                return _stream.AsObservable();
            }
        }

        public void Put(MessageBase message)
        {
            _stream.OnNext(message);
        }

        public void PutError(string message)
        {
            Put(new ErrorMessage(message));
        }

        public void PutInfo(string message)
        {
            Put(new InformationMessage(message));
        }

        public void PutWarning(string message)
        {
            Put(new WarningMessage(message));
        }

        private static IMessages _instance = new Messages();

        public static IMessages Instance
        {
            get { return _instance; }
        }

        public static void SetInstance(IMessages instance)
        {
            _instance = instance;
        }
    }

    public abstract class MessageBase
    {
        private readonly string _text;

        protected MessageBase(string text)
        {
            _text = text;
        }

        public string Text
        {
            get { return _text; }
        }
    }

    public class ErrorMessage : MessageBase
    {
        public ErrorMessage(string text)
            : base(text)
        {
        }
    }

    public class WarningMessage : MessageBase
    {
        public WarningMessage(string text)
            : base(text)
        {
        }
    }

    public class InformationMessage : MessageBase
    {
        public InformationMessage(string text)
            : base(text)
        {
        }
    }
}
