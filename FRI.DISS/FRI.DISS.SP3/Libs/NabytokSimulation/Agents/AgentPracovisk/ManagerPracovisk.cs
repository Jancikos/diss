using OSPABA;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Simulation;
namespace FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentPracovisk
{
	/*!
	 * pokusi sa priradit pracovisko pre dany nabytok
	 * 
	 * ak nema aktualne volne pracovisko, tak ziadost zaradi do radu
	 * 
	 * 
	 */
	//meta! id="30"
	public class ManagerPracovisk : OSPABA.Manager
	{
		public ManagerPracovisk(int id, OSPABA.Simulation mySim, Agent myAgent) :
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

		/*!
		 * request na priradene pracovbisku sa moze spravit len ak ten agent pracovisk  
		 * 
		 * 
		 * meisto sa moze 
		 */
		//meta! sender="AgentModelu", id="32", type="Request"
		public void ProcessRequestResponsePriradPracovisko(MessageForm message)
		{
		}

		//meta! sender="AgentModelu", id="34", type="Notice"
		public void ProcessNoticePracoviskoUvolnene(MessageForm message)
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
			case Mc.NoticePracoviskoUvolnene:
				ProcessNoticePracoviskoUvolnene(message);
			break;

			case Mc.RequestResponsePriradPracovisko:
				ProcessRequestResponsePriradPracovisko(message);
			break;

			default:
				ProcessDefault(message);
			break;
			}
		}
		//meta! tag="end"
		public new AgentPracovisk MyAgent
		{
			get
			{
				return (AgentPracovisk)base.MyAgent;
			}
		}
	}
}
