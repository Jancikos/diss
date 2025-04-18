using OSPABA;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Simulation;
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
		}

		//meta! sender="ProcessVykonajOperaciu", id="67", type="Finish"
		public void ProcessFinish(MessageForm message)
		{
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
