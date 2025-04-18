using OSPABA;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Simulation;
namespace FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentModelu
{
	/*!
	 * Spracovava zoznam objednavok a stara sa o postupne vykonavanie prace na objednavkach
	 */
	//meta! id="1"
	public class ManagerModelu : OSPABA.Manager
	{
		public ManagerModelu(int id, OSPABA.Simulation mySim, Agent myAgent) :
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

		//meta! sender="AgentOkolia", id="8", type="Notice"
		public void ProcessNoticePrichodObjednavka(MessageForm message)
		{
		}

		/*!
		 * posle oparaciu, ktoru chce vykonat
		 * 
		 */
		//meta! sender="AgentStolarov", id="22", type="Response"
		public void ProcessRequestResponseVykonajOperaciu(MessageForm message)
		{
		}

		/*!
		 * request na priradene pracovbisku sa moze spravit len ak ten agent pracovisk  
		 * 
		 * 
		 * meisto sa moze 
		 */
		//meta! sender="AgentPracovisk", id="32", type="Response"
		public void ProcessRequestResponsePriradPracovisko(MessageForm message)
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
			case Mc.RequestResponsePriradPracovisko:
				ProcessRequestResponsePriradPracovisko(message);
			break;

			case Mc.RequestResponseVykonajOperaciu:
				ProcessRequestResponseVykonajOperaciu(message);
			break;

			case Mc.NoticePrichodObjednavka:
				ProcessNoticePrichodObjednavka(message);
			break;

			default:
				ProcessDefault(message);
			break;
			}
		}
		//meta! tag="end"
		public new AgentModelu MyAgent
		{
			get
			{
				return (AgentModelu)base.MyAgent;
			}
		}
	}
}
