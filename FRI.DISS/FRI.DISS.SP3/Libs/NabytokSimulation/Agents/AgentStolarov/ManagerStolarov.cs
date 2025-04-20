using OSPABA;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Simulation;
using FRI.DISS.SP3.Libs.NabytokSimulation.Entities;
namespace FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentStolarov
{
	/*!
	 * ku operacii sa pokusa priradit stolara. 
	 * ak aktualne taky stolar nie je volny, tak zaradi danu operaciu do radu
	 * 
	 * tiez riadi presun stolara medzi pracoviskami alebo skladom
	 */
	//meta! id="10"
	public class ManagerStolarov : OSPABA.Manager
	{
		public ManagerStolarov(int id, OSPABA.Simulation mySim, Agent myAgent) :
			base(id, mySim, myAgent)
		{
			Init();
		}

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Setup component for the next replication

			if (PetriNet != null)
			{
				PetriNet.Clear();
			}
		}

		//meta! sender="AgentPresunuStolarov", id="61", type="Response"
		public void ProcessRequestResponsePresunSklad(MessageForm message)
		{
		}

		//meta! sender="AgentPresunuStolarov", id="60", type="Response"
		public void ProcessRequestResponsePresunPracoviska(MessageForm message)
		{
		}

		//meta! sender="AgentStolariB", id="25", type="Response"
		public void ProcessRequestResponseDajStolaraAgentStolariB(MessageForm message)
		{
		}

		/*!
		 * akcia neposuva cas
		 * vrati null ak stolar nie je volny
		 * 
		 * zaroven nastavi daneho stolara na obsadeny
		 */
		//meta! sender="AgentStolariC", id="26", type="Response"
		public void ProcessRequestResponseDajStolaraAgentStolariC(MessageForm message)
		{
		}

		//meta! sender="AgentStolariA", id="24", type="Response"
		public void ProcessRequestResponseDajStolaraAgentStolariA(MessageForm message)
		{
		}

		/*!
		 * posle oparaciu, ktoru chce vykonat
		 * 
		 */
		//meta! sender="AgentModelu", id="22", type="Request"
		public void ProcessRequestResponseVykonajOperaciu(MessageForm message)
		{
            var myMsg = (MyMessage)message;
            if (myMsg.Nabytok is null)
                throw new InvalidOperationException("Nabytok cannot be null");

            if (myMsg.Nabytok.AllWorkDone)
                throw new InvalidOperationException("Nabytok is already done");

            var nextOperation = myMsg.Nabytok.MapStateToNextOperation();
            var stolariTypes = myMsg.Nabytok.MapOperationToStolarTypes();

            Stolar? stolar = null;
            foreach (var stolariType in stolariTypes)
            {
                // try to get free stolar of given type

                // TODO
            }

            if (stolar is null)
            {
                // no free stolar of given type => add to waiting queue
                MyAgent.OperationsQueues[nextOperation].Enqueue(myMsg);
                return;
            }

            // starts the operation
            _handleOperation(myMsg, stolar);

		}

        private void _handleOperation(MyMessage myMsg, Stolar stolar)
        {
            var nextOperation = myMsg.Nabytok!.MapStateToNextOperation();

            if (nextOperation == NabytokOperation.Rezanie)
            {
                _handleOperationRezanie(myMsg, stolar);
                return;
            }

            var nabytok = myMsg.Nabytok!;
            var pracovisko = nabytok.Pracovisko!;

            if (stolar.CurrentPracovisko != pracovisko)
            {
                // presun stolara na pracovisko

                // TODO

                return;
            }

            // vykonaj operaciu
            var operationMsg = (MyMessage)myMsg.CreateCopy();
            operationMsg.Addressee = MyAgent.FindAssistant(SimId.ProcessVykonajOperaciu);
            StartContinualAssistant(operationMsg);
        }


        private void _handleOperationRezanie(MyMessage myMsg, Stolar stolar)
        {
            // TODO
            throw new NotImplementedException();
        }

        //meta! sender="ProcessVykonajOperaciu", id="67", type="Finish"
        public void ProcessFinish(MessageForm message)
		{
            var  myMsg = (MyMessage)message;

            var nabytok = myMsg.Nabytok!;
            nabytok.State = nabytok.GetNextState();

            // uvolni stolara
            _tryFreeStolar(myMsg.Stolar!);
            myMsg.Stolar = null;

            // vrat response
            myMsg.Code = Mc.RequestResponseVykonajOperaciu;
            Response(myMsg);
		}

        private void _tryFreeStolar(Stolar stolar)
        {
            // TODO
            throw new NotImplementedException();
        }

        //meta! userInfo="Process messages defined in code", id="0"
        public void ProcessDefault(MessageForm message)
		{
			switch (message.Code)
			{
			}
		}

		//meta! userInfo="Generated code: do not modify", tag="begin"
		public void Init()
		{
		}

		override public void ProcessMessage(MessageForm message)
		{
			switch (message.Code)
			{
			case Mc.RequestResponseDajStolara:
				switch (message.Sender.Id)
				{
				case SimId.AgentStolariB:
					ProcessRequestResponseDajStolaraAgentStolariB(message);
				break;

				case SimId.AgentStolariC:
					ProcessRequestResponseDajStolaraAgentStolariC(message);
				break;

				case SimId.AgentStolariA:
					ProcessRequestResponseDajStolaraAgentStolariA(message);
				break;
				}
			break;

			case Mc.RequestResponseVykonajOperaciu:
				ProcessRequestResponseVykonajOperaciu(message);
			break;

			case Mc.Finish:
				ProcessFinish(message);
			break;

			case Mc.RequestResponsePresunPracoviska:
				ProcessRequestResponsePresunPracoviska(message);
			break;

			case Mc.RequestResponsePresunSklad:
				ProcessRequestResponsePresunSklad(message);
			break;

			default:
				ProcessDefault(message);
			break;
			}
		}
		//meta! tag="end"
		public new AgentStolarov MyAgent
		{
			get
			{
				return (AgentStolarov)base.MyAgent;
			}
		}
	}
}
