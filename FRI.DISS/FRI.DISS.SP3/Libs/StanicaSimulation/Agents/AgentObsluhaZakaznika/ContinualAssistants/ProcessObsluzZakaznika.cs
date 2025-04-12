using OSPABA;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Simulation;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentObsluhaZakaznika;
namespace FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentObsluhaZakaznika.ContinualAssistants
{
	//meta! id="38"
	public class ProcessObsluzZakaznika : OSPABA.Process
	{
		public ProcessObsluzZakaznika(int id, OSPABA.Simulation mySim, CommonAgent myAgent) :
			base(id, mySim, myAgent)
		{
		}

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Setup component for the next replication
		}

		//meta! sender="AgentObsluhaZakaznika", id="39", type="Start"
		public void ProcessStart(MessageForm message)
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
		override public void ProcessMessage(MessageForm message)
		{
			switch (message.Code)
			{
			case Mc.Start:
				ProcessStart(message);
			break;

			default:
				ProcessDefault(message);
			break;
			}
		}
		//meta! tag="end"
		public new AgentObsluhaZakaznika MyAgent
		{
			get
			{
				return (AgentObsluhaZakaznika)base.MyAgent;
			}
		}
	}
}