using OSPABA;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Simulation;
namespace FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentStanica
{
	//meta! id="5"
	public class ManagerStanica : OSPABA.Manager
	{
		public ManagerStanica(int id, OSPABA.Simulation mySim, Agent myAgent) :
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

		//meta! sender="AgentModelu", id="23", type="Notice"
		public void ProcessNoticePrichodZakaznika(MessageForm message)
		{
		}

		//meta! sender="AgentObsluhaZakaznika", id="28", type="Response"
		public void ProcessRequestResponseObluzZakaznika(MessageForm message)
		{
		}

		/*!
		 * Z dveri ku podladni a naopak
		 */
		//meta! sender="AgentPresunZakaznika", id="27", type="Response"
		public void ProcessRequestResponsePresunZakaznika(MessageForm message)
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
			case Mc.NoticePrichodZakaznika:
				ProcessNoticePrichodZakaznika(message);
			break;

			case Mc.RequestResponsePresunZakaznika:
				ProcessRequestResponsePresunZakaznika(message);
			break;

			case Mc.RequestResponseObluzZakaznika:
				ProcessRequestResponseObluzZakaznika(message);
			break;

			default:
				ProcessDefault(message);
			break;
			}
		}
		//meta! tag="end"
		public new AgentStanica MyAgent
		{
			get
			{
				return (AgentStanica)base.MyAgent;
			}
		}
	}
}
