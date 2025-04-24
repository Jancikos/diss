using OSPABA;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Simulation;
using FRI.DISS.SP3.Libs.NabytokSimulation.Entities;
using FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentModelu;
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
            _handleOperation((MyMessage)message);
		}

		//meta! sender="AgentPresunuStolarov", id="60", type="Response"
		public void ProcessRequestResponsePresunPracoviska(MessageForm message)
		{
            _handleOperation((MyMessage)message);
		}

		// sender="AgentStolariA|AgentStolariB|AgentStolariC", type="Response"
		public void ProcessRequestResponseDajStolara(MessageForm message)
		{
            var myMsg = (MyMessage)message;

            if (myMsg.Stolar is not null)
            {
                // starts the operation
                _handleOperation(myMsg);
                return;
            }

            if (myMsg.StolarTypes.Count > 0)
            {
                // try to get another stolar
                var nextStolarMsg = (MyMessage)myMsg.CreateCopy();
                nextStolarMsg.Addressee = MySim.FindAgent(Mc.GetAgentByStolarType(nextStolarMsg.StolarTypes.Dequeue()));
                Request(nextStolarMsg);
                return;
            }

            // no free stolar of given type => add to waiting queue
            MyAgent.OperationsQueues[myMsg.Nabytok!.MapStateToNextOperation()!].Enqueue(myMsg);
		}

		//meta! sender="AgentStolariB", id="25", type="Response"
		public void ProcessRequestResponseDajStolaraAgentStolariB(MessageForm message)
		{
            ProcessRequestResponseDajStolara(message);
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
            ProcessRequestResponseDajStolara(message);
		}

		//meta! sender="AgentStolariA", id="24", type="Response"
		public void ProcessRequestResponseDajStolaraAgentStolariA(MessageForm message)
		{
            ProcessRequestResponseDajStolara(message);
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

            if (myMsg.Stolar is not null)
            {
                // stolar is already assigned
                _handleOperation(myMsg);
                return;
            }

            // get free stolar of given type
            var stolariTypes = myMsg.Nabytok.MapOperationToStolarTypes();

            myMsg.Code = Mc.RequestResponseDajStolara;
            foreach (var stolariType in stolariTypes)
            {
                myMsg.StolarTypes.Enqueue(stolariType);
            }

            // try to get free stolar of given type
            ProcessRequestResponseDajStolara(myMsg);
		}

        private void _handleOperation(MyMessage myMsg)
        {
            var nabytok = myMsg.Nabytok!;
            var nextOperation = myMsg.Nabytok!.MapStateToNextOperation();
            var pracovisko = nabytok.Pracovisko!;
            var stolar = myMsg.Stolar!;

            if (nextOperation == NabytokOperation.PripravaMaterialu)
            {
                // lebo tato operacia sa vykonava na pracovisku
                pracovisko = Pracovisko.Sklad;

                // poznac ze sa na danej objednavke uz zacalo pracovat
                ((AgentModelu.AgentModelu)MySim.FindAgent(SimId.AgentModelu)).ObjednavkyWorkStarted[nabytok.Objednavka!.Id] = nabytok.Objednavka;
            }

            if (stolar.CurrentPracovisko!.Id != pracovisko.Id)
            {
                // presun stolara na pracovisko
                var presunMsg = (MyMessage)myMsg.CreateCopy();
                presunMsg.Addressee = MySim.FindAgent(SimId.AgentPresunuStolarov);
                presunMsg.Pracovisko = pracovisko;
                presunMsg.Code = (stolar.IsInWarehouse || pracovisko.IsWarehouse)
                    ? Mc.RequestResponsePresunSklad
                    : Mc.RequestResponsePresunPracoviska;

                Request(presunMsg);
                return;
            }

            // vykonaj operaciu
            var operationMsg = (MyMessage)myMsg.CreateCopy();
            operationMsg.Addressee = MyAgent.FindAssistant(SimId.ProcessVykonajOperaciu);
            StartContinualAssistant(operationMsg);
        }

        //meta! sender="ProcessVykonajOperaciu", id="67", type="Finish"
        public void ProcessFinish(MessageForm message)
		{
            var  myMsg = (MyMessage)message;

            var nabytok = myMsg.Nabytok!;
            nabytok.State = nabytok.GetNextState();

            if (nabytok.State != NabytokState.PripravenyMaterial) 
            {
                // uvolni stolara
                _tryFreeStolar(myMsg.Stolar!);
                myMsg.Stolar = null;
            }

            // vrat response
            myMsg.Code = Mc.RequestResponseVykonajOperaciu;
            Response(myMsg);
		}

        private void _tryFreeStolar(Stolar stolar)
        {
            // try find next operation
            foreach (var operation in Nabytok.MapStolarTypeToOperations(stolar.Type))
            {
                if (MyAgent.OperationsQueues[operation].Count > 0)
                {
                    // get next operation
                    var nextOperationMsg = MyAgent.OperationsQueues[operation].Dequeue();
                    nextOperationMsg.Stolar = stolar;
                    _handleOperation(nextOperationMsg);
                    return; // stolar will not be free
                }
            }

            // no more operations => free stolar
            var freeStolarMsg = new MyMessage(MySim)
            {
                Code = Mc.NoticeStolarUvolneny,
                Stolar = stolar,
                Addressee = MySim.FindAgent(Mc.GetAgentByStolarType(stolar.Type))
            };
            Notice(freeStolarMsg);
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
