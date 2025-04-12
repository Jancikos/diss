using OSPABA;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Simulation;
namespace FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentModelu
{
	/*!
	 * Tiez by som ho považoval za agenta celej stanice
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

		//meta! sender="AgentOkolia", id="13", type="Notice"
		public void ProcessNoticePrichodZakaznika(MessageForm message)
		{
		}

		//meta! sender="AgentStanica", id="24", type="Notice"
		public void ProcessNoticeOdchodZakaznika(MessageForm message)
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

			case Mc.NoticeOdchodZakaznika:
				ProcessNoticeOdchodZakaznika(message);
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