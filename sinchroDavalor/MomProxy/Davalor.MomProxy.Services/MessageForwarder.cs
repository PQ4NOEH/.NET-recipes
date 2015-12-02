using Davalor.MomProxy.Domain;
using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Davalor.MomProxy.Domain.Services;
using Davalor.MomProxy.Domain.Configuration;
using Davalor.MomProxy.Domain.Quota;
using Davalor.Base.Library.Guards;
using Davalor.Toolkit.Extensions;

namespace Davalor.MomProxy.Services
{
    public class MessageForwarder : IMessageForwarder
    {
        bool _forwardMessages = true;
        readonly ConcurrentQueue<string> _messagesQueue = new ConcurrentQueue<string>();
        readonly IMomRepository _momRepository;
        readonly IServiceEvents _mediator;
        readonly string _topic;
        readonly IQuota _quota;
        Task _backGroundThread;

        public MessageForwarder(
            NotNullOrWhiteSpaceString topic, 
            NotNullable<IMomRepository> momRepository,
            NotNullable<IServiceEvents> mediator,
            NotNullable<IQuota> quota)
        {
            _topic = topic;
            _momRepository = momRepository.Value;
            _mediator = mediator.Value;
            _quota = quota.Value;
        }
        public string Topic { get { return _topic; } }

        public void AddMessage(string message)
        {
            _messagesQueue.Enqueue(message);
        }

        public void StartForwarding()
        {
            if (_backGroundThread == null)
            {
                _forwardMessages = true;
                _backGroundThread = Task.Run(() => SendMessages());
            }
        }

        public void StopForwarding()
        {
            if (_forwardMessages == true )
            {
                _forwardMessages = false;
                _backGroundThread.Wait();
                _backGroundThread = null;
            }
        }

        void SendMessages()
        {
            while (_forwardMessages)
            {
                int numberOfMessages = _messagesQueue.Count;
                if (_quota.Fullfills(numberOfMessages)) //solo se envian los mensajes si se cumple con la cuota definida
                {
                    var messagesToSend = new List<string>(numberOfMessages);
                    string messsage;
                    numberOfMessages.Times(new Action(() => //cargo en una lista los mensajes que se van a enviar
                    {
                        _messagesQueue.TryPeek(out messsage);
                        messagesToSend.Add(messsage);
                    }));
                    _momRepository.SendMessages(_topic, messagesToSend.Select(s=>new NotNullOrWhiteSpaceString(s)).ToList()); //Envio todos los mensajes en bloque
                    messagesToSend.ForEach(m => //Por cada mensaje enviado: 
                    {
                        _messagesQueue.TryDequeue(out messsage); //1.- Lo quito de la cola interna
                        _mediator.SentIncommingMessage(messsage); //2.- Aviso al mediador
                    });
                }
            }
        }
    }
}
